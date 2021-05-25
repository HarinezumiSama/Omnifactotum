@echo off

setlocal
set TAG=%~nx0

set PS_ExePath=%SystemRoot%\System32\WindowsPowerShell\v1.0\powershell.exe

if not exist "%PS_ExePath%" (
    echo [%TAG%] ERROR: PowerShell executable is not found at "%PS_ExePath%".
    exit /b 127
)

"%PS_ExePath%" -ExecutionPolicy RemoteSigned -NonInteractive -Command "%~dpn0.ps1" %*
exit /b %ERRORLEVEL%
