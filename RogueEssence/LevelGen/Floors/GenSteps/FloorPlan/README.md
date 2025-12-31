# FloorPlan

Grid-based room layout generation steps. These steps create the fundamental room and corridor structure of dungeon floors using RogueElements grid algorithms.

## Key Files

| File | Description |
|------|-------------|
| `GridPathBranchSpread.cs` | Branching path layout with spread parameter |
| `GridPathTiered.cs` | Multi-tier floor layout for large dungeons |
| `AddLargeRoomStep.cs` | Adds oversized rooms spanning multiple grid cells |
| `CombineGridRoomStep.cs` | Combines adjacent grid cells into larger rooms |
| `ICombineGridRoomStep.cs` | Interface for room combination steps |
| `MarkAsHallStep.cs` | Marks rooms as hallways for spawn exclusion |

## Relationships

- Uses RogueElements grid path algorithms
- Produces grid layouts consumed by **Rooms/** generators
- Combined rooms affect **Spawning/** entity placement

## Usage

```csharp
// Create branching path layout
var pathStep = new GridPathBranchSpread<MapGenContext>();
pathStep.RoomRatio = 70;
pathStep.BranchRatio = 50;
floorGen.GenSteps.Add(priority, pathStep);

// Add large combined rooms
floorGen.GenSteps.Add(priority, new CombineGridRoomStep<MapGenContext>(rate));
```
