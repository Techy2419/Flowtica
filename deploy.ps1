# Flowtica - Build and Deploy Script
Write-Host "🚀 Flowtica - Build and Deploy Script" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan

Write-Host ""
Write-Host "📦 Building React Dashboard..." -ForegroundColor Yellow

# Navigate to dashboard folder
Set-Location "flowtica-dashboard"

# Install dependencies
Write-Host "Installing dependencies..." -ForegroundColor Green
npm install

# Build the project
Write-Host "Building for production..." -ForegroundColor Green
npm run build

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed!" -ForegroundColor Red
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host "✅ Build successful!" -ForegroundColor Green
Write-Host ""
Write-Host "📁 Build output created in: flowtica-dashboard\build\" -ForegroundColor Cyan
Write-Host ""
Write-Host "🌐 Next steps:" -ForegroundColor Yellow
Write-Host "1. Go to https://dash.cloudflare.com/pages" -ForegroundColor White
Write-Host "2. Create new project from GitHub: Techy2419/Flowtica" -ForegroundColor White
Write-Host "3. Use these settings:" -ForegroundColor White
Write-Host "   - Framework: Create React App" -ForegroundColor Gray
Write-Host "   - Build command: npm run build" -ForegroundColor Gray
Write-Host "   - Output directory: build" -ForegroundColor Gray
Write-Host "   - Root directory: flowtica-dashboard" -ForegroundColor Gray
Write-Host "4. Add environment variables:" -ForegroundColor White
Write-Host "   - REACT_APP_SUPABASE_URL=https://tvupomcmuqzmlevmpcnx.supabase.co" -ForegroundColor Gray
Write-Host "   - REACT_APP_SUPABASE_ANON_KEY=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InR2dXBvbWNtdXF6bWxldm1wY254Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTcyMDg1MzMsImV4cCI6MjA3Mjc4NDUzM30.wz2Ztmz7XBa986S1r6oLkQeznqzY9d0enngzBLrXajg" -ForegroundColor Gray
Write-Host "5. Click Deploy!" -ForegroundColor White
Write-Host ""
Write-Host "🎯 Your app will be available at: https://flowtica-dashboard.pages.dev" -ForegroundColor Green
Write-Host ""
Read-Host "Press Enter to exit"
