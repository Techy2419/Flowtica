# 🚀 Flowtica Deployment Guide

## 🌐 Web Dashboard Deployment (Cloudflare Pages)

### Step 1: Deploy to Cloudflare Pages
1. Go to [dash.cloudflare.com](https://dash.cloudflare.com) and sign in
2. Navigate to "Pages" in the sidebar
3. Click "Create a project"
4. Connect your GitHub account and select repository: `Techy2419/Flowtica`
5. Configure the project:
   - **Project name**: `flowtica-dashboard`
   - **Production branch**: `master`
   - **Framework preset**: `Create React App`
   - **Build command**: `npm run build`
   - **Build output directory**: `build`
   - **Root directory**: `flowtica-dashboard`
6. Add Environment Variables:
   - `REACT_APP_SUPABASE_URL`: `https://tvupomcmuqzmlevmpcnx.supabase.co`
   - `REACT_APP_SUPABASE_ANON_KEY`: `eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InR2dXBvbWNtdXF6bWxldm1wY254Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTcyMDg1MzMsImV4cCI6MjA3Mjc4NDUzM30.wz2Ztmz7XBa986S1r6oLkQeznqzY9d0enngzBLrXajg`
7. Click "Save and Deploy"

### Step 2: Get Your Live URL
After deployment, you'll get a URL like: `https://flowtica-dashboard.pages.dev`

## 🎵 Spotify App Configuration

### Step 1: Create Spotify App
1. Go to [Spotify Developer Dashboard](https://developer.spotify.com/dashboard)
2. Click "Create App"
3. Fill in:
   - **App Name**: `Flowtica`
   - **App Description**: `Personal productivity tracker`
   - **Website**: `https://your-cloudflare-url.pages.dev`
   - **Redirect URI**: `https://your-cloudflare-url.pages.dev/callback`
4. Click "Save"

### Step 2: Get Credentials
1. Copy your **Client ID**
2. Copy your **Client Secret**
3. Note your **Redirect URI**

## 🖥️ Desktop App Configuration

### Step 1: Update Desktop App Settings
1. Run the Flowtica desktop app
2. Click "Settings"
3. Enter your Spotify credentials:
   - **Spotify Access Token**: Get from Spotify Web API Console
4. Enter Supabase credentials (already configured):
   - **Supabase URL**: `https://tvupomcmuqzmlevmpcnx.supabase.co`
   - **Supabase Key**: `eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InR2dXBvbWNtdXF6bWxldm1wY254Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTcyMDg1MzMsImV4cCI6MjA3Mjc4NDUzM30.wz2Ztmz7XBa986S1r6oLkQeznqzY9d0enngzBLrXajg`

### Step 2: Test the Sync
1. Start a session in the desktop app
2. Open your Vercel dashboard URL
3. You should see real-time data from your session!

## 🔧 Troubleshooting

### Desktop App Not Syncing
- Check that Supabase credentials are correct in Settings
- Restart the desktop app after changing settings
- Check the console output for error messages

### Dashboard Shows No Data
- Ensure the desktop app is running and has an active session
- Check that Supabase credentials match in both apps
- Verify the Vercel deployment has the correct environment variables

### Spotify Integration Issues
- Make sure your Spotify app redirect URI matches your Vercel URL
- Get a fresh access token from Spotify Web API Console
- Check that the token hasn't expired

## 📱 Production URLs

Once deployed, your app will be available at:
- **Web Dashboard**: `https://your-app-name.pages.dev`
- **GitHub Repository**: `https://github.com/Techy2419/Flowtica`
- **Supabase Database**: `https://tvupomcmuqzmlevmpcnx.supabase.co`

## 🎯 Next Steps

1. **Deploy to Cloudflare Pages** (get live URL)
2. **Configure Spotify App** (use Cloudflare Pages URL as redirect)
3. **Test Desktop Sync** (verify data appears in dashboard)
4. **Share with Users** (provide download links and setup instructions)

---

**Your Flowtica app is now production-ready!** 🚀✨
