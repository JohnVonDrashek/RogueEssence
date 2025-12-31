# Common

Core quadtree implementation providing the fundamental spatial partitioning algorithm. This is the shared foundation for all quadtree variants.

## Key Files

| File | Description |
|------|-------------|
| `QuadTreeCommon.cs` | Main quadtree class with insert, remove, and query operations |
| `QuadTreeNodeCommon.cs` | Quadtree node with subdivision and object management |
| `QuadTreeObject.cs` | Wrapper for objects stored in the quadtree |

## Relationships

- Extended by **QTreePoint/** for point-based queries
- Used by **Dungeon/Maps/** for entity management
- Provides efficient O(log n) spatial queries

## Usage

```csharp
// The common implementation is used through specific quadtree types
QuadTreeCommon<T> tree = new QuadTreeCommon<T>(bounds);
tree.Insert(obj, objBounds);
List<T> results = tree.Query(queryBounds);
```
