# Ground

Ground/overworld scene system for non-dungeon gameplay areas. Provides real-time movement, NPC interaction, and exploration in towns and hub areas using AABB collision detection.

## Key Files

| File | Description |
|------|-------------|
| `GroundScene.cs` | Main ground scene handling input, updates, and rendering |
| `BaseGroundScene.cs` | Base class for ground scenes with common functionality |
| `GSceneMap.cs` | Ground scene map management |
| `GSceneZone.cs` | Ground scene zone/area transitions |
| `GroundChar.cs` | Ground character entity with physics and animation |
| `GroundAction.cs` | Character actions in ground mode (walk, interact, etc.) |
| `GroundAI.cs` | Base AI interface for ground NPCs |
| `GroundAIUser.cs` | AI controller implementation |
| `GroundScriptedAI.cs` | Lua script-driven ground AI |
| `GroundTask.cs` | Task-based AI behavior system |
| `BaseTaskUser.cs` | Base class for task-executing entities |
| `GroundContext.cs` | Context for ground interactions |
| `GroundDebug.cs` | Debug utilities for ground mode |
| `GroundItemEvent.cs` | Item interaction events |

## Subdirectories

| Directory | Purpose |
|-----------|---------|
| `AABB/` | Axis-aligned bounding box collision system |
| `Maps/` | Ground map entities and structure |

## Relationships

- Uses **Content/** for rendering characters and maps
- **Lua/** scripts drive cutscenes and NPC behavior
- **Menu/** displays dialogue and interaction UI
- Transitions to **Dungeon/** when entering dungeons
- **Data/GameProgress** tracks unlocked areas

## Usage

```csharp
// The ground scene is a singleton
GroundScene.Instance.ProcessInput(input);

// Move a character
groundChar.Move(direction, speed);

// Interact with an object
yield return groundChar.Interact(target);
```
