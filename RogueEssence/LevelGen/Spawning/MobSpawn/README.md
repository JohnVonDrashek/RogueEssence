# MobSpawn

Enemy and NPC spawn definitions. Defines what monsters spawn and with what properties (level, moves, items, etc.).

## Key Files

| File | Description |
|------|-------------|
| `MobSpawn.cs` | Base mob spawn class with species, level, and modifiers |
| `MobSpawnExtra.cs` | Additional spawn properties (held items, status, etc.) |
| `MobSpawnCheck.cs` | Condition checks for conditional spawning |

## Relationships

- Used by **TeamSpawner/** for team composition
- Configured in **Data/ZoneData** spawn tables
- Creates **Dungeon/Characters/** instances

## Usage

```csharp
// Define a mob spawn
MobSpawn spawn = new MobSpawn();
spawn.BaseForm = new MonsterID("rattata", 0, 0, Gender.Male);
spawn.Level = new RandRange(3, 6);

// Add extra properties
spawn.Intrinsic = "run_away";
spawn.SpawnFeatures.Add(new MobSpawnItem("oran_berry"));
```
