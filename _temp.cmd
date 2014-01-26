@echo off

msbuild Omnifactotum.csproj /target:Rebuild /p:Configuration=Release || exit /b 1
md ..\..\bin\NuGet || exit /b 2
nuget pack Omnifactotum.csproj -OutputDirectory ..\..\bin\NuGet\ -Properties Configuration=Release -Exclude *\*.txt -Verbosity detailed || exit /b 3