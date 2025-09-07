using System;
using System.Collections.Generic;

namespace WrapticaDesktop.Models
{
    public class SessionData
    {
        public string SessionId { get; set; } = Guid.NewGuid().ToString();
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string SessionName { get; set; } = "Focus Session";
        public List<AppUsage> AppUsages { get; set; } = new List<AppUsage>();
        public List<SpotifyTrack> SpotifyTracks { get; set; } = new List<SpotifyTrack>();
        public TimeSpan Duration => (EndTime ?? DateTime.Now) - StartTime;
        public bool IsActive => EndTime == null;
    }

    public class AppUsage
    {
        public string AppName { get; set; } = string.Empty;
        public string ProcessName { get; set; } = string.Empty;
        public string WindowTitle { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public TimeSpan Duration { get; set; }
    }

    public class SpotifyTrack
    {
        public string TrackId { get; set; } = string.Empty;
        public string TrackName { get; set; } = string.Empty;
        public string ArtistName { get; set; } = string.Empty;
        public string AlbumName { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public TimeSpan Duration { get; set; }
        public bool IsPlaying { get; set; }
    }
}
