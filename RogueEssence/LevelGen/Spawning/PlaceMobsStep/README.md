# PlaceMobsStep

Enemy placement algorithms determining where spawned enemies are positioned on the floor. Different strategies for different dungeon styles.

## Key Files

| File | Description |
|------|-------------|
| `PlaceMobsStep.cs` | Base class for mob placement steps |
| `PlaceRandomMobsStep.cs` | Random placement across walkable tiles |
| `PlaceDisconnectedMobsStep.cs` | Places mobs in disconnected regions |
| `PlaceNearSpawnableMobsStep.cs` | Places mobs near specific spawn points |
| `PlaceRadiusMobsStep.cs` | Places mobs within radius of a point |
| `PlaceTerrainMobsStep.cs` | Places mobs on specific terrain types |
| `PlaceNoLocMobsStep.cs` | Places mobs without location restrictions |

## Relationships

- Uses **MultiTeamSpawner/** for team creation
- Places into **MapGenContext** floor
- Respects room filters from **Rooms/**

## Usage

```csharp
// Random mob placement
var placeStep = new PlaceRandomMobsStep<MapGenContext>(spawner);
floorGen.GenSteps.Add(placeStep);

// Terrain-specific placement (water monsters in water)
var waterStep = new PlaceTerrainMobsStep<MapGenContext>(spawner, "water");
```
