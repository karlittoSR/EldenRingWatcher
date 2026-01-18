using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EldenRingWatcher
{
    public class FlagEditorForm : Form
    {
        private DataGridView flagsGrid = null!;
        private Button addButton = null!;
        private Button deleteButton = null!;
        private Button saveButton = null!;
        private Button cancelButton = null!;
        private List<FlagEntry> flags = new();

        public class FlagEntry
        {
            public uint Flag { get; set; }
            public string Token { get; set; } = "";
        }

        public List<FlagEntry> Flags
        {
            get => flags;
            set
            {
                flags = new List<FlagEntry>(value);
                RefreshGrid();
            }
        }

        public FlagEditorForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            // Form properties
            Text = "Edit Event Flags";
            Size = new Size(700, 500);
            MinimumSize = new Size(600, 400);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = false;

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

            // Header Panel
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(30, 30, 30),
                Padding = new Padding(15)
            };

            var titleLabel = new Label
            {
                Text = "Event Flags Configuration",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 200, 100),
                AutoSize = true,
                Location = new Point(15, 10)
            };

            var subtitleLabel = new Label
            {
                Text = "Add, edit, or remove event flags to monitor",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.LightGray,
                AutoSize = true,
                Location = new Point(15, 40)
            };

            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(subtitleLabel);

            // Content Panel with padding
            var contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(20, 20, 20),
                Padding = new Padding(10)
            };

            // DataGridView for flags
            flagsGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.FromArgb(20, 20, 20),
                BorderStyle = BorderStyle.None,
                GridColor = Color.FromArgb(60, 60, 60),
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(30, 30, 30),
                    ForeColor = Color.White,
                    SelectionBackColor = Color.FromArgb(0, 120, 215),
                    SelectionForeColor = Color.White,
                    Font = new Font("Segoe UI", 10)
                },
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(40, 40, 40),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    SelectionBackColor = Color.FromArgb(40, 40, 40)
                },
                EnableHeadersVisualStyles = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            contentPanel.Controls.Add(flagsGrid);

            // Define columns
            flagsGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Flag",
                HeaderText = "Flag ID",
                DataPropertyName = "Flag",
                FillWeight = 30
            });

            flagsGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Token",
                HeaderText = "Token Name",
                DataPropertyName = "Token",
                FillWeight = 70
            });

            // Button Panel
            var buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 80,
                BackColor = Color.FromArgb(40, 40, 40),
                Padding = new Padding(15)
            };

            // Top row buttons (Add/Delete)
            addButton = new Button
            {
                Text = "Add",
                Location = new Point(15, 10),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            addButton.FlatAppearance.BorderColor = Color.FromArgb(0, 100, 195);
            addButton.Click += AddButton_Click;

            deleteButton = new Button
            {
                Text = "Delete",
                Location = new Point(125, 10),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(180, 50, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            deleteButton.FlatAppearance.BorderColor = Color.FromArgb(160, 40, 40);
            deleteButton.Click += DeleteButton_Click;

            // Bottom row buttons (Save/Cancel)
            saveButton = new Button
            {
                Text = "Save",
                Location = new Point(buttonPanel.Width - 230, 45),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(0, 150, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            saveButton.FlatAppearance.BorderColor = Color.FromArgb(0, 130, 70);
            saveButton.Click += SaveButton_Click;

            cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(buttonPanel.Width - 120, 45),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            cancelButton.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
            cancelButton.Click += CancelButton_Click;

            buttonPanel.Controls.Add(addButton);
            buttonPanel.Controls.Add(deleteButton);
            buttonPanel.Controls.Add(saveButton);
            buttonPanel.Controls.Add(cancelButton);

            // Add controls to form
            Controls.Add(contentPanel);
            Controls.Add(buttonPanel);
            Controls.Add(headerPanel);
        }

        private void RefreshGrid()
        {
            flagsGrid.Rows.Clear();
            foreach (var flag in flags)
            {
                flagsGrid.Rows.Add(flag.Flag, flag.Token);
            }
        }

        private void AddButton_Click(object? sender, EventArgs e)
        {
            using var addDialog = new AddFlagDialog();
            if (addDialog.ShowDialog() == DialogResult.OK)
            {
                var newFlag = new FlagEntry
                {
                    Flag = addDialog.FlagId,
                    Token = addDialog.TokenName
                };
                flags.Add(newFlag);
                RefreshGrid();
            }
        }

        private void DeleteButton_Click(object? sender, EventArgs e)
        {
            if (flagsGrid.SelectedRows.Count == 0)
            {
                Program.ShowMessageSilent("Please select a flag to delete.", "No Selection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selectedIndex = flagsGrid.SelectedRows[0].Index;
            var flag = flags[selectedIndex];

            var result = Program.ShowMessageSilent(
                $"Are you sure you want to delete flag {flag.Flag} ({flag.Token})?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                flags.RemoveAt(selectedIndex);
                RefreshGrid();
            }
        }

        private void SaveButton_Click(object? sender, EventArgs e)
        {
            // Update flags from grid (in case user edited directly)
            flags.Clear();
            foreach (DataGridViewRow row in flagsGrid.Rows)
            {
                if (row.Cells[0].Value != null && row.Cells[1].Value != null)
                {
                    if (uint.TryParse(row.Cells[0].Value.ToString(), out uint flagId))
                    {
                        flags.Add(new FlagEntry
                        {
                            Flag = flagId,
                            Token = row.Cells[1].Value.ToString() ?? ""
                        });
                    }
                }
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void CancelButton_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }

    public class AddFlagDialog : Form
    {
        private TextBox flagIdTextBox = null!;
        private TextBox tokenTextBox = null!;
        private Button okButton = null!;
        private Button cancelButton = null!;

        public uint FlagId { get; private set; }
        public string TokenName { get; private set; } = "";

        public AddFlagDialog()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            Text = "Add New Flag";
            Size = new Size(400, 200);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.FromArgb(30, 30, 30);

            // Create tooltip
            var toolTip = new ToolTip
            {
                BackColor = Color.FromArgb(50, 50, 50),
                ForeColor = Color.White
            };

            // Flag ID Label
            var flagIdLabel = new Label
            {
                Text = "Flag ID:",
                Location = new Point(20, 25),
                Size = new Size(100, 20),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10)
            };

            // Flag ID TextBox
            flagIdTextBox = new TextBox
            {
                Location = new Point(130, 23),
                Size = new Size(240, 25),
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            toolTip.SetToolTip(flagIdTextBox, "Enter the numeric flag ID to monitor\n(e.g., 1000, 2500, etc.)");

            // Token Label
            var tokenLabel = new Label
            {
                Text = "Token Name:",
                Location = new Point(20, 65),
                Size = new Size(100, 20),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10)
            };

            // Token TextBox
            tokenTextBox = new TextBox
            {
                Location = new Point(130, 63),
                Size = new Size(240, 25),
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            toolTip.SetToolTip(tokenTextBox, "Enter a descriptive name for this flag\n(e.g., 'Boss Defeated', 'Item Collected')");

            // OK Button
            okButton = new Button
            {
                Text = "OK",
                Location = new Point(190, 120),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            okButton.FlatAppearance.BorderColor = Color.FromArgb(0, 100, 195);
            okButton.Click += OkButton_Click;

            // Cancel Button
            cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(280, 120),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            cancelButton.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
            cancelButton.Click += CancelButton_Click;

            // Add controls
            Controls.Add(flagIdLabel);
            Controls.Add(flagIdTextBox);
            Controls.Add(tokenLabel);
            Controls.Add(tokenTextBox);
            Controls.Add(okButton);
            Controls.Add(cancelButton);

            AcceptButton = okButton;
            CancelButton = cancelButton;
        }

        private void OkButton_Click(object? sender, EventArgs e)
        {
            if (!uint.TryParse(flagIdTextBox.Text, out uint flagId))
            {
                Program.ShowMessageSilent("Please enter a valid numeric flag ID.", "Invalid Input",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(tokenTextBox.Text))
            {
                Program.ShowMessageSilent("Please enter a token name.", "Invalid Input",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            FlagId = flagId;
            TokenName = tokenTextBox.Text.Trim();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void CancelButton_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
