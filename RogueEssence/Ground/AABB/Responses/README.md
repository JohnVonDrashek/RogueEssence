# Responses

Collision response behaviors defining how entities react when colliding with obstacles. Different responses create different movement feels.

## Key Files

| File | Description |
|------|-------------|
| `ICollisionResponse.cs` | Interface for collision response handlers |
| `SlideResponse.cs` | Slide along obstacles on collision |
| `BounceResponse.cs` | Bounce off obstacles on collision |
| `TouchResponse.cs` | Stop on first collision contact |
| `CrossResponse.cs` | Pass through obstacles (trigger only) |

## Relationships

- Used by **AABB** collision system
- **GroundChar** uses SlideResponse for player movement
- Different responses for different entity types

## Usage

```csharp
// Move with slide response (default for characters)
ICollisionResponse response = new SlideResponse();
world.Move(box, velocity, response);

// Move with bounce response (for projectiles)
world.Move(box, velocity, new BounceResponse());
```
