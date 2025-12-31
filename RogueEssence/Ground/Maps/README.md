# Maps

Ground map structure and entities. Contains the GroundMap class and all entity types that can be placed in ground/overworld areas.

## Key Files

| File | Description |
|------|-------------|
| `GroundMap.cs` | Main ground map class with entities, collision, and scripts |
| `GroundEntity.cs` | Base class for all ground map entities |
| `GroundObject.cs` | Interactive object in ground maps (signs, chests, etc.) |
| `GroundSpawner.cs` | Character spawn point definition |
| `GroundMarker.cs` | Invisible marker for scripting locations |
| `GroundWall.cs` | Collision wall entity |
| `GroundAnim.cs` | Decorative animation entity |
| `EntityLayer.cs` | Layer system for rendering order |
| `AnimLayer.cs` | Animation layer for decorations |

## Relationships

- Used by **GroundScene** for gameplay
- **GroundChar** instances are spawned at **GroundSpawner** locations
- **GroundObject** entities trigger **Lua/** scripts
- **AABB/** provides collision for GroundWall

## Usage

```csharp
// Load a ground map
GroundMap map = DataManager.Instance.GetGround(mapName);

// Add an entity
GroundObject obj = new GroundObject(position, "door");
map.AddObject(obj);

// Find spawner by name
GroundSpawner spawn = map.GetSpawner("player_start");
```
