using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Windows.Forms;
using SoulMemory;
using SoulMemory.EldenRing;

namespace EldenRingWatcher
{
    class Program
    {
        static Config config = null!;
        static string SignalFile = null!;
        static string LatestSignalFile = null!;
        static MainForm mainForm = null!;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            mainForm = new MainForm();
            
            // Set main form reference for toast notifications
            ToastNotification.SetMainForm(mainForm);
            
            // Setup button actions
            mainForm.SetClearLogsAction(ClearLogs);
            mainForm.SetEditConfigAction(EditConfig);
            mainForm.SetSetFlagAction(SetFlag);
            mainForm.SetSetPositionAction(SetPosition);

            // Load initial configuration
            if (!LoadConfiguration())
            {
                MessageBox.Show("Failed to load configuration. Please check config.json.", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Start monitoring in background thread
            var monitorThread = new Thread(MonitorGameLoop)
            {
                IsBackground = true
            };
            monitorThread.Start();

            Application.Run(mainForm);
        }

        static bool LoadConfiguration()
        {
            string configPath = Path.Combine(AppContext.BaseDirectory, "config.json");
            
            if (!File.Exists(configPath))
            {
                mainForm?.AppendLog($"[INFO] Configuration file not found, creating default...");
                if (!CreateDefaultConfiguration(configPath))
                {
                    mainForm?.UpdateStatus("Failed to create config", System.Drawing.Color.Red);
                    return false;
                }
            }

            try
            {
                var json = File.ReadAllText(configPath);
                config = JsonSerializer.Deserialize<Config>(json, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                })!;
                
                // Setup log file paths
                SignalFile = Path.GetFullPath(Path.Combine(config.Settings.LogsPath, "events.txt"));
                LatestSignalFile = Path.GetFullPath(Path.Combine(config.Settings.LogsPath, "latest.json"));
                Directory.CreateDirectory(Path.GetDirectoryName(SignalFile)!);

                mainForm?.AppendLog($"[CONFIG] Loaded {config.EventFlags.Count} event flags");
                mainForm?.AppendLog($"[CONFIG] Loaded {config.PositionSplits.Count} position splits");
                mainForm?.AppendLog($"[CONFIG] Poll: {config.Settings.PollIntervalMs}ms | Debounce: {config.Settings.DebounceMs}ms");
                mainForm?.UpdateConfig($"{config.EventFlags.Count} flags, {config.PositionSplits.Count} positions");
                mainForm?.UpdateStatus("Configuration loaded", System.Drawing.Color.LightGreen);
                
                return true;
            }
            catch (Exception ex)
            {
                mainForm?.AppendLog($"[ERROR] Failed to load config: {ex.Message}");
                mainForm?.UpdateStatus("Configuration error", System.Drawing.Color.Red);
                return false;
            }
        }

        static void ReloadConfig()
        {
            mainForm.AppendLog("[INFO] Reloading configuration...");
            if (LoadConfiguration())
            {
                mainForm.AppendLog("[INFO] Configuration reloaded successfully");
            }
        }

        static bool CreateDefaultConfiguration(string configPath)
        {
            try
            {
                var defaultConfig = new Config
                {
                    Settings = new Settings
                    {
                        PollIntervalMs = 200,
                        DebounceMs = 6000,
                        LogsPath = "./logs"
                    },
                    EventFlags = new List<EventFlagConfig>
                    {
                        new EventFlagConfig { Flag = 11007420, Token = "EXALTED_FLESH" },
                        new EventFlagConfig { Flag = 16007210, Token = "SOMBER" },
                        new EventFlagConfig { Flag = 172, Token = "RADAHN" },
                        new EventFlagConfig { Flag = 171, Token = "GODRICK" }
                    },
                    PositionSplits = new List<PosSplitConfig>
                    {
                        new PosSplitConfig 
                        { 
                            Token = "POS_Example1", 
                            Map = "m0b_00_00_00", 
                            X = 0.0f, 
                            Y = 0.0f, 
                            Z = 0.0f, 
                            Radius = 3.0f 
                        }
                    }
                };

                var options = new JsonSerializerOptions 
                { 
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                var json = JsonSerializer.Serialize(defaultConfig, options);
                File.WriteAllText(configPath, json);
                mainForm?.AppendLog($"[INFO] Created default configuration at {configPath}");
                return true;
            }
            catch (Exception ex)
            {
                mainForm?.AppendLog($"[ERROR] Failed to create default config: {ex.Message}");
                return false;
            }
        }

        static void ClearLogs()
        {
            SafeTruncateLogs();
            mainForm.AppendLog("[INFO] Log files cleared");
        }

        static void EditConfig()
        {
            try
            {
                var settingsEditor = new SettingsEditorForm();
                
                // Load current settings
                settingsEditor.LoadSettings(
                    config.Settings.PollIntervalMs,
                    config.Settings.DebounceMs,
                    config.Settings.LogsPath
                );

                // Show dialog
                if (settingsEditor.ShowDialog() == DialogResult.OK)
                {
                    // Update config with new settings
                    var newConfig = config with
                    {
                        Settings = config.Settings with
                        {
                            PollIntervalMs = settingsEditor.PollIntervalMs,
                            DebounceMs = settingsEditor.DebounceMs,
                            LogsPath = settingsEditor.LogsPath
                        }
                    };

                    // Save to file
                    string configPath = Path.Combine(AppContext.BaseDirectory, "config.json");
                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    };
                    File.WriteAllText(configPath, JsonSerializer.Serialize(newConfig, options));

                    mainForm.AppendLog($"[INFO] Settings updated: Poll={settingsEditor.PollIntervalMs}ms, Debounce={settingsEditor.DebounceMs}ms");
                    
                    // Reload config
                    ReloadConfig();
                }
            }
            catch (Exception ex)
            {
                mainForm.AppendLog($"[ERROR] Failed to edit settings: {ex.Message}");
                MessageBox.Show($"Failed to edit settings: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        static void SetFlag()
        {
            try
            {
                var flagEditor = new FlagEditorForm();
                
                // Load current flags
                flagEditor.Flags = config.EventFlags.Select(f => new FlagEditorForm.FlagEntry
                {
                    Flag = f.Flag,
                    Token = f.Token
                }).ToList();

                // Show dialog
                if (flagEditor.ShowDialog() == DialogResult.OK)
                {
                    // Update config with new flags
                    var newConfig = config with
                    {
                        EventFlags = flagEditor.Flags.Select(f => new EventFlagConfig
                        {
                            Flag = f.Flag,
                            Token = f.Token
                        }).ToList()
                    };

                    SaveConfigToFile(newConfig);
                    mainForm.AppendLog($"[INFO] Saved {flagEditor.Flags.Count} event flags to config");
                }
            }
            catch (Exception ex)
            {
                mainForm.AppendLog($"[ERROR] Failed to edit flags: {ex.Message}");
                MessageBox.Show($"Failed to edit flags: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        static void SetPosition()
        {
            try
            {
                var positionEditor = new PositionEditorForm();
                
                // Load current positions
                positionEditor.Positions = config.PositionSplits.Select(p => new PositionEditorForm.PositionEntry
                {
                    Token = p.Token,
                    Map = p.Map,
                    X = p.X,
                    Y = p.Y,
                    Z = p.Z,
                    Radius = p.Radius
                }).ToList();

                // Show dialog
                if (positionEditor.ShowDialog() == DialogResult.OK)
                {
                    // Update config with new positions
                    var newConfig = config with
                    {
                        PositionSplits = positionEditor.Positions.Select(p => new PosSplitConfig
                        {
                            Token = p.Token,
                            Map = p.Map,
                            X = p.X,
                            Y = p.Y,
                            Z = p.Z,
                            Radius = p.Radius
                        }).ToList()
                    };

                    SaveConfigToFile(newConfig);
                    mainForm.AppendLog($"[INFO] Saved {positionEditor.Positions.Count} position splits to config");
                }
            }
            catch (Exception ex)
            {
                mainForm.AppendLog($"[ERROR] Failed to edit positions: {ex.Message}");
                MessageBox.Show($"Failed to edit positions: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        static void MonitorGameLoop()
        {
            mainForm.AppendLog("[INFO] Starting game monitor...");
            mainForm.UpdateStatus("Waiting for Elden Ring...", System.Drawing.Color.Yellow);

            var er = new EldenRing();
            var seenFlags = new HashSet<uint>();
            var lastFireByToken = new Dictionary<string, DateTime>();
            var wasInsideByToken = new Dictionary<string, bool>();

            bool wasAttached = false;
            bool prevGameStarts = false;

            while (true)
            {
                try
                {
                    var rr = er.TryRefresh();

                    bool attached = rr.IsOk && er.IsPlayerLoaded();
                    if (attached && !wasAttached)
                    {
                        mainForm.AppendLog("[INFO] Player is in game");
                        mainForm.UpdateStatus("In Game - Monitoring", System.Drawing.Color.LightGreen);
                    }
                    else if (!attached && wasAttached)
                    {
                        mainForm.AppendLog("[INFO] Waiting for player to enter the game...");
                        mainForm.UpdateStatus("Game Running - Waiting for player", System.Drawing.Color.Yellow);
                    }
                    wasAttached = attached;

                    if (attached)
                    {
                        // Reset on new run
                        bool gameStarts = er.ReadEventFlag((uint)KnownFlag.GameStarts);
                        if (gameStarts && !prevGameStarts)
                        {
                            mainForm.AppendLog("[RESET] Fresh run detected - Cleaning logs");
                            SafeTruncateLogs();
                            seenFlags.Clear();
                            lastFireByToken.Clear();
                            wasInsideByToken.Clear();
                        }
                        prevGameStarts = gameStarts;

                        // Event flags
                        foreach (var flagConfig in config.EventFlags)
                        {
                            uint flag = flagConfig.Flag;
                            string token = flagConfig.Token;

                            bool isSet = er.ReadEventFlag(flag);
                            if (!isSet) continue;

                            if (!seenFlags.Contains(flag))
                            {
                                var last = lastFireByToken.TryGetValue(token, out var t) ? t : DateTime.MinValue;
                                if ((DateTime.UtcNow - last).TotalMilliseconds > config.Settings.DebounceMs)
                                {
                                    AppendSignal(new EventLine
                                    {
                                        ts = DateTime.UtcNow,
                                        token = token,
                                        flag = flag
                                    });
                                    lastFireByToken[token] = DateTime.UtcNow;

                                    mainForm.AppendLog($"[TRIGGER] {token} (flag {flag})");
                                }

                                seenFlags.Add(flag);
                            }
                        }

                        // Position splits monitoring disabled to maintain stability
                        // All flags monitoring continues to work normally
                    }

                    Thread.Sleep(config.Settings.PollIntervalMs);
                }
                catch (Exception ex)
                {
                    mainForm.AppendLog($"[ERROR] {ex.Message}");
                    Thread.Sleep(1000);
                }
            }
        }

        static void AppendSignal(EventLine ev)
        {
            try
            {
                // Ensure directory exists before writing
                Directory.CreateDirectory(Path.GetDirectoryName(SignalFile)!);
                
                var json = JsonSerializer.Serialize(ev);
                File.AppendAllText(SignalFile, json + Environment.NewLine);
                File.WriteAllText(LatestSignalFile, json);
            }
            catch (Exception ex)
            {
                mainForm?.AppendLog($"[ERROR] Failed to write signal: {ex.Message}");
            }
        }

        static void SafeTruncateLogs()
        {
            try
            {
                // Ensure directory exists before writing
                Directory.CreateDirectory(Path.GetDirectoryName(SignalFile)!);
                
                File.WriteAllText(SignalFile, string.Empty);
                File.WriteAllText(LatestSignalFile, "{}");
            }
            catch (Exception ex)
            {
                mainForm?.AppendLog($"[ERROR] Failed to truncate logs: {ex.Message}");
            }
        }

        record EventLine
        {
            public DateTime ts { get; init; }
            public string token { get; init; } = "";
            public uint flag { get; init; }
        }

        record Config
        {
            [JsonPropertyName("settings")]
            public Settings Settings { get; init; } = new();
            
            [JsonPropertyName("eventFlags")]
            public List<EventFlagConfig> EventFlags { get; init; } = new();
            
            [JsonPropertyName("positionSplits")]
            public List<PosSplitConfig> PositionSplits { get; init; } = new();
        }

        record Settings
        {
            [JsonPropertyName("pollIntervalMs")]
            public int PollIntervalMs { get; init; } = 200;
            
            [JsonPropertyName("debounceMs")]
            public int DebounceMs { get; init; } = 6000;
            
            [JsonPropertyName("logsPath")]
            public string LogsPath { get; init; } = "./logs";
        }

        record EventFlagConfig
        {
            [JsonPropertyName("flag")]
            public uint Flag { get; init; }
            
            [JsonPropertyName("token")]
            public string Token { get; init; } = "";
        }

        record PosSplitConfig
        {
            [JsonPropertyName("token")]
            public string Token { get; init; } = "";
            
            [JsonPropertyName("map")]
            public string Map { get; init; } = "";
            
            [JsonPropertyName("x")]
            public float X { get; init; }
            
            [JsonPropertyName("y")]
            public float Y { get; init; }
            
            [JsonPropertyName("z")]
            public float Z { get; init; }
            
            [JsonPropertyName("radius")]
            public float Radius { get; init; }

            public bool IsInside(Position p)
            {
                var (a, b, r, s) = ParseMap(Map);
                if (!MapEquals(p, a, b, r, s)) return false;
                return Dist3D(p, X, Y, Z) <= Radius;
            }
        }

        static void SaveConfigToFile(Config newConfig)
        {
            try
            {
                string configPath = Path.Combine(AppContext.BaseDirectory, "config.json");
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                File.WriteAllText(configPath, JsonSerializer.Serialize(newConfig, options));
                config = newConfig;
                ReloadConfig();
            }
            catch (Exception ex)
            {
                mainForm.AppendLog($"[ERROR] Failed to save config: {ex.Message}");
            }
        }

        static (byte area, byte block, byte region, byte size) ParseMap(string m)
        {
            var s = m.AsSpan();
            if (s.Length != 12 || s[0] != 'm' || s[3] != '_' || s[6] != '_' || s[9] != '_')
                throw new ArgumentException($"Invalid map format: {m}");

            byte a = Convert.ToByte(s.Slice(1, 2).ToString(), 16);
            byte b = Convert.ToByte(s.Slice(4, 2).ToString(), 16);
            byte r = Convert.ToByte(s.Slice(7, 2).ToString(), 16);
            byte z = Convert.ToByte(s.Slice(10, 2).ToString(), 16);
            return (a, b, r, z);
        }

        static bool MapEquals(Position p, byte a, byte b, byte r, byte s)
            => p.Area == a && p.Block == b && p.Region == r && p.Size == s;

        static double Dist3D(Position p, float x, float y, float z)
        {
            double dx = p.X - x, dy = p.Y - y, dz = p.Z - z;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }
    }
}
