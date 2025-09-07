# Flowtica Desktop Tracker

The Windows desktop application for tracking productivity sessions and Spotify playback.

## Features

- **Application Tracking**: Monitors active Windows applications every 2 seconds
- **Spotify Integration**: Tracks currently playing music via Spotify Web API
- **Session Management**: Start/stop sessions with system tray interface
- **Local Storage**: Saves session data as JSON files
- **System Tray**: Runs quietly in the background

## Prerequisites

- Windows 10/11
- .NET 8 Runtime
- Spotify Developer Account (for music tracking)

## Building and Running

### Using Visual Studio
1. Open `WrapticaDesktop.csproj` in Visual Studio
2. Build the solution (Ctrl+Shift+B)
3. Run the application (F5)

### Using Command Line
```bash
cd FlowticaDesktop
dotnet build
dotnet run
```

## Configuration

### Spotify Setup
1. Go to [Spotify Developer Dashboard](https://developer.spotify.com/dashboard)
2. Create a new app
3. Get your Client ID and Client Secret
4. Use the Spotify Web API Console to get an access token
5. Right-click the system tray icon → Settings
6. Enter your access token

### Session Data Location
Session data is stored in:
```
%APPDATA%\Flowtica\session_*.json
```

## Usage

1. **Start the Application**: Run the executable
2. **Configure Spotify**: Set up your access token in settings
3. **Start a Session**: Right-click tray icon → "Start Session"
4. **Work Normally**: The app tracks your applications and music
5. **Stop Session**: Right-click tray icon → "Stop Session"
6. **View Data**: Right-click tray icon → "View Sessions"

## System Tray Interface

- **Start Session**: Begin tracking a new session
- **Stop Session**: End the current active session
- **Settings**: Configure Spotify access token
- **View Sessions**: Open session history window
- **Exit**: Close the application

## Data Models

### SessionData
- Session ID, name, start/end times
- Array of app usage records
- Array of Spotify track records

### AppUsage
- Application name and process name
- Window title and timestamp
- Duration of usage

### SpotifyTrack
- Track name, artist, album
- Playback status and timestamp
- Track duration

## Troubleshooting

### Common Issues

1. **Spotify not tracking**:
   - Verify your access token is valid
   - Check that Spotify is playing music
   - Ensure internet connection

2. **App not tracking**:
   - Run as administrator if needed
   - Check Windows permissions
   - Verify .NET 8 is installed

3. **System tray not visible**:
   - Check Windows notification area settings
   - Look for hidden icons
   - Restart the application

### Logs
Check the console output for error messages. Common errors include:
- Invalid Spotify access token
- Permission denied for app tracking
- Network connectivity issues

## Development

### Project Structure
```
FlowticaDesktop/
├── Models/           # Data models (SessionData, AppUsage, etc.)
├── Services/         # Core services
│   ├── AppTracker.cs     # Windows app tracking
│   ├── SpotifyTracker.cs # Spotify API integration
│   └── SessionManager.cs # Session data management
├── Properties/       # Application settings
└── MainForm.cs       # Main application form
```

### Adding Features
1. Create new models in `Models/` folder
2. Add services in `Services/` folder
3. Update `MainForm.cs` for UI changes
4. Test thoroughly on Windows

### Dependencies
- `Newtonsoft.Json` - JSON serialization
- `System.Windows.Forms` - Windows Forms UI
- `System.Drawing.Common` - Graphics and icons

## Security Notes

- The app requires access to active window information
- Spotify access tokens should be kept secure
- Session data is stored locally by default
- No data is transmitted without explicit cloud sync setup

## Performance

- Minimal CPU usage (polls every 2 seconds)
- Low memory footprint
- Efficient JSON storage
- Background operation optimized
