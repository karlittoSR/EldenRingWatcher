# Elden Ring Watcher

A real-time game monitoring tool for Elden Ring speedrunners. Tracks event flags and player positions to automatically log game state changes for streaming integration and run analysis. Included with FPS Patch positions of the Death's Poker AnyM Glitchless route as an example.

## Features

- **Real-time Event Monitoring**: Tracks Elden Ring event flags (boss defeats, item pickups, key moments)
- **Position Tracking**: Monitors player position changes to detect location-based milestones
- **GET POSITION Button**: Automatically capture current player position with one click - retrieves map ID, X/Y/Z coordinates, and auto-fills form fields
- **Drag & Drop Reordering**: Reorder flags and positions by dragging rows in the editor windows
- **Toast Notifications**: Non-intrusive status messages for user feedback
- **Auto-save Configuration**: Settings automatically save when closing dialogs
- **Automatic Logging**: Outputs events to two files for integration with streaming tools:
  - `events.txt` - Complete history of all triggered events
  - `latest.json` - Latest event (ideal for file watchers like streamer.bot)
- **Easy Configuration**: User-friendly UI with dedicated editors for events, positions, and settings
- **No Installation Required**: Single self-contained executable

## Installation

1. Download `EldenRingWatcher.exe` from [Releases](https://github.com/karlittoSR/EldenRingWatcher/releases)
2. Place it in your desired folder (e.g., `C:\Games\EldenRingWatcher\`)
3. Run it - a default `config.json` will be created automatically on first run
4. Customize the configuration as needed using the UI or by editing config.json directly

## Usage

### Running the Application

Extract the package to whatever destination.
Simply double-click `EldenRingWatcher.exe`. 
- On first run, a default `config.json` will be created in the same folder with example event flags and position splits
- A `logs/` folder will be created for output files (events.txt and latest.json). You can change the destination of this folder in the app.
- The monitoring window will appear and start tracking game state in real-time

### Main Interface

- **Clear Logs**: Clears the log display (doesn't delete output files)
- **Set Config**: Opens settings editor for poll interval, debounce time, and log output path
- **Set Flag**: Opens editor to add/remove/modify event flags to track
- **Set Position**: Opens editor to add/remove/modify position-based milestones with **GET POSITION** button
- **ℹ️ Info**: Shows version and feature information

#### GET POSITION Button

The **GET POSITION** button in the Position Editor allows you to automatically capture your current in-game position:
1. Click **GET POSITION** while your character is at the desired location
2. The button validates that the game is running, your character is loaded, and you're in-game
3. Your current position is instantly retrieved and populated:
   - Map ID (formatted as m##_##_##_##)
   - X, Y, Z coordinates (3 decimal precision)
   - Radius automatically set to 3 (configurable)
4. Click **OK** to confirm and add the position split

### Configuration

Edit `config.json` to customize monitoring behavior:

```json
{
  "settings": {
    "pollIntervalMs": 200,        // How often to check game state (ms)
    "debounceMs": 6000,           // Minimum time between duplicate events (ms)
    "logsPath": "C:/logs"         // Where to save event logs
  },
  "eventFlags": [
    { "flag": 11007420, "token": "EXALTED_FLESH" },
    { "flag": 16007210, "token": "SOMBER" }
  ],
  "positionSplits": [
    {
      "token": "POS_Checkpoint1",
      "map": "m0b_00_00_00",
      "x": -0.277,
      "y": 6.094,
      "z": -43.115,
      "radius": 3.0
    }
  ]
}
```

**Parameters**:
- `pollIntervalMs`: Lower values = more responsive but higher CPU usage (recommended: 100-500ms)
- `debounceMs`: Prevents duplicate events from same flag within this time window
- `logsPath`: Path where `events.txt` and `latest.json` are written (default: `./logs` - relative to app folder)
- `eventFlags`: Elden Ring flag IDs (use SoulMemory library documentation for reference)
- `positionSplits`: Location-based triggers with map coordinates and detection radius

### Output Files

**events.txt**:
```
[2026-01-18 14:23:45] RADAHN
[2026-01-18 14:24:12] GODRICK
[2026-01-18 14:25:33] RUNE
```

**latest.json**:
```json
{
  "token": "RADAHN",
  "timestamp": "2026-01-18T14:23:45.1234567",
  "flag": 172
}
```

## Integration Examples

### streamer.bot
Configure a file watcher on `latest.json` to trigger actions (scene changes, sound effects, overlays) whenever a new event is detected.

### OBS
Use a script to read `events.txt` and display recent events as an overlay.

## Requirements

- Windows 10 or later (x64)
- .NET Runtime 7.0 (included in the executable)
- Elden Ring running !

## Troubleshooting

**Application won't start:**
- Ensure `config.json` exists in the same folder as the executable
- Try deleting `config.json` to regenerate it with defaults

**Events not being tracked:**
- Check that event flag IDs in config.json are correct
- Verify `logsPath` directory exists and is writable
- Increase `pollIntervalMs` if game is unresponsive

**Position tracking not working:**
- Confirm map ID and coordinates are accurate
- Increase `radius` value to make detection zone larger
- Adjust `debounceMs` to avoid duplicate triggers

## License

Free to use and distribute. Created for the speedrunning community.

## Credits

- Built with C# and Windows Forms
- Uses SoulMemory library for game memory access

---

**Version**: 3.6.1  
**Last Updated**: January 18, 2026

### Changelog

**v3.6.1 - Quality of Life Improvements**
- ✨ **Toast Notifications**: Non-intrusive status messages replace message boxes
- ✨ **Auto-save Config**: Settings save automatically when closing dialogs (no manual reload needed)
- ✨ **Drag & Drop Reordering**: Drag rows in flag and position lists to reorder entries
- 🔧 Code refactoring to reduce duplication

**v3.6.0 - Position Capture**
- ✨ GET POSITION button to capture player coordinates with one click
- ✨ Auto-population of map ID, X/Y/Z coordinates in position editor
- 🛠️ Enhanced validation for position capture (game attached, player loaded, in-game state, no blackscreen)
- 🔧 Single-file publishing configuration
