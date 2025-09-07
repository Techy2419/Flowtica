using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WrapticaDesktop.Models;

namespace WrapticaDesktop.Services
{
    public class SpotifyTracker
    {
        private readonly HttpClient _httpClient;
        private readonly System.Threading.Timer _spotifyTimer;
        private readonly object _lockObject = new object();
        private bool _isTracking = false;
        private string? _accessToken;
        private SpotifyTrack? _currentTrack;

        public event EventHandler<SpotifyTrack>? TrackChanged;

        public SpotifyTracker()
        {
            _httpClient = new HttpClient();
            _spotifyTimer = new System.Threading.Timer(PollSpotify, null, Timeout.Infinite, Timeout.Infinite);
        }

        public void SetAccessToken(string accessToken)
        {
            _accessToken = accessToken;
        }

        public void StartTracking()
        {
            lock (_lockObject)
            {
                _isTracking = true;
                _spotifyTimer.Change(0, 5000); // Poll every 5 seconds
            }
        }

        public void StopTracking()
        {
            lock (_lockObject)
            {
                _isTracking = false;
                _spotifyTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        private async void PollSpotify(object? state)
        {
            if (!_isTracking || string.IsNullOrEmpty(_accessToken)) return;

            try
            {
                var currentTrack = await GetCurrentlyPlayingTrack();
                if (currentTrack != null)
                {
                    lock (_lockObject)
                    {
                        // Check if track has changed
                        if (_currentTrack == null || 
                            _currentTrack.TrackId != currentTrack.TrackId ||
                            _currentTrack.IsPlaying != currentTrack.IsPlaying)
                        {
                            _currentTrack = currentTrack;
                            TrackChanged?.Invoke(this, currentTrack);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error polling Spotify: {ex.Message}");
            }
        }

        private async Task<SpotifyTrack?> GetCurrentlyPlayingTrack()
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://api.spotify.com/v1/me/player/currently-playing");
                request.Headers.Add("Authorization", $"Bearer {_accessToken}");

                var response = await _httpClient.SendAsync(request);
                
                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    // No track currently playing
                    return new SpotifyTrack
                    {
                        TrackId = "",
                        TrackName = "No track playing",
                        ArtistName = "",
                        AlbumName = "",
                        Timestamp = DateTime.Now,
                        IsPlaying = false
                    };
                }

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Spotify API error: {response.StatusCode}");
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                var spotifyResponse = JsonSerializer.Deserialize<SpotifyApiResponse>(json);

                if (spotifyResponse?.Item == null) return null;

                return new SpotifyTrack
                {
                    TrackId = spotifyResponse.Item.Id,
                    TrackName = spotifyResponse.Item.Name,
                    ArtistName = string.Join(", ", spotifyResponse.Item.Artists.Select(a => a.Name)),
                    AlbumName = spotifyResponse.Item.Album.Name,
                    Timestamp = DateTime.Now,
                    Duration = TimeSpan.FromMilliseconds(spotifyResponse.Item.DurationMs),
                    IsPlaying = spotifyResponse.IsPlaying
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting Spotify track: {ex.Message}");
                return null;
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
            _spotifyTimer?.Dispose();
        }
    }

    // Spotify API Response Models
    public class SpotifyApiResponse
    {
        public bool IsPlaying { get; set; }
        public SpotifyTrackItem? Item { get; set; }
    }

    public class SpotifyTrackItem
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<SpotifyArtist> Artists { get; set; } = new List<SpotifyArtist>();
        public SpotifyAlbum Album { get; set; } = new SpotifyAlbum();
        public int DurationMs { get; set; }
    }

    public class SpotifyArtist
    {
        public string Name { get; set; } = string.Empty;
    }

    public class SpotifyAlbum
    {
        public string Name { get; set; } = string.Empty;
    }
}
