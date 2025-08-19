#Requires -Version 5.1

using assembly System.Xml.Linq

using namespace System
using namespace System.Diagnostics
using namespace System.IO
using namespace System.Management.Automation
using namespace System.Net
using namespace System.Xml
using namespace System.Xml.Linq

[CmdletBinding(PositionalBinding = $false)]
param
(
    [Parameter()]
    [ValidateSet('Debug', 'Release')]
    [string] $BuildConfiguration = 'Debug',

    [Parameter()]
    [ValidateSet('Any CPU')]
    [string] $BuildPlatform = 'Any CPU',

    [Parameter()]
    [switch] $EnableDotCover,

    [Parameter()]
    [AllowNull()]
    [AllowEmptyString()]
    [string] $PrereleaseSuffix = '-debug',

    [Parameter()]
    [switch] $AppveyorBuild,

    [Parameter()]
    [string] $AppveyorArtifactsSubdirectory = '.artifacts',

    [Parameter()]
    [string] $AppveyorBinariesSubdirectory = 'src\.out\bin',

    [Parameter()]
    [string] $AppveyorNuGetPackageSubdirectory = 'src\.out\NuGet',

    [Parameter()]
    [AllowNull()]
    [AllowEmptyString()]
    [string] $AppveyorSourceCodeRevisionId = $null,

    [Parameter()]
    [AllowNull()]
    [AllowEmptyString()]
    [string] $AppveyorSourceCodeBranch = $null,

    [Parameter()]
    [AllowNull()]
    [AllowEmptyString()]
    [string] $AppveyorBuildNumber = $null,

    [Parameter()]
    [AllowNull()]
    [AllowEmptyString()]
    [string] $AppveyorOriginalBuildVersion = $env:APPVEYOR_BUILD_VERSION,

    [Parameter()]
    [AllowNull()]
    [AllowEmptyString()]
    [string] $AppveyorDeploymentFlagVariableName = $null,

    [Parameter()]
    [AllowNull()]
    [AllowEmptyString()]
    [string] $AppveyorDeploymentVersionVariableName = $null
)
begin
{
    $Script:ErrorActionPreference = [System.Management.Automation.ActionPreference]::Stop
    Microsoft.PowerShell.Core\Set-StrictMode -Version 1
    $Global:ProgressPreference = [System.Management.Automation.ActionPreference]::SilentlyContinue

    [ValidateNotNullOrEmpty()] [string] $workspaceRootDirectoryPath = $PSScriptRoot
    [string] $solutionFilePattern = '*.sln'
    [string] $buildPropsFilePattern = 'Directory.Build.props'
    [string] $projectPlatform = 'AnyCPU'

    class FileXmlData
    {
        [string] $FilePath
        [xml] $Document

        FileXmlData([string] $filePath)
        {
            if ([string]::IsNullOrWhiteSpace($filePath))
            {
                throw [ArgumentException]::new('The file path cannot be blank.', 'filePath')
            }

            [xml] $xmlDocument = [xml]::new()
            $xmlDocument.Load($filePath) | Out-Null

            $this.FilePath = $filePath
            $this.Document = $xmlDocument
        }

        [string] GetSingleNodeText([string] $xPath)
        {
            if ([string]::IsNullOrWhiteSpace($xPath))
            {
                throw [ArgumentException]::new('The XPath cannot be blank.', 'xPath')
            }

            [XmlElement[]] $elements = @($this.Document.SelectNodes($xPath))
            if ($elements.Count -ne 1)
            {
                throw "There must be exactly one element matching XPath ""$xPath"" in ""$($this.FilePath)"", but found: $($elements.Count)."
            }

            [XmlElement] $element = $elements[0]
            return $element.InnerText
        }

        [string] GetProjectPropertyText([string] $propertyName)
        {
            if ([string]::IsNullOrWhiteSpace($propertyName))
            {
                throw [ArgumentException]::new('The property name cannot be blank.', 'propertyName')
            }

            return $this.GetSingleNodeText("/Project/PropertyGroup/$propertyName")
        }
    }

    function Get-ErrorDetails([ValidateNotNull()] [System.Management.Automation.ErrorRecord] $errorRecord = $_)
    {
        [ValidateNotNull()] [System.Exception] $exception = $errorRecord.Exception
        while ($exception -is [System.Management.Automation.RuntimeException] -and $exception.InnerException -ne $null)
        {
            $exception = $exception.InnerException
        }

        [string[]] $lines = `
        @(
            $exception.Message,
            '',
            '<<<',
            "Exception: '$($exception.GetType().FullName)'",
            "FullyQualifiedErrorId: '$($errorRecord.FullyQualifiedErrorId)'"
        )

        if (![string]::IsNullOrWhiteSpace($errorRecord.ScriptStackTrace))
        {
            $lines += `
            @(
                '',
                'Script stack trace:',
                '-------------------',
                $($errorRecord.ScriptStackTrace)
            )
        }

        if (![string]::IsNullOrWhiteSpace($exception.StackTrace))
        {
            $lines += `
            @(
                '',
                'Exception stack trace:',
                '----------------------',
                $($exception.StackTrace)
            )
        }

        $lines += '>>>'

        return ($lines -join ([System.Environment]::NewLine))
    }

    function Write-MajorSeparator
    {
        [CmdletBinding(PositionalBinding = $false)]
        param ()
        process
        {
            Write-Host ''
            Write-Host -ForegroundColor Magenta ('=' * 100)
            Write-Host ''
        }
    }

    function Get-ApplicationPath
    {
        [CmdletBinding(PositionalBinding = $false)]
        param
        (
            [Parameter(Position = 0, ValueFromPipeline = $true)]
            [string] $Name
        )
        process
        {
            if ([string]::IsNullOrWhiteSpace($Name))
            {
                throw [ArgumentException]::new('The application name cannot be blank.', 'Name')
            }

            [string[]] $paths = Get-Command -ErrorAction SilentlyContinue -CommandType Application -Name $Name `
                | Select-Object -ExpandProperty Path

            [string] $path = if ([object]::ReferenceEquals($paths, $null) -or $paths.Count -eq 0) { $null } else { $paths[0] }
            if ([string]::IsNullOrEmpty($path))
            {
                [string] $errorMessage = "The application ""$Name"" is not found."

                if ($ErrorActionPreference -eq [ActionPreference]::Continue)
                {
                    Write-Warning $errorMessage
                    return $null
                }

                if ($ErrorActionPreference -in @([ActionPreference]::Ignore, [ActionPreference]::SilentlyContinue))
                {
                    return $null
                }

                throw $errorMessage
            }
            Write-Verbose "Application ""$Name"" has been resolved to ""$path""."
            return $path
        }
    }

    function Execute-Command
    {
        [CmdletBinding(PositionalBinding = $false)]
        param
        (
            [Parameter()]
            [string] $Title,

            [Parameter(Position = 0)]
            [string] $Command,

            [Parameter(Position = 1, ValueFromRemainingArguments = $true)]
            [string[]] $CommandArguments = @()
        )

        if ([string]::IsNullOrWhiteSpace($Title))
        {
            throw [ArgumentException]::new('The command title cannot be blank.', 'Title')
        }
        if ([string]::IsNullOrWhiteSpace($Command))
        {
            throw [ArgumentException]::new('The command cannot be blank.', 'Command')
        }
        if ([object]::ReferenceEquals($CommandArguments, $null))
        {
            throw [ArgumentNullException]::new('CommandArguments')
        }

        Write-Host ''
        Write-Host -ForegroundColor Green "$($Title)..."

        Write-Verbose -Verbose "Executing <""$Command"" $CommandArguments>"

        [int] $exitCode = [int]::MinValue
        [Stopwatch] $stopwatch = [Stopwatch]::StartNew()
        try
        {
            $ErrorActionPreference = [System.Management.Automation.ActionPreference]::SilentlyContinue
            & cmd /c """$Command"" $CommandArguments" 2`>`&1
            $exitCode = if (Test-Path Variable:\LASTEXITCODE) { $LASTEXITCODE } else { [int]::MinValue }
        }
        finally
        {
            $ErrorActionPreference = [System.Management.Automation.ActionPreference]::Stop
            $stopwatch.Stop()
        }

        [bool] $isSuccessful = $exitCode -eq 0
        if (!$isSuccessful)
        {
            throw "$($Title) - FAILED (exit code: $($exitCode), time elapsed: $($stopwatch.Elapsed))."
        }

        Write-Host -ForegroundColor Green "$($Title) - DONE (exit code: $($exitCode), time elapsed: $($stopwatch.Elapsed))."
        Write-Host ''
    }

    function Ensure-CleanDirectory
    {
        [CmdletBinding(PositionalBinding = $false)]
        param
        (
            [Parameter(Position = 0, ValueFromPipeline = $true)]
            [Alias('Path')]
            [string] $LiteralPath
        )
        process
        {
            if (Test-Path $LiteralPath)
            {
                Write-Host "Deleting ""$LiteralPath""."
                Remove-Item -Path $LiteralPath -Recurse -Force | Out-Null
            }

            Write-Host "Creating directory ""$LiteralPath""."
            New-Item -Path $LiteralPath -ItemType Directory -Force | Out-Null
        }
    }

    function Resolve-WorkspacePath
    {
        [CmdletBinding(PositionalBinding = $false)]
        param
        (
            [Parameter(Position = 0, ValueFromPipeline = $true)]
            [Alias('Path')]
            [string] $RelativePath
        )
        process
        {
            if ([string]::IsNullOrWhiteSpace($RelativePath))
            {
                throw [ArgumentException]::new('The relative path cannot be blank.', 'RelativePath')
            }

            return [Path]::GetFullPath([Path]::Combine($workspaceRootDirectoryPath, $RelativePath))
        }
    }

    function Find-SingleFileInWorkspace
    {
        [CmdletBinding(PositionalBinding = $false)]
        param
        (
            [Parameter(Position = 0, ValueFromPipeline = $true)]
            [string] $Pattern
        )
        process
        {
            if ([string]::IsNullOrWhiteSpace($Pattern))
            {
                throw [ArgumentException]::new('The file pattern cannot be blank.', 'Pattern')
            }

            [string[]] $allFoundFilePaths = `
                Get-ChildItem -Path $workspaceRootDirectoryPath -Include $Pattern -Recurse -Force -File `
                    | Select-Object -ExpandProperty FullName

            if ($allFoundFilePaths.Count -ne 1)
            {
                throw "There must be exactly one file matching ""$Pattern"" within ""$workspaceRootDirectoryPath"", but found: $($allFoundFilePaths.Count)."
            }

            return $allFoundFilePaths[0]
        }
    }
}
process
{
    [Console]::ResetColor()
    Write-MajorSeparator

    [Stopwatch] $entireBuildStopwatch = [Stopwatch]::StartNew()
    try
    {
        Write-Host -ForegroundColor Green "BuildConfiguration: ""$BuildConfiguration"""
        Write-Host -ForegroundColor Green "BuildPlatform: ""$BuildPlatform"""
        Write-Host -ForegroundColor Green "EnableDotCover: $EnableDotCover"
        Write-Host -ForegroundColor Green "PrereleaseSuffix: ""$PrereleaseSuffix"""
        Write-Host ''
        Write-Host -ForegroundColor Green "AppveyorBuild: $AppveyorBuild"
        if ($AppveyorBuild)
        {
            Write-Host -ForegroundColor Green "AppveyorArtifactsSubdirectory: ""$AppveyorArtifactsSubdirectory"""
            Write-Host -ForegroundColor Green "AppveyorBinariesSubdirectory: ""$AppveyorBinariesSubdirectory"""
            Write-Host -ForegroundColor Green "AppveyorNuGetPackageSubdirectory: ""$AppveyorNuGetPackageSubdirectory"""
            Write-Host -ForegroundColor Green "AppveyorSourceCodeRevisionId: ""$AppveyorSourceCodeRevisionId"""
            Write-Host -ForegroundColor Green "AppveyorSourceCodeBranch: ""$AppveyorSourceCodeBranch"""
            Write-Host -ForegroundColor Green "AppveyorBuildNumber: ""$AppveyorBuildNumber"""
            Write-Host -ForegroundColor Green "AppveyorOriginalBuildVersion: ""$AppveyorOriginalBuildVersion"""
            Write-Host -ForegroundColor Green "AppveyorDeployFlagVariableName: ""$AppveyorDeploymentFlagVariableName"""
            Write-Host -ForegroundColor Green "AppveyorDeploymentVersionVariableName: ""$AppveyorDeploymentVersionVariableName"""
        }

        Write-MajorSeparator

        if ([string]::IsNullOrWhiteSpace($BuildConfiguration))
        {
            throw [ArgumentException]::new('The build configuration cannot be blank.', 'BuildConfiguration')
        }
        if ([string]::IsNullOrWhiteSpace($BuildPlatform))
        {
            throw [ArgumentException]::new('The build platform cannot be blank.', 'BuildPlatform')
        }

        if (![string]::IsNullOrEmpty($PrereleaseSuffix))
        {
            [string] $prereleaseSuffixPattern = '^\-[0-9A-Za-z\-\.]+$'
            if ($PrereleaseSuffix -cnotmatch $prereleaseSuffixPattern)
            {
                throw [ArgumentException]::new(
                    ("""$PrereleaseSuffix"" is not a valid semantic version pre-release suffix" `
                        + ". Must match the regular expression: $prereleaseSuffixPattern"),
                    'PrereleaseSuffix')
            }
        }

        [int] $resolvedBuildNumber = 0

        [string] $sevenZipExecutablePath = $null
        [string] $resolvedArtifactsDirectoryPath = $null
        if ($AppveyorBuild)
        {
            if ([string]::IsNullOrWhiteSpace($AppveyorArtifactsSubdirectory))
            {
                throw [ArgumentException]::new(
                    'The artifacts subdirectory cannot be blank when AppveyorBuild is ON.',
                    'AppveyorArtifactsSubdirectory')
            }
            if ([string]::IsNullOrWhiteSpace($AppveyorBinariesSubdirectory))
            {
                throw [ArgumentException]::new(
                    'The subirectory that contains binaries cannot be blank when AppveyorBuild is ON.',
                    'AppveyorBinariesSubdirectory')
            }
            if ([string]::IsNullOrWhiteSpace($AppveyorNuGetPackageSubdirectory))
            {
                throw [ArgumentException]::new(
                    'The subirectory that NuGet package(s) cannot be blank when AppveyorBuild is ON.',
                    '$AppveyorNuGetPackageSubdirectory')
            }
            if ([string]::IsNullOrWhiteSpace($AppveyorSourceCodeRevisionId))
            {
                throw [ArgumentException]::new(
                    'The source code revision ID cannot be blank when the AppveyorBuild switch is ON.',
                    'AppveyorSourceCodeRevisionId')
            }
            if ($AppveyorSourceCodeRevisionId -cnotmatch '^[0-9a-fA-F]{40}$')
            {
                throw [ArgumentException]::new(
                    "The specified source code revision ID ""$AppveyorSourceCodeRevisionId"" is invalid.",
                    'AppveyorSourceCodeRevisionId')
            }
            if ([string]::IsNullOrWhiteSpace($AppveyorSourceCodeBranch))
            {
                throw [ArgumentException]::new(
                    'The Appveyor source code branch cannot be blank when the AppveyorBuild switch is ON.',
                    'AppveyorSourceCodeBranch')
            }
            if ([string]::IsNullOrWhiteSpace($AppveyorBuildNumber))
            {
                throw [ArgumentException]::new(
                    'The Appveyor build number cannot be blank when the AppveyorBuild switch is ON.',
                    'AppveyorBuildNumber')
            }
            if (![int]::TryParse($AppveyorBuildNumber, [ref] $resolvedBuildNumber) -or $resolvedBuildNumber -le 0)
            {
                throw [ArgumentException]::new(
                    "The specified Appveyor build number ""$AppveyorBuildNumber"" is not a positive integer number.",
                    'AppveyorBuildNumber')
            }
            if ([string]::IsNullOrWhiteSpace($AppveyorOriginalBuildVersion))
            {
                throw [ArgumentException]::new(
                    'The original Appveyor build version cannot be blank when the AppveyorBuild switch is ON.',
                    'AppveyorOriginalBuildVersion')
            }
            if ([string]::IsNullOrWhiteSpace($AppveyorDeploymentFlagVariableName))
            {
                throw [ArgumentException]::new(
                    'The name of the deployment flag environment variable cannot be blank when the AppveyorBuild switch is ON.',
                    'AppveyorDeploymentFlagVariableName')
            }
            if ([string]::IsNullOrWhiteSpace($AppveyorDeploymentVersionVariableName))
            {
                throw [ArgumentException]::new(
                    'The name of the deployment version environment variable cannot be blank when the AppveyorBuild switch is ON.',
                    'AppveyorDeploymentVersionVariableName')
            }

            $resolvedArtifactsDirectoryPath = $AppveyorArtifactsSubdirectory | Resolve-WorkspacePath
            Ensure-CleanDirectory -LiteralPath $resolvedArtifactsDirectoryPath

            $sevenZipExecutablePath = Get-ApplicationPath -Verbose -Name '7z.exe'
        }

        function Execute-SevenZip
        {
            [CmdletBinding(PositionalBinding = $false)]
            param
            (
                [Parameter()]
                [ValidateNotNullOrEmpty()]
                [string] $Description = $([ArgumentNullException]::new('Description')),

                [Parameter()]
                [ValidateNotNullOrEmpty()]
                [string] $ArchiveFilePath = $([ArgumentNullException]::new('ArchiveFilePath')),

                [Parameter(ValueFromRemainingArguments = $true)]
                [ValidateNotNullOrEmpty()]
                [string[]] $Items = $([ArgumentNullException]::new('Items'))
            )
            process
            {
                if ([string]::IsNullOrWhiteSpace($sevenZipExecutablePath))
                {
                    throw '7-Zip executable path is not assigned.'
                }

                [string[]] $processedItems = @($Items | % { """$_""" })

                Execute-Command `
                    -Verbose `
                    -Title "* 7-Zip: Archive $Description" `
                    -Command $sevenZipExecutablePath `
                    -CommandArguments `
                        (
                            @(
                                'a'
                                '-y'
                                '-tzip'
                                '-r'
                                '-mx1'
                                '-bd'
                                """$ArchiveFilePath"""
                                '--'
                            ) `
                            + $processedItems
                        )
            }
        }

        [ValidateNotNullOrEmpty()] [string] $solutionFilePath = $solutionFilePattern | Find-SingleFileInWorkspace
        [ValidateNotNullOrEmpty()] [string] $solutionDirectoryPath = [Path]::GetDirectoryName($solutionFilePath)
        [ValidateNotNullOrEmpty()] [string] $solutionNameOnly = [Path]::GetFileNameWithoutExtension($solutionFilePath)

        [ValidateNotNullOrEmpty()] [string] $buildPropsFilePath = $buildPropsFilePattern | Find-SingleFileInWorkspace
        [FileXmlData] $buildPropsFileXmlData = [FileXmlData]::new($buildPropsFilePath)
        [string] $versionString = $buildPropsFileXmlData.GetProjectPropertyText('Version')

        [version] $version = $null
        if (![version]::TryParse($versionString, [ref] $version) -or $version.Revision -ge 0)
        {
            throw "Invalid version ""$versionString"" at XPath ""$versionElementPath"" in ""$buildPropsFilePath""."
        }

        [string] $resolvedPrereleaseSuffix = `
            if ([string]::IsNullOrEmpty($PrereleaseSuffix))
            {
                $null
            }
            else
            {
                "$PrereleaseSuffix.$resolvedBuildNumber"
            }

        [string] $dateStamp = [datetime]::UtcNow.ToString('yyyyMMddTHHmmss"Z"')

        [string] $shortRevisionId = `
            if ($AppveyorBuild -and $AppveyorSourceCodeRevisionId)
            {
                $AppveyorSourceCodeRevisionId.ToLowerInvariant().Substring(0, 16)
            }
            else
            {
                $null
            }

        [string] $informationalVersionRevisionPrefix = `
            if ($shortRevisionId)
            {
                "$shortRevisionId."
            }
            else
            {
                $null
            }

        [string] $informationalVersion = "$($version)$($resolvedPrereleaseSuffix)+$($informationalVersionRevisionPrefix)$($dateStamp)"

        if ($AppveyorBuild)
        {
            Update-AppveyorBuild `
                -Version "v$($version): $AppveyorOriginalBuildVersion"

            Set-AppveyorBuildVariable `
                -Verbose `
                -Name $AppveyorDeploymentVersionVariableName `
                -Value "v$version [build $resolvedBuildNumber] [$dateStamp]"
        }

        [ValidateNotNullOrEmpty()] [string] $testProjectFileNameOnly = "$solutionNameOnly.Tests"
        [ValidateNotNullOrEmpty()] [string] $testProjectFilePath = [Path]::Combine($solutionDirectoryPath, $testProjectFileNameOnly, "$testProjectFileNameOnly.csproj")

        [FileXmlData] $testProjectFileXmlData = [FileXmlData]::new($testProjectFilePath)
        [string] $targetFrameworksString = $testProjectFileXmlData.GetProjectPropertyText('TargetFrameworks')

        [string[]] $testFrameworks = $targetFrameworksString -csplit ';'
        if ($testFrameworks.Count -eq 0)
        {
            throw "No target frameworks defined in ""$testProjectFilePath"" at ""$targetFrameworksElementPath""."
        }

        [string] $temporaryDirectoryPath = [Path]::GetFullPath([Path]::Combine($PSScriptRoot, '.temp'))
        Ensure-CleanDirectory -LiteralPath $temporaryDirectoryPath

        Write-MajorSeparator
        [ValidateNotNullOrEmpty()] [string] $dotNetCliPath = Get-ApplicationPath -Verbose -Name dotnet

        Write-MajorSeparator
        Execute-Command -Title '* DotNet CLI Version' -Command $dotNetCliPath -CommandArguments '--version'

        Write-MajorSeparator
        Execute-Command -Title '* DotNet SDKs' -Command $dotNetCliPath -CommandArguments '--list-sdks'

        function Create-DotNetCliExecuteCommandParameters
        {
            [CmdletBinding(PositionalBinding = $false)]
            param
            (
                [Parameter()]
                [switch] $NoBuildConfiguration,

                [Parameter()]
                [string] $TitleDetails = $null,

                [Parameter(Position = 0)]
                [string] $Command,

                [Parameter(Position = 1, ValueFromRemainingArguments = $true)]
                [string[]] $CommandArguments = @()
            )
            process
            {
                if ([string]::IsNullOrWhiteSpace($Command))
                {
                    throw [ArgumentException]::new('The command cannot be blank.', 'Command')
                }
                if ([object]::ReferenceEquals($CommandArguments, $null))
                {
                    throw [ArgumentNullException]::new('CommandArguments')
                }

                [string[]] $commonCommandArguments = `
                    @(
                        """$solutionFilePath"""
                        '--verbosity:normal' # quiet, minimal, normal, detailed, and diagnostic
                        "-p:IsAppveyorBuild=$AppveyorBuild"
                        "-p:Version=""$version"""
                        "-p:FileVersion=""$version.$resolvedBuildNumber"""
                        "-p:InformationalVersion=""$informationalVersion"""
                        "-p:VersionSuffix=""$resolvedPrereleaseSuffix"""
                    )

                if (!$NoBuildConfiguration)
                {
                    $commonCommandArguments += "--configuration:""$BuildConfiguration"""
                }

                [string] $title = "* DotNet CLI: $Command"
                if (![string]::IsNullOrWhiteSpace($TitleDetails))
                {
                    $title += " ($($TitleDetails.Trim()))"
                }

                return `
                    @{
                        Title = $title
                        Command = $dotNetCliPath
                        CommandArguments = (@($Command) + $commonCommandArguments + $CommandArguments)
                    }
            }
        }

        function Execute-DotNetCli
        {
            [CmdletBinding(PositionalBinding = $false)]
            param
            (
                [Parameter()]
                [switch] $NoBuildConfiguration,

                [Parameter(Position = 0)]
                [string] $Command,

                [Parameter(Position = 1, ValueFromRemainingArguments = $true)]
                [string[]] $CommandArguments = @()
            )
            process
            {
                [hashtable] $executeCommandParameters = Create-DotNetCliExecuteCommandParameters `
                    -NoBuildConfiguration:$NoBuildConfiguration `
                    -Command:$Command `
                    -CommandArguments:$CommandArguments

                Write-MajorSeparator
                Execute-Command @executeCommandParameters
            }
        }

        [string] $coverageOutputDirectoryPath = $null
        if ($EnableDotCover)
        {
            # Watch: https://github.com/dotnet/msbuild/issues/3911
            $coverageOutputDirectoryPath = [Path]::GetFullPath([Path]::Combine($solutionDirectoryPath, '.out', 'binCoverage'))
            Ensure-CleanDirectory -LiteralPath $coverageOutputDirectoryPath
        }

        Execute-DotNetCli clean
        Execute-DotNetCli -NoBuildConfiguration restore --force --no-cache

        [string[]] $dotNetBuildCommandArguments = `
            @(
                '--no-incremental'
                '--no-restore'
                "-p:Platform=""$BuildPlatform"""
            )

        Execute-DotNetCli build @dotNetBuildCommandArguments

        [string] $archiveVersionSuffix = $null
        if ($AppveyorBuild)
        {
            $archiveVersionSuffix = "-v$($version).$($resolvedBuildNumber)$($PrereleaseSuffix).rev-$($shortRevisionId)"

            [string] $binariesBaseDirectoryPath = $AppveyorBinariesSubdirectory | Resolve-WorkspacePath
            [string] $binariesDirectoryPath = [Path]::Combine($binariesBaseDirectoryPath, $projectPlatform, $BuildConfiguration)
            [string] $binariesArchiveFilePath = "$resolvedArtifactsDirectoryPath\$($solutionNameOnly).Binaries$($archiveVersionSuffix).zip"

            Write-MajorSeparator

            Execute-SevenZip `
                -Description 'Binaries' `
                -ArchiveFilePath $binariesArchiveFilePath `
                -Items "$binariesDirectoryPath\*.*"

            Write-MajorSeparator

            [string] $packageId = $solutionNameOnly

            [psobject] $publishedPackageInfo = Invoke-RestMethod `
                -Verbose `
                -UseBasicParsing `
                -Method Get `
                -Uri "https://api.nuget.org/v3/registration5-gz-semver2/$([WebUtility]::UrlEncode($packageId.ToLowerInvariant()))/index.json"

            [bool] $isPatchBranch = $AppveyorSourceCodeBranch -cmatch '^patch\/v(?<major>\d+)\.(?<minor>\d+)\.x$'

            [bool] $shouldCopyPackageToArtifacts = $false
            if ($isPatchBranch)
            {
                [version] $packagePatchVersionBase = [version]::new([int]$Matches['major'], [int]$Matches['minor'], 0)
                Write-Host "Patch version base: ""$packagePatchVersionBase""."

                [version] $latestPublishedPatchVersion = $packagePatchVersionBase
                foreach ($packageInfoItem in $publishedPackageInfo.items)
                {
                    foreach ($packageInfoSubitem in $packageInfoItem.items)
                    {
                        [ValidateNotNullOrEmpty()] [string] $itemVersionString = $packageInfoSubitem.catalogEntry.version
                        [version] $itemVersion = [version]::Parse($itemVersionString)
                        if ($itemVersion.Major -eq $packagePatchVersionBase.Major -and $itemVersion.Minor -eq $packagePatchVersionBase.Minor)
                        {
                            if ($latestPublishedPatchVersion -lt $itemVersion)
                            {
                                $latestPublishedPatchVersion = $itemVersion
                            }
                        }
                    }
                }

                Write-Host "The current package version is ""$version""."
                Write-Host "The latest published PATCH version is ""$latestPublishedPatchVersion""."

                if ($latestPublishedPatchVersion -lt $version)
                {
                    $shouldCopyPackageToArtifacts = $true
                }
                else
                {
                    Write-Warning `
                        -WarningAction Continue `
                        -Message ("The current package version is ""$version""" `
                            + ", but the PATCH version ""$latestPublishedPatchVersion"" is already published" `
                            + ". Skipping to publish the NuGet package ""$packageId"".")

                    Set-AppveyorBuildVariable -Verbose -Name $AppveyorDeploymentFlagVariableName -Value 'false'
                    Set-AppveyorBuildVariable -Verbose -Name $AppveyorDeploymentVersionVariableName -Value ([string]::Empty)
                }
            }
            else
            {
                [version] $latestPublishedPackageVersion = [version]::new(0, 0)
                foreach ($packageInfoItem in $publishedPackageInfo.items)
                {
                    [ValidateNotNullOrEmpty()] [string] $itemVersionString = $packageInfoItem.upper
                    [version] $itemVersion = [version]::Parse($itemVersionString)
                    if ($latestPublishedPackageVersion -lt $itemVersion)
                    {
                        $latestPublishedPackageVersion = $itemVersion
                    }
                }

                Write-Host "The current package version is ""$version""."
                Write-Host "The latest published package version is ""$latestPublishedPackageVersion""."

                if ($latestPublishedPackageVersion -lt $version)
                {
                    $shouldCopyPackageToArtifacts = $true
                }
                else
                {
                    Write-Warning `
                        -WarningAction Continue `
                        -Message ("The current package version is ""$version""" `
                            + ", but the version ""$latestPublishedPackageVersion"" is already published" `
                            + ". Skipping to publish the NuGet package ""$packageId"".")

                    Set-AppveyorBuildVariable -Verbose -Name $AppveyorDeploymentFlagVariableName -Value 'false'
                    Set-AppveyorBuildVariable -Verbose -Name $AppveyorDeploymentVersionVariableName -Value ([string]::Empty)
                }
            }

            if ($shouldCopyPackageToArtifacts)
            {
                [string] $nuGetPackageDirectoryPath = $AppveyorNuGetPackageSubdirectory | Resolve-WorkspacePath

                Copy-Item `
                    -Verbose `
                    -Path "$nuGetPackageDirectoryPath\*.*nupkg" `
                    -Destination $resolvedArtifactsDirectoryPath `
                    -Recurse `
                    | Out-Null
            }
        }

        [string] $snapshotFileBaseName = "$([Path]::GetFileNameWithoutExtension($solutionFilePath)).CoverageResult"

        foreach ($testFramework in $testFrameworks)
        {
            [string] $coverageSnapshotFilePath = $null
            [string] $dotCoverExecutablePath = $null

            [string[]] $testExecutionCliOptions = `
                @(
                    '--no-build'
                    '--logger:trx'
                    '--logger:html'
                    '--logger:console'
                    "--framework:""$testFramework"""
                )

            if ($AppveyorBuild)
            {
                $testExecutionCliOptions += @('--test-adapter-path:.', '--logger:Appveyor')
            }

            $testExecutionCliOptions += `
                @(
                    '--',
                    "NUnit.DefaultTestNamePattern=""[$testFramework]{m}{a}#{i}"""
                )

            [hashtable] $testExecutionCommandParameters = `
                Create-DotNetCliExecuteCommandParameters -TitleDetails $testFramework test @testExecutionCliOptions

            if ($EnableDotCover)
            {
                [string] $dotCoverExecutableName = 'dotcover.exe'

                $dotCoverExecutablePath = Get-ApplicationPath -Verbose -Name $dotCoverExecutableName -ErrorAction SilentlyContinue
                if ([string]::IsNullOrEmpty($dotCoverExecutablePath))
                {
                    Write-MajorSeparator

                    [string] $chocolateyExecutablePath = Get-ApplicationPath -Verbose -Name 'choco.exe'
                    Execute-Command -Title 'Install dotCover CLI' $chocolateyExecutablePath install --yes --no-progress --version=2025.1.5 dotcover-cli
                    $dotCoverExecutablePath = Get-ApplicationPath -Verbose -Name $dotCoverExecutableName
                }

                $coverageSnapshotFilePath = [Path]::Combine(
                    $coverageOutputDirectoryPath,
                    $snapshotFileBaseName,
                    "$snapshotFileBaseName.$testFramework.dcvr")

                [XDocument] $coverageAnalysisParametersDocument = [XDocument]::new(
                    [XElement]::new(
                        'AnalyseParams',
                        @(
                            [XElement]::new('TargetExecutable', $testExecutionCommandParameters.Command),
                            [XElement]::new('TargetArguments', ($testExecutionCommandParameters.CommandArguments -join ' ')),
                            [XElement]::new('AnalyzeTargetArguments', 'False'),
                            [XElement]::new('Output', $coverageSnapshotFilePath),
                            [XElement]::new(
                                'Filters',
                                @(
                                    [XElement]::new(
                                        'ExcludeFilters',
                                        @(
                                            [XElement]::new(
                                                'FilterEntry',
                                                @(
                                                    [XElement]::new('ModuleMask', '*.Tests'),
                                                    [XElement]::new('ClassMask', '*'),
                                                    [XElement]::new('FunctionMask', '*')
                                                ))
                                        ))
                                ))
                        )))

                [string] $coverageAnalysisParametersFilePath = [Path]::Combine(
                    $temporaryDirectoryPath,
                    'CoverageAnalysis.Parameters.xml')

                $coverageAnalysisParametersDocument.Save($coverageAnalysisParametersFilePath, [SaveOptions]::None) | Out-Null

                [hashtable] $newTestExecutionCommandParameters = `
                    @{
                        Title = "$($testExecutionCommandParameters.Title) (via dotCover)"
                        Command = $dotCoverExecutablePath
                        CommandArguments = @('cover', """$coverageAnalysisParametersFilePath""")
                    }

                $testExecutionCommandParameters = $newTestExecutionCommandParameters
            }

            Write-MajorSeparator
            [ErrorRecord] $testExecutionErrorRecord = $null
            try
            {
                Execute-Command @testExecutionCommandParameters
            }
            catch
            {
                if (!$EnableDotCover -and !$AppveyorBuild)
                {
                    throw
                }

                $testExecutionErrorRecord = $_

                Write-Host ''
                Write-Warning -WarningAction Continue (Get-ErrorDetails)
                Write-Host ''
            }

            if ($EnableDotCover)
            {
                if (![File]::Exists($coverageSnapshotFilePath))
                {
                    if ($testExecutionErrorRecord)
                    {
                        Write-Error -ErrorAction Stop $testExecutionErrorRecord
                    }

                    throw "Coverage snapshot file ""$coverageSnapshotFilePath"" is not found."
                }

                [string] $coverageHtmlReportFilePath = [Path]::ChangeExtension($coverageSnapshotFilePath, '.html')
                [string] $coverageJsonReportFilePath = [Path]::ChangeExtension($coverageSnapshotFilePath, '.json')

                [XDocument] $coverageReportParametersDocument = [XDocument]::new(
                    [XElement]::new(
                        'ReportParams',
                        @(
                            [XElement]::new('Source', $coverageSnapshotFilePath),
                            [XElement]::new('ReportType', 'HTML,JSON'),
                            [XElement]::new('Output', $coverageHtmlReportFilePath),
                            [XElement]::new('Output', $coverageJsonReportFilePath)
                        )))

                [string] $coverageReportParametersFilePath =  [Path]::Combine($temporaryDirectoryPath, 'CoverageReport.Parameters.xml')
                $coverageReportParametersDocument.Save($coverageReportParametersFilePath, [SaveOptions]::None) | Out-Null

                Write-MajorSeparator
                Execute-Command -Title "Creating dotCover report(s)" $dotCoverExecutablePath report """$coverageReportParametersFilePath"""
            }

            if ($testExecutionErrorRecord)
            {
                Write-Error -ErrorAction Stop $testExecutionErrorRecord
            }
        }

        Write-MajorSeparator

        if ($AppveyorBuild)
        {
            [string] $testResultsSubdirectory = $buildPropsFileXmlData.GetProjectPropertyText('__X_TestResultsSubdirectory')

            [string] $binariesDirectoryPath = $AppveyorBinariesSubdirectory | Resolve-WorkspacePath
            [string] $testResultsDirectoryPath = [Path]::Combine($binariesDirectoryPath, $projectPlatform, $BuildConfiguration, $testProjectFileNameOnly, $testResultsSubdirectory)
            [string] $testResultsArchiveFilePath = "$resolvedArtifactsDirectoryPath\$($solutionNameOnly).$($testResultsSubdirectory)$($archiveVersionSuffix).zip"

            Execute-SevenZip `
                -Description 'Test Results' `
                -ArchiveFilePath $testResultsArchiveFilePath `
                -Items "$testResultsDirectoryPath\*.*"

            Write-MajorSeparator

            if ($EnableDotCover)
            {
                [string] $coverageResultDirectoryPath = [Path]::Combine($coverageOutputDirectoryPath, $snapshotFileBaseName)
                [string] $coverageResultArchiveFilePath = "$resolvedArtifactsDirectoryPath\$($snapshotFileBaseName)$($archiveVersionSuffix).zip"

                Execute-SevenZip `
                    -Description 'Coverage Results' `
                    -ArchiveFilePath $coverageResultArchiveFilePath `
                    -Items "$coverageResultDirectoryPath\*.*"

                Write-MajorSeparator
            }
        }
    }
    catch
    {
        [string] $errorDetails = Get-ErrorDetails
        [Console]::ResetColor()
        Write-MajorSeparator
        Write-Host -ForegroundColor Red $errorDetails
        Write-MajorSeparator

        throw
    }
    finally
    {
        Write-Host -ForegroundColor Cyan "* Total build time: $($entireBuildStopwatch.Elapsed)"
        Write-MajorSeparator
    }
}