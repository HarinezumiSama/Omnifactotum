@echo off

verify invalid params >nul
setlocal enableextensions enabledelayedexpansion
if errorlevel 1 goto EXT_ERROR

set EnableNuGetPackageRestore=true

set SRC_PROJECT_PATH=%~dp0\src\Omnifactotum\Omnifactotum.csproj
set REL_NOTES_PATH=%~dp0\src\Omnifactotum.ReleaseNotes.txt
set NG_PROJECT_PATH=%SRC_PROJECT_PATH%
set PKG_PATH=%~dp0\bin\NuGet

(set LF=^

)

for /f %%A in ('copy /Z "%~dpf0" nul') do set "CR=%%A"

set PkgReleaseNotes=
if exist "!REL_NOTES_PATH!" (
    for /f "usebackq Tokens=* Delims=" %%A in ("!REL_NOTES_PATH!") do set PkgReleaseNotes=!PkgReleaseNotes!!CR!!LF!%%A
)

::echo !PkgReleaseNotes!
::goto :EOF

echo Building project...
"%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" "%SRC_PROJECT_PATH%" /target:Rebuild /p:Configuration=Release /p:Platform=AnyCPU || exit /b 1
echo Building project - DONE.

echo.
echo (Re)creating directory for the package '%PKG_PATH%'...
if exist "%PKG_PATH%" (
    rd /s /q "%PKG_PATH%" || exit /b 2
)
md "%PKG_PATH%" || exit /b 3
echo (Re)creating directory for the package - DONE.

:: ----------

echo.
echo Creating package...
:: [vitalii.maklai] The backslash (\) before the double quote (") must be escaped with itself (\) for the command line parameters to be parsed properly
nuget pack "%NG_PROJECT_PATH%" -Verbosity detailed -OutputDirectory "%PKG_PATH%\\" -Symbols -Properties Configuration=Release;Platform=AnyCPU;PkgReleaseNotes="!PkgReleaseNotes!" || exit /b 4
echo Creating package - DONE

echo.
echo * FINISHED.
goto :EOF

:: ----------------------------------------------------------------------------------------------------

:EXT_ERROR
echo.
echo * ERROR: Unable to turn on Command Shell extensions.
goto ERROR

:: ----------------------------------------------------------------------------------------------------

:ERROR
echo.
echo. *** ERROR ***
if /i "%~1" equ "" (
    pause
)
exit /b 100
