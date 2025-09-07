using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WrapticaDesktop.Models;

namespace WrapticaDesktop.Services
{
    public class SessionManager
    {
        private readonly string _dataDirectory;
        private SessionData? _currentSession;
        private readonly object _lockObject = new object();
        private readonly HttpClient _httpClient;

        public event EventHandler<SessionData>? SessionStarted;
        public event EventHandler<SessionData>? SessionEnded;

        public SessionManager()
        {
            _dataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Flowtica");
            Directory.CreateDirectory(_dataDirectory);
            _httpClient = new HttpClient();
        }

        public bool IsSessionActive => _currentSession?.IsActive ?? false;

        public SessionData? CurrentSession => _currentSession;

        public async Task<SessionData> StartSessionAsync(string sessionName = "Focus Session")
        {
            lock (_lockObject)
            {
                if (_currentSession?.IsActive == true)
                {
                    throw new InvalidOperationException("A session is already active. Please end the current session first.");
                }

                _currentSession = new SessionData
                {
                    SessionId = Guid.NewGuid().ToString(),
                    StartTime = DateTime.Now,
                    SessionName = sessionName
                };

                SessionStarted?.Invoke(this, _currentSession);
                return _currentSession;
            }
        }

        public async Task<SessionData> EndSessionAsync()
        {
            lock (_lockObject)
            {
                if (_currentSession?.IsActive != true)
                {
                    throw new InvalidOperationException("No active session to end.");
                }

                _currentSession.EndTime = DateTime.Now;
                var completedSession = _currentSession;
                _currentSession = null;

                SessionEnded?.Invoke(this, completedSession);
                return completedSession;
            }
        }

        public void AddAppUsage(AppUsage appUsage)
        {
            lock (_lockObject)
            {
                if (_currentSession?.IsActive == true)
                {
                    _currentSession.AppUsages.Add(appUsage);
                }
            }
        }

        public void AddSpotifyTrack(SpotifyTrack spotifyTrack)
        {
            lock (_lockObject)
            {
                if (_currentSession?.IsActive == true)
                {
                    _currentSession.SpotifyTracks.Add(spotifyTrack);
                }
            }
        }

        public async Task SaveSessionAsync(SessionData session)
        {
            try
            {
                var fileName = $"session_{session.SessionId}_{session.StartTime:yyyyMMdd_HHmmss}.json";
                var filePath = Path.Combine(_dataDirectory, fileName);
                
                var json = JsonConvert.SerializeObject(session, Formatting.Indented);
                await File.WriteAllTextAsync(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving session: {ex.Message}");
            }
        }

        public async Task UploadToSupabaseAsync(SessionData session)
        {
            try
            {
                var settings = Properties.Settings.Default;
                var supabaseUrl = settings.SupabaseUrl;
                var supabaseKey = settings.SupabaseKey;

                if (string.IsNullOrEmpty(supabaseUrl) || string.IsNullOrEmpty(supabaseKey))
                {
                    Console.WriteLine("Supabase not configured, skipping cloud upload");
                    return;
                }

                var sessionData = new
                {
                    session_id = session.SessionId,
                    start_time = session.StartTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    end_time = session.EndTime?.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    session_name = session.SessionName,
                    app_usages = session.AppUsages.Select(app => new
                    {
                        app_name = app.AppName,
                        process_name = app.ProcessName,
                        window_title = app.WindowTitle,
                        timestamp = app.Timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        duration = app.Duration.ToString()
                    }).ToArray(),
                    spotify_tracks = session.SpotifyTracks.Select(track => new
                    {
                        track_id = track.TrackId,
                        track_name = track.TrackName,
                        artist_name = track.ArtistName,
                        album_name = track.AlbumName,
                        timestamp = track.Timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        duration = track.Duration.ToString(),
                        is_playing = track.IsPlaying
                    }).ToArray()
                };

                var json = JsonConvert.SerializeObject(sessionData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(HttpMethod.Post, $"{supabaseUrl}/rest/v1/sessions")
                {
                    Content = content
                };
                request.Headers.Add("apikey", supabaseKey);
                request.Headers.Add("Authorization", $"Bearer {supabaseKey}");
                request.Headers.Add("Prefer", "return=minimal");

                var response = await _httpClient.SendAsync(request);
                
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Session uploaded to Supabase successfully");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error uploading to Supabase: {response.StatusCode} - {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading session to Supabase: {ex.Message}");
            }
        }

        public async Task<List<SessionData>> LoadSessionsAsync()
        {
            try
            {
                var sessions = new List<SessionData>();
                var files = Directory.GetFiles(_dataDirectory, "session_*.json");

                foreach (var file in files)
                {
                    try
                    {
                        var json = await File.ReadAllTextAsync(file);
                        var session = JsonConvert.DeserializeObject<SessionData>(json);
                        if (session != null)
                        {
                            sessions.Add(session);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error loading session from {file}: {ex.Message}");
                    }
                }

                return sessions.OrderByDescending(s => s.StartTime).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading sessions: {ex.Message}");
                return new List<SessionData>();
            }
        }

        public async Task<SessionData?> LoadSessionAsync(string sessionId)
        {
            try
            {
                var files = Directory.GetFiles(_dataDirectory, $"session_{sessionId}_*.json");
                if (files.Length == 0) return null;

                var json = await File.ReadAllTextAsync(files[0]);
                return JsonConvert.DeserializeObject<SessionData>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading session {sessionId}: {ex.Message}");
                return null;
            }
        }

        public void DeleteSession(string sessionId)
        {
            try
            {
                var files = Directory.GetFiles(_dataDirectory, $"session_{sessionId}_*.json");
                foreach (var file in files)
                {
                    File.Delete(file);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting session {sessionId}: {ex.Message}");
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}