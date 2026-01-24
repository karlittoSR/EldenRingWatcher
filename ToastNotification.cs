using System;
using System.Drawing;
using System.Windows.Forms;

namespace EldenRingWatcher
{
    /// <summary>
    /// Non-intrusive toast notification system for status messages
    /// Positions relative to main application window
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
        private static Form? _mainForm;

        /// <summary>
        /// Set the main form reference for positioning toasts relative to it
        /// </summary>
        public static void SetMainForm(Form mainForm)
        {
            _mainForm = mainForm;
        }

        /// <summary>
        /// Show a toast notification that auto-dismisses after specified duration
        /// </summary>
        public static void Show(string message, NotificationType type = NotificationType.Info, int durationMs = 3000)
        {
            // Close any existing toast
            _currentToast?.Invoke(new Action(() => _currentToast?.Close()));

            // Calculate dimensions with padding for text wrapping
            const int maxWidth = 200;
            const int minHeight = 35;
            const int padding = 8;

            var toastForm = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                BackColor = GetBackgroundColor(type),
                ForeColor = Color.White,
                StartPosition = FormStartPosition.Manual,
                ShowInTaskbar = false,
                TopMost = true,
                Padding = new Padding(padding),
                AutoSize = false
            };

            // Create label for message with text wrapping
            var label = new Label
            {
                Text = message,
                ForeColor = Color.White,
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                Margin = new Padding(0)
            };

            // Measure text to determine label size
            using (var g = toastForm.CreateGraphics())
            {
                var textSize = g.MeasureString(message, label.Font, maxWidth - (padding * 2));
                label.Width = maxWidth;
                label.Height = Math.Max((int)textSize.Height + (padding * 2), minHeight - (padding * 2));
            }

            toastForm.Width = maxWidth;
            toastForm.Height = label.Height + (padding * 2);
            toastForm.Controls.Add(label);

            // Position at bottom-right of main form, or screen if no main form
            if (_mainForm != null && _mainForm.Visible)
            {
                toastForm.Location = new Point(
                    _mainForm.Right - toastForm.Width - 20,
                    _mainForm.Bottom - toastForm.Height - 20
                );
            }
            else
            {
                var screen = Screen.PrimaryScreen ?? throw new InvalidOperationException("No primary screen found");
                toastForm.Location = new Point(
                    screen.WorkingArea.Right - toastForm.Width - 20,
                    screen.WorkingArea.Bottom - toastForm.Height - 20
                );
            }

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
