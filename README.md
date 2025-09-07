# Flowtica - Personal Productivity Tracker

Flowtica is a Windows-only personal productivity tracker that monitors active desktop apps and Spotify music playback during defined work or focus sessions. It generates custom wrapped reports showing the most used applications, top songs, and favorite artists from each session.

## Features

- **Desktop App Tracking**: Monitors active Windows applications every few seconds
- **Spotify Integration**: Tracks currently playing music during sessions
- **Session Management**: Start/stop focus sessions with system tray interface
- **Web Dashboard**: Beautiful React dashboard to view session summaries and analytics
- **Cloud Sync**: Supabase backend for data storage and retrieval
- **Local Storage**: JSON file storage for offline data

## Technology Stack

- **Desktop App**: C# .NET 8 with Windows Forms
- **Backend**: Supabase (PostgreSQL)
- **Frontend**: React with TypeScript
- **Charts**: Recharts for data visualization
- **Icons**: Lucide React

## Project Structure

```
Flowtica/
├── FlowticaDesktop/          # C# Desktop Application
│   ├── Models/               # Data models
│   ├── Services/             # Core services (tracking, Spotify, sessions)
│   ├── Properties/           # Application settings
│   └── MainForm.cs           # Main application form
├── flowtica-dashboard/       # React Web Dashboard
│   ├── src/
│   │   ├── components/       # React components
│   │   ├── services/         # API services
│   │   └── types/            # TypeScript types
│   └── public/
├── supabase-setup/           # Database schema and setup
└── README.md
```

## Quick Start

### Phase 1: Desktop Tracker Setup

1. **Prerequisites**:
   - Windows 10/11
   - .NET 8 SDK
   - Visual Studio Community (recommended)

2. **Build the Desktop App**:
   ```bash
   cd FlowticaDesktop
   dotnet build
   dotnet run
   ```

3. **Configure Spotify**:
   - Get a Spotify access token from the [Spotify Developer Dashboard](https://developer.spotify.com/dashboard)
   - Right-click the system tray icon and select "Settings"
   - Enter your Spotify access token

4. **Start Tracking**:
   - Right-click the system tray icon
   - Select "Start Session" to begin tracking
   - Select "Stop Session" to end tracking

### Phase 2: Web Dashboard Setup

1. **Prerequisites**:
   - Node.js 16+
   - Supabase account

2. **Setup Supabase**:
   - Create a new Supabase project
   - Run the SQL schema from `supabase-setup/schema.sql`
   - Get your project URL and anon key

3. **Setup React Dashboard**:
   ```bash
   cd flowtica-dashboard
   npm install
   cp env.example .env.local
   # Edit .env.local with your Supabase credentials
   npm start
   ```

4. **Configure Desktop App for Cloud Sync**:
   - Update the desktop app to send data to Supabase
   - Add your Supabase URL and API key to the desktop app settings

## Development Phases

### Phase 1: Core Desktop Tracker ✅
- [x] Windows app for tracking active applications
- [x] Spotify Web API integration
- [x] Session start/stop mechanism
- [x] Local JSON data storage
- [x] System tray interface

### Phase 2: Basic Cloud Sync & Dashboard ✅
- [x] Supabase database setup
- [x] React web dashboard
- [x] Session data visualization
- [x] Cloud data storage

### Phase 3: User Authentication (Planned)
- [ ] Supabase Auth integration
- [ ] Multi-user support
- [ ] User-specific session data

### Phase 4: Advanced Features (Planned)
- [ ] Session templates
- [ ] Smart reminders
- [ ] Enhanced analytics
- [ ] Mobile app

### Phase 5: Distribution (Planned)
- [ ] Windows installer
- [ ] Auto-updater
- [ ] User documentation

## Usage

### Desktop App
1. The app runs in the system tray
2. Right-click the tray icon to access options:
   - **Start Session**: Begin tracking applications and Spotify
   - **Stop Session**: End current session and save data
   - **Settings**: Configure Spotify access token
   - **View Sessions**: See local session history
   - **Exit**: Close the application

### Web Dashboard
1. Open the React dashboard in your browser
2. View session summaries with:
   - Total sessions and duration
   - Top applications used
   - Most played artists and songs
   - Productivity insights

## Data Privacy

- All data is stored locally on your machine
- Cloud sync is optional and uses Supabase
- No data is shared with third parties
- You control your own data

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For issues and questions:
1. Check the GitHub Issues page
2. Create a new issue with detailed information
3. Include your Windows version and .NET version

## Roadmap

- [ ] Mobile companion app
- [ ] Team collaboration features
- [ ] Advanced analytics and insights
- [ ] Integration with other productivity tools
- [ ] Custom session templates
- [ ] Automated reporting
