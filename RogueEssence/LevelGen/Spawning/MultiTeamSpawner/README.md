# MultiTeamSpawner

Multi-team spawning patterns for creating multiple enemy groups. Handles batch spawning of several teams across the floor.

## Key Files

| File | Description |
|------|-------------|
| `IMultiTeamSpawner.cs` | Interface for multi-team spawners |
| `LoopedTeamSpawner.cs` | Spawns teams in a loop until count reached |
| `LoopedRandTeamSpawner.cs` | Random selection with looped spawning |
| `PresetMultiTeamSpawner.cs` | Fixed preset team list spawner |
| `TeamContextSpawner.cs` | Context-aware team spawning |

## Relationships

- Used by **PlaceMobsStep/** for enemy group placement
- Contains **TeamSpawner/** for individual team creation
- Configured per-floor in generation

## Usage

```csharp
// Spawn 5-8 teams using a looped spawner
var multiSpawner = new LoopedTeamSpawner<MapGenContext>(teamSpawner);
multiSpawner.SpawnCount = new RandRange(5, 9);
```
