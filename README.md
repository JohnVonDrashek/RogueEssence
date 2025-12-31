# RogueEssence

[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-purple)](https://dotnet.microsoft.com/)
[![FNA](https://img.shields.io/badge/FNA-XNA%20Compatible-green)](https://fna-xna.github.io/)
[![License](https://img.shields.io/badge/License-MIT-blue)](LICENSE)

A Pokemon Mystery Dungeon-style roguelike game engine built with C#, FNA, and Lua scripting. RogueEssence provides a complete framework for creating turn-based dungeon crawlers with procedural generation, real-time overworld exploration, and extensive modding support.

## Features

- **Turn-Based Dungeon Gameplay** - Classic Mystery Dungeon mechanics with grid-based movement and combat
- **Procedural Generation** - Flexible dungeon generation system via [RogueElements](https://github.com/audinowho/RogueElements)
- **Real-Time Overworld** - Hub areas with AABB collision and scripted NPCs
- **Lua Scripting** - Full game logic customization via NLua
- **Avalonia Editor** - Visual map and data editors for content creation
- **Multiplayer Support** - P2P trading, rescue mail, and matchmaking server
- **Mod Support** - JSON-based data with patch system for mods

## Prerequisites

```bash
# Initialize submodules (FNA, NLua, RogueElements)
git submodule update --init --recursive

# Install .NET 6 runtime (required for WaypointServer)
brew install dotnet@6

# Optional: Add alias to ~/.zshrc
echo 'alias dotnet6="/opt/homebrew/opt/dotnet@6/bin/dotnet"' >> ~/.zshrc
```

## Build & Run

```bash
# Build entire solution
dotnet build RogueEssence.sln

# Run multiplayer server (requires .NET 6)
/opt/homebrew/opt/dotnet@6/bin/dotnet run --project WaypointServer
```

> **Note:** RogueEssence is an engine library. To run the full game + editor, use [PMDODump](https://github.com/audinowho/PMDODump):

```bash
# Clone the full game
git clone --recursive https://github.com/audinowho/PMDODump ~/code/PMDODump

# Build and run
cd ~/code/PMDODump
dotnet run --project PMDC/PMDC/PMDC.csproj
```

## Project Structure

```
RogueEssence/
├── RogueEssence/                    # Core engine library
│   ├── Content/                     # Graphics, audio, asset loading
│   ├── Data/                        # Game data (monsters, items, skills)
│   ├── Dungeon/                     # Turn-based dungeon gameplay
│   ├── Ground/                      # Real-time overworld system
│   ├── LevelGen/                    # Procedural floor generation
│   ├── Lua/                         # Lua scripting bindings
│   ├── Menu/                        # UI menu system
│   ├── Network/                     # P2P trading and rescue
│   └── Scene/                       # Scene management
├── RogueEssence.Editor.Avalonia/    # Visual editor (MVVM)
│   ├── DataEditor/                  # Generic data editing
│   ├── ViewModels/                  # Editor logic
│   └── Views/                       # AXAML UI definitions
├── WaypointServer/                  # Multiplayer matchmaking server
├── FNA/                             # XNA-compatible framework (submodule)
├── NLua/                            # Lua scripting engine (submodule)
└── RogueElements/                   # Procedural generation (submodule)
```

## Documentation

Each directory contains a detailed README.md. Key starting points:

| Document | Description |
|----------|-------------|
| [CLAUDE.md](CLAUDE.md) | AI-optimized project context and architecture |
| [RogueEssence/README.md](RogueEssence/README.md) | Core engine overview |
| [RogueEssence/Dungeon/README.md](RogueEssence/Dungeon/README.md) | Dungeon gameplay system |
| [RogueEssence/LevelGen/README.md](RogueEssence/LevelGen/README.md) | Procedural generation |
| [RogueEssence/Lua/README.md](RogueEssence/Lua/README.md) | Scripting API |
| [RogueEssence.Editor.Avalonia/README.md](RogueEssence.Editor.Avalonia/README.md) | Editor guide |
| [WaypointServer/README.md](WaypointServer/README.md) | Server setup and protocol |

## Architecture

### Singleton Managers

```csharp
DataManager.InitInstance();
GameManager.Instance.Begin();
DungeonScene.Instance.EnterFloor(0);
```

### Coroutine System

```csharp
yield return CoroutineManager.Instance.StartCoroutine(SomeAction());
yield return new WaitForFrames(30);
```

### Scene Flow

`GameManager.CurrentScene` cycles between:
- `SplashScene` → `TitleScene` → `GroundScene` ↔ `DungeonScene`

## File Formats

| Extension | Purpose |
|-----------|---------|
| `.json` | Data files (monsters, items, skills) |
| `.jsonpatch` | Mod patches |
| `.rsmap` | Dungeon map |
| `.rsground` | Ground/overworld map |
| `.rssv` | Save file |
| `.rsrec` | Replay recording |
| `.sosmail` / `.aokmail` | Rescue mail |

## Dependencies

- **[FNA](https://fna-xna.github.io/)** - XNA-compatible game framework
- **[NLua](https://github.com/NLua/NLua)** - Lua scripting for .NET
- **[RogueElements](https://github.com/audinowho/RogueElements)** - Procedural dungeon generation
- **[LiteNetLib](https://github.com/RevenantX/LiteNetLib)** - UDP networking
- **[Avalonia](https://avaloniaui.net/)** - Cross-platform UI framework

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

Third-party licenses are documented in the [Licenses/](Licenses/) directory.

---

![Repobeats analytics](https://repobeats.axiom.co/api/embed/placeholder.svg "Repobeats analytics image")
