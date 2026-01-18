using System;
using System.Drawing;
using System.Windows.Forms;

namespace EldenRingWatcher
{
    /// <summary>
    /// Non-intrusive toast notification system for status messages
    /// </summary>
    public static class ToastNotification
    {
        public enum NotificationType
        {
            Success,
            Error,
            Warning,
            Info
        }

        private static Form? _currentToast;

        /// <summary>
        /// Show a toast notification that auto-dismisses after specified duration
        /// </summary>
        public static void Show(string message, NotificationType type = NotificationType.Info, int durationMs = 3000)
        {
            // Close any existing toast
            _currentToast?.Invoke(new Action(() => _currentToast?.Close()));

            var toastForm = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                BackColor = GetBackgroundColor(type),
                ForeColor = Color.White,
                StartPosition = FormStartPosition.Manual,
                ShowInTaskbar = false,
                TopMost = true,
                Width = 350,
                Height = 60,
                Padding = new Padding(15)
            };

            // Position at bottom-right of screen
            var screen = Screen.PrimaryScreen ?? throw new InvalidOperationException("No primary screen found");
            toastForm.Location = new Point(
                screen.WorkingArea.Right - toastForm.Width - 20,
                screen.WorkingArea.Bottom - toastForm.Height - 20
            );

            // Create label for message
            var label = new Label
            {
                Text = message,
                ForeColor = Color.White,
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 10F, FontStyle.Regular)
            };

            toastForm.Controls.Add(label);

            // Show toast
            _currentToast = toastForm;
            toastForm.Show();

            // Auto-dismiss after duration
            var timer = new System.Windows.Forms.Timer
            {
                Interval = durationMs
            };
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                timer.Dispose();
                toastForm.Close();
                toastForm.Dispose();
                if (_currentToast == toastForm)
                    _currentToast = null;
            };
            timer.Start();
        }

        private static Color GetBackgroundColor(NotificationType type) => type switch
        {
            NotificationType.Success => Color.FromArgb(76, 175, 80),    // Green
            NotificationType.Error => Color.FromArgb(244, 67, 54),      // Red
            NotificationType.Warning => Color.FromArgb(255, 152, 0),    // Orange
            _ => Color.FromArgb(33, 150, 243)                            // Blue
        };
    }
}
