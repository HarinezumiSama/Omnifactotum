#Requires -Version 5

using assembly Microsoft.Build.Tasks.v4.0

using namespace System
using namespace System.Diagnostics
using namespace System.IO
using namespace System.Management.Automation
using namespace System.Reflection
using namespace System.Security.Cryptography
using namespace System.Security.Cryptography.X509Certificates
using namespace System.Text.RegularExpressions
using namespace System.Xml
using namespace Microsoft.Build.Tasks

[CmdletBinding(PositionalBinding = $false)]
param
(
    [Parameter()]
    [string] $SolutionFile = 'src\Omnifactotum.sln',

    [Parameter()]
    [string] $BuildConfiguration = 'Release',

    [Parameter()]
    [string] $BuildPlatform = 'Any CPU'
)
begin
{
    $Script:ErrorActionPreference = [System.Management.Automation.ActionPreference]::Stop
    Microsoft.PowerShell.Core\Set-StrictMode -Version 1

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
            [Parameter(ValueFromPipeline = $true, Position = 0)]
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
                [string] $message = "The application ""$Name"" is not found."
                if ($ErrorActionPreference -notin @([ActionPreference]::SilentlyContinue, [ActionPreference]::Ignore))
                {
                    throw $message
                }

                Write-Verbose $message
                return $null
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

            [Parameter()]
            [string] $Command,

            [Parameter(ValueFromRemainingArguments = $true)]
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

        Write-Host
        Write-Host "$($Title)..."

        Write-Verbose "Executing <""$Command"" $CommandArguments>"

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

        Write-Host "$($Title) - DONE (exit code: $($exitCode), time elapsed: $($stopwatch.Elapsed))."
    }

    [ValidateNotNullOrEmpty()] [string] $root = $PSScriptRoot
}
process
{
    try
    {
        Write-MajorSeparator

        if ([string]::IsNullOrWhiteSpace($SolutionFile))
        {
            throw [ArgumentException]::new('The relative path to the solution file cannot be blank.', 'SolutionFile')
        }
        if ([string]::IsNullOrWhiteSpace($BuildConfiguration))
        {
            throw [ArgumentException]::new('The build configuration cannot be blank.', 'BuildConfiguration')
        }
        if ([string]::IsNullOrWhiteSpace($BuildPlatform))
        {
            throw [ArgumentException]::new('The build platform cannot be blank.', 'BuildPlatform')
        }

        [string] $dotNetCliPath = Get-ApplicationPath -Verbose -Name dotnet
        [string] $solutionFilePath = [Path]::Combine($root, $SolutionFile)

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
                        '--verbosity'
                        'normal' # quiet, minimal, normal, detailed, and diagnostic
                    )

                if (!$NoBuildConfiguration)
                {
                    $commonCommandArguments += `
                        @(
                            '--configuration'
                            """$BuildConfiguration"""
                        )
                }

                Write-MajorSeparator

                Execute-Command `
                    -Title "DotNet CLI: $Command" `
                    -Command $dotNetCliPath `
                    -CommandArguments (@($Command) + $commonCommandArguments + $CommandArguments)
            }
        }

        Execute-DotNetCli clean
        Execute-DotNetCli -NoBuildConfiguration restore --force --no-cache
        Execute-DotNetCli build --no-incremental --no-restore "-p:Platform=""$BuildPlatform"""
        Execute-DotNetCli test --no-build --logger trx --logger html --logger console

        Write-MajorSeparator
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
}