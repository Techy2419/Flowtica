using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WrapticaDesktop.Models;

namespace WrapticaDesktop
{
    public partial class SessionsForm : Form
    {
        private ListView _sessionsListView;
        private Button _closeButton;
        private Button _deleteButton;
        private Button _exportButton;
        private Button _refreshButton;
        private Label _titleLabel;
        private Panel _mainPanel;
        private readonly List<SessionData> _sessions;

        // Modern UI Colors
        private readonly Color _primaryColor = Color.FromArgb(102, 126, 234);
        private readonly Color _secondaryColor = Color.FromArgb(118, 75, 162);
        private readonly Color _successColor = Color.FromArgb(16, 185, 129);
        private readonly Color _dangerColor = Color.FromArgb(239, 68, 68);
        private readonly Color _backgroundColor = Color.FromArgb(248, 250, 252);
        private readonly Color _cardColor = Color.White;
        private readonly Color _textColor = Color.FromArgb(30, 41, 59);
        private readonly Color _mutedColor = Color.FromArgb(107, 114, 128);

        public SessionsForm(List<SessionData> sessions)
        {
            _sessions = sessions;
            InitializeComponent();
            SetupModernUI();
            LoadSessions();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form properties
            this.Text = "Flowtica Sessions";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.Sizable;
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
                Text = "📊 Your Sessions",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = _textColor,
                Location = new Point(20, 20),
                Size = new Size(960, 40),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Sessions list view
            _sessionsListView = new ListView
            {
                Location = new Point(20, 80),
                Size = new Size(940, 500),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                MultiSelect = true,
                Font = new Font("Segoe UI", 9F),
                BackColor = _cardColor,
                ForeColor = _textColor,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Add columns with modern styling
            _sessionsListView.Columns.Add("Session Name", 200);
            _sessionsListView.Columns.Add("Start Time", 150);
            _sessionsListView.Columns.Add("Duration", 100);
            _sessionsListView.Columns.Add("Apps Tracked", 100);
            _sessionsListView.Columns.Add("Songs Played", 100);
            _sessionsListView.Columns.Add("Status", 80);
            _sessionsListView.Columns.Add("Productivity Score", 120);

            // Buttons
            _refreshButton = new Button
            {
                Text = "🔄 Refresh",
                Location = new Point(20, 600),
                Size = new Size(100, 40),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = _primaryColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _refreshButton.FlatAppearance.BorderSize = 0;
            _refreshButton.Click += OnRefreshClicked;

            _deleteButton = new Button
            {
                Text = "🗑️ Delete Selected",
                Location = new Point(130, 600),
                Size = new Size(140, 40),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = _dangerColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _deleteButton.FlatAppearance.BorderSize = 0;
            _deleteButton.Click += OnDeleteClicked;

            _exportButton = new Button
            {
                Text = "📤 Export Selected",
                Location = new Point(280, 600),
                Size = new Size(140, 40),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = _successColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _exportButton.FlatAppearance.BorderSize = 0;
            _exportButton.Click += OnExportClicked;

            _closeButton = new Button
            {
                Text = "❌ Close",
                Location = new Point(880, 600),
                Size = new Size(80, 40),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = _mutedColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };
            _closeButton.FlatAppearance.BorderSize = 0;

            // Add controls to main panel
            _mainPanel.Controls.Add(_titleLabel);
            _mainPanel.Controls.Add(_sessionsListView);
            _mainPanel.Controls.Add(_refreshButton);
            _mainPanel.Controls.Add(_deleteButton);
            _mainPanel.Controls.Add(_exportButton);
            _mainPanel.Controls.Add(_closeButton);

            // Add main panel to form
            this.Controls.Add(_mainPanel);

            this.ResumeLayout(false);
        }

        private void SetupModernUI()
        {
            // Add hover effects and modern styling
            foreach (Control control in _mainPanel.Controls)
            {
                if (control is Button button)
                {
                    button.MouseEnter += (s, e) => button.BackColor = Color.FromArgb(button.BackColor.R - 20, button.BackColor.G - 20, button.BackColor.B - 20);
                    button.MouseLeave += (s, e) => button.BackColor = GetOriginalButtonColor(button);
                }
            }

            // Add double-click handler for sessions
            _sessionsListView.DoubleClick += OnSessionDoubleClick;
        }

        private Color GetOriginalButtonColor(Button button)
        {
            if (button == _refreshButton) return _primaryColor;
            if (button == _deleteButton) return _dangerColor;
            if (button == _exportButton) return _successColor;
            return _mutedColor;
        }

        private void LoadSessions()
        {
            _sessionsListView.Items.Clear();

            foreach (var session in _sessions)
            {
                var item = new ListViewItem(session.SessionName);
                item.SubItems.Add(session.StartTime.ToString("yyyy-MM-dd HH:mm:ss"));
                item.SubItems.Add(session.Duration.ToString(@"hh\:mm\:ss"));
                item.SubItems.Add(session.AppUsages.Count.ToString());
                item.SubItems.Add(session.SpotifyTracks.Count.ToString());
                item.SubItems.Add(session.IsActive ? "Active" : "Completed");
                
                // Calculate productivity score
                var productivityScore = CalculateProductivityScore(session);
                item.SubItems.Add($"{productivityScore:F1}%");

                item.Tag = session;

                if (session.IsActive)
                {
                    item.BackColor = Color.FromArgb(240, 253, 244);
                    item.ForeColor = Color.FromArgb(22, 101, 52);
                }
                else
                {
                    // Color code based on productivity score
                    if (productivityScore >= 80)
                        item.BackColor = Color.FromArgb(240, 253, 244);
                    else if (productivityScore >= 60)
                        item.BackColor = Color.FromArgb(254, 249, 195);
                    else
                        item.BackColor = Color.FromArgb(254, 242, 242);
                }

                _sessionsListView.Items.Add(item);
            }
        }

        private double CalculateProductivityScore(SessionData session)
        {
            // Simple productivity score based on app usage patterns
            var productiveApps = new[] { "Code", "Visual Studio", "Chrome", "Firefox", "Notepad++", "Sublime Text", "Atom" };
            var unproductiveApps = new[] { "Steam", "Discord", "Telegram", "WhatsApp", "Facebook", "Twitter", "Instagram" };

            var productiveTime = session.AppUsages
                .Where(app => productiveApps.Any(pa => app.AppName.Contains(pa, StringComparison.OrdinalIgnoreCase)))
                .Sum(app => app.Duration.TotalMinutes);

            var unproductiveTime = session.AppUsages
                .Where(app => unproductiveApps.Any(ua => app.AppName.Contains(ua, StringComparison.OrdinalIgnoreCase)))
                .Sum(app => app.Duration.TotalMinutes);

            var totalTime = session.Duration.TotalMinutes;
            if (totalTime == 0) return 50; // Default score

            var score = ((productiveTime - unproductiveTime) / totalTime) * 100;
            return Math.Max(0, Math.Min(100, score + 50)); // Normalize to 0-100
        }

        private void OnRefreshClicked(object? sender, EventArgs e)
        {
            LoadSessions();
        }

        private void OnDeleteClicked(object? sender, EventArgs e)
        {
            var selectedItems = _sessionsListView.SelectedItems.Cast<ListViewItem>().ToList();
            if (selectedItems.Count == 0)
            {
                MessageBox.Show("Please select sessions to delete.", "No Selection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show(
                $"Are you sure you want to delete {selectedItems.Count} selected session(s)?", 
                "Confirm Delete", 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                foreach (var item in selectedItems)
                {
                    if (item.Tag is SessionData session)
                    {
                        _sessions.Remove(session);
                    }
                }

                LoadSessions();
                MessageBox.Show("Selected sessions deleted successfully.", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void OnExportClicked(object? sender, EventArgs e)
        {
            var selectedItems = _sessionsListView.SelectedItems.Cast<ListViewItem>().ToList();
            if (selectedItems.Count == 0)
            {
                MessageBox.Show("Please select sessions to export.", "No Selection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "JSON files (*.json)|*.json|CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                saveDialog.FileName = $"flowtica_sessions_{DateTime.Now:yyyyMMdd_HHmmss}";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var selectedSessions = selectedItems
                            .Where(item => item.Tag is SessionData)
                            .Select(item => item.Tag as SessionData)
                            .ToList();

                        if (saveDialog.FilterIndex == 1) // JSON
                        {
                            var json = Newtonsoft.Json.JsonConvert.SerializeObject(selectedSessions, Newtonsoft.Json.Formatting.Indented);
                            System.IO.File.WriteAllText(saveDialog.FileName, json);
                        }
                        else if (saveDialog.FilterIndex == 2) // CSV
                        {
                            var csv = GenerateCSV(selectedSessions);
                            System.IO.File.WriteAllText(saveDialog.FileName, csv);
                        }

                        MessageBox.Show($"Sessions exported successfully to {saveDialog.FileName}", "Success", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error exporting sessions: {ex.Message}", "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private string GenerateCSV(List<SessionData?> sessions)
        {
            var csv = new System.Text.StringBuilder();
            csv.AppendLine("Session Name,Start Time,End Time,Duration,Apps Tracked,Songs Played,Productivity Score");

            foreach (var session in sessions.Where(s => s != null))
            {
                var productivityScore = CalculateProductivityScore(session!);
                csv.AppendLine($"\"{session!.SessionName}\",\"{session.StartTime:yyyy-MM-dd HH:mm:ss}\",\"{session.EndTime:yyyy-MM-dd HH:mm:ss}\",\"{session.Duration:hh\\:mm\\:ss}\",{session.AppUsages.Count},{session.SpotifyTracks.Count},{productivityScore:F1}%");
            }

            return csv.ToString();
        }

        private void OnSessionDoubleClick(object? sender, EventArgs e)
        {
            if (_sessionsListView.SelectedItems.Count > 0)
            {
                var selectedItem = _sessionsListView.SelectedItems[0];
                if (selectedItem.Tag is SessionData session)
                {
                    var detailsForm = new SessionDetailsForm(session);
                    detailsForm.ShowDialog();
                }
            }
        }
    }
}