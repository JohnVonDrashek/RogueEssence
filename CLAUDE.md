# RogueEssence

Pokemon Mystery Dungeon-style roguelike engine built with C#, FNA, and Lua scripting.

## Prerequisites

```bash
# Initialize submodules
git submodule update --init --recursive

# Install .NET 6 runtime (required for WaypointServer)
brew install dotnet@6

# Add alias to ~/.zshrc (optional but recommended)
echo 'alias dotnet6="/opt/homebrew/opt/dotnet@6/bin/dotnet"' >> ~/.zshrc
```

## Build & Run

```bash
# Build
dotnet build RogueEssence.sln

# Run multiplayer server (requires .NET 6)
/opt/homebrew/opt/dotnet@6/bin/dotnet run --project WaypointServer
# Or with alias: dotnet6 run --project WaypointServer
```

**Note:** RogueEssence and Editor are libraries, not standalone executables. To run the full game + editor, use [PMDODump](https://github.com/audinowho/PMDODump):

```bash
# Clone the full game (in ~/code)
git clone --recursive https://github.com/audinowho/PMDODump ~/code/PMDODump

# Build and run
cd ~/code/PMDODump
dotnet run --project PMDC/PMDC/PMDC.csproj
```

## Architecture

### Projects

| Project | Purpose |
|---------|---------|
| `RogueEssence/` | Core game engine library |
| `RogueEssence.Editor.Avalonia/` | Map/data editor (Avalonia UI) |
| `WaypointServer/` | Multiplayer matchmaking server |

### Dependencies

- **FNA** - XNA-compatible game framework (graphics, audio, input)
- **NLua** - Lua scripting engine
- **RogueElements** - Procedural dungeon generation library
- **LiteNetLib** - Networking

## Key Entry Points

| File | Purpose |
|------|---------|
| `GameBase.cs` | XNA Game class, bootstrap, main loop |
| `Scene/GameManager.cs` | Scene management, audio, transitions |
| `Data/DataManager.cs` | All game data (monsters, items, skills, zones) |
| `Dungeon/DungeonScene.cs` | Turn-based dungeon gameplay |
| `Ground/GroundScene.cs` | Real-time overworld gameplay |
| `Lua/LuaEngine.cs` | Script engine, event callbacks |

## Directory Guide

### RogueEssence/ (Core Engine)

| Directory | Purpose |
|-----------|---------|
| `Content/` | Graphics/sound asset loading (`GraphicsManager`, `SoundManager`) |
| `Data/` | Game data types (`MonsterData`, `SkillData`, `ItemData`, `ZoneData`) |
| `Dungeon/` | Turn-based dungeon system (characters, tiles, maps, turns, effects) |
| `Ground/` | Real-time overworld (AABB collision, ground maps, AI) |
| `LevelGen/` | Procedural floor generation (extends RogueElements) |
| `Lua/` | Script bindings (`ScriptGame`, `ScriptDungeon`, `ScriptUI`) |
| `Menu/` | All UI menus (dialogue, inventory, team, settings) |
| `Network/` | P2P trading, rescue mail |
| `Scene/` | Scene base classes, coroutine manager |
| `Dev/` | Editor integration, reflection utilities |

### Data Subsystem (`Data/`)

```
DataManager.DataType flags:
  Monster, Skill, Item, Intrinsic, Status, MapStatus,
  Terrain, Tile, Zone, Emote, AutoTile, Element,
  GrowthGroup, SkillGroup, AI, Rank, Skin
```

Data files: `Data/*.json`, Maps: `Data/Map/*.rsmap`, Grounds: `Data/Ground/*.rsground`

### Dungeon Subsystem (`Dungeon/`)

| Subdirectory | Purpose |
|--------------|---------|
| `Characters/` | `Character`, `CharAction`, `CharAnimation`, `StatusEffect` |
| `GameEffects/` | Battle events (`BattleContext`, `BattleEvent`, `GameEvent`) |
| `Maps/` | `Map`, `MapItem`, `MapStatus`, `MapLayer` |
| `Tiles/` | `Tile`, `AutoTile`, `EffectTile`, `TerrainTile` |
| `Turns/` | Turn order system (`TurnOrder`, `TurnState`) |
| `QuadTrees/` | Spatial indexing for entities |

### LevelGen Subsystem (`LevelGen/`)

| Subdirectory | Purpose |
|--------------|---------|
| `Floors/GenSteps/` | Floor generation steps |
| `Spawning/` | Item/enemy placement (`MobSpawn`, `TeamSpawner`) |
| `Tiles/` | Tile placement steps |
| `Zones/` | Zone segment configuration |

### Lua Scripting (`Lua/`)

Script bindings exposed to Lua:
- `ScriptGame` - Save/load, game flow
- `ScriptDungeon` - Dungeon mechanics
- `ScriptGround` - Ground map control
- `ScriptUI` - Menus, dialogue
- `ScriptSound` - Audio
- `ScriptStrings` - Localization

Map callbacks: `Init`, `Enter`, `Exit`, `Update`, `GameSave`, `GameLoad`
Zone callbacks: `Init`, `EnterSegment`, `ExitSegment`, `Rescued`

## Patterns & Conventions

### Singleton Managers
All major subsystems use `InitInstance()` / `Instance` pattern:
```csharp
DataManager.InitInstance();
GameManager.Instance.Begin();
DungeonScene.Instance.EnterFloor(0);
```

### Coroutine System
Game logic uses `IEnumerator<YieldInstruction>` coroutines:
```csharp
yield return CoroutineManager.Instance.StartCoroutine(SomeAction());
yield return new WaitForFrames(30);
```

### Scene Flow
`GameManager.CurrentScene` cycles between:
- `SplashScene` - Startup
- `TitleScene` - Main menu
- `GroundScene` - Overworld
- `DungeonScene` - Dungeons
- `*EditScene` - Editor modes

### Naming
- PascalCase for classes, methods, properties
- camelCase for local variables, parameters
- `I*` prefix for interfaces
- `*Data` suffix for data classes
- `*Scene` suffix for scene classes
- `*Manager` suffix for singleton managers

## Editor (Avalonia)

MVVM architecture:
- `Views/` - AXAML UI definitions
- `ViewModels/` - UI logic
- `DataEditor/` - Generic data editing components
- `Converters/` - XAML value converters

Key editors:
- `MapEditForm` - Dungeon map editor
- `GroundEditForm` - Ground map editor
- `DataListForm` - Data type browser

## Testing

Maps can be tested via editor without full game:
- `GameManager.TestWarp(mapName, isGround, seed)`
- `GameManager.DebugWarp(zoneLoc, seed)`

## File Formats

| Extension | Purpose |
|-----------|---------|
| `.json` | Data files (monsters, items, etc.) |
| `.jsonpatch` | Mod patches |
| `.rsmap` | Dungeon map |
| `.rsground` | Ground map |
| `.rssv` | Save file |
| `.rsqs` | Quicksave |
| `.rsrec` | Replay |
| `.sosmail` | Rescue request |
| `.aokmail` | Rescue response |
