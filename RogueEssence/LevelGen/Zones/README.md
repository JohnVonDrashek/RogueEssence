# Zones

Zone-level generation configuration for dungeon-wide settings. Defines properties that apply across multiple floors of a dungeon.

## Key Files

| File | Description |
|------|-------------|
| `ZoneSegment.cs` | Segment of floors with shared generation settings |

## Subdirectories

| Directory | Purpose |
|-----------|---------|
| `ZoneSteps/` | Zone-wide generation steps |

## Relationships

- Configured in **Data/ZoneData**
- Contains floor ranges with shared settings
- Applies zone steps before floor generation

## Usage

```csharp
// Define a zone segment (floors 1-10)
ZoneSegment segment = new ZoneSegment(1, 10);
segment.ZoneSteps.Add(new ItemSpawnZoneStep(spawnTable));
```
