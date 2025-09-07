using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WrapticaDesktop.Models;

namespace WrapticaDesktop
{
    public partial class SessionDetailsForm : Form
    {
        private readonly SessionData _session;
        private ListView _appsListView;
        private ListView _tracksListView;
        private Label _titleLabel;
        private Panel _mainPanel;
        private TabControl _tabControl;
        private Label _statsLabel;

        // Modern UI Colors
        private readonly Color _primaryColor = Color.FromArgb(102, 126, 234);
        private readonly Color _secondaryColor = Color.FromArgb(118, 75, 162);
        private readonly Color _successColor = Color.FromArgb(16, 185, 129);
        private readonly Color _dangerColor = Color.FromArgb(239, 68, 68);
        private readonly Color _backgroundColor = Color.FromArgb(248, 250, 252);
        private readonly Color _cardColor = Color.White;
        private readonly Color _textColor = Color.FromArgb(30, 41, 59);
        private readonly Color _mutedColor = Color.FromArgb(107, 114, 128);

        public SessionDetailsForm(SessionData session)
        {
            _session = session;
            InitializeComponent();
            SetupModernUI();
            LoadSessionData();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form properties
            this.Text = $"Session Details - {_session.SessionName}";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;
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
                Text = $"📊 {_session.SessionName}",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = _textColor,
                Location = new Point(20, 20),
                Size = new Size(760, 40),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Stats label
            _statsLabel = new Label
            {
                Text = $"Duration: {_session.Duration:hh\\:mm\\:ss} | Apps: {_session.AppUsages.Count} | Songs: {_session.SpotifyTracks.Count}",
                Font = new Font("Segoe UI", 10F),
                ForeColor = _mutedColor,
                Location = new Point(20, 70),
                Size = new Size(760, 25)
            };

            // Tab control
            _tabControl = new TabControl
            {
                Location = new Point(20, 110),
                Size = new Size(760, 400),
                Font = new Font("Segoe UI", 9F),
                BackColor = _cardColor
            };

            // Apps tab
            var appsTab = new TabPage("💻 Applications");
            _appsListView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Font = new Font("Segoe UI", 9F),
                BackColor = _cardColor,
                ForeColor = _textColor
            };
            _appsListView.Columns.Add("Application", 200);
            _appsListView.Columns.Add("Process", 150);
            _appsListView.Columns.Add("Window Title", 300);
            _appsListView.Columns.Add("Duration", 100);
            appsTab.Controls.Add(_appsListView);

            // Tracks tab
            var tracksTab = new TabPage("🎵 Music");
            _tracksListView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Font = new Font("Segoe UI", 9F),
                BackColor = _cardColor,
                ForeColor = _textColor
            };
            _tracksListView.Columns.Add("Track Name", 250);
            _tracksListView.Columns.Add("Artist", 200);
            _tracksListView.Columns.Add("Album", 200);
            _tracksListView.Columns.Add("Duration", 100);
            _tracksListView.Columns.Add("Status", 80);
            tracksTab.Controls.Add(_tracksListView);

            _tabControl.TabPages.Add(appsTab);
            _tabControl.TabPages.Add(tracksTab);

            // Close button
            var closeButton = new Button
            {
                Text = "❌ Close",
                Location = new Point(700, 530),
                Size = new Size(80, 40),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = _mutedColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };
            closeButton.FlatAppearance.BorderSize = 0;

            // Add controls to main panel
            _mainPanel.Controls.Add(_titleLabel);
            _mainPanel.Controls.Add(_statsLabel);
            _mainPanel.Controls.Add(_tabControl);
            _mainPanel.Controls.Add(closeButton);

            // Add main panel to form
            this.Controls.Add(_mainPanel);

            this.ResumeLayout(false);
        }

        private void SetupModernUI()
        {
            // Add modern styling to tab control
            _tabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            _tabControl.DrawItem += (sender, e) =>
            {
                var tab = _tabControl.TabPages[e.Index];
                var rect = _tabControl.GetTabRect(e.Index);
                
                e.Graphics.FillRectangle(new SolidBrush(_cardColor), rect);
                e.Graphics.DrawString(tab.Text, new Font("Segoe UI", 9F, FontStyle.Bold), 
                    new SolidBrush(_textColor), rect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            };
        }

        private void LoadSessionData()
        {
            LoadAppsData();
            LoadTracksData();
        }

        private void LoadAppsData()
        {
            _appsListView.Items.Clear();

            // Group apps by name and calculate total duration
            var appGroups = _session.AppUsages
                .GroupBy(app => app.AppName)
                .Select(g => new
                {
                    AppName = g.Key,
                    ProcessName = g.First().ProcessName,
                    TotalDuration = TimeSpan.FromTicks(g.Sum(app => app.Duration.Ticks)),
                    WindowTitles = g.Select(app => app.WindowTitle).Distinct().ToList()
                })
                .OrderByDescending(app => app.TotalDuration)
                .ToList();

            foreach (var app in appGroups)
            {
                var item = new ListViewItem(app.AppName);
                item.SubItems.Add(app.ProcessName);
                item.SubItems.Add(string.Join(", ", app.WindowTitles.Take(3)));
                item.SubItems.Add(app.TotalDuration.ToString(@"hh\:mm\:ss"));
                item.Tag = app;

                // Color code based on usage time
                if (app.TotalDuration.TotalMinutes > 30)
                    item.BackColor = Color.FromArgb(240, 253, 244);
                else if (app.TotalDuration.TotalMinutes > 10)
                    item.BackColor = Color.FromArgb(254, 249, 195);

                _appsListView.Items.Add(item);
            }
        }

        private void LoadTracksData()
        {
            _tracksListView.Items.Clear();

            // Group tracks by name and count plays
            var trackGroups = _session.SpotifyTracks
                .GroupBy(track => new { track.TrackName, track.ArtistName })
                .Select(g => new
                {
                    TrackName = g.Key.TrackName,
                    ArtistName = g.Key.ArtistName,
                    AlbumName = g.First().AlbumName,
                    PlayCount = g.Count(),
                    TotalDuration = TimeSpan.FromTicks(g.Sum(track => track.Duration.Ticks)),
                    IsPlaying = g.Any(track => track.IsPlaying)
                })
                .OrderByDescending(track => track.PlayCount)
                .ToList();

            foreach (var track in trackGroups)
            {
                var item = new ListViewItem(track.TrackName);
                item.SubItems.Add(track.ArtistName);
                item.SubItems.Add(track.AlbumName);
                item.SubItems.Add(track.TotalDuration.ToString(@"hh\:mm\:ss"));
                item.SubItems.Add(track.IsPlaying ? "Playing" : "Played");
                item.Tag = track;

                // Color code based on play count
                if (track.PlayCount > 3)
                    item.BackColor = Color.FromArgb(240, 253, 244);
                else if (track.PlayCount > 1)
                    item.BackColor = Color.FromArgb(254, 249, 195);

                _tracksListView.Items.Add(item);
            }
        }
    }
}
