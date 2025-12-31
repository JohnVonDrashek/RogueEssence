# TeamSpawner

Team composition spawners creating enemy teams with one or more members. Defines how individual enemy groups are composed.

## Key Files

| File | Description |
|------|-------------|
| `TeamSpawner.cs` | Base team spawner interface |
| `PoolTeamSpawner.cs` | Spawns from a weighted pool of mob definitions |
| `SpecificTeamSpawner.cs` | Spawns a specific predefined team |

## Relationships

- Uses **MobSpawn/** for individual enemy definitions
- Used by **MultiTeamSpawner/** for batch spawning
- Creates **Dungeon/Team** instances

## Usage

```csharp
// Pool-based team spawning
var teamSpawner = new PoolTeamSpawner<MapGenContext>();
teamSpawner.Spawns.Add(mobSpawn1, 100);
teamSpawner.Spawns.Add(mobSpawn2, 50);

// Specific team (boss room)
var bossSpawner = new SpecificTeamSpawner(bossMob);
```
