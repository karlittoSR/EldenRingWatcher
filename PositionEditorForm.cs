using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SoulMemory;
using SoulMemory.EldenRing;

namespace EldenRingWatcher
{
    public class PositionEditorForm : Form
    {
        private DataGridView positionsGrid = null!;
        private Button addButton = null!;
        private Button deleteButton = null!;
        private Button saveButton = null!;
        private Button cancelButton = null!;
        private List<PositionEntry> positions = new();

        public class PositionEntry
        {
            public string Token { get; set; } = "";
            public string Map { get; set; } = "";
            public float X { get; set; }
            public float Y { get; set; }
            public float Z { get; set; }
            public float Radius { get; set; }
        }

        public List<PositionEntry> Positions
        {
            get => positions;
            set
            {
                positions = new List<PositionEntry>(value);
                RefreshGrid();
            }
        }

        public PositionEditorForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            // Form properties
            Text = "Edit Position Splits";
            Size = new Size(900, 500);
            MinimumSize = new Size(800, 400);
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
                Text = "Position Splits Configuration",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 200, 100),
                AutoSize = true,
                Location = new Point(15, 10)
            };

            var subtitleLabel = new Label
            {
                Text = "Add, edit, or remove position-based splits for Elden Ring",
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

            // DataGridView for positions
            positionsGrid = new DataGridView
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
                    Font = new Font("Segoe UI", 9)
                },
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(40, 40, 40),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    SelectionBackColor = Color.FromArgb(40, 40, 40)
                },
                EnableHeadersVisualStyles = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                SelectionMode = DataGridViewSelectionMode.CellSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            // Cell editing
            positionsGrid.CellDoubleClick += PositionsGrid_CellDoubleClick;
            positionsGrid.CellEndEdit += PositionsGrid_CellEndEdit;

            contentPanel.Controls.Add(positionsGrid);

            // Define columns
            positionsGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Token",
                HeaderText = "Token",
                DataPropertyName = "Token",
                FillWeight = 25
            });

            positionsGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Map",
                HeaderText = "Map",
                DataPropertyName = "Map",
                FillWeight = 20
            });

            positionsGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "X",
                HeaderText = "X",
                DataPropertyName = "X",
                FillWeight = 13
            });

            positionsGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Y",
                HeaderText = "Y",
                DataPropertyName = "Y",
                FillWeight = 13
            });

            positionsGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Z",
                HeaderText = "Z",
                DataPropertyName = "Z",
                FillWeight = 13
            });

            positionsGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Radius",
                HeaderText = "Radius",
                DataPropertyName = "Radius",
                FillWeight = 13
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
                Text = "Add Position",
                Location = new Point(15, 10),
                Size = new Size(110, 30),
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
                Text = "Delete Position",
                Location = new Point(135, 10),
                Size = new Size(110, 30),
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
                Text = "Save Changes",
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
            positionsGrid.Rows.Clear();
            // Sort by Token alphabetically
            var sortedPositions = positions.OrderBy(p => p.Token).ToList();
            foreach (var pos in sortedPositions)
            {
                positionsGrid.Rows.Add(pos.Token, pos.Map, pos.X, pos.Y, pos.Z, pos.Radius);
            }
            // Update the positions list to match the sorted order
            positions = sortedPositions;
        }

        private void PositionsGrid_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            // Allow double-click to edit cell
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                positionsGrid.BeginEdit(selectAll: true);
            }
        }

        private void PositionsGrid_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
        {
            // Update the positions list when editing ends
            if (e.RowIndex >= 0 && e.RowIndex < positions.Count)
            {
                try
                {
                    var row = positionsGrid.Rows[e.RowIndex];
                    var pos = positions[e.RowIndex];
                    
                    switch (e.ColumnIndex)
                    {
                        case 0: // Token
                            pos.Token = row.Cells[0].Value?.ToString() ?? "";
                            break;
                        case 1: // Map
                            pos.Map = row.Cells[1].Value?.ToString() ?? "";
                            break;
                        case 2: // X
                            if (float.TryParse(row.Cells[2].Value?.ToString() ?? "", out var x))
                                pos.X = x;
                            break;
                        case 3: // Y
                            if (float.TryParse(row.Cells[3].Value?.ToString() ?? "", out var y))
                                pos.Y = y;
                            break;
                        case 4: // Z
                            if (float.TryParse(row.Cells[4].Value?.ToString() ?? "", out var z))
                                pos.Z = z;
                            break;
                        case 5: // Radius
                            if (float.TryParse(row.Cells[5].Value?.ToString() ?? "", out var radius))
                                pos.Radius = radius;
                            break;
                    }
                }
                catch { /* Ignore parse errors */ }
            }
        }

        private void AddButton_Click(object? sender, EventArgs e)
        {
            using var addDialog = new AddPositionDialog();
            if (addDialog.ShowDialog() == DialogResult.OK)
            {
                var newPos = new PositionEntry
                {
                    Token = addDialog.TokenName,
                    Map = addDialog.MapId,
                    X = addDialog.PosX,
                    Y = addDialog.PosY,
                    Z = addDialog.PosZ,
                    Radius = addDialog.PosRadius
                };
                positions.Add(newPos);
                RefreshGrid();
            }
        }

        private void DeleteButton_Click(object? sender, EventArgs e)
        {
            // In CellSelect mode, find the selected row from selected cells
            int selectedIndex = -1;
            if (positionsGrid.SelectedCells.Count > 0)
            {
                selectedIndex = positionsGrid.SelectedCells[0].RowIndex;
            }

            if (selectedIndex < 0)
            {
                ToastNotification.Show("Please select a position to delete.", ToastNotification.NotificationType.Info);
                return;
            }

            var pos = positions[selectedIndex];

            var result = MessageBox.Show(
                $"Are you sure you want to delete position '{pos.Token}' on map {pos.Map}?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                positions.RemoveAt(selectedIndex);
                RefreshGrid();
                ToastNotification.Show($"Position deleted: {pos.Token}", ToastNotification.NotificationType.Success);
            }
        }

        private void SaveButton_Click(object? sender, EventArgs e)
        {
            // Update positions from grid (in case user edited directly)
            // Columns: [0]=Token, [1]=Map, [2]=X, [3]=Y, [4]=Z, [5]=Radius
            positions.Clear();
            foreach (DataGridViewRow row in positionsGrid.Rows)
            {
                if (row.Cells[0].Value != null && row.Cells[1].Value != null)
                {
                    try
                    {
                        positions.Add(new PositionEntry
                        {
                            Token = row.Cells[0].Value?.ToString() ?? "",
                            Map = row.Cells[1].Value?.ToString() ?? "",
                            X = float.Parse(row.Cells[2].Value?.ToString() ?? "0"),
                            Y = float.Parse(row.Cells[3].Value?.ToString() ?? "0"),
                            Z = float.Parse(row.Cells[4].Value?.ToString() ?? "0"),
                            Radius = float.Parse(row.Cells[5].Value?.ToString() ?? "0")
                        });
                    }
                    catch
                    {
                        // Skip invalid rows
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

    public class AddPositionDialog : Form
    {
        private TextBox tokenTextBox = null!;
        private TextBox mapTextBox = null!;
        private TextBox xTextBox = null!;
        private TextBox yTextBox = null!;
        private TextBox zTextBox = null!;
        private TextBox radiusTextBox = null!;
        private Button getPositionButton = null!;
        private Button okButton = null!;
        private Button cancelButton = null!;
        private bool isFetchingPosition = false;

        public string TokenName { get; private set; } = "";
        public string MapId { get; private set; } = "";
        public float PosX { get; private set; }
        public float PosY { get; private set; }
        public float PosZ { get; private set; }
        public float PosRadius { get; private set; }

        public AddPositionDialog()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            Text = "Add New Position Split";
            Size = new Size(450, 500);
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

            int yPos = 20;
            int labelX = 20;
            int textBoxX = 130;
            int textBoxWidth = 290;
            int spacing = 40;

            // Token
            AddLabelAndTextBox("Token Name:", ref tokenTextBox, ref yPos, labelX, textBoxX, textBoxWidth, spacing,
                toolTip, "Descriptive name for this position split\n(e.g., 'Boss Arena', 'Treasure Chest')");
            
            // Map
            AddLabelAndTextBox("Map ID:", ref mapTextBox, ref yPos, labelX, textBoxX, textBoxWidth, spacing,
                toolTip, "Map location in format: m##_##_##_##\n(e.g., m60_00_00_00)");

            // X
            AddLabelAndTextBox("X Position:", ref xTextBox, ref yPos, labelX, textBoxX, textBoxWidth, spacing,
                toolTip, "X coordinate in the world\n(floating point number)");

            // Y
            AddLabelAndTextBox("Y Position:", ref yTextBox, ref yPos, labelX, textBoxX, textBoxWidth, spacing,
                toolTip, "Y coordinate in the world\n(floating point number)");

            // Z
            AddLabelAndTextBox("Z Position:", ref zTextBox, ref yPos, labelX, textBoxX, textBoxWidth, spacing,
                toolTip, "Z coordinate in the world\n(floating point number)");

            // Radius
            AddLabelAndTextBox("Radius:", ref radiusTextBox, ref yPos, labelX, textBoxX, textBoxWidth, spacing,
                toolTip, "Detection radius around the position\n(must be greater than 0)");
            
            // Initialize radius with default value
            radiusTextBox.Text = "3";

            // GET POSITION Button
            getPositionButton = new Button
            {
                Text = "GET POSITION",
                Location = new Point(20, yPos + 10),
                Size = new Size(400, 30),
                BackColor = Color.FromArgb(0, 150, 200),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            getPositionButton.FlatAppearance.BorderColor = Color.FromArgb(0, 120, 170);
            getPositionButton.Click += GetPositionButton_Click;
            toolTip.SetToolTip(getPositionButton, "Click to retrieve your current in-game position\n(Elden Ring must be running with player loaded)\nWaits 1 second after game loads to ensure stability");
            Controls.Add(getPositionButton);

            yPos += 50;
            okButton = new Button
            {
                Text = "OK",
                Location = new Point(240, yPos),
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
                Location = new Point(330, yPos),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            cancelButton.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
            cancelButton.Click += CancelButton_Click;

            Controls.Add(okButton);
            Controls.Add(cancelButton);

            AcceptButton = okButton;
            CancelButton = cancelButton;
        }

        private void AddLabelAndTextBox(string labelText, ref TextBox textBox, ref int yPos, 
            int labelX, int textBoxX, int textBoxWidth, int spacing, ToolTip toolTip, string? tooltip = null)
        {
            var label = new Label
            {
                Text = labelText,
                Location = new Point(labelX, yPos + 2),
                Size = new Size(100, 20),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10)
            };

            textBox = new TextBox
            {
                Location = new Point(textBoxX, yPos),
                Size = new Size(textBoxWidth, 25),
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            if (tooltip != null)
            {
                toolTip.SetToolTip(textBox, tooltip);
            }

            Controls.Add(label);
            Controls.Add(textBox);

            yPos += spacing;
        }

        private void GetPositionButton_Click(object? sender, EventArgs e)
        {
            // Prevent multiple concurrent calls with double-check
            if (isFetchingPosition || !getPositionButton.Enabled)
                return;

            isFetchingPosition = true;
            getPositionButton.Enabled = false;
            getPositionButton.Text = "Fetching position...";
            Application.DoEvents(); // Force UI update

            try
            {
                try
                {
                    var er = new EldenRing();
                    
                    // Check 1: Can we connect to the game? Retry up to 5 times with delay
                    bool connected = false;
                    for (int attempt = 0; attempt < 5; attempt++)
                    {
                        var refreshResult = er.TryRefresh();
                        if (refreshResult.IsOk)
                        {
                            connected = true;
                            break;
                        }
                        if (attempt < 4) // Don't sleep on last attempt
                        {
                            System.Threading.Thread.Sleep(200);
                        }
                    }

                    if (!connected)
                    {
                        ToastNotification.Show("Failed to connect to Elden Ring. Make sure the game is running and try again.", ToastNotification.NotificationType.Error);
                        return;
                    }

                    // Check 2: Is player loaded?
                    if (!er.IsPlayerLoaded())
                    {
                        ToastNotification.Show("Player is not loaded. Please ensure your character is in-game.", ToastNotification.NotificationType.Warning);
                        return;
                    }

                    // Check 3: Are we in the game world?
                    if (er.GetScreenState() != ScreenState.InGame)
                    {
                        ToastNotification.Show("You are not in-game. Please exit menus and be in the game world.", ToastNotification.NotificationType.Warning);
                        return;
                    }

                    // Check 4: Is blackscreen active?
                    if (er.IsBlackscreenActive())
                    {
                        ToastNotification.Show("Blackscreen is active. Please wait for the game to fully load.", ToastNotification.NotificationType.Warning);
                        return;
                    }

                    // Check 5: Wait a moment to ensure game is fully loaded before reading position
                    // This prevents crashes during the player load transition
                    System.Threading.Thread.Sleep(500);

                    // Try to get position - wrapped in try-catch
                    Position? pos = null;
                    try
                    {
                        pos = er.GetPosition();
                    }
                    catch (Exception getPosEx)
                    {
                        ToastNotification.Show($"Failed to read position: {getPosEx.Message}. Try again in a moment.", ToastNotification.NotificationType.Error);
                        return;
                    }

                    if (pos == null)
                    {
                        ToastNotification.Show("Could not retrieve position. Try again.", ToastNotification.NotificationType.Error);
                        return;
                    }

                    // Format map ID from position components
                    string mapId = $"m{pos.Area:x2}_{pos.Block:x2}_{pos.Region:x2}_{pos.Size:x2}";
                    
                    // Populate fields with current position
                    mapTextBox.Text = mapId;
                    xTextBox.Text = pos.X.ToString("F3");
                    yTextBox.Text = pos.Y.ToString("F3");
                    zTextBox.Text = pos.Z.ToString("F3");
                    
                    // Set radius to 3 if not already set
                    if (string.IsNullOrWhiteSpace(radiusTextBox.Text) || radiusTextBox.Text == "0")
                    {
                        radiusTextBox.Text = "3";
                    }

                    ToastNotification.Show($"Position retrieved: {mapId}", ToastNotification.NotificationType.Success, 2000);
                }
                catch (Exception ex)
                {
                    // Catch any unexpected errors
                    ToastNotification.Show($"Unexpected error: {ex.Message}", ToastNotification.NotificationType.Error);
                }
            }
            finally
            {
                isFetchingPosition = false;
                getPositionButton.Enabled = true;
                getPositionButton.Text = "GET POSITION";
                Application.DoEvents(); // Force UI update
            }
        }

        private void OkButton_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tokenTextBox.Text))
            {
                ToastNotification.Show("Please enter a token name.", ToastNotification.NotificationType.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(mapTextBox.Text))
            {
                ToastNotification.Show("Please enter a map ID.", ToastNotification.NotificationType.Warning);
                return;
            }

            if (!float.TryParse(xTextBox.Text, out float x))
            {
                ToastNotification.Show("Please enter a valid X position.", ToastNotification.NotificationType.Warning);
                return;
            }

            if (!float.TryParse(yTextBox.Text, out float y))
            {
                ToastNotification.Show("Please enter a valid Y position.", ToastNotification.NotificationType.Warning);
                return;
            }

            if (!float.TryParse(zTextBox.Text, out float z))
            {
                ToastNotification.Show("Please enter a valid Z position.", ToastNotification.NotificationType.Warning);
                return;
            }

            if (!float.TryParse(radiusTextBox.Text, out float radius) || radius <= 0)
            {
                ToastNotification.Show("Please enter a valid radius (must be greater than 0).", ToastNotification.NotificationType.Warning);
                return;
            }

            TokenName = tokenTextBox.Text.Trim();
            MapId = mapTextBox.Text.Trim();
            PosX = x;
            PosY = y;
            PosZ = z;
            PosRadius = radius;

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
