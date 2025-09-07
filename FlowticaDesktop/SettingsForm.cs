using System;
using System.Drawing;
using System.Windows.Forms;
using WrapticaDesktop.Services;

namespace WrapticaDesktop
{
    public partial class SettingsForm : Form
    {
        private readonly SpotifyTracker? _spotifyTracker;
        private TextBox _accessTokenTextBox;
        private Button _saveButton;
        private Button _cancelButton;
        private Label _instructionsLabel;
        private LinkLabel _spotifyLinkLabel;
        private Label _titleLabel;
        private Panel _mainPanel;
        private GroupBox _spotifyGroup;
        private GroupBox _supabaseGroup;
        private TextBox _supabaseUrlTextBox;
        private TextBox _supabaseKeyTextBox;
        private Label _statusLabel;

        // Modern UI Colors
        private readonly Color _primaryColor = Color.FromArgb(102, 126, 234);
        private readonly Color _secondaryColor = Color.FromArgb(118, 75, 162);
        private readonly Color _successColor = Color.FromArgb(16, 185, 129);
        private readonly Color _dangerColor = Color.FromArgb(239, 68, 68);
        private readonly Color _backgroundColor = Color.FromArgb(248, 250, 252);
        private readonly Color _cardColor = Color.White;
        private readonly Color _textColor = Color.FromArgb(30, 41, 59);
        private readonly Color _mutedColor = Color.FromArgb(107, 114, 128);

        public SettingsForm(SpotifyTracker? spotifyTracker)
        {
            _spotifyTracker = spotifyTracker;
            InitializeComponent();
            LoadSettings();
            SetupModernUI();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form properties
            this.Text = "Flowtica Settings";
            this.Size = new Size(600, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = _backgroundColor;
            this.Font = new Font("Segoe UI", 9F);

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
                Text = "⚙️ Flowtica Settings",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = _textColor,
                Location = new Point(20, 20),
                Size = new Size(560, 40),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Spotify Group
            _spotifyGroup = new GroupBox
            {
                Text = "🎵 Spotify Integration",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = _textColor,
                Location = new Point(20, 80),
                Size = new Size(560, 250),
                BackColor = _cardColor
            };

            // Instructions
            _instructionsLabel = new Label
            {
                Text = "To track Spotify playback, you need to provide a Spotify access token.\n\n" +
                       "1. Go to the Spotify Developer Dashboard\n" +
                       "2. Create a new app and get your Client ID and Client Secret\n" +
                       "3. Use the Spotify Web API Console to get an access token\n" +
                       "4. Paste the access token below:",
                Location = new Point(20, 30),
                Size = new Size(520, 100),
                Font = new Font("Segoe UI", 9F),
                ForeColor = _mutedColor
            };

            // Spotify link
            _spotifyLinkLabel = new LinkLabel
            {
                Text = "🔗 Open Spotify Developer Dashboard",
                Location = new Point(20, 140),
                Size = new Size(300, 20),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                LinkColor = _primaryColor,
                VisitedLinkColor = _primaryColor
            };
            _spotifyLinkLabel.LinkClicked += OnSpotifyLinkClicked;

            // Access token text box
            _accessTokenTextBox = new TextBox
            {
                Location = new Point(20, 170),
                Size = new Size(520, 25),
                Font = new Font("Consolas", 9F),
                PlaceholderText = "Paste your Spotify access token here...",
                BorderStyle = BorderStyle.FixedSingle
            };

            // Supabase Group
            _supabaseGroup = new GroupBox
            {
                Text = "☁️ Cloud Sync (Supabase)",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = _textColor,
                Location = new Point(20, 350),
                Size = new Size(560, 200),
                BackColor = _cardColor
            };

            // Supabase URL
            var supabaseUrlLabel = new Label
            {
                Text = "Supabase URL:",
                Location = new Point(20, 30),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = _textColor
            };

            _supabaseUrlTextBox = new TextBox
            {
                Location = new Point(20, 55),
                Size = new Size(520, 25),
                Font = new Font("Consolas", 9F),
                PlaceholderText = "https://your-project.supabase.co",
                BorderStyle = BorderStyle.FixedSingle
            };

            // Supabase Key
            var supabaseKeyLabel = new Label
            {
                Text = "Supabase Anon Key:",
                Location = new Point(20, 90),
                Size = new Size(120, 20),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = _textColor
            };

            _supabaseKeyTextBox = new TextBox
            {
                Location = new Point(20, 115),
                Size = new Size(520, 25),
                Font = new Font("Consolas", 9F),
                PlaceholderText = "Your Supabase anon key...",
                BorderStyle = BorderStyle.FixedSingle
            };

            // Status label
            _statusLabel = new Label
            {
                Text = "Status: Ready to configure",
                Location = new Point(20, 150),
                Size = new Size(520, 20),
                Font = new Font("Segoe UI", 9F),
                ForeColor = _successColor
            };

            // Buttons
            _saveButton = new Button
            {
                Text = "💾 Save Settings",
                Location = new Point(400, 570),
                Size = new Size(120, 40),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = _successColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };
            _saveButton.FlatAppearance.BorderSize = 0;
            _saveButton.Click += OnSaveClicked;

            _cancelButton = new Button
            {
                Text = "❌ Cancel",
                Location = new Point(530, 570),
                Size = new Size(80, 40),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = _dangerColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };
            _cancelButton.FlatAppearance.BorderSize = 0;

            // Add controls to groups
            _spotifyGroup.Controls.Add(_instructionsLabel);
            _spotifyGroup.Controls.Add(_spotifyLinkLabel);
            _spotifyGroup.Controls.Add(_accessTokenTextBox);

            _supabaseGroup.Controls.Add(supabaseUrlLabel);
            _supabaseGroup.Controls.Add(_supabaseUrlTextBox);
            _supabaseGroup.Controls.Add(supabaseKeyLabel);
            _supabaseGroup.Controls.Add(_supabaseKeyTextBox);
            _supabaseGroup.Controls.Add(_statusLabel);

            // Add controls to main panel
            _mainPanel.Controls.Add(_titleLabel);
            _mainPanel.Controls.Add(_spotifyGroup);
            _mainPanel.Controls.Add(_supabaseGroup);
            _mainPanel.Controls.Add(_saveButton);
            _mainPanel.Controls.Add(_cancelButton);

            // Add main panel to form
            this.Controls.Add(_mainPanel);

            this.ResumeLayout(false);
        }

        private void SetupModernUI()
        {
            // Add subtle shadows and modern styling
            foreach (Control control in _mainPanel.Controls)
            {
                if (control is GroupBox groupBox)
                {
                    groupBox.Paint += (sender, e) =>
                    {
                        var rect = groupBox.ClientRectangle;
                        rect.Width -= 1;
                        rect.Height -= 1;
                        e.Graphics.DrawRectangle(new Pen(Color.FromArgb(229, 231, 235), 1), rect);
                    };
                }
            }
        }

        private void LoadSettings()
        {
            try
            {
                var settings = Properties.Settings.Default;
                _accessTokenTextBox.Text = settings.SpotifyAccessToken ?? string.Empty;
                _supabaseUrlTextBox.Text = settings.SupabaseUrl ?? string.Empty;
                _supabaseKeyTextBox.Text = settings.SupabaseKey ?? string.Empty;
                
                UpdateStatus();
            }
            catch (Exception ex)
            {
                _statusLabel.Text = $"Error loading settings: {ex.Message}";
                _statusLabel.ForeColor = _dangerColor;
            }
        }

        private void UpdateStatus()
        {
            var hasSpotify = !string.IsNullOrEmpty(_accessTokenTextBox.Text);
            var hasSupabase = !string.IsNullOrEmpty(_supabaseUrlTextBox.Text) && !string.IsNullOrEmpty(_supabaseKeyTextBox.Text);
            
            if (hasSpotify && hasSupabase)
            {
                _statusLabel.Text = "✅ All services configured and ready";
                _statusLabel.ForeColor = _successColor;
            }
            else if (hasSpotify || hasSupabase)
            {
                _statusLabel.Text = "⚠️ Partial configuration - some features may not work";
                _statusLabel.ForeColor = Color.FromArgb(245, 158, 11);
            }
            else
            {
                _statusLabel.Text = "❌ No services configured - using local storage only";
                _statusLabel.ForeColor = _dangerColor;
            }
        }

        private void OnSaveClicked(object? sender, EventArgs e)
        {
            try
            {
                var spotifyToken = _accessTokenTextBox.Text.Trim();
                var supabaseUrl = _supabaseUrlTextBox.Text.Trim();
                var supabaseKey = _supabaseKeyTextBox.Text.Trim();

                // Validate Spotify token format
                if (!string.IsNullOrEmpty(spotifyToken) && !spotifyToken.StartsWith("BQC"))
                {
                    MessageBox.Show("Please enter a valid Spotify access token.\n\nToken should start with 'BQC'", "Invalid Token", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validate Supabase URL
                if (!string.IsNullOrEmpty(supabaseUrl) && !supabaseUrl.Contains("supabase.co"))
                {
                    MessageBox.Show("Please enter a valid Supabase URL.\n\nURL should contain 'supabase.co'", "Invalid URL", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validate Supabase Key
                if (!string.IsNullOrEmpty(supabaseKey) && !supabaseKey.StartsWith("eyJ"))
                {
                    MessageBox.Show("Please enter a valid Supabase anon key.\n\nKey should start with 'eyJ'", "Invalid Key", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Set the access token in the Spotify tracker
                if (!string.IsNullOrEmpty(spotifyToken))
                {
                    _spotifyTracker?.SetAccessToken(spotifyToken);
                }

                // Save to settings
                var settings = Properties.Settings.Default;
                settings.SpotifyAccessToken = spotifyToken;
                settings.SupabaseUrl = supabaseUrl;
                settings.SupabaseKey = supabaseKey;
                settings.Save();

                UpdateStatus();

                MessageBox.Show("Settings saved successfully!\n\n" +
                    (string.IsNullOrEmpty(spotifyToken) ? "⚠️ Spotify not configured - music tracking disabled\n" : "✅ Spotify configured\n") +
                    (string.IsNullOrEmpty(supabaseUrl) || string.IsNullOrEmpty(supabaseKey) ? "⚠️ Supabase not configured - using local storage only\n" : "✅ Supabase configured\n") +
                    "\nRestart the application for all changes to take effect.", 
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving settings: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnSpotifyLinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://developer.spotify.com/dashboard",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening browser: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            // Add event handlers for real-time status updates
            _accessTokenTextBox.TextChanged += (s, e) => UpdateStatus();
            _supabaseUrlTextBox.TextChanged += (s, e) => UpdateStatus();
            _supabaseKeyTextBox.TextChanged += (s, e) => UpdateStatus();
        }
    }
}