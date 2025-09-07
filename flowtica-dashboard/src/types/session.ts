export interface AppUsage {
  appName: string;
  processName: string;
  windowTitle: string;
  timestamp: string;
  duration: string;
}

export interface SpotifyTrack {
  trackId: string;
  trackName: string;
  artistName: string;
  albumName: string;
  timestamp: string;
  duration: string;
  isPlaying: boolean;
}

export interface SessionData {
  sessionId: string;
  startTime: string;
  endTime?: string;
  sessionName: string;
  appUsages: AppUsage[];
  spotifyTracks: SpotifyTrack[];
  isActive: boolean;
}

export interface TopApp {
  name: string;
  usage: number;
  percentage: number;
}

export interface TopArtist {
  name: string;
  playCount: number;
  percentage: number;
}

export interface TopSong {
  name: string;
  artist: string;
  playCount: number;
  percentage: number;
}

export interface SessionSummary {
  totalSessions: number;
  totalDuration: string;
  topApps: TopApp[];
  topArtists: TopArtist[];
  topSongs: TopSong[];
  averageSessionDuration: string;
  mostProductiveDay: string;
  mostProductiveHour: number;
}