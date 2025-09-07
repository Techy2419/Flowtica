using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using WrapticaDesktop.Models;

namespace WrapticaDesktop.Services
{
    public class AppTracker
    {
        private readonly System.Threading.Timer _trackingTimer;
        private readonly Dictionary<string, DateTime> _appStartTimes = new Dictionary<string, DateTime>();
        private readonly object _lockObject = new object();
        private bool _isTracking = false;

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder text, int count);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        public event EventHandler<AppUsage>? AppChanged;

        public AppTracker()
        {
            _trackingTimer = new System.Threading.Timer(TrackCurrentApp, null, Timeout.Infinite, Timeout.Infinite);
        }

        public void StartTracking()
        {
            lock (_lockObject)
            {
                _isTracking = true;
                _trackingTimer.Change(0, 2000); // Track every 2 seconds
            }
        }

        public void StopTracking()
        {
            lock (_lockObject)
            {
                _isTracking = false;
                _trackingTimer.Change(Timeout.Infinite, Timeout.Infinite);
                _appStartTimes.Clear();
            }
        }

        private void TrackCurrentApp(object? state)
        {
            if (!_isTracking) return;

            try
            {
                var currentApp = GetCurrentActiveApp();
                if (currentApp != null)
                {
                    lock (_lockObject)
                    {
                        var now = DateTime.Now;
                        var appKey = $"{currentApp.ProcessName}|{currentApp.WindowTitle}";

                        // If this is a new app or the window title changed
                        if (!_appStartTimes.ContainsKey(appKey))
                        {
                            // Record the previous app's usage if it exists
                            if (_appStartTimes.Count > 0)
                            {
                                var previousApp = _appStartTimes.First();
                                var previousAppParts = previousApp.Key.Split('|');
                                var previousUsage = new AppUsage
                                {
                                    AppName = previousAppParts[0],
                                    ProcessName = previousAppParts[0],
                                    WindowTitle = previousAppParts[1],
                                    Timestamp = previousApp.Value,
                                    Duration = now - previousApp.Value
                                };
                                AppChanged?.Invoke(this, previousUsage);
                            }

                            // Clear old entries and add new one
                            _appStartTimes.Clear();
                            _appStartTimes[appKey] = now;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error but don't stop tracking
                Console.WriteLine($"Error tracking app: {ex.Message}");
            }
        }

        private AppUsage? GetCurrentActiveApp()
        {
            try
            {
                IntPtr hWnd = GetForegroundWindow();
                if (hWnd == IntPtr.Zero) return null;

                // Get window title
                const int nChars = 256;
                System.Text.StringBuilder Buff = new System.Text.StringBuilder(nChars);
                int length = GetWindowText(hWnd, Buff, nChars);

                if (length == 0) return null;

                // Get process ID
                GetWindowThreadProcessId(hWnd, out uint processId);
                Process? process = Process.GetProcessById((int)processId);

                if (process == null) return null;

                return new AppUsage
                {
                    AppName = process.ProcessName,
                    ProcessName = process.ProcessName,
                    WindowTitle = Buff.ToString(),
                    Timestamp = DateTime.Now
                };
            }
            catch
            {
                return null;
            }
        }

        public void Dispose()
        {
            _trackingTimer?.Dispose();
        }
    }
}
