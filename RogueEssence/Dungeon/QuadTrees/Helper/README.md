# Helper

Utility functions for quadtree operations including rectangle calculations and spatial math.

## Key Files

| File | Description |
|------|-------------|
| `RectangleHelper.cs` | Rectangle intersection and containment utilities |

## Relationships

- Used by **Common/** for spatial calculations
- Provides helper methods for bounds checking

## Usage

```csharp
// Check if rectangles intersect
bool intersects = RectangleHelper.Intersects(rect1, rect2);
```
