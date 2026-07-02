#Requires -Version 5.1

using assembly System.Xml.Linq

using namespace System
using namespace System.Diagnostics
using namespace System.IO
using namespace System.Management.Automation
using namespace System.Net
using namespace System.Text
using namespace System.Xml
using namespace System.Xml.Linq

[System.Diagnostics.CodeAnalysis.SuppressMessage('PSAvoidUsingCmdletAliases', '')]
[System.Diagnostics.CodeAnalysis.SuppressMessage('PSUseApprovedVerbs', '')]
[CmdletBinding(PositionalBinding = $false)]
param
(
    [Parameter()]
    [ValidateSet('Debug', 'Release')]
    [string] $BuildConfiguration = 'Debug',

    [Parameter()]
    [AllowNull()]
    [AllowEmptyString()]
    [string] $PrereleaseSuffix = '-debug',

    [Parameter()]
    [AllowNull()]
    [scriptblock] $TestFrameworkFilter = $null,

    [Parameter()]
    [switch] $AppveyorBuild,

    [Parameter()]
    [string] $AppveyorArtifactsSubdirectory = '.artifacts',

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

            if (![File]::Exists($filePath))
            {
                throw [FileNotFoundException]::new("File ""$filePath"" is not found.")
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

        [string] GetAttributeValue([string] $xPath)
        {
            if ([string]::IsNullOrWhiteSpace($xPath))
            {
                throw [ArgumentException]::new('The XPath cannot be blank.', 'xPath')
            }

            [XmlNode[]] $nodes = @($this.Document.SelectNodes($xPath))
            if ($nodes.Count -ne 1)
            {
                throw "There must be exactly one node matching XPath ""$xPath"" in ""$($this.FilePath)"", but found: $($nodes.Count)."
            }

            [XmlNode] $node = $nodes[0]
            return $node.'#text'
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

    function Remove-TrailingPathSeparator
    {
        [CmdletBinding(PositionalBinding = $false)]
        param
        (
            [Parameter(Position = 0, ValueFromPipeline = $true)]
            [string] $Value
        )
        process
        {
            if ([string]::IsNullOrEmpty($Value))
            {
                throw [ArgumentException]::new('The value cannot be null or empty.', 'Value')
            }

            return $Value.TrimEnd([char[]]@([Path]::DirectorySeparatorChar, [Path]::AltDirectorySeparatorChar))
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

    function Remove-FileSystemObjectForced
    {
        [CmdletBinding(PositionalBinding = $false)]
        param
        (
            [Parameter(Position = 0, ValueFromPipeline = $true)]
            [Alias('Path')]
            [string] $LiteralPath,

            [Parameter()]
            [ValidateRange(0, [byte]::MaxValue)]
            [int] $MaxRetryCount = 10,

            [Parameter()]
            [timespan] $MaxRetryDelay = $([timespan]::FromSeconds(5))
        )
        begin
        {
            [timespan] $minMaxRetryDelay = [timespan]::FromSeconds(1)
        }
        process
        {
            [timespan] $resolvedMaxRetryDelay = if ($MaxRetryDelay -lt $minMaxRetryDelay) { $minMaxRetryDelay } else { $MaxRetryDelay }
            [timespan] $resolvedMinRetryDelay = [timespan]::FromTicks($resolvedMaxRetryDelay.Ticks / 10)

            if (![File]::Exists($LiteralPath) -and ![Directory]::Exists($LiteralPath))
            {
                return
            }

            Write-Host "Deleting ""$LiteralPath""."

            [int] $retriesLeft = $MaxRetryCount + 1
            while ($retriesLeft -gt 0)
            {
                $retriesLeft--

                try
                {
                    Remove-Item -Path $LiteralPath -Recurse -Force | Out-Null
                    break
                }
                catch
                {
                    if (!(Test-Path $LiteralPath))
                    {
                        break
                    }

                    [Exception] $exception = $_.Exception

                    if ($retriesLeft -le 0)
                    {
                        Write-Host -ForegroundColor Red "Failed to delete ""$LiteralPath"" (retries: $MaxRetryCount): [$($exception.GetType().FullName)] $($exception.Message)"
                        throw
                    }

                    [timespan] $currentDelay = [timespan]::FromTicks((Get-Random -Minimum $resolvedMinRetryDelay.Ticks -Maximum ($resolvedMaxRetryDelay.Ticks + 1)))

                    Write-Warning `
                        -WarningAction Continue `
                        -Message ("Could not delete ""$LiteralPath"". Retrying in $currentDelay. Retries left: $retriesLeft." `
                            + "$([Environment]::NewLine)[$($exception.GetType().FullName)] $($exception.Message)")

                    [System.Threading.Thread]::Sleep($currentDelay) | Out-Null
                }
            }
        }
    }

    function Ensure-CleanDirectory
    {
        [CmdletBinding(PositionalBinding = $false)]
        param
        (
            [Parameter(Position = 0, ValueFromPipeline = $true)]
            [Alias('Path')]
            [string] $LiteralPath,

            [Parameter()]
            [ValidateRange(0, [byte]::MaxValue)]
            [int] $MaxRetryCount = 10,

            [Parameter()]
            [timespan] $MaxRetryDelay = $([timespan]::FromSeconds(5))
        )
        process
        {
            Remove-FileSystemObjectForced -LiteralPath $LiteralPath -MaxRetryCount $MaxRetryCount -MaxRetryDelay $MaxRetryDelay

            Write-Host "Creating directory ""$LiteralPath""."
            New-Item -Force -ItemType Directory -Path $LiteralPath | Out-Null
        }
    }

    function Ensure-Directories
    {
        [CmdletBinding(PositionalBinding = $false)]
        param
        (
            [Parameter()]
            [string[]] $DirectoryPaths = $null,

            [Parameter()]
            [string[]] $FilePaths = $null
        )
        process
        {
            [string[]] $allDirectoryPaths = @()

            if ($DirectoryPaths -is [object])
            {
                $allDirectoryPaths += $DirectoryPaths
            }

            if ($FilePaths -is [object])
            {
                $allDirectoryPaths += @($FilePaths | % { [Path]::GetDirectoryName($_) })
            }

            $allDirectoryPaths = $allDirectoryPaths | Sort-Object -Unique
            foreach ($item in $allDirectoryPaths)
            {
                if ([Directory]::Exists($item))
                {
                    Write-Verbose "Directory ""$item"" already exists."
                    continue
                }

                Write-Host "Creating directory ""$item""."
                New-Item -Force -ItemType Directory -Path $item | Out-Null
            }
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

    function Find-SingleFileSystemObject
    {
        [CmdletBinding(PositionalBinding = $false)]
        param
        (
            [Parameter()]
            [string] $RootDirectory,

            [Parameter()]
            [switch] $File,

            [Parameter()]
            [switch] $Directory,

            [Parameter()]
            [switch] $Recurse,

            [Parameter()]
            [string] $FilterWildcard = $null,

            [Parameter()]
            [scriptblock] $FilterScript = $null
        )
        process
        {
            if ([string]::IsNullOrWhiteSpace($RootDirectory))
            {
                throw [ArgumentException]::new('The root directory cannot be blank.', 'RootDirectory')
            }

            if ($File -and $Directory)
            {
                throw [ArgumentException]::new('Invalid combination of parameters: a file system object cannot be both a file and a directory at the same time.')
            }

            [hashtable] $commandParameters = `
            @{
                Force = $true
                Recurse = $Recurse
                File = $File
                Directory = $Directory
                LiteralPath = $RootDirectory
            }

            if (![string]::IsNullOrEmpty($FilterWildcard))
            {
                $commandParameters.Filter = $FilterWildcard
            }

            [scriptblock] $resolvedFilterScript = if ($FilterScript -is [scriptblock]) { $FilterScript } else { { $true } }

            [string[]] $allFoundFilePaths = @(Get-ChildItem @commandParameters | ? $resolvedFilterScript | Select-Object -ExpandProperty FullName)

            if ($allFoundFilePaths.Count -ne 1)
            {
                [string] $criteriaString = "file: $File, directory: $Directory, recursive: $Recurse"
                if (![string]::IsNullOrEmpty($FilterWildcard))
                {
                    $criteriaString += ", filter wildcard: '$FilterWildcard'"
                }
                if ($FilterScript -is [scriptblock])
                {
                    $criteriaString += ", filter script: { $FilterScript }"
                }

                throw "There must be exactly one file matching the specified criteria within ""$RootDirectory"", but found: $($allFoundFilePaths.Count). Criteria: $criteriaString"
            }

            return $allFoundFilePaths[0]
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
            return $(Find-SingleFileSystemObject -RootDirectory $workspaceRootDirectoryPath -Recurse -File -FilterWildcard $Pattern)
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
        Write-Host -ForegroundColor Green "PrereleaseSuffix: ""$PrereleaseSuffix"""
        Write-Host ''
        Write-Host -ForegroundColor Green "AppveyorBuild: $AppveyorBuild"
        if ($AppveyorBuild)
        {
            Write-Host -ForegroundColor Green "AppveyorArtifactsSubdirectory: ""$AppveyorArtifactsSubdirectory"""
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

                [string] $resolvedArchiveFilePath = [Path]::GetFullPath($ArchiveFilePath)

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
                                """$resolvedArchiveFilePath"""
                                '--'
                            ) `
                            + $processedItems
                        )
            }
        }

        [ValidateNotNullOrEmpty()] [string] $solutionFilePath = $solutionFilePattern | Find-SingleFileInWorkspace
        [ValidateNotNullOrEmpty()] [string] $solutionDirectoryPath = [Path]::GetDirectoryName($solutionFilePath)
        [ValidateNotNullOrEmpty()] [string] $solutionNameOnly = [Path]::GetFileNameWithoutExtension($solutionFilePath)

        function Get-ProjectFilePath
        {
            [CmdletBinding(PositionalBinding = $false)]
            param
            (
                [Parameter(Position = 0, ValueFromPipeline = $true)]
                [ValidateNotNullOrEmpty()]
                [string] $ProjectName = $([ArgumentNullException]::new('ProjectName'))
            )
            process
            {
                [string] $result = [Path]::Combine($solutionDirectoryPath, $ProjectName, "$ProjectName.csproj")

                if (![File]::Exists($result))
                {
                    throw [FileNotFoundException]::new("The project file ""$result"" corresponding to the project ""$ProjectName"" is not found.")
                }

                return $result
            }
        }

        function Get-MSBuildPropertyValues
        {
            [CmdletBinding(PositionalBinding = $false)]
            param
            (
                [Parameter()]
                [ValidateNotNullOrEmpty()]
                [string] $ProjectName = $([ArgumentNullException]::new('ProjectName')),

                [Parameter()]
                [ValidateNotNull()]
                [string[]] $Properties = $([ArgumentNullException]::new('Properties'))
            )
            process
            {
                if ($Properties -isnot [object] -or $Properties.Count -eq 0)
                {
                    throw [ArgumentException]::new('No properties are specified.', 'Properties')
                }

                [string] $projectPath = Get-ProjectFilePath -ProjectName $ProjectName

                # Always adding a non-existent property to make the number of properties > 1, which makes CLI to return JSON
                [string] $nonExistentPropertyName = "stub_$([Guid]::NewGuid().ToString('N').Substring(0, 11))"
                [string] $propertiesString = (@($nonExistentPropertyName) + @($Properties)) -join ','

                [string] $commandResult = Execute-DotNetCli `
                    -TitleDetails 'Get properties' `
                    -ProjectPath:$projectPath `
                    -Verbosity quiet `
                    -Command build "--target:""$initialTargets""" "--getProperty:""$propertiesString"""

                #Write-Verbose -Verbose "[Get-MSBuildPropertyValues:RAW]$([Environment]::NewLine)$commandResult"

                [ValidateNotNull()] [psobject] $propertiesContainerObject = `
                    try
                    {
                        ConvertFrom-Json -InputObject $commandResult
                    }
                    catch
                    {
                        [string] $errorDetails = Get-ErrorDetails
                        [string] $newLine = [Environment]::NewLine
                        [string] $outputDetails = "$($newLine)<<<$($newLine)$commandResult$($newLine)>>>"

                        throw "[Get-MSBuildPropertyValues] Cannot deserialize JSON output of the DotNet CLI command:$($outputDetails)$($newLine)$($newLine)* ERROR: $($errorDetails)"
                    }

                #Write-Verbose -Verbose "[Get-MSBuildPropertyValues:JSON]$([Environment]::NewLine)$($propertiesContainerObject | ConvertTo-Json -Compress)"

                [ValidateNotNull()] [psobject] $propertiesObject = $propertiesContainerObject.Properties

                [psobject] $result = [psobject]::new()
                foreach ($property in $Properties)
                {
                    [string] $value = $propertiesObject.$property
                    $result | Add-Member -MemberType NoteProperty -Name $property -Value $value
                }

                #Write-Verbose -Verbose "[Get-MSBuildPropertyValues]$([Environment]::NewLine)$($result | ConvertTo-Json -Compress)"

                return $result
            }
        }

        function Get-SingleMSBuildPropertyValue
        {
            [CmdletBinding(PositionalBinding = $false)]
            param
            (
                [Parameter()]
                [ValidateNotNullOrEmpty()]
                [string] $ProjectName = $([ArgumentNullException]::new('ProjectName')),

                [Parameter()]
                [ValidateNotNullOrEmpty()]
                [string] $Property = $([ArgumentNullException]::new('Property'))
            )
            process
            {
                return $(Get-MSBuildPropertyValues -ProjectName $ProjectName -Properties $Property | Select-Object -ExpandProperty $Property)
            }
        }

        [string] $mainProjectFileName = $solutionNameOnly

        [ValidateNotNullOrEmpty()] [string] $buildPropsFilePath = $buildPropsFilePattern | Find-SingleFileInWorkspace
        [FileXmlData] $buildPropsFileXmlData = [FileXmlData]::new($buildPropsFilePath)

        [ValidateNotNullOrEmpty()] [string] $initialTargets = $buildPropsFileXmlData.GetAttributeValue('/Project/@InitialTargets')

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

        [string] $testProjectSuffix = '.Tests'
        [string[]] $testProjectNames = Get-ChildItem -Recurse:$false -Directory -Name -Path "$solutionDirectoryPath/*$testProjectSuffix"
        if ($testProjectNames.Count -eq 0)
        {
            throw "No test project directories are found in ""$solutionDirectoryPath""."
        }

        [string[]] $testFrameworks = @()
        foreach ($testProjectName in $testProjectNames)
        {
            [string] $testProjectFilePath = Get-ProjectFilePath -ProjectName $testProjectName
            [FileXmlData] $testProjectFileXmlData = [FileXmlData]::new($testProjectFilePath)
            [string] $targetFrameworksString = $testProjectFileXmlData.GetProjectPropertyText('TargetFrameworks')

            $testFrameworks += $targetFrameworksString -csplit ';'
        }

        $testFrameworks = $testFrameworks | Select-Object -Unique
        if ($testFrameworks.Count -eq 0)
        {
            throw "No target frameworks are defined in the test projects: $(($testProjectNames | % { "'$_'" }) -join ', ')."
        }


        Write-Host -ForegroundColor Green  "Discovered test projects: $(($testProjectNames | % { "'$_'" }) -join ', ')."
        Write-Host -ForegroundColor Green  "Discovered frameworks for test run: $(($testFrameworks | % { "'$_'" }) -join ', ')."

        if ($TestFrameworkFilter -is [scriptblock])
        {
            $testFrameworks = $testFrameworks | ? $TestFrameworkFilter
            if ($testFrameworks.Count -eq 0)
            {
                throw [ArgumentException]::new('After applying the filter, there are no test frameworks to run for.', 'TestFrameworkFilter')
            }

            Write-Host -ForegroundColor Green  "Discovered frameworks for test run (after filtering): $(($testFrameworks | % { "'$_'" }) -join ', ')."
        }

        Write-Host ''

        Write-MajorSeparator
        [ValidateNotNullOrEmpty()] [string] $dotNetCliPath = Get-ApplicationPath -Verbose -Name dotnet

        [string] $startupStackName = [Guid]::NewGuid().ToString('N')
        Push-Location -LiteralPath $solutionDirectoryPath -StackName $startupStackName
        try
        {
            Write-MajorSeparator
            Execute-Command -Title '* DotNet CLI Version' -Command $dotNetCliPath -CommandArguments '--version'

            Write-MajorSeparator
            Execute-Command -Title '* DotNet CLI Information' -Command $dotNetCliPath -CommandArguments '--info'

            Write-MajorSeparator
            Execute-Command -Title '* DotNet SDKs' -Command $dotNetCliPath -CommandArguments '--list-sdks'
        }
        finally
        {
            Pop-Location -StackName $startupStackName
        }

        function Create-DotNetCliExecuteCommandParameters
        {
            [CmdletBinding(PositionalBinding = $false)]
            param
            (
                [Parameter()]
                [ValidateNotNullOrEmpty()]
                [string] $ProjectPath = $solutionFilePath,

                [Parameter()]
                [switch] $NoBuildConfiguration,

                [Parameter()]
                [AllowNull()]
                [AllowEmptyString()]
                [string] $TitleDetails = $null,

                [Parameter()]
                [ValidateSet('quiet', 'minimal', 'normal', 'detailed', 'diagnostic')]
                [string] $Verbosity = 'normal',

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
                        """$ProjectPath"""
                        "--verbosity:$Verbosity"
                        "--property:IsAppveyorBuild=$AppveyorBuild"
                        "--property:ContinuousIntegrationBuild=$AppveyorBuild"
                        "--property:Version=""$version"""
                        "--property:FileVersion=""$version.$resolvedBuildNumber"""
                        "--property:InformationalVersion=""$informationalVersion"""
                        "--property:VersionSuffix=""$resolvedPrereleaseSuffix"""
                    )

                if ($AppveyorBuild)
                {
                    if (![string]::IsNullOrWhiteSpace($AppveyorSourceCodeBranch))
                    {
                        $commonCommandArguments += "--property:__X_SourceCodeBranch=""$AppveyorSourceCodeBranch"""
                    }

                    if (![string]::IsNullOrWhiteSpace($AppveyorSourceCodeRevisionId))
                    {
                        $commonCommandArguments += "--property:__X_SourceCodeRevisionId=""$AppveyorSourceCodeRevisionId"""
                    }
                }

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
                [ValidateNotNullOrEmpty()]
                [string] $ProjectPath = $solutionFilePath,

                [Parameter()]
                [switch] $NoBuildConfiguration,

                [Parameter()]
                [AllowNull()]
                [AllowEmptyString()]
                [string] $TitleDetails = $null,

                [Parameter()]
                [ValidateSet('quiet', 'minimal', 'normal', 'detailed', 'diagnostic')]
                [string] $Verbosity = 'normal',

                [Parameter(Position = 0)]
                [string] $Command,

                [Parameter(Position = 1, ValueFromRemainingArguments = $true)]
                [string[]] $CommandArguments = @()
            )
            process
            {
                [hashtable] $executeCommandParameters = Create-DotNetCliExecuteCommandParameters `
                    -ProjectPath:$ProjectPath `
                    -NoBuildConfiguration:$NoBuildConfiguration `
                    -TitleDetails:$TitleDetails `
                    -Verbosity:$Verbosity `
                    -Command:$Command `
                    -CommandArguments:$CommandArguments

                Write-MajorSeparator

                [string] $localStackName = [Guid]::NewGuid().ToString('N')
                Push-Location -LiteralPath $solutionDirectoryPath -StackName $localStackName
                try
                {
                    Execute-Command @executeCommandParameters
                }
                finally
                {
                    Pop-Location -StackName $localStackName
                }
            }
        }

        [psobject] $commonBuildProperties = Get-MSBuildPropertyValues `
            -ProjectName $mainProjectFileName `
            -Properties `
                @(
                    '__X_BaseBinPath'
                    '__X_TestResultsDirectory'
                    '__X_CoverageResultsDirectory'
                    'PackageOutputPath'
                )

        [string] $binariesBaseDirectoryPath = $commonBuildProperties.'__X_BaseBinPath' | Remove-TrailingPathSeparator
        [string] $testOutputDirectoryPath = $commonBuildProperties.'__X_TestResultsDirectory' | Remove-TrailingPathSeparator
        [string] $coverageOutputDirectoryPath = $commonBuildProperties.'__X_CoverageResultsDirectory' | Remove-TrailingPathSeparator
        [string] $nuGetPackageDirectoryPath = $commonBuildProperties.'PackageOutputPath' | Remove-TrailingPathSeparator

        Write-Host -ForegroundColor Green "Binaries base directory: ""$binariesBaseDirectoryPath"""
        Write-Host -ForegroundColor Green "Test output directory: ""$testOutputDirectoryPath"""
        Write-Host -ForegroundColor Green "Coverage output directory: ""$coverageOutputDirectoryPath"""
        Write-Host -ForegroundColor Green "NuGet package directory: ""$nuGetPackageDirectoryPath"""

        Execute-DotNetCli clean
        Execute-DotNetCli -NoBuildConfiguration restore --force --no-cache

        [string[]] $dotNetBuildCommandArguments = `
            @(
                '--no-restore'
                '--no-incremental'
                '--disable-build-servers'
            )

        Execute-DotNetCli build @dotNetBuildCommandArguments

        [string] $archiveVersionSuffix = $null
        if ($AppveyorBuild)
        {
            $archiveVersionSuffix = "-v$($version).$($resolvedBuildNumber)$($PrereleaseSuffix).rev-$($shortRevisionId)"

            [string] $binariesArchiveFilePath = "$resolvedArtifactsDirectoryPath/$($solutionNameOnly).Binaries$($archiveVersionSuffix).zip"

            Write-MajorSeparator

            Execute-SevenZip `
                -Description 'Binaries' `
                -ArchiveFilePath $binariesArchiveFilePath `
                -Items "$binariesBaseDirectoryPath/*.*"

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
                Copy-Item `
                    -Verbose `
                    -Path "$nuGetPackageDirectoryPath/*.*nupkg" `
                    -Destination $resolvedArtifactsDirectoryPath `
                    -Recurse `
                    | Out-Null
            }
        }

        Write-MajorSeparator

        [string] $reportGeneratorExecutableName = 'reportgenerator.exe'
        [string] $reportGeneratorExecutablePath = Get-ApplicationPath -Verbose -Name $reportGeneratorExecutableName -ErrorAction SilentlyContinue
        if ([string]::IsNullOrEmpty($reportGeneratorExecutablePath))
        {
            Write-MajorSeparator

            Execute-Command -Title 'Install ReportGenerator .NET tool' $dotNetCliPath tool install --global dotnet-reportgenerator-globaltool
            $reportGeneratorExecutablePath = Get-ApplicationPath -Verbose -Name $reportGeneratorExecutableName
        }

        [string] $coberturaCoverageFileName = 'coverage.cobertura.xml'

        [string[]] $coverageReportTypes = `
            @(
                'Badges'
                'Cobertura'
                'Markdown'
                'MarkdownSummaryGithub'
                'Html_Light'
                'Html_Dark'
                'TextSummary'
            )

        foreach ($testFramework in $testFrameworks)
        {
            [string[]] $testExecutionCliArguments = `
                @(
                    '--no-build'
                    '--logger:trx'
                    '--logger:html'
                    '--logger:console'
                    "--framework:""$testFramework"""
                    '--collect:"XPlat Code Coverage;Format=cobertura"'
                )

            if ($AppveyorBuild)
            {
                $testExecutionCliArguments += @('--logger:Appveyor')
            }

            $testExecutionCliArguments += `
                @(
                    '--',
                    "NUnit.DefaultTestNamePattern=""[$testFramework]{m}{a}#{i}"""
                )

            Write-MajorSeparator
            Execute-DotNetCli -TitleDetails $testFramework -Command test @testExecutionCliArguments

            [string] $testOutputDestinationBase = [Path]::Combine($testOutputDirectoryPath, $testFramework)
            foreach ($testProjectName in $testProjectNames)
            {
                [string] $testResultsDirectory = Get-SingleMSBuildPropertyValue -ProjectName $testProjectName -Property 'VSTestResultsDirectory' | Remove-TrailingPathSeparator
                if (![Directory]::Exists($testResultsDirectory))
                {
                    Write-Warning -WarningAction Continue "Not collecting test results of '$testProjectName' for '$testFramework': directory ""$testResultsDirectory"" is not found."
                    continue
                }

                [string[]] $fileList = @()
                $fileList += Find-SingleFileSystemObject -RootDirectory $testResultsDirectory -File -FilterWildcard '*.trx'
                $fileList += Find-SingleFileSystemObject -RootDirectory $testResultsDirectory -File -FilterWildcard '*.html'

                [string] $coberturaDirectory = Find-SingleFileSystemObject `
                    -RootDirectory $testResultsDirectory `
                    -Directory `
                    -FilterScript { [guid] $g = [guid]::Empty; [Guid]::TryParse($_.Name, [ref] $g) }

                $fileList += Find-SingleFileSystemObject -RootDirectory $coberturaDirectory -File -FilterWildcard $coberturaCoverageFileName

                [string] $testOutputDestination = [Path]::Combine($testOutputDestinationBase, $testProjectName)
                Write-Host "Copying test artifacts to ""$testOutputDestination""."
                Ensure-Directories -DirectoryPaths $testOutputDestination
                Copy-Item -LiteralPath $fileList -Destination $testOutputDestination
                Remove-FileSystemObjectForced -LiteralPath $testResultsDirectory

                Write-MajorSeparator

                foreach ($coverageReportType in $coverageReportTypes)
                {
                    [string[]] $commandArguments = `
                        @(
                            """-reports:$testOutputDestinationBase/**/$coberturaCoverageFileName"""
                            """-targetdir:$coverageOutputDirectoryPath/$testFramework/$coverageReportType"""
                            """-reporttypes:$coverageReportType"""
                        )

                    Execute-Command `
                        -Title "Create Coverage report ($testFramework|$coverageReportType)" `
                        -Command $reportGeneratorExecutablePath `
                        -CommandArguments $commandArguments
                }
            }
        }

        Write-MajorSeparator

        if ($AppveyorBuild)
        {
            [string] $testResultsSubdirectory = [Path]::GetFileName($testOutputDirectoryPath)
            [string] $testResultsArchiveFilePath = "$resolvedArtifactsDirectoryPath/$($solutionNameOnly).$($testResultsSubdirectory)$($archiveVersionSuffix).zip"

            Execute-SevenZip `
                -Description 'Test Results' `
                -ArchiveFilePath $testResultsArchiveFilePath `
                -Items "$testOutputDirectoryPath/*.*"

            Write-MajorSeparator

            [string] $coverageResultsSubdirectory = [Path]::GetFileName($coverageOutputDirectoryPath)
            [string] $coverageResultsArchiveFilePath = "$resolvedArtifactsDirectoryPath/$($solutionNameOnly).$($coverageResultsSubdirectory)$($archiveVersionSuffix).zip"

            Execute-SevenZip `
                -Description 'Coverage Results' `
                -ArchiveFilePath $coverageResultsArchiveFilePath `
                -Items "$coverageOutputDirectoryPath/*.*"

            Write-MajorSeparator
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