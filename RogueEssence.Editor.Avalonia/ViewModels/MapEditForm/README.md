# ViewModels/MapEditForm

## Description

Contains ViewModels for the dungeon map editor, used for creating procedurally-generated dungeon floors. Unlike ground maps, dungeon maps are designed for combat encounters with terrain, traps, items, and monster spawns. The editor provides extensive tabs for all dungeon elements.

## Key Files

| File | Description |
|------|-------------|
| `MapEditViewModel.cs` | Root ViewModel aggregating all dungeon editor tabs |
| `MapTabTexturesViewModel.cs` | Floor/wall texture painting with tileset browser |
| `MapTabDecorationsViewModel.cs` | Decoration layer management and painting |
| `MapTabTerrainViewModel.cs` | Terrain type assignment (ground, water, lava, wall) |
| `MapTabTilesViewModel.cs` | Effect tile placement (traps, triggers, warps) |
| `MapTabItemsViewModel.cs` | Item spawn placement and configuration |
| `MapTabEntitiesViewModel.cs` | Monster team placement and configuration |
| `MapTabEntrancesViewModel.cs` | Entry/exit point management |
| `MapTabSpawnsViewModel.cs` | Dynamic spawn table configuration |
| `MapTabEffectsViewModel.cs` | Map-wide effect configuration |
| `MapTabPropertiesViewModel.cs` | Map metadata and settings |

## Subdirectory

| Directory | Description |
|-----------|-------------|
| `Teams/` | ViewModels for monster team configuration |

## Relationships

- **Views/MapEditForm/**: Corresponding View files
- **Map**: The core dungeon map type being edited
- **Terrain**: Tile-based terrain system
- **MapGenContext**: Procedural generation context

## MapEditViewModel

Aggregates all tab ViewModels:

```csharp
public class MapEditViewModel : ViewModelBase
{
    public MapEditViewModel()
    {
        Textures = new MapTabTexturesViewModel();
        Decorations = new MapTabDecorationsViewModel();
        Terrain = new MapTabTerrainViewModel();
        Tiles = new MapTabTilesViewModel();
        Items = new MapTabItemsViewModel();
        Entities = new MapTabEntitiesViewModel();
        Entrances = new MapTabEntrancesViewModel();
        Spawns = new MapTabSpawnsViewModel();
        Effects = new MapTabEffectsViewModel();
        Properties = new MapTabPropertiesViewModel();
    }

    public string CurrentFile { get; set; }
    // Tab ViewModels...
}
```

## Tab Descriptions

### Textures Tab
- Floor texture painting
- Wall texture painting
- Autotile support
- Multiple decoration layers

### Decorations Tab
- Overlay decoration placement
- Layer management
- Animated decorations

### Terrain Tab
- Terrain type painting
- Ground / Wall / Water / Lava / Abyss
- Terrain effects configuration

### Tiles Tab
- Trap placement
- Trigger tiles
- Warp tiles
- Custom effect tiles

### Items Tab
- Ground item placement
- Money/Poke placement
- Item properties (sticky, cursed, etc.)
- Spawn probability weights

### Entities Tab
- Monster team placement
- Individual monster configuration
- AI behavior settings
- Patrol paths

### Entrances Tab
- Stairs up/down placement
- Warp destinations
- Named entry points

### Spawns Tab
- Monster spawn tables
- Item spawn tables
- Trap spawn tables
- Floor-specific overrides

### Effects Tab
- Weather effects
- Map-wide status effects
- Visibility settings
- Time limits

### Properties Tab
- Map name and ID
- Dimensions
- Default tileset
- Music selection
- Visibility range

## Usage

The MapEditViewModel is created when opening the dungeon editor:

```csharp
var mapVM = new MapEditViewModel();
mapVM.LoadMap(dungeonMapData);

MapEditForm form = new MapEditForm { DataContext = mapVM };
form.Show();
```

### Terrain Editing Flow

```csharp
// User selects terrain type
mapVM.Terrain.SelectedTerrain = TerrainType.Water;

// User clicks on map
mapVM.Terrain.PaintTerrain(clickPosition);
// Updates terrain data and visual display
```

### Entity Placement Flow

```csharp
// Configure monster team
var team = new TeamViewModel();
team.AddMember(monsterId: "pikachu", level: 10);

// Place on map
mapVM.Entities.PlaceTeam(team, position);
```
