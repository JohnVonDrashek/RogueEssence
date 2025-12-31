# QTreePoint

Point-based quadtree implementation for entities represented as single points. Optimized for character and item position queries.

## Key Files

| File | Description |
|------|-------------|
| `QuadTreePointNode.cs` | Point quadtree node with point-specific query logic |
| `IPointQuadStorable.cs` | Interface for objects that can be stored by point position |

## Relationships

- Extends **Common/** quadtree implementation
- Used by **Maps/** for character position queries
- Objects implement `IPointQuadStorable` to be stored

## Usage

```csharp
// Objects must implement IPointQuadStorable
public class Character : IPointQuadStorable
{
    public Loc Position { get; }
}

// Query characters at a point
Character found = pointTree.GetObjectAt(position);
```
