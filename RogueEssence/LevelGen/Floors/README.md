# Floors

Floor layout generation containing generation steps that define room arrangement, corridor connections, and overall floor structure.

## Subdirectories

| Directory | Purpose |
|-----------|---------|
| `GenSteps/` | Individual generation steps for floor creation |

## Relationships

- Used by **LevelGen/IFloorGen** implementations
- Steps defined in **GenSteps/** are executed in priority order
- Produces floor layouts consumed by **Spawning/**

## Usage

```csharp
// Floor generation is handled through GenSteps
FloorPlan floorPlan = new FloorPlan();
floorPlan.GenSteps.Add(priority, new GridPathBranch<MapGenContext>());
```
