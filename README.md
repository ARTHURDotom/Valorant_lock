# Valorant Auto Lock

A clean, simple Windows application that automatically locks in your chosen Valorant agent as soon as agent select starts.

## Features

- **Automatic Detection**: Detects when you enter agent select in Valorant
- **Per-Map Configuration**: Set a different agent for each map, or use one global agent
- **Smart Skipping**: If a map has no agent configured, it does nothing and lets you pick manually
- **Natural Timing**: Randomized delay (300-800ms by default) before locking to feel more human
- **Clean Dark UI**: Valorant-style dark theme interface
- **Settings Persistence**: All settings saved locally on your computer
- **Multi-Language**: English, French, Spanish, German
- **Single Executable**: No installation required, just run the .exe
- **System Tray**: Minimize to tray, auto-start minimized option
- **Session Stats**: Track how many agents you've locked this session

## How It Works

The app reads Valorant's local Riot Client API (via the lockfile at `%LOCALAPPDATA%\Riot Games\Riot Client\Config\lockfile`) to detect when you're in a pregame lobby. When agent select becomes active, it sends a lock request for your configured agent.

**No memory reading, no injection, no screen scraping** - uses only official local endpoints.

## Requirements

- Windows 10/11 (64-bit)
- Valorant installed and running
- .NET 8 Runtime (included in the single-file build)

## Usage

1. **Start Valorant** and get to the main menu
2. **Run `ValorantAutoLock.exe`**
3. **Configure your agents**:
   - **Global Mode**: Pick one agent to lock on all maps (or selected maps)
   - **Per Map Mode**: Assign a specific agent for each map. Maps with "No agent" are skipped
4. **Adjust delay** (optional): Min/Max delay before locking (default 300-800ms)
5. **Click "Arm"** - the app will now wait for a match
6. **Play!** - When you enter agent select, your agent will be locked automatically

## Settings

| Setting | Description |
|---------|-------------|
| Selection Mode | Global (one agent) or Per Map (different agent per map) |
| Default Agent | Agent to use in Global mode |
| Agent per Map | Assign agents to specific maps in Per Map mode |
| Min/Max Delay | Random delay range before locking (ms) |
| Select Before Lock | Send select request before lock (more reliable) |
| Auto Re-Arm | Automatically re-arm after match ends |
| Poll Interval | How often to check for matches (ms) |
| Language | UI language (EN/FR/ES/DE) |
| Start Minimized | Start app hidden in system tray |
| Minimize to Tray | Minimize to tray instead of taskbar |

## Building from Source

```bash
# Requires .NET 8 SDK
cd ValorantAutoLock
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

Output: `bin/Release/net8.0-windows/win-x64/publish/ValorantAutoLock.exe`

## Safety & Ban Risk

- Uses only Riot's official local HTTP API
- No memory manipulation, no DLL injection, no overlays
- Same method used by popular open-source tools (val-insta-locker, LOCKIN, V-Core)
- Randomized delay makes it appear more natural
- **Use at your own risk** - Riot's ToS prohibits automation, but this method has been used for years with extremely low ban rates

## Supported Maps (Current Rotation)

- Ascent, Bind, Haven, Icebox, Breeze, Fracture, Pearl, Lotus, Split, Sunset, Abyss, Summit

## Supported Agents (29 Total)

All current agents including Miks (released March 2026).

## License

MIT License - Free to use and modify.

## Credits

Built with inspiration from:
- [val-insta-locker](https://github.com/daniilsys/val-insta-locker) (Rust/Tauri)
- [LOCKIN](https://github.com/Ruchuee/LOCKIN) (Rust)
- [V-Core](https://github.com/haarisxk/V-Core) (Python)