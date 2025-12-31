# Rand

Random spawn selection and probability utilities. Provides weighted random selection for items, enemies, and other spawnable content.

## Key Files

| File | Description |
|------|-------------|
| `SpawnDict.cs` | Dictionary mapping spawn IDs to weighted probabilities |
| `SpawnRangeDict.cs` | Spawn dictionary with floor range restrictions |
| `ISpawnDict.cs` | Interface for spawn dictionaries |
| `ISpawnRangeDict.cs` | Interface for range-based spawn dictionaries |
| `CategorySpawnChooser.cs` | Chooser that selects from categories then items |
| `RandDecay.cs` | Decaying probability for spawn frequency |

## Relationships

- Used by **Spawning/** for random entity selection
- Configured in **Data/ZoneData** for each dungeon
- Supports floor-range filtering for progression

## Usage

```csharp
// Create spawn table with weights
SpawnDict<string> itemSpawns = new SpawnDict<string>();
itemSpawns.Add("potion", 100);  // Common
itemSpawns.Add("elixir", 10);   // Rare

// Select random item
string item = itemSpawns.Pick(rand);

// Range-based spawns (floors 3-7)
SpawnRangeDict<string> rangeSpawns = new SpawnRangeDict<string>();
rangeSpawns.Add("strong_enemy", new IntRange(3, 8), 50);
```
