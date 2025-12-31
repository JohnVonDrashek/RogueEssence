# AABB

Axis-Aligned Bounding Box collision detection system for ground scenes. Provides real-time collision detection and response for smooth character movement.

## Key Files

| File | Description |
|------|-------------|
| `World.cs` | Main collision world managing all collidable entities |
| `IWorld.cs` | Interface for collision worlds |
| `Box.cs` | AABB box representing a collidable entity |
| `IBox.cs` | Interface for collidable boxes |
| `Hit.cs` | Collision hit result with contact information |
| `IHit.cs` | Interface for collision results |
| `Collision.cs` | Collision detection algorithms |
| `ICollision.cs` | Interface for collision detection |
| `Grid.cs` | Spatial grid for broad-phase collision |
| `GridWorld.cs` | Grid-based collision world |
| `GridBlockWorld.cs` | Grid world with tile blocking |
| `Movement.cs` | Movement data structure |
| `IMovement.cs` | Interface for movement requests |

## Subdirectories

| Directory | Purpose |
|-----------|---------|
| `Responses/` | Collision response behaviors |

## Relationships

- Used by **GroundChar** for movement collision
- **Ground/Maps/** provides collidable geometry
- Enables smooth sliding along walls and obstacles

## Usage

```csharp
// Create a collision box for an entity
Box box = new Box(position, width, height);
world.Add(box);

// Move with collision response
Hit hit = world.Move(box, velocity, response);
```
