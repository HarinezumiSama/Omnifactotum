#Requires -Version 5

using namespace System
using namespace System.Diagnostics
using namespace System.IO
using namespace System.Management.Automation
using namespace System.Reflection
using namespace System.Xml

[CmdletBinding(PositionalBinding = $false)]
param (
    [Parameter()]
    [string] $SolutionFile = 'src\Omnifactotum.sln',

    [Parameter()]
    [string] $ProjectFile = 'src\Omnifactotum\Omnifactotum.csproj',

    [Parameter()]
    [string] $ReleaseNotesFile = 'src\Omnifactotum.ReleaseNotes.txt',

    [Parameter()]
    [string] $OutDir = 'bin\AnyCpu\Release',

    [Parameter()]
    [string] $PackageDir = 'bin\NuGet',

    [Parameter()]
    [string] $MSBuildVersion = '15.0',

    [Parameter()]
    [string] $BuildConfiguration = 'Release',

    [Parameter()]
    [string] $BuildPlatform = 'Any CPU',

    [Parameter()]
    [string] $ProjectPlatform = 'AnyCPU',

    [Parameter()]
    [string] $NUnitConsolePath = "$($Env:ProgramFiles)\NUnit\console\stable\nunit3-console.exe",

    [Parameter()]
    [string] $NuGetPath = 'nuget.exe'
)
BEGIN
{
    $Script:ErrorActionPreference = [System.Management.Automation.ActionPreference]::Stop
    Set-StrictMode -Version 1

    function Get-MsBuildExecutablePath
    {
        [CmdletBinding(PositionalBinding = $false)]
        param (
            [Parameter(Mandatory = $false, ValueFromPipeline = $true, Position = 0)]
            [string] $Version
        )
        BEGIN
        {
            function Private:Get-MsBuildExecutablePathInternal([string] $Version)
            {
                [version] $msBuildVersion = $null
                if (![version]::TryParse($Version, [ref] $msBuildVersion) -or $msBuildVersion.Build -ge 0 -or $msBuildVersion.Revision -ge 0)
                {
                    throw "The specified MSBuild version is invalid: '$Version'. Must be in the format 'Major.Minor' (for example '14.0')."
                }

                [string] $programFilesX86Path = if ([Environment]::Is64BitOperatingSystem)
                {
                    ${env:ProgramFiles(x86)}
                }
                else
                {
                    ${env:ProgramFiles}
                }

                [version] $minSupportedVersion = [version]::new(12, 0)

                if ($msBuildVersion -lt $minSupportedVersion)
                {
                    throw "The version of MSBuild prior to $minSupportedVersion is not supported."
                }

                if ($msBuildVersion.Major -lt 15)
                {
                    return [Path]::Combine($programFilesX86Path, "MSBuild", $msBuildVersion, "Bin\MSBuild.exe")
                }

                [string[]] $subdirs = @('BuildTools', 'Enterprise', 'Professional', 'Community')
                foreach ($subdir in $subdirs)
                {
                    [string] $path = [Path]::Combine(
                        $programFilesX86Path,
                        'Microsoft Visual Studio\2017',
                        $subdir,
                        'MSBuild',
                        $msBuildVersion.ToString(),
                        'Bin\MSBuild.exe')

                    if ([File]::Exists($path))
                    {
                        return $path
                    }
                }

                throw "The executable of MSBuild version '$msBuildVersion' is not found."
            }
        }
        PROCESS
        {
            [ValidateNotNullOrEmpty()] [string] $msBuildFullPath = Get-MsBuildExecutablePathInternal $Version

            if (![File]::Exists($msBuildFullPath))
            {
                throw "MSBuild version '$Version' is not found at ""$msBuildFullPath""."
            }

            Write-Verbose "MSBuild $Version has been resolved to ""$msBuildFullPath""."

            return $msBuildFullPath
        }
    }

    function Get-ApplicationPath
    {
        [CmdletBinding(PositionalBinding = $false)]
        param (
            [Parameter(ValueFromPipeline = $true, Position = 0)]
            [string] $Name
        )
        PROCESS
        {
            if ([string]::IsNullOrWhiteSpace($Name))
            {
                throw [ArgumentException]::new("The application name must be specified.", 'Name')
            }

            [string[]] $paths = (Get-Command $Name -CommandType Application -ErrorAction SilentlyContinue).Definition
            [string] $path = if ($paths -ne $null -and $paths.Count -ne 0) { $paths[0] } else { $null }
            if ([string]::IsNullOrEmpty($path))
            {
                if ($ErrorActionPreference -ne [ActionPreference]::SilentlyContinue)
                {
                    throw "The application ""$Name"" is not found."
                }

                return $null
            }

            Write-Verbose "Application ""$Name"" has been resolved to ""$path""."

            return $path
        }
    }

    function Delete-ItemIfExists
    {
        [CmdletBinding(PositionalBinding = $false)]
        param (
            [Parameter(ValueFromPipeline = $true, Position = 0)]
            [string] $LiteralPath,

            [Parameter(ValueFromPipeline = $false)]
            [switch] $FileOnly,

            [Parameter(ValueFromPipeline = $false)]
            [switch] $DirectoryOnly

        )
        PROCESS
        {
            if ([string]::IsNullOrWhiteSpace($LiteralPath))
            {
                throw [ArgumentException]::new("The path must be specified.", 'LiteralPath')
            }
            if ($FileOnly -and $DirectoryOnly)
            {
                throw [ArgumentException]::new("Invalid combination of enforcing options.")
            }

            if ([File]::Exists($LiteralPath))
            {
                if ($DirectoryOnly)
                {
                    throw "Cannot delete directory ""$LiteralPath"" since it is file."
                }

                Write-Host "Deleting file ""$LiteralPath""."
                Remove-Item -LiteralPath $LiteralPath -Force | Out-Null
            }
            elseif ([Directory]::Exists($LiteralPath))
            {
                if ($FileOnly)
                {
                    throw "Cannot delete file ""$LiteralPath"" since it is directory."
                }

                Write-Host "Deleting directory ""$LiteralPath""."
                Remove-Item -LiteralPath $LiteralPath -Recurse -Force | Out-Null
            }
        }
    }

    function Execute-Command
    {
        [CmdletBinding(PositionalBinding = $false)]
        param (
            [Parameter(Mandatory = $false)]
            [string] $Title,

            [Parameter(Mandatory = $false)]
            [string] $Command,

            [Parameter(Mandatory = $false, ValueFromRemainingArguments = $true)]
            [string[]] $CommandArguments = @()
        )

        if ([string]::IsNullOrWhiteSpace($Title))
        {
            throw [ArgumentException]::new("The command title must be specified.", 'Title')
        }
        if ([string]::IsNullOrWhiteSpace($Command))
        {
            throw [ArgumentException]::new("The command must be specified.", 'Command')
        }
        if ($CommandArguments -eq $null)
        {
            throw [ArgumentNullException]::new('CommandArguments')
        }

        Write-Host
        Write-Host "$($Title)..."

        Write-Verbose "Executing <""$Command"" $CommandArguments>"

        [Stopwatch] $stopwatch = [Stopwatch]::StartNew()

        $ErrorActionPreference = [System.Management.Automation.ActionPreference]::SilentlyContinue
        & cmd /c """$Command"" $CommandArguments" 2`>`&1
        [int] $exitCode = $LASTEXITCODE
        $ErrorActionPreference = [System.Management.Automation.ActionPreference]::Stop
        $stopwatch.Stop()

        [bool] $isSuccessful = $exitCode -eq 0
        if (!$isSuccessful)
        {
            throw "$($Title) - FAILED (exit code: $($exitCode), time elapsed: $($stopwatch.Elapsed))."
        }

        Write-Host "$($Title) - DONE (exit code: $($exitCode), time elapsed: $($stopwatch.Elapsed))."
    }

    function Get-BinaryPath
    {
        [CmdletBinding(PositionalBinding = $false)]
        param (
            [Parameter(Position = 0, ValueFromPipeline = $true)]
            [string] $ProjectFilePath,

            [Parameter()]
            [string] $Configuration,

            [Parameter()]
            [string] $Platform
        )
        PROCESS
        {
            [xml] $projectFile = [xml](Get-Content -LiteralPath $ProjectFilePath -Raw)

            [string] $assemblyName = (@($projectFile.Project.PropertyGroup) | ? { $_.AssemblyName } ).AssemblyName
            if ([string]::IsNullOrWhiteSpace($assemblyName))
            {
                throw "Unable to determine 'AssemblyName' in the project file ""$ProjectFilePath""."
            }

            [string] $assemblyOutputType = (@($projectFile.Project.PropertyGroup) | ? { $_.OutputType } ).OutputType
            if ($assemblyOutputType -ine 'Library')
            {
                throw "Unsupported 'OutputType' in the project file ""$ProjectFilePath"": $($assemblyOutputType)."
            }

            [string] $outputFileName = "$assemblyName.dll"

            [string] $filter = "*== '$($Configuration)|$($Platform)'*"
            [XmlElement[]] $specificPropertyGroups = @($projectFile.Project.PropertyGroup) | ? { $_.Condition -ilike $filter }
            if ($specificPropertyGroups.Count -ne 1)
            {
                throw "Configuration/platform specific element is not found in the project file ""$ProjectFilePath""."
            }
            [XmlElement] $specificPropertyGroup = $specificPropertyGroups[0]

            [string] $outputDir = $specificPropertyGroup.OutputPath
            if ([string]::IsNullOrWhiteSpace($outputDir))
            {
                throw "Unable to determine the output directory in the project file ""$ProjectFilePath""."
            }

            [string] $projectDir = [Path]::GetDirectoryName($ProjectFilePath)
            [string] $outputFilePath = [Path]::GetFullPath([Path]::Combine($projectDir, $outputDir, $outputFileName))
            if (![File]::Exists($outputFilePath))
            {
                throw [FileNotFoundException]::new("The output assembly ""$outputFilePath"" is not found.", $outputFilePath)
            }

            return $outputFilePath
        }
    }

    [ValidateNotNullOrEmpty()] [string] $root = $PSScriptRoot
}
PROCESS
{
    Write-Host
    Write-Host "Building NuGet package..." -ForegroundColor Green

    [string] $msBuildFullPath = Get-MsBuildExecutablePath -Version $MSBuildVersion
    [string] $nuGetFullPath = Get-ApplicationPath -Name $NuGetPath
    [string] $nunitConsoleFullPath = Get-ApplicationPath -Name $NUnitConsolePath

    [string] $nuGetConfigFileName = '.nuget\Nuget.config'
    [string] $nuGetConfigFilePath = [Path]::Combine($root, $nuGetConfigFileName)

    [string] $solutionFilePath = [Path]::Combine($root, $SolutionFile)
    [string] $projectFilePath = [Path]::Combine($root, $ProjectFile)
    [string] $nuspecFilePath = [Path]::ChangeExtension($projectFilePath, '.nuspec')
    [string] $nuspecTemplateFilePath = $nuspecFilePath + '.template'
    [string] $releaseNotesFilePath = [Path]::Combine($root, $ReleaseNotesFile)

    [string] $outDirPath = [Path]::Combine($root, $OutDir)
    [string] $packageDirPath = [Path]::Combine($root, $PackageDir)

    [bool] $isSigningEnabled = $env:SIGN_OMNIFACTOTUM -eq '1'

    @($outDirPath, $packageDirPath) | Delete-ItemIfExists

    [string[]] $nugetRestoreArguments = `
    @(
        "restore",
        """$solutionFilePath""",
        "-NoCache",
        "-NonInteractive"
    )

    if ([File]::Exists($nuGetConfigFilePath))
    {
        $nugetRestoreArguments += @("-ConfigFile", """$nuGetConfigFilePath""")
    }

    Execute-Command -Title "Restoring packages for ""$solutionFilePath""" -Command $nuGetFullPath -CommandArguments $nugetRestoreArguments

    [string[]] $buildArguments = `
    @(
        """$solutionFilePath""",
        "/target:Rebuild",
        "/p:Configuration=""$BuildConfiguration""",
        "/p:Platform=""$BuildPlatform"""
    )

    Execute-Command -Title "Building ""$solutionFilePath""" -Command $msBuildFullPath -CommandArguments $buildArguments

    [string] $testDllPath = [Path]::Combine($outDirPath, "Tests\Omnifactotum.Tests.dll")
    [string] $workingDirectory = [Path]::GetDirectoryName($testDllPath)
    [string] $resultFilePath = [Path]::ChangeExtension($testDllPath, "TestResult.xml")

    [string[]] $nunitArguments = `
    @(
        $testDllPath,
        "--work=""$workingDirectory""",
        "--result=""$resultFilePath""",
        "--labels=All",
        "--stoponerror",
        "--inprocess",
        "--dispose-runners"
    )

    Execute-Command -Title "Running automated tests via NUnit" -Command $nunitConsoleFullPath -CommandArguments $nunitArguments

    if (!$isSigningEnabled)
    {
        throw "ERROR: The assembly signing is not enabled. The NuGet package will not be created."
    }

    New-Item -Path $packageDirPath -ItemType Directory -Force | Out-Null

    $nuspecFilePath | Delete-ItemIfExists -FileOnly

    [string] $outputFilePath = Get-BinaryPath -ProjectFilePath $projectFilePath -Configuration $BuildConfiguration -Platform $ProjectPlatform
    [Assembly] $outputAssembly = [Assembly]::LoadFrom($outputFilePath)
    [version] $assemblyVersion = $outputAssembly.GetName().Version

    [string] $assemblyCompany = (@($outputAssembly.GetCustomAttributes([AssemblyCompanyAttribute], $false))[0]).Company
    [string] $assemblyDescription = (@($outputAssembly.GetCustomAttributes([AssemblyDescriptionAttribute], $false))[0]).Description
    [string] $assemblyCopyright = (@($outputAssembly.GetCustomAttributes([AssemblyCopyrightAttribute], $false))[0]).Copyright

    [string] $releaseNotes = [File]::ReadAllText($releaseNotesFilePath)

    [xml] $nuspecContent = [xml](Get-Content -LiteralPath $nuspecTemplateFilePath -Raw)
    [XmlElement] $metadata = $nuspecContent.package.metadata
    $metadata.version = $assemblyVersion.ToString()
    $metadata.authors = $assemblyCompany
    $metadata.owners = $assemblyCompany
    $metadata.description = $assemblyDescription
    $metadata.releaseNotes = $releaseNotes
    $metadata.copyright = $assemblyCopyright

    $nuspecContent.Save($nuspecFilePath)
    try
    {
        [string[]] $nugetPackArguments = `
        @(
            "pack",
            """$projectFilePath""",
            "-Verbosity",
            "detailed",
            "-OutputDirectory",
            """$packageDirPath""",
            "-Symbols",
            "-Properties",
            "Configuration=""$BuildConfiguration"";Platform=""$ProjectPlatform"""
        )

        Execute-Command -Title "Packaging ""$projectFilePath""" -Command $nuGetFullPath -CommandArguments $nugetPackArguments
    }
    finally
    {
        $nuspecFilePath | Delete-ItemIfExists -FileOnly
    }

    Write-Host "Building NuGet package - COMPLETED." -ForegroundColor Green
}