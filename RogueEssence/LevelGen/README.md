# LevelGen

Procedural level generation system using the RogueElements library. Generates dungeon floors with rooms, corridors, items, enemies, and traps through a step-based pipeline.

## Key Files

| File | Description |
|------|-------------|
| `MapGenContext.cs` | Map generation context containing the in-progress map and all generation state |
| `IFloorGen.cs` | Interface for floor generation with step pipeline |
| `IPostProcMap.cs` | Interface for post-processing map features |
| `PostProcTile.cs` | Post-processing tile markers |
| `RangeDict.cs` | Dictionary with range-based key lookups |

## Subdirectories

| Directory | Purpose |
|-----------|---------|
| `Floors/` | Floor generation steps for room layout and structure |
| `Rand/` | Random spawn selection and probability utilities |
| `Spawning/` | Entity spawning systems for items, enemies, and traps |
| `Tiles/` | Tile placement and terrain generation |
| `Zones/` | Zone-wide generation configuration |

## Relationships

- Uses **RogueElements** library for base generation algorithms
- Produces **Dungeon/Maps/Map** instances
- Configured by **Data/ZoneData** for each dungeon
- Uses **Data/** for item, monster, and tile definitions

## Architecture

Generation follows a step-based pipeline:
1. Zone steps configure floor range settings
2. Floor steps generate room layout
3. Tile steps fill terrain details
4. Spawn steps place items, enemies, and traps

## Usage

```csharp
// Generate a floor from zone data
IFloorGen floorGen = zoneData.Floors[floorIndex];
Map map = floorGen.Generate(rand, zoneContext);

// Add a custom generation step
floorGen.GenSteps.Add(new PlaceRandomMobsStep<MapGenContext>());
```
