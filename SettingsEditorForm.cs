using System;
using System.Drawing;
using System.Windows.Forms;

namespace EldenRingWatcher
{
    public class SettingsEditorForm : Form
    {
        private TextBox pollIntervalTextBox = null!;
        private TextBox debounceTextBox = null!;
        private TextBox logsPathTextBox = null!;
        private Button browseButton = null!;
        private Button saveButton = null!;
        private Button cancelButton = null!;

        public int PollIntervalMs { get; private set; } = 200;
        public int DebounceMs { get; private set; } = 6000;
        public string LogsPath { get; private set; } = "";

        public SettingsEditorForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            // Form properties
            Text = "Edit Settings";
            Size = new Size(600, 300);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.FromArgb(30, 30, 30);

            // Set icon (same as main form)
            try
            {
                string iconPath = System.IO.Path.Combine(AppContext.BaseDirectory, "icon.ico");
                if (System.IO.File.Exists(iconPath))
                {
                    Icon = new Icon(iconPath);
                }
            }
            catch { /* Ignore if icon not found */ }

            // Create tooltip
            var toolTip = new ToolTip
            {
                BackColor = Color.FromArgb(50, 50, 50),
                ForeColor = Color.White
            };

            // Poll Interval Label & TextBox
            var pollLabel = new Label
            {
                Text = "Poll Interval (ms):",
                Location = new Point(20, 25),
                Size = new Size(150, 20),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10)
            };

            pollIntervalTextBox = new TextBox
            {
                Location = new Point(180, 23),
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Text = "200"
            };
            toolTip.SetToolTip(pollIntervalTextBox, "How often to check for game events\nLower = more responsive, higher = less CPU usage\n(e.g., 200ms)");

            // Debounce Label & TextBox
            var debounceLabel = new Label
            {
                Text = "Debounce (ms):",
                Location = new Point(20, 65),
                Size = new Size(150, 20),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10)
            };

            debounceTextBox = new TextBox
            {
                Location = new Point(180, 63),
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Text = "6000"
            };
            toolTip.SetToolTip(debounceTextBox, "Time to wait before considering a flag as truly triggered\nPrevents duplicate triggers from rapid flag changes\n(e.g., 6000ms = 6 seconds)");

            // Logs Path Label & TextBox
            var logsLabel = new Label
            {
                Text = "Logs Path:",
                Location = new Point(20, 105),
                Size = new Size(150, 20),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10)
            };

            logsPathTextBox = new TextBox
            {
                Location = new Point(180, 103),
                Size = new Size(250, 25),
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            toolTip.SetToolTip(logsPathTextBox, "Directory where real time events are logged\n(full path, e.g., F:\\Speedrun\\livesplit\\logs)");

            // Browse Button
            browseButton = new Button
            {
                Text = "Browse...",
                Location = new Point(440, 103),
                Size = new Size(100, 25),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            browseButton.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
            browseButton.Click += BrowseButton_Click;

            // Save Button
            saveButton = new Button
            {
                Text = "Save",
                Location = new Point(340, 200),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(0, 150, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            saveButton.FlatAppearance.BorderColor = Color.FromArgb(0, 130, 70);
            saveButton.Click += SaveButton_Click;

            // Cancel Button
            cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(450, 200),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            cancelButton.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
            cancelButton.Click += CancelButton_Click;

            // Add controls
            Controls.Add(pollLabel);
            Controls.Add(pollIntervalTextBox);
            Controls.Add(debounceLabel);
            Controls.Add(debounceTextBox);
            Controls.Add(logsLabel);
            Controls.Add(logsPathTextBox);
            Controls.Add(browseButton);
            Controls.Add(saveButton);
            Controls.Add(cancelButton);

            AcceptButton = saveButton;
            CancelButton = cancelButton;
        }

        private void BrowseButton_Click(object? sender, EventArgs e)
        {
            using var folderDialog = new FolderBrowserDialog
            {
                Description = "Select the logs directory",
                RootFolder = Environment.SpecialFolder.MyComputer,
                SelectedPath = logsPathTextBox.Text
            };

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                logsPathTextBox.Text = folderDialog.SelectedPath;
            }
        }

        private void SaveButton_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(pollIntervalTextBox.Text, out int pollInterval) || pollInterval <= 0)
            {
                MessageBox.Show("Please enter a valid poll interval (must be greater than 0).", "Invalid Input",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(debounceTextBox.Text, out int debounce) || debounce < 0)
            {
                MessageBox.Show("Please enter a valid debounce value (must be 0 or greater).", "Invalid Input",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(logsPathTextBox.Text))
            {
                MessageBox.Show("Please enter a logs path.", "Invalid Input",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            PollIntervalMs = pollInterval;
            DebounceMs = debounce;
            LogsPath = logsPathTextBox.Text.Trim();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void CancelButton_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        public void LoadSettings(int pollInterval, int debounce, string logsPath)
        {
            pollIntervalTextBox.Text = pollInterval.ToString();
            debounceTextBox.Text = debounce.ToString();
            logsPathTextBox.Text = logsPath;
        }
    }
}
