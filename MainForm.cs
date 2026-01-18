using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace EldenRingWatcher
{
    public class MainForm : Form
    {
        private TextBox logTextBox = null!;
        private Label statusLabel = null!;
        private Label configLabel = null!;
        private Button reloadButton = null!;
        private Button clearLogsButton = null!;
        private Button editConfigButton = null!;
        private Button setFlagButton = null!;
        private Button setPositionButton = null!;

        public MainForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            // Form properties
            Text = "Elden Ring Watcher";
            Size = new Size(800, 600);
            MinimumSize = new Size(600, 400);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.Sizable;

            // Set icon
            try
            {
                string iconPath = System.IO.Path.Combine(AppContext.BaseDirectory, "icon.ico");
                if (System.IO.File.Exists(iconPath))
                {
                    Icon = new Icon(iconPath);
                }
            }
            catch { /* Ignore if icon not found */ }

            // Header Panel
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                BackColor = Color.FromArgb(30, 30, 30),
                Padding = new Padding(15)
            };

            // Version Label
            var versionLabel = new Label
            {
                Text = $"Elden Ring Watcher v{GetVersion()}",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 200, 100),
                AutoSize = true,
                Location = new Point(15, 15)
            };

            // Status Label
            statusLabel = new Label
            {
                Text = "Status: Initializing...",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(15, 50)
            };

            // Config Label
            configLabel = new Label
            {
                Text = "Configuration: Loading...",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.LightGray,
                AutoSize = true,
                Location = new Point(15, 75)
            };

            // Info Button (top right)
            var infoButton = new Button
            {
                Text = "ℹ",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Size = new Size(40, 40),
                Location = new Point(headerPanel.Width - 60, 15),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.FromArgb(255, 200, 100),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            infoButton.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
            infoButton.Click += (s, e) =>
            {
                Program.ShowMessageSilent(
                    $"ELDEN RING WATCHER v{GetVersion()}\n" +
                    "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n" +
                    "A real-time game monitoring tool for speedrunners.\n\n" +
                    "It tracks:\n" +
                    "• Event flags (game state changes)\n" +
                    "• Player positions\n\n" +
                    "Automatically logs detected events to 2 files:\n" +
                    "• events.txt - Complete history of all triggered flags\n" +
                    "• latest.json - Latest event (for file watchers like streamer.bot)\n\n" +
                    "Use this data to trigger automatic actions in your streaming setup.\n\n" +
                    "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n" +
                    "© 2026\n" +
                    "Free to use and distribute",
                    "About Elden Ring Watcher",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            };

            headerPanel.Controls.Add(versionLabel);
            headerPanel.Controls.Add(statusLabel);
            headerPanel.Controls.Add(configLabel);
            headerPanel.Controls.Add(infoButton);

            // Create tooltip for main buttons
            var toolTip = new ToolTip
            {
                BackColor = Color.FromArgb(50, 50, 50),
                ForeColor = Color.White
            };

            // Button Panel with fixed width buttons
            var buttonPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.FromArgb(40, 40, 40),
                Padding = new Padding(10),
                AutoScroll = false
            };

            reloadButton = new Button
            {
                Text = "Reload Config",
                Location = new Point(10, 10),
                Size = new Size(120, 30),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            reloadButton.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
            toolTip.SetToolTip(reloadButton, "Reload configuration manually if any change has been made to config.json");

            clearLogsButton = new Button
            {
                Text = "Clear Logs",
                Location = new Point(140, 10),
                Size = new Size(120, 30),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            clearLogsButton.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
            toolTip.SetToolTip(clearLogsButton, "Clear all event and log files manually, even tho it should be done after each character creation");

            editConfigButton = new Button
            {
                Text = "Set Config",
                Location = new Point(270, 10),
                Size = new Size(120, 30),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            editConfigButton.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
            toolTip.SetToolTip(editConfigButton, "Edit settings: poll interval, debounce time, and output logs path");

            setFlagButton = new Button
            {
                Text = "Set Flag",
                Location = new Point(400, 10),
                Size = new Size(120, 30),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            setFlagButton.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
            toolTip.SetToolTip(setFlagButton, "Add, edit, or delete event flags to monitor");

            setPositionButton = new Button
            {
                Text = "Set Position",
                Location = new Point(530, 10),
                Size = new Size(120, 30),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            setPositionButton.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
            toolTip.SetToolTip(setPositionButton, "Add, edit, or delete position-based flags");

            buttonPanel.Controls.Add(reloadButton);
            buttonPanel.Controls.Add(clearLogsButton);
            buttonPanel.Controls.Add(editConfigButton);
            buttonPanel.Controls.Add(setFlagButton);
            buttonPanel.Controls.Add(setPositionButton);

            // Log TextBox
            logTextBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                BackColor = Color.FromArgb(20, 20, 20),
                ForeColor = Color.FromArgb(255, 165, 0),
                Font = new Font("Courier New", 12),
                Padding = new Padding(5),
                BorderStyle = BorderStyle.None
            };

            // Add controls to form
            Controls.Add(logTextBox);
            Controls.Add(buttonPanel);
            Controls.Add(headerPanel);
        }

        private string GetVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly()
                .GetCustomAttribute<System.Reflection.AssemblyInformationalVersionAttribute>()?
                .InformationalVersion ?? "Unknown";
        }

        public void AppendLog(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(AppendLog), message);
                return;
            }

            logTextBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\r\n");
            logTextBox.SelectionStart = logTextBox.Text.Length;
            logTextBox.ScrollToCaret();
        }

        public void UpdateStatus(string status, Color? color = null)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string, Color?>(UpdateStatus), status, color);
                return;
            }

            statusLabel.Text = $"Status: {status}";
            statusLabel.ForeColor = color ?? Color.White;
        }

        public void UpdateConfig(string configInfo)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(UpdateConfig), configInfo);
                return;
            }

            configLabel.Text = $"Configuration: {configInfo}";
        }

        public void SetReloadAction(Action action)
        {
            reloadButton.Click += (s, e) => action();
        }

        public void SetClearLogsAction(Action action)
        {
            clearLogsButton.Click += (s, e) => action();
        }

        public void SetEditConfigAction(Action action)
        {
            editConfigButton.Click += (s, e) => action();
        }

        public void SetSetFlagAction(Action action)
        {
            setFlagButton.Click += (s, e) => action();
        }

        public void SetSetPositionAction(Action action)
        {
            setPositionButton.Click += (s, e) => action();
        }
    }
}
