# 🚀 Flowtica - Complete Setup Instructions

## Overview
Flowtica is a beautiful, production-ready personal productivity tracker that monitors your desktop applications and Spotify music during work sessions.

## 🖥️ Desktop Application Setup

### Prerequisites
- Windows 10/11
- .NET 8.0 SDK (Download from: https://dotnet.microsoft.com/download/dotnet/8.0)

### Installation
1. Navigate to the `FlowticaDesktop` folder
2. Run: `dotnet build`
3. Run: `dotnet run`

### Configuration
1. Click the **Settings** button in the main window
2. **Spotify Setup:**
   - Go to https://developer.spotify.com/dashboard
   - Create a new app
   - Get your Client ID and Client Secret
   - Use Spotify Web API Console to get an access token
   - Paste the token in the Spotify section
3. **Supabase Setup:**
   - URL: `https://tvupomcmuqzmlevmpcnx.supabase.co`
   - Anon Key: `eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InR2dXBvbWNtdXF6bWxldm1wY254Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTcyMDg1MzMsImV4cCI6MjA3Mjc4NDUzM30.wz2Ztmz7XBa986S1r6oLkQeznqzY9d0enngzBLrXajg`

## 🌐 Web Dashboard Setup

### Prerequisites
- Node.js 16+ (Download from: https://nodejs.org/)
- npm (comes with Node.js)

### Installation
1. Navigate to the `flowtica-dashboard` folder
2. Run: `npm install`
3. Create a `.env.local` file with:
   ```
   REACT_APP_SUPABASE_URL=https://tvupomcmuqzmlevmpcnx.supabase.co
   REACT_APP_SUPABASE_ANON_KEY=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InR2dXBvbWNtdXF6bWxldm1wY254Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTcyMDg1MzMsImV4cCI6MjA3Mjc4NDUzM30.wz2Ztmz7XBa986S1r6oLkQeznqzY9d0enngzBLrXajg
   ```
4. Run: `npm start`
5. Open http://localhost:3000

## 🎯 How to Use

### Desktop App
1. **Start a Session:** Click "Start Session" and enter a session name
2. **Monitor Activity:** The app tracks your active applications and Spotify music
3. **View Analytics:** Click "Sessions" to see detailed session history
4. **Open Dashboard:** Click "Dashboard" to open the web interface

### Web Dashboard
1. **Real-time Data:** Automatically updates with data from your desktop sessions
2. **Analytics:** View productivity scores, top apps, music, and insights
3. **Session History:** Browse and analyze all your past sessions
4. **Export Data:** Download session data in JSON or CSV format

## 🔧 Features

### Desktop Application
- ✅ **Modern Windows UI** - Beautiful, standard Windows application interface
- ✅ **System Tray Integration** - Minimizes to system tray, accessible via tray icon
- ✅ **Real-time Tracking** - Monitors applications and Spotify every few seconds
- ✅ **Session Management** - Start/stop sessions with custom names
- ✅ **Settings Panel** - Configure Spotify and Supabase credentials
- ✅ **Session History** - View detailed session analytics
- ✅ **Cloud Sync** - Automatic upload to Supabase database

### Web Dashboard
- ✅ **Beautiful Modern UI** - Responsive design with gradients and animations
- ✅ **Real-time Updates** - Auto-refreshes every 30 seconds
- ✅ **Advanced Analytics** - Productivity scores, top apps, music insights
- ✅ **Session Management** - View, delete, and export sessions
- ✅ **No Mock Data** - Only shows real data from your sessions
- ✅ **Mobile Responsive** - Works on desktop, tablet, and mobile

## 🚀 Production Deployment

### Desktop App
- Build: `dotnet publish -c Release -r win-x64 --self-contained`
- Creates a standalone executable in `bin/Release/net8.0-windows/win-x64/publish/`

### Web Dashboard
- Build: `npm run build`
- Deploy the `build` folder to Vercel, Netlify, or any static hosting service

## 📊 Database Schema

The Supabase database includes:
- **sessions** table with app usage and Spotify track data
- **Real-time sync** between desktop app and web dashboard
- **Automatic data persistence** and retrieval

## 🎨 UI/UX Highlights

- **Modern Design** - Gradient backgrounds, smooth animations, beautiful typography
- **Intuitive Navigation** - Easy-to-use interface with clear visual feedback
- **Real-time Feedback** - Live updates and progress indicators
- **Responsive Layout** - Works perfectly on all screen sizes
- **Color-coded Analytics** - Visual indicators for productivity levels

## 🔒 Security

- **Local Storage** - Session data stored locally as backup
- **Cloud Sync** - Secure upload to Supabase with API keys
- **No User Data Collection** - All data stays on your devices and Supabase

## 📱 System Requirements

- **Desktop App:** Windows 10/11, .NET 8.0 Runtime
- **Web Dashboard:** Modern web browser (Chrome, Firefox, Safari, Edge)
- **Internet:** Required for Supabase sync and Spotify integration

---

**Flowtica** - Track your productivity, understand your habits, and optimize your workflow! 🚀📊✨
