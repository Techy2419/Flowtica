using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WrapticaDesktop.Models;
using WrapticaDesktop.Services;

namespace WrapticaDesktop
{
    public partial class MainForm : Form
    {
        private NotifyIcon? _notifyIcon;
        private ContextMenuStrip? _contextMenu;
        private AppTracker? _appTracker;
        private SpotifyTracker? _spotifyTracker;
        private SessionManager? _sessionManager;
        private bool _isSessionActive = false;
        private System.Windows.Forms.Timer? _uiUpdateTimer;

        // Modern UI Colors
        private readonly Color _primaryColor = Color.FromArgb(102, 126, 234);
        private readonly Color _secondaryColor = Color.FromArgb(118, 75, 162);
        private readonly Color _successColor = Color.FromArgb(16, 185, 129);
        private readonly Color _dangerColor = Color.FromArgb(239, 68, 68);
        private readonly Color _backgroundColor = Color.FromArgb(248, 250, 252);
        private readonly Color _cardColor = Color.White;

        public MainForm()
        {
            InitializeComponent();
            InitializeServices();
            SetupSystemTray();
            SetupEventHandlers();
            SetupUIUpdateTimer();
            LoadSettings();
        }

        private void InitializeServices()
        {
            _appTracker = new AppTracker();
            _spotifyTracker = new SpotifyTracker();
            _sessionManager = new SessionManager();
        }

        private void SetupSystemTray()
        {
            // Create a custom icon for the system tray
            var icon = CreateTrayIcon();
            
            _notifyIcon = new NotifyIcon
            {
                Icon = icon,
                Text = "Flowtica - Personal Productivity Tracker",
                Visible = true
            };

            _contextMenu = new ContextMenuStrip();
            _contextMenu.BackColor = _cardColor;
            _contextMenu.ForeColor = Color.FromArgb(30, 41, 59);
            _contextMenu.Font = new Font("Segoe UI", 9F);

            // Add menu items with modern styling
            var startItem = _contextMenu.Items.Add("🚀 Start Session", null, OnStartSession);
            startItem.BackColor = _successColor;
            startItem.ForeColor = Color.White;
            startItem.Font = new Font("Segoe UI", 9F, FontStyle.Bold);

            var stopItem = _contextMenu.Items.Add("⏹️ Stop Session", null, OnStopSession);
            stopItem.BackColor = _dangerColor;
            stopItem.ForeColor = Color.White;
            stopItem.Font = new Font("Segoe UI", 9F, FontStyle.Bold);

            _contextMenu.Items.Add("-");
            
            var settingsItem = _contextMenu.Items.Add("⚙️ Settings", null, OnSettings);
            var sessionsItem = _contextMenu.Items.Add("📊 View Sessions", null, OnViewSessions);
            var dashboardItem = _contextMenu.Items.Add("🌐 Open Dashboard", null, OnOpenDashboard);
            
            _contextMenu.Items.Add("-");
            var exitItem = _contextMenu.Items.Add("❌ Exit", null, OnExit);
            exitItem.BackColor = _dangerColor;
            exitItem.ForeColor = Color.White;

            _notifyIcon.ContextMenuStrip = _contextMenu;
            _notifyIcon.DoubleClick += OnNotifyIconDoubleClick;

            UpdateContextMenu();
        }

        private Icon CreateTrayIcon()
        {
            // Create a beautiful 16x16 icon for the system tray
            var bitmap = new Bitmap(16, 16);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                
                // Create gradient background
                var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    new Rectangle(0, 0, 16, 16),
                    _primaryColor,
                    _secondaryColor,
                    45f);
                
                g.FillEllipse(brush, 2, 2, 12, 12);
                
                // Add a small "F" for Flowtica
                using (var font = new Font("Arial", 8, FontStyle.Bold))
                {
                    var textBrush = new SolidBrush(Color.White);
                    g.DrawString("F", font, textBrush, 4, 2);
                }
            }
            
            return Icon.FromHandle(bitmap.GetHicon());
        }

        private void SetupEventHandlers()
        {
            if (_appTracker != null)
            {
                _appTracker.AppChanged += OnAppChanged;
            }

            if (_spotifyTracker != null)
            {
                _spotifyTracker.TrackChanged += OnSpotifyTrackChanged;
            }

            if (_sessionManager != null)
            {
                _sessionManager.SessionStarted += OnSessionStarted;
                _sessionManager.SessionEnded += OnSessionEnded;
            }
        }

        private void SetupUIUpdateTimer()
        {
            _uiUpdateTimer = new System.Windows.Forms.Timer();
            _uiUpdateTimer.Interval = 1000; // Update every second
            _uiUpdateTimer.Tick += OnUIUpdate;
            _uiUpdateTimer.Start();
        }

        private void LoadSettings()
        {
            try
            {
                var settings = Properties.Settings.Default;
                if (!string.IsNullOrEmpty(settings.SpotifyAccessToken))
                {
                    _spotifyTracker?.SetAccessToken(settings.SpotifyAccessToken);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading settings: {ex.Message}");
            }
        }

        private void OnAppChanged(object? sender, AppUsage appUsage)
        {
            if (_sessionManager != null && _isSessionActive)
            {
                _sessionManager.AddAppUsage(appUsage);
            }
        }

        private void OnSpotifyTrackChanged(object? sender, SpotifyTrack spotifyTrack)
        {
            if (_sessionManager != null && _isSessionActive)
            {
                _sessionManager.AddSpotifyTrack(spotifyTrack);
            }
        }

        private async void OnSessionStarted(object? sender, SessionData session)
        {
            _isSessionActive = true;
            UpdateContextMenu();
            UpdateNotifyIconText($"Flowtica - Session Active: {session.SessionName}");
            
            if (_appTracker != null)
            {
                _appTracker.StartTracking();
            }

            if (_spotifyTracker != null)
            {
                _spotifyTracker.StartTracking();
            }

            ShowBalloonTip("Session Started", $"Started tracking session: {session.SessionName}", ToolTipIcon.Info);
        }

        private async void OnSessionEnded(object? sender, SessionData session)
        {
            _isSessionActive = false;
            UpdateContextMenu();
            UpdateNotifyIconText("Flowtica - Personal Productivity Tracker");

            if (_appTracker != null)
            {
                _appTracker.StopTracking();
            }

            if (_spotifyTracker != null)
            {
                _spotifyTracker.StopTracking();
            }

            if (_sessionManager != null)
            {
                await _sessionManager.SaveSessionAsync(session);
                await _sessionManager.UploadToSupabaseAsync(session);
            }

            ShowBalloonTip("Session Ended", $"Session completed. Duration: {session.Duration:hh\\:mm\\:ss}", ToolTipIcon.Info);
        }

        private void OnUIUpdate(object? sender, EventArgs e)
        {
            if (_isSessionActive && _sessionManager?.CurrentSession != null)
            {
                var session = _sessionManager.CurrentSession;
                var duration = DateTime.Now - session.StartTime;
                UpdateNotifyIconText($"Flowtica - {session.SessionName} ({duration:hh\\:mm\\:ss})");
            }
        }

        private async void OnStartSession(object? sender, EventArgs e)
        {
            if (_sessionManager == null) return;

            try
            {
                var sessionName = Microsoft.VisualBasic.Interaction.InputBox(
                    "Enter session name:", 
                    "Start New Session", 
                    $"Focus Session {DateTime.Now:HH:mm}");

                if (!string.IsNullOrEmpty(sessionName))
                {
                    await _sessionManager.StartSessionAsync(sessionName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting session: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void OnStopSession(object? sender, EventArgs e)
        {
            if (_sessionManager == null) return;

            try
            {
                await _sessionManager.EndSessionAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error stopping session: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnSettings(object? sender, EventArgs e)
        {
            var settingsForm = new SettingsForm(_spotifyTracker);
            settingsForm.ShowDialog();
        }

        private async void OnViewSessions(object? sender, EventArgs e)
        {
            if (_sessionManager == null) return;

            try
            {
                var sessions = await _sessionManager.LoadSessionsAsync();
                var sessionsForm = new SessionsForm(sessions);
                sessionsForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading sessions: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnOpenDashboard(object? sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "http://localhost:3000",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening dashboard: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnExit(object? sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to exit Flowtica?",
                "Exit Flowtica",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void OnNotifyIconDoubleClick(object? sender, EventArgs e)
        {
            if (_isSessionActive)
            {
                OnStopSession(sender, e);
            }
            else
            {
                OnStartSession(sender, e);
            }
        }

        private void UpdateContextMenu()
        {
            if (_contextMenu == null) return;

            _contextMenu.Items[0].Enabled = !_isSessionActive; // Start Session
            _contextMenu.Items[1].Enabled = _isSessionActive;  // Stop Session
        }

        private void UpdateNotifyIconText(string text)
        {
            if (_notifyIcon != null)
            {
                _notifyIcon.Text = text;
            }
        }

        private void ShowBalloonTip(string title, string text, ToolTipIcon icon)
        {
            if (_notifyIcon != null)
            {
                _notifyIcon.ShowBalloonTip(3000, title, text, icon);
            }
        }

        protected override void SetVisibleCore(bool value)
        {
            base.SetVisibleCore(false); // Hide the form
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
                return;
            }
            base.OnFormClosing(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _appTracker?.Dispose();
                _spotifyTracker?.Dispose();
                _notifyIcon?.Dispose();
                _uiUpdateTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}