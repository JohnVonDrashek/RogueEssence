# Spawning

Entity spawning systems for placing items, enemies, and interactive elements on generated floors. Handles both spawn selection and placement algorithms.

## Key Files

| File | Description |
|------|-------------|
| `BoxSpawner.cs` | Spawns items in box/container patterns |
| `BulkSpawner.cs` | Spawns multiple entities in bulk |
| `MoneyDivSpawner.cs` | Divides money spawns across the floor |
| `MoneySpawn.cs` | Money drop spawn definition |
| `MoneySpawnRange.cs` | Range-based money spawning |
| `MapGenEntrance.cs` | Entrance/exit placement |
| `DisconnectedSpawnStep.cs` | Spawns in disconnected floor regions |
| `NearSpawnableSpawnStep.cs` | Spawns near valid spawn locations |

## Subdirectories

| Directory | Purpose |
|-----------|---------|
| `MobSpawn/` | Enemy/NPC spawn definitions |
| `MultiTeamSpawner/` | Multi-team spawn patterns |
| `PlaceMobsStep/` | Enemy placement algorithms |
| `TeamSpawner/` | Team composition spawners |

## Relationships

- Uses **Rand/** for random selection
- Configured by **Floors/GenSteps/** spawning steps
- Places entities into **MapGenContext**

## Usage

```csharp
// Spawn items in boxes
var boxSpawner = new BoxSpawner<MapGenContext>(itemSpawnList);

// Place spawn step in floor generation
floorGen.GenSteps.Add(new ItemSpawnStep<MapGenContext>(spawner));
```
