@echo off
echo 🚀 Flowtica - Build and Deploy Script
echo =====================================

echo.
echo 📦 Building React Dashboard...
cd flowtica-dashboard
call npm install
call npm run build
if %errorlevel% neq 0 (
    echo ❌ Build failed!
    pause
    exit /b 1
)

echo ✅ Build successful!
echo.
echo 📁 Build output created in: flowtica-dashboard\build\
echo.
echo 🌐 Next steps:
echo 1. Go to https://dash.cloudflare.com/pages
echo 2. Create new project from GitHub: Techy2419/Flowtica
echo 3. Use these settings:
echo    - Framework: Create React App
echo    - Build command: npm run build
echo    - Output directory: build
echo    - Root directory: flowtica-dashboard
echo 4. Add environment variables:
echo    - REACT_APP_SUPABASE_URL=https://tvupomcmuqzmlevmpcnx.supabase.co
echo    - REACT_APP_SUPABASE_ANON_KEY=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InR2dXBvbWNtdXF6bWxldm1wY254Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTcyMDg1MzMsImV4cCI6MjA3Mjc4NDUzM30.wz2Ztmz7XBa986S1r6oLkQeznqzY9d0enngzBLrXajg
echo 5. Click Deploy!
echo.
echo 🎯 Your app will be available at: https://flowtica-dashboard.pages.dev
echo.
pause
