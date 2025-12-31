# ViewModels/GroundEditForm

## Description

Contains ViewModels for the ground map editor, used for creating overworld/town areas. Ground maps are non-procedural maps where players walk around, interact with NPCs, and trigger story events. The editor provides tabs for textures, decorations, walls/collision, entities, properties, strings, and scripting.

## Key Files

| File | Description |
|------|-------------|
| `GroundEditViewModel.cs` | Root ViewModel aggregating all ground editor tabs |
| `GroundTabTexturesViewModel.cs` | Texture layer painting with tile browser |
| `GroundTabDecorationsViewModel.cs` | Decoration object placement and configuration |
| `GroundTabWallsViewModel.cs` | Collision/wall painting for walkability |
| `GroundTabEntitiesViewModel.cs` | NPC, object, and marker entity management |
| `GroundTabPropertiesViewModel.cs` | Map metadata (name, music, dimensions) |
| `GroundTabStringsViewModel.cs` | Map-specific localized strings |
| `GroundTabScriptViewModel.cs` | Lua script management for map events |
| `EntityBrowserViewModel.cs` | Entity type browser and property editor |
| `MapString.cs` | Model for map string entries |
| `ScriptItem.cs` | Model for script file references |
| `EntScriptItem.cs` | Model for entity-attached scripts |

## Subdirectory

| Directory | Description |
|-----------|-------------|
| `Layers/` | ViewModels for layer management (texture, animation, entity layers) |

## Relationships

- **Views/GroundEditForm/**: Corresponding View files
- **GroundMap**: The core game type being edited
- **GroundEntity**: NPCs, objects, markers placed on the map
- **Layers/**: Layer panel ViewModels for z-order management

## GroundEditViewModel

Aggregates all tab ViewModels:

```csharp
public class GroundEditViewModel : ViewModelBase
{
    public GroundEditViewModel()
    {
        Textures = new GroundTabTexturesViewModel();
        Decorations = new GroundTabDecorationsViewModel();
        Walls = new GroundTabWallsViewModel();
        Entities = new GroundTabEntitiesViewModel();
        Properties = new GroundTabPropertiesViewModel();
        Strings = new GroundTabStringsViewModel();
        Script = new GroundTabScriptViewModel();
    }

    public string CurrentFile { get; set; }
    // Tab ViewModels...
}
```

## Tab Descriptions

### Textures Tab
- Layer-based texture painting
- Tile brush selection via TileBrowser
- Multiple texture layers with z-ordering
- Fill and rectangle tools

### Decorations Tab
- Decoration object placement
- Animation preview
- Layer assignment
- Transform controls (flip, rotate)

### Walls Tab
- Collision map painting
- Binary walkable/blocked tiles
- Visual overlay on texture

### Entities Tab
- NPC character placement
- Interactive object placement
- Spawn point markers
- Entry/exit markers
- Entity property editing

### Properties Tab
- Map name and ID
- Dimensions (resize dialog)
- Background music selection
- Scroll bounds configuration

### Strings Tab
- Map-specific text strings
- Dialogue and signage
- Localization keys

### Script Tab
- Lua script file association
- Entry point configuration
- Script function listing

## Usage

The GroundEditViewModel is created when opening the ground editor:

```csharp
var groundVM = new GroundEditViewModel();
groundVM.LoadMap(groundMapData);

GroundEditForm form = new GroundEditForm { DataContext = groundVM };
form.Show();
```

### Entity Editing Flow

```csharp
// User clicks on map
var entity = groundVM.Entities.GetEntityAt(clickPosition);

if (entity != null)
{
    // Show entity browser with properties
    groundVM.EntityBrowser.LoadEntity(entity);
}
else if (groundVM.Entities.PlaceMode)
{
    // Place new entity
    groundVM.Entities.PlaceEntity(selectedEntityType, clickPosition);
}
```
