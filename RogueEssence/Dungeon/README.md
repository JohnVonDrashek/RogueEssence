# Dungeon

Core dungeon gameplay system including turn-based combat, character management, map handling, and the main dungeon scene. This is the heart of the roguelike gameplay loop.

## Key Files

| File | Description |
|------|-------------|
| `DungeonScene.cs` | Main dungeon scene handling gameplay loop, input, and rendering |
| `BaseDungeonScene.cs` | Base class for dungeon scenes with common functionality |
| `DSceneAction.cs` | Dungeon scene action processing and battle flow |
| `DSceneMap.cs` | Dungeon scene map management and visibility |
| `DSceneZone.cs` | Dungeon scene zone/floor transitions |
| `Zone.cs` | Dungeon zone containing multiple floors |
| `ZoneManager.cs` | Manages active zones and floor state |
| `ZoneLoc.cs` | Zone location identifier (zone ID + floor number) |
| `SegLoc.cs` | Segment location within a floor |
| `Team.cs` | Party team management with members and inventory |
| `InvItem.cs` | Inventory item instance in team bag |
| `PickupItem.cs` | Item on the ground that can be picked up |
| `GameAction.cs` | Player action request structure |
| `ActionResult.cs` | Result of an action attempt |
| `Fov.cs` | Field of view calculation for visibility |
| `EXPGain.cs` | Experience point gain data |
| `DirRemap.cs` | Direction remapping utilities |
| `Intrinsic.cs` | Active intrinsic ability instance |
| `GameEventOwner.cs` | Event owner for battle event system |
| `IActionContext.cs` | Interface for action contexts |
| `MockActionContext.cs` | Mock context for testing |

## Subdirectories

| Directory | Purpose |
|-----------|---------|
| `Characters/` | Character entities, actions, and animations |
| `GameEffects/` | Battle events, status effects, and game mechanics |
| `Maps/` | Dungeon map data structures |
| `QuadTrees/` | Spatial partitioning for efficient queries |
| `Tiles/` | Tile definitions and autotiling |
| `Turns/` | Turn order and character indexing |

## Relationships

- Uses **Data/** for monster, skill, and item definitions
- Uses **Content/** for rendering characters and maps
- **LevelGen/** generates Map instances for this module
- **Lua/** scripts can control dungeon events
- **Menu/** overlays UI during gameplay

## Usage

```csharp
// The dungeon scene is a singleton
DungeonScene.Instance.ProcessPlayerInput(input);

// Access current map
Map map = ZoneManager.Instance.CurrentMap;

// Process a turn
yield return CoroutineManager.Instance.StartCoroutine(DungeonScene.Instance.ProcessTurn());
```
