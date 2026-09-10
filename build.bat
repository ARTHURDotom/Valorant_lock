@echo off
echo Building Valorant Auto Lock...

dotnet publish ValorantAutoLock.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishReadyToRun=true -p:EnableCompressionInSingleFile=true -p:DebugType=none -p:DebugSymbols=false -o publish

if %errorlevel% neq 0 (
    echo Build failed!
    exit /b %errorlevel%
)

echo.
echo Build successful!
echo Output: publish\ValorantAutoLock.exe
echo.
echo You can now run publish\ValorantAutoLock.exe
pause