# QuadTrees

Spatial partitioning data structures for efficient entity queries. QuadTrees enable fast lookup of characters and objects within a region without checking every entity.

## Key Files

| File | Description |
|------|-------------|
| `QuadTreePoint.cs` | Point-based quadtree for entity positions |

## Subdirectories

| Directory | Purpose |
|-----------|---------|
| `Common/` | Core quadtree implementation shared by all variants |
| `Helper/` | Utility functions for quadtree operations |
| `QTreePoint/` | Point-based quadtree node implementation |

## Relationships

- Used by **Maps/** for spatial entity queries
- **Characters/** are stored in quadtrees for collision detection
- **Hitbox** queries use quadtrees for area effects

## Usage

```csharp
// Query entities in a rectangle
var results = quadTree.GetObjects(boundingRect);

// Insert an entity
quadTree.Insert(character);

// Remove an entity
quadTree.Remove(character);
```
