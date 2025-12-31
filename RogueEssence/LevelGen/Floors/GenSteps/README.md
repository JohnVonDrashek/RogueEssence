# GenSteps

Floor generation steps implementing specific aspects of dungeon floor creation. Each step handles one part of generation (layout, textures, items, mobs, etc.).

## Key Files

| File | Description |
|------|-------------|
| `MapTextureStep.cs` | Applies tileset textures to floor terrain |
| `MapNameIDStep.cs` | Sets floor name and identification |
| `MapEventStep.cs` | Adds map event handlers |
| `MapExtraStatusStep.cs` | Adds persistent map statuses |
| `ItemSpawnStep.cs` | Places items on the floor |
| `MobSpawnStep.cs` | Configures enemy spawn tables |
| `MoneySpawnStep.cs` | Places money drops |
| `TileSpawnStep.cs` | Places traps and effect tiles |
| `MappedRoomStep.cs` | Places pre-designed room layouts |
| `ScriptGenStep.cs` | Executes Lua scripts during generation |
| `DetourStep.cs` | Creates detour paths in hallways |

## Subdirectories

| Directory | Purpose |
|-----------|---------|
| `FloorPlan/` | Grid and room layout generation steps |
| `Rooms/` | Room shape generators and filters |
| `Tiles/` | Terrain and tile manipulation steps |

## Relationships

- Steps are added to **IFloorGen** with priority ordering
- Use **MapGenContext** for accessing generation state
- Reference **Data/** for spawn tables and configurations

## Usage

```csharp
// Add item spawning to a floor
floorGen.GenSteps.Add(new ItemSpawnStep<MapGenContext>(spawnTable));

// Add texture step for tileset
floorGen.GenSteps.Add(new MapTextureStep<MapGenContext>(tileset, wall, secondary));
```
