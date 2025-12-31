# Data

Game data management system including all game content definitions, serialization, save/load functionality, and the central DataManager. This module defines the data structures for monsters, items, skills, zones, and manages game progress.

## Key Files

| File | Description |
|------|-------------|
| `DataManager.cs` | Central data manager - loads/saves all game data, manages caches, handles saves |
| `Serializer.cs` | JSON serialization utilities with type handling |
| `GameProgress.cs` | Player save data including team, inventory, dungeon progress, and unlocks |
| `MonsterData.cs` | Monster species definitions with stats, types, and abilities |
| `MonsterForm.cs` | Monster form variants (different forms of same species) |
| `SkillData.cs` | Skill/move definitions with effects and battle data |
| `ItemData.cs` | Item definitions with use effects and properties |
| `IntrinsicData.cs` | Passive ability definitions |
| `StatusData.cs` | Status condition definitions and effects |
| `MapStatusData.cs` | Map-wide status effects (weather, etc.) |
| `ZoneData.cs` | Dungeon zone definitions with floor generation settings |
| `TerrainData.cs` | Terrain type definitions (water, lava, walls, etc.) |
| `TileData.cs` | Tile effect definitions (traps, stairs, etc.) |
| `ElementData.cs` | Type/element definitions with effectiveness chart |
| `GrowthData.cs` | Experience growth rate curves |
| `AutoTileData.cs` | Autotile definitions for map rendering |
| `EmoteData.cs` | Emote bubble configurations |
| `RankData.cs` | Exploration rank definitions |
| `SkinData.cs` | Shiny/alternate skin definitions |
| `BattleData.cs` | Battle action data structures |
| `ActiveEffect.cs` | Active effect event definitions |
| `PassiveData.cs` | Passive effect definitions |
| `EntryDataIndex.cs` | Index of all data entries by type |
| `IEntryData.cs` | Interface for all data entry types |
| `BaseData.cs` | Base class for data entries |
| `LearnableSkill.cs` | Skill learning requirements |
| `PromoteBranch.cs` | Evolution/promotion definitions |
| `ReplayData.cs` | Replay recording data structure |
| `RecordHeaderData.cs` | Save file header metadata |
| `StartParams.cs` | Game start configuration |
| `SkillGroupData.cs` | Skill category groupings |

## Subdirectories

| Directory | Purpose |
|-----------|---------|
| `AI/` | AI behavior definitions and tactics |
| `Rescue/` | Rescue mail data structures |

## Relationships

- **DataManager** is accessed throughout the engine to get game content
- **Dungeon/** uses data definitions for gameplay mechanics
- **LevelGen/** references ZoneData for procedural generation
- **Menu/** displays data to players
- **Lua/** can access and modify data through scripting

## Usage

```csharp
// Get monster data
MonsterData monster = DataManager.Instance.GetMonster("bulbasaur");

// Get skill data
SkillData skill = DataManager.Instance.GetSkill("tackle");

// Save game progress
DataManager.Instance.SaveMainGameState();

// Load a zone
ZoneData zone = DataManager.Instance.GetZone("test_dungeon");
```
