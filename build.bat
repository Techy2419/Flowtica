@echo off
echo Building Flowtica Desktop Application...
cd FlowticaDesktop
dotnet build --configuration Release
if %ERRORLEVEL% neq 0 (
    echo Build failed!
    pause
    exit /b 1
)
echo Desktop app built successfully!

echo.
echo Building React Dashboard...
cd ..\flowtica-dashboard
npm install
if %ERRORLEVEL% neq 0 (
    echo npm install failed!
    pause
    exit /b 1
)
npm run build
if %ERRORLEVEL% neq 0 (
    echo React build failed!
    pause
    exit /b 1
)
echo Dashboard built successfully!

echo.
echo All builds completed successfully!
pause
