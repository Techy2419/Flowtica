using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WrapticaDesktop.Models;
using WrapticaDesktop.Services;

namespace WrapticaDesktop
{
    public partial class MainWindow : Form
    {
        private NotifyIcon? _notifyIcon;
        private AppTracker? _appTracker;
        private SpotifyTracker? _spotifyTracker;
        private SessionManager? _sessionManager;
        private bool _isSessionActive = false;
        private System.Windows.Forms.Timer? _uiUpdateTimer;

        // UI Controls
        private Panel _mainPanel;
        private Label _titleLabel;
        private Label _statusLabel;
        private Button _startStopButton;
        private Button _settingsButton;
        private Button _sessionsButton;
        private Button _dashboardButton;
        private Label _sessionInfoLabel;
        private Label _appsLabel;
        private Label _musicLabel;
        private ListBox _appsListBox;
        private ListBox _musicListBox;
        private ProgressBar _sessionProgressBar;
        private Label _sessionTimeLabel;

        // Modern UI Colors
        private readonly Color _primaryColor = Color.FromArgb(102, 126, 234);
        private readonly Color _secondaryColor = Color.FromArgb(118, 75, 162);
        private readonly Color _successColor = Color.FromArgb(16, 185, 129);
        private readonly Color _dangerColor = Color.FromArgb(239, 68, 68);
        private readonly Color _backgroundColor = Color.FromArgb(248, 250, 252);
        private readonly Color _cardColor = Color.White;
        private readonly Color _textColor = Color.FromArgb(30, 41, 59);
        private readonly Color _mutedColor = Color.FromArgb(107, 114, 128);

        public MainWindow()
        {
            InitializeComponent();
            InitializeServices();
            SetupSystemTray();
            SetupEventHandlers();
            SetupUIUpdateTimer();
            LoadSettings();
            UpdateUI();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form properties
            this.Text = "Flowtica - Personal Productivity Tracker";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = _backgroundColor;
            this.Font = new Font("Segoe UI", 9F);
            this.Icon = SystemIcons.Application;

            // Main panel
            _mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = _backgroundColor
            };

            // Title
            _titleLabel = new Label
            {
                Text = "🚀 Flowtica",
                Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                ForeColor = _textColor,
                Location = new Point(20, 20),
                Size = new Size(400, 50),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Status label
            _statusLabel = new Label
            {
                Text = "Ready to start tracking",
                Font = new Font("Segoe UI", 12F),
                ForeColor = _mutedColor,
                Location = new Point(20, 80),
                Size = new Size(400, 30)
            };

            // Start/Stop button
            _startStopButton = new Button
            {
                Text = "▶️ Start Session",
                Location = new Point(20, 120),
                Size = new Size(150, 50),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                BackColor = _successColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _startStopButton.FlatAppearance.BorderSize = 0;
            _startStopButton.Click += OnStartStopClicked;

            // Session info
            _sessionInfoLabel = new Label
            {
                Text = "No active session",
                Font = new Font("Segoe UI", 10F),
                ForeColor = _textColor,
                Location = new Point(200, 120),
                Size = new Size(300, 30)
            };

            // Session progress
            _sessionProgressBar = new ProgressBar
            {
                Location = new Point(200, 150),
                Size = new Size(300, 20),
                Style = ProgressBarStyle.Continuous
            };

            _sessionTimeLabel = new Label
            {
                Text = "00:00:00",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = _primaryColor,
                Location = new Point(520, 150),
                Size = new Size(100, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Apps section
            _appsLabel = new Label
            {
                Text = "📱 Tracked Applications",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = _textColor,
                Location = new Point(20, 200),
                Size = new Size(200, 30)
            };

            _appsListBox = new ListBox
            {
                Location = new Point(20, 240),
                Size = new Size(350, 150),
                Font = new Font("Consolas", 9F),
                BackColor = _cardColor,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Music section
            _musicLabel = new Label
            {
                Text = "🎵 Current Music",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = _textColor,
                Location = new Point(400, 200),
                Size = new Size(200, 30)
            };

            _musicListBox = new ListBox
            {
                Location = new Point(400, 240),
                Size = new Size(350, 150),
                Font = new Font("Consolas", 9F),
                BackColor = _cardColor,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Action buttons
            _settingsButton = new Button
            {
                Text = "⚙️ Settings",
                Location = new Point(20, 420),
                Size = new Size(100, 40),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = _primaryColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _settingsButton.FlatAppearance.BorderSize = 0;
            _settingsButton.Click += OnSettingsClicked;

            _sessionsButton = new Button
            {
                Text = "📊 Sessions",
                Location = new Point(140, 420),
                Size = new Size(100, 40),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = _secondaryColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _sessionsButton.FlatAppearance.BorderSize = 0;
            _sessionsButton.Click += OnSessionsClicked;

            _dashboardButton = new Button
            {
                Text = "🌐 Dashboard",
                Location = new Point(260, 420),
                Size = new Size(100, 40),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = _successColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _dashboardButton.FlatAppearance.BorderSize = 0;
            _dashboardButton.Click += OnDashboardClicked;

            // Add controls to main panel
            _mainPanel.Controls.Add(_titleLabel);
            _mainPanel.Controls.Add(_statusLabel);
            _mainPanel.Controls.Add(_startStopButton);
            _mainPanel.Controls.Add(_sessionInfoLabel);
            _mainPanel.Controls.Add(_sessionProgressBar);
            _mainPanel.Controls.Add(_sessionTimeLabel);
            _mainPanel.Controls.Add(_appsLabel);
            _mainPanel.Controls.Add(_appsListBox);
            _mainPanel.Controls.Add(_musicLabel);
            _mainPanel.Controls.Add(_musicListBox);
            _mainPanel.Controls.Add(_settingsButton);
            _mainPanel.Controls.Add(_sessionsButton);
            _mainPanel.Controls.Add(_dashboardButton);

            // Add main panel to form
            this.Controls.Add(_mainPanel);

            this.ResumeLayout(false);
        }

        private void InitializeServices()
        {
            _appTracker = new AppTracker();
            _spotifyTracker = new SpotifyTracker();
            _sessionManager = new SessionManager();
        }

        private void SetupSystemTray()
        {
            var icon = CreateTrayIcon();
            
            _notifyIcon = new NotifyIcon
            {
                Icon = icon,
                Text = "Flowtica - Personal Productivity Tracker",
                Visible = true
            };

            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Show Flowtica", null, (s, e) => { this.Show(); this.WindowState = FormWindowState.Normal; });
            contextMenu.Items.Add("-");
            contextMenu.Items.Add("Exit", null, (s, e) => Application.Exit());

            _notifyIcon.ContextMenuStrip = contextMenu;
            _notifyIcon.DoubleClick += (s, e) => { this.Show(); this.WindowState = FormWindowState.Normal; };

            // Hide to system tray on minimize
            this.Resize += (s, e) => {
                if (this.WindowState == FormWindowState.Minimized)
                {
                    this.Hide();
                    _notifyIcon.ShowBalloonTip(1000, "Flowtica", "Minimized to system tray", ToolTipIcon.Info);
                }
            };
        }

        private Icon CreateTrayIcon()
        {
            var bitmap = new Bitmap(16, 16);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                
                var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    new Rectangle(0, 0, 16, 16),
                    _primaryColor,
                    _secondaryColor,
                    45f);
                
                g.FillEllipse(brush, 2, 2, 12, 12);
                
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
                
                // Log Supabase configuration status
                if (!string.IsNullOrEmpty(settings.SupabaseUrl) && !string.IsNullOrEmpty(settings.SupabaseKey))
                {
                    Console.WriteLine("Supabase configured - cloud sync enabled");
                }
                else
                {
                    Console.WriteLine("Supabase not configured - using local storage only");
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
                UpdateAppsList();
            }
        }

        private void OnSpotifyTrackChanged(object? sender, SpotifyTrack spotifyTrack)
        {
            if (_sessionManager != null && _isSessionActive)
            {
                _sessionManager.AddSpotifyTrack(spotifyTrack);
                UpdateMusicList();
            }
        }

        private async void OnSessionStarted(object? sender, SessionData session)
        {
            _isSessionActive = true;
            UpdateUI();
            
            if (_appTracker != null)
            {
                _appTracker.StartTracking();
            }

            if (_spotifyTracker != null)
            {
                _spotifyTracker.StartTracking();
            }

            _notifyIcon?.ShowBalloonTip(3000, "Session Started", $"Started tracking session: {session.SessionName}", ToolTipIcon.Info);
        }

        private async void OnSessionEnded(object? sender, SessionData session)
        {
            _isSessionActive = false;
            UpdateUI();

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

            _notifyIcon?.ShowBalloonTip(3000, "Session Ended", $"Session completed. Duration: {session.Duration:hh\\:mm\\:ss}", ToolTipIcon.Info);
        }

        private void OnUIUpdate(object? sender, EventArgs e)
        {
            if (_isSessionActive && _sessionManager?.CurrentSession != null)
            {
                var session = _sessionManager.CurrentSession;
                var duration = DateTime.Now - session.StartTime;
                _sessionTimeLabel.Text = duration.ToString(@"hh\:mm\:ss");
                _sessionProgressBar.Value = Math.Min(100, (int)(duration.TotalMinutes * 2)); // 50 minutes = 100%
            }
        }

        private async void OnStartStopClicked(object? sender, EventArgs e)
        {
            if (_sessionManager == null) return;

            try
            {
                if (_isSessionActive)
                {
                    await _sessionManager.EndSessionAsync();
                }
                else
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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnSettingsClicked(object? sender, EventArgs e)
        {
            var settingsForm = new SettingsForm(_spotifyTracker);
            if (settingsForm.ShowDialog() == DialogResult.OK)
            {
                LoadSettings();
            }
        }

        private async void OnSessionsClicked(object? sender, EventArgs e)
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

        private void OnDashboardClicked(object? sender, EventArgs e)
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

        private void UpdateUI()
        {
            if (_isSessionActive)
            {
                _startStopButton.Text = "⏹️ Stop Session";
                _startStopButton.BackColor = _dangerColor;
                _statusLabel.Text = "Session active - tracking productivity";
                _statusLabel.ForeColor = _successColor;
                _sessionInfoLabel.Text = _sessionManager?.CurrentSession?.SessionName ?? "Active Session";
            }
            else
            {
                _startStopButton.Text = "▶️ Start Session";
                _startStopButton.BackColor = _successColor;
                _statusLabel.Text = "Ready to start tracking";
                _statusLabel.ForeColor = _mutedColor;
                _sessionInfoLabel.Text = "No active session";
                _sessionTimeLabel.Text = "00:00:00";
                _sessionProgressBar.Value = 0;
            }

            UpdateAppsList();
            UpdateMusicList();
        }

        private void UpdateAppsList()
        {
            _appsListBox.Items.Clear();
            if (_sessionManager?.CurrentSession?.AppUsages != null)
            {
                var appGroups = _sessionManager.CurrentSession.AppUsages
                    .GroupBy(app => app.AppName)
                    .Select(g => new { Name = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .Take(10);

                foreach (var app in appGroups)
                {
                    _appsListBox.Items.Add($"{app.Name} ({app.Count}x)");
                }
            }
        }

        private void UpdateMusicList()
        {
            _musicListBox.Items.Clear();
            if (_sessionManager?.CurrentSession?.SpotifyTracks != null)
            {
                var recentTracks = _sessionManager.CurrentSession.SpotifyTracks
                    .OrderByDescending(t => t.Timestamp)
                    .Take(10);

                foreach (var track in recentTracks)
                {
                    _musicListBox.Items.Add($"{track.TrackName} - {track.ArtistName}");
                }
            }
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
