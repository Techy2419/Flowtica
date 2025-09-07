# 🚀 Flowtica - Build & Deploy Commands

## 📋 Cloudflare Pages Configuration

### **Build Settings:**
- **Framework preset**: `Create React App`
- **Build command**: `npm run build`
- **Build output directory**: `build`
- **Root directory**: `flowtica-dashboard`

### **Environment Variables:**
```
REACT_APP_SUPABASE_URL=https://tvupomcmuqzmlevmpcnx.supabase.co
REACT_APP_SUPABASE_ANON_KEY=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InR2dXBvbWNtdXF6bWxldm1wY254Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTcyMDg1MzMsImV4cCI6MjA3Mjc4NDUzM30.wz2Ztmz7XBa986S1r6oLkQeznqzY9d0enngzBLrXajg
```

## 🖥️ Local Build Commands

### **Test React Dashboard Locally:**
```bash
# Navigate to dashboard folder
cd flowtica-dashboard

# Install dependencies
npm install

# Start development server
npm start
```

### **Build React Dashboard for Production:**
```bash
# Navigate to dashboard folder
cd flowtica-dashboard

# Install dependencies (if not already done)
npm install

# Build for production
npm run build

# Test the build locally
npx serve -s build -l 3000
```

### **Build Desktop App:**
```bash
# Navigate to desktop folder
cd FlowticaDesktop

# Build the desktop application
dotnet build

# Run the desktop application
dotnet run
```

## 🌐 Cloudflare Pages Deploy Commands

### **Option 1: Automatic Deploy (Recommended)**
1. Go to [dash.cloudflare.com](https://dash.cloudflare.com) → Pages
2. Click "Create a project"
3. Connect GitHub repository: `Techy2419/Flowtica`
4. Use the build settings above
5. Click "Save and Deploy"

### **Option 2: Manual Deploy with Wrangler CLI**
```bash
# Install Wrangler CLI
npm install -g wrangler

# Login to Cloudflare
wrangler login

# Navigate to dashboard folder
cd flowtica-dashboard

# Build the project
npm run build

# Deploy to Cloudflare Pages
wrangler pages deploy build --project-name=flowtica-dashboard
```

### **Option 3: Deploy from GitHub Actions**
Create `.github/workflows/deploy.yml`:
```yaml
name: Deploy to Cloudflare Pages

on:
  push:
    branches: [ master ]
  pull_request:
    branches: [ master ]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: '18'
          
      - name: Install dependencies
        run: |
          cd flowtica-dashboard
          npm install
          
      - name: Build
        run: |
          cd flowtica-dashboard
          npm run build
          
      - name: Deploy to Cloudflare Pages
        uses: cloudflare/pages-action@v1
        with:
          apiToken: ${{ secrets.CLOUDFLARE_API_TOKEN }}
          accountId: ${{ secrets.CLOUDFLARE_ACCOUNT_ID }}
          projectName: flowtica-dashboard
          directory: flowtica-dashboard/build
```

## 🔧 Quick Test Commands

### **Test Everything Locally:**
```bash
# Terminal 1: Start React dashboard
cd flowtica-dashboard
npm start

# Terminal 2: Start desktop app
cd FlowticaDesktop
dotnet run
```

### **Verify Build Output:**
```bash
# Build and check output
cd flowtica-dashboard
npm run build
ls -la build/
```

## 📱 Expected URLs After Deployment

- **Cloudflare Pages**: `https://flowtica-dashboard.pages.dev`
- **GitHub Repository**: `https://github.com/Techy2419/Flowtica`
- **Supabase Database**: `https://tvupomcmuqzmlevmpcnx.supabase.co`

## 🎯 Deployment Checklist

- [ ] Repository pushed to GitHub
- [ ] Cloudflare Pages project created
- [ ] Build settings configured correctly
- [ ] Environment variables added
- [ ] Build command: `npm run build`
- [ ] Output directory: `build`
- [ ] Root directory: `flowtica-dashboard`
- [ ] Deploy successful
- [ ] Test live URL
- [ ] Configure Spotify OAuth with live URL

---

**Ready to deploy!** 🚀✨
