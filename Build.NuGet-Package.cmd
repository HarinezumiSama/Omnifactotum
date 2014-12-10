@echo off

:: TODO: Convert this script to a PowerShell script

setlocal

verify invalid params >nul
setlocal enableextensions enabledelayedexpansion
if errorlevel 1 goto EXT_ERROR

set EnableNuGetPackageRestore=true

set SRC_PROJECT_PATH=%~dp0\src\Omnifactotum\Omnifactotum.csproj
set SRC_SOLUTION_PATH=%~dp0\src\Omnifactotum.sln
set REL_NOTES_PATH=%~dp0\src\Omnifactotum.ReleaseNotes.txt
set NG_PROJECT_PATH=%SRC_PROJECT_PATH%
set OUT_DIR=%~dp0\bin\AnyCpu\Release
set PKG_PATH=%~dp0\bin\NuGet

if "%NUnitBinDir%" equ "" set NUnitBinDir=C:\Program Files (x86)\NUnit\bin
set NUNIT_CONSOLE=%NUnitBinDir%\nunit-console.exe

if not exist "%NUNIT_CONSOLE%" (echo * ERROR: NUnit Console executable is not found at "%NUNIT_CONSOLE%". ^(Is the NUnitBinDir environment variable properly set^?^) & goto ERROR)

(set LF=^

)

for /f %%A in ('copy /Z "%~dpf0" nul') do set "CR=%%A"

set PkgReleaseNotes=
if exist "!REL_NOTES_PATH!" (
    for /f "usebackq Tokens=* Delims=" %%A in ("!REL_NOTES_PATH!") do set PkgReleaseNotes=!PkgReleaseNotes!!CR!!LF!%%A
)

::set PkgReleaseNotes=!PkgReleaseNotes:^<=^&lt^;!
::set PkgReleaseNotes=!PkgReleaseNotes:^>=^&gt^;!

::echo !PkgReleaseNotes!
::goto :EOF

echo * Cleaning the output directory...
if exist "%OUT_DIR%" (
    rd /s /q "%OUT_DIR%" || goto ERROR
)
echo * Cleaning the output directory - DONE.

echo.
echo * Building project...
"%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" "%SRC_SOLUTION_PATH%" /target:Rebuild /p:Configuration="Release" /p:Platform="Any CPU" || goto ERROR
echo * Building project - DONE.

echo.
echo * Running automated tests...
"%NUNIT_CONSOLE%" "%OUT_DIR%\Tests\Omnifactotum.Tests.dll" /basepath="%OUT_DIR%\Tests" /work="%OUT_DIR%\Tests" /labels /stoponerror /noshadow || goto ERROR
echo * Running automated tests - DONE.


if "%SIGN_OMNIFACTOTUM%" neq "1" (
    echo.
    echo *** ERROR: The assembly signing is not enabled. The NuGet package will not be created.
    goto ERROR
)

echo.
echo * (Re)creating directory for the package '%PKG_PATH%'...
if exist "%PKG_PATH%" (
    rd /s /q "%PKG_PATH%" || goto ERROR
)
md "%PKG_PATH%" || goto ERROR
echo * (Re)creating directory for the package - DONE.

:: ----------

echo.
echo * Creating package...
:: [vitalii.maklai] The backslash (\) before the double quote (") must be escaped with itself (\) for the command line parameters to be parsed properly
nuget pack "%NG_PROJECT_PATH%" -Verbosity detailed -OutputDirectory "%PKG_PATH%\\" -Symbols -Properties Configuration=Release;Platform=AnyCPU;PkgReleaseNotes="!PkgReleaseNotes!" || goto ERROR
echo * Creating package - DONE.

echo.
echo * FINISHED.
goto :EOF

:: ----------------------------------------------------------------------------------------------------

:EXT_ERROR
echo.
echo *** ERROR: Unable to turn on Command Shell extensions.
goto ERROR

:: ----------------------------------------------------------------------------------------------------

:ERROR
echo.
echo. *** ERROR ***
if /i "%~1" equ "" (
    pause
)
exit /b 1
