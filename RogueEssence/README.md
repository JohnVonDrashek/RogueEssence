# RogueEssence

The core engine for RogueEssence, a roguelike dungeon crawler game framework built on MonoGame/FNA. This directory contains the main game logic, rendering systems, data management, and all subsystems that power the game.

## Key Files

| File | Description |
|------|-------------|
| `GameBase.cs` | Main game class inheriting from MonoGame's `Game`, handles initialization, update loop, and rendering |
| `DiagManager.cs` | Diagnostic and settings manager - handles logging, input recording/replay, and configuration |
| `PathMod.cs` | Mod system and path resolution - manages mod loading, namespaces, and file path fallbacks |
| `Settings.cs` | User settings and preferences storage including audio, display, and input configurations |
| `Text.cs` | Localization and text formatting system for multi-language support |
| `FrameInput.cs` | Input handling structure for keyboard, gamepad, and mouse input per frame |
| `FrameTick.cs` | Frame-based timing utilities for consistent game updates |
| `Primitives2D.cs` | 2D primitive drawing utilities for debug rendering |
| `InputManager.cs` | High-level input management and input state tracking |
| `CollectionExt.cs` | Extension methods for collections |
| `Versioning.cs` | Game version information |

## Subdirectories

| Directory | Purpose |
|-----------|---------|
| `Content/` | Graphics, audio, and asset management systems |
| `Data/` | Game data definitions, serialization, and the DataManager |
| `Dev/` | Development tools, editor support, and import helpers |
| `Dungeon/` | Dungeon gameplay systems, characters, maps, and turn-based logic |
| `Ground/` | Ground/overworld scene systems with AABB collision |
| `LevelGen/` | Procedural level generation using RogueElements |
| `Lua/` | Lua scripting engine integration via MoonSharp |
| `Menu/` | UI menu system with dialogue, settings, and game menus |
| `Network/` | Multiplayer networking and rescue mail systems |
| `Properties/` | Assembly information |
| `Scene/` | Scene management, game states, and coroutine system |

## Relationships

- **RogueElements**: External library providing dungeon generation algorithms
- **MonoGame/FNA**: Game framework for cross-platform graphics and input
- **MoonSharp**: Lua scripting engine for game logic customization
- **Newtonsoft.Json**: JSON serialization for data files

## Architecture Overview

The engine follows a scene-based architecture managed by `GameManager`:
1. `GameBase` initializes the MonoGame framework and starts background loading
2. `DiagManager` loads settings and initializes logging
3. `DataManager` loads game data (monsters, items, skills, zones)
4. Scenes (`DungeonScene`, `GroundScene`, `TitleScene`) handle specific game states
5. `MenuManager` overlays UI on top of active scenes

## Usage

```csharp
// The game is initialized via GameBase
// Main entry point creates and runs the game
using (GameBase game = new GameBase())
{
    game.Run();
}
```
