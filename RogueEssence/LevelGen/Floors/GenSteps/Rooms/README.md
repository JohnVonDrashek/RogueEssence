# Rooms

Room shape generators and room filtering utilities. Defines various room shapes and selection criteria for dungeon room generation.

## Key Files

| File | Description |
|------|-------------|
| `RoomGenLoadMap.cs` | Loads pre-designed room layouts from map files |
| `RoomGenDiamond.cs` | Diamond-shaped room generator |
| `RoomGenPlus.cs` | Plus/cross-shaped room generator |
| `RoomGenTriangle.cs` | Triangle-shaped room generator |
| `RoomGenCoated.cs` | Room with decorative border coating |
| `RoomGenPostProcSpecific.cs` | Room with specific post-processing |
| `RoomFilterIndex.cs` | Filters rooms by index for specific placement |
| `IndexRoom.cs` | Room with specific index marker |
| `ImmutableRoom.cs` | Room that cannot be modified by later steps |
| `ColumnHallBrush.cs` | Hallway brush that adds column decorations |

## Relationships

- Used by **FloorPlan/** steps for room creation
- **RoomGenLoadMap** references map files from **Data/**
- Filters control which rooms receive items/enemies

## Usage

```csharp
// Use diamond-shaped rooms
var roomGen = new RoomGenDiamond<MapGenContext>(minSize, maxSize);

// Load pre-made room layout
var roomGen = new RoomGenLoadMap<MapGenContext>("special_room");

// Filter rooms for special spawns
var filter = new RoomFilterIndex(true, 0); // Only first room
```
