# Views/MapEditForm

## Description

Contains Views for the dungeon map editor, used for creating procedurally-generated dungeon floors. The editor provides a comprehensive tabbed interface for editing all aspects of dungeon maps including textures, terrain, tiles, items, monsters, entrances, spawns, effects, and properties.

## Key Files

| File | Description |
|------|-------------|
| `MapEditForm.axaml` / `.cs` | Main dungeon editor window with canvas and tabs |
| `MapTabTextures.axaml` / `.cs` | Texture painting for floors and walls |
| `MapTabDecorations.axaml` / `.cs` | Decoration layer editing |
| `MapTabTerrain.axaml` / `.cs` | Terrain type painting (ground, water, lava, wall) |
| `MapTabTiles.axaml` / `.cs` | Effect tile placement (traps, triggers) |
| `MapTabItems.axaml` / `.cs` | Item spawn placement |
| `MapTabEntities.axaml` / `.cs` | Monster team placement |
| `MapTabEntrances.axaml` / `.cs` | Entry/exit point management |
| `MapTabSpawns.axaml` / `.cs` | Spawn table configuration |
| `MapTabEffects.axaml` / `.cs` | Map-wide effect settings |
| `MapTabProperties.axaml` / `.cs` | Map metadata and settings |

## Subdirectory

| Directory | Description |
|-----------|-------------|
| `Teams/` | Monster team configuration window |

## Relationships

- **ViewModels/MapEditForm/**: Corresponding ViewModels
- **DevForm**: Opens MapEditForm for dungeon editing
- **Map**: Core dungeon map type being edited
- **Terrain**: Tile-based terrain system

## MapEditForm Layout

```
+------------------------------------------------------------------+
| Map Editor: mystery_dungeon_f5.pokemon    [-][O][X]              |
+------------------------------------------------------------------+
| File  Edit  View  Tools                                          |
+------------------------------------------------------------------+
| [Tex][Deco][Ter][Tile][Item][Ent][Entr][Spawn][FX][Props]       |
+------------------------------------------------------------------+
|                              |                                    |
|     (Active Tab Panel)       |        (Map Canvas)                |
|                              |                                    |
|     - Tool-specific          |        [Dungeon map display       |
|       controls               |         with terrain overlay       |
|     - Brush options          |         and entity markers]        |
|                              |                                    |
+------------------------------+------------------------------------+
| Tool: [Draw v] | Terrain: Ground | Pos: (15, 22) | Zoom: [100%] |
+------------------------------------------------------------------+
```

## Tab Views

### MapTabTerrain
```
+----------------------------------+
| Terrain Type:                    |
| (*) Ground    ( ) Water          |
| ( ) Lava      ( ) Wall           |
| ( ) Abyss     ( ) Unbreakable    |
+----------------------------------+
| Tool:                            |
| (*) Draw  ( ) Rect  ( ) Fill     |
+----------------------------------+
| Brush Size: [1 v]                |
+----------------------------------+
```

### MapTabTiles
```
+----------------------------------+
| Tile Effects:                    |
| +------------------------------+ |
| | Spike Trap                   | |
| | Warp Tile                    | |
| | Trigger Plate                | |
| | Wonder Tile                  | |
| +------------------------------+ |
+----------------------------------+
| Selected: Spike Trap             |
| Damage: [10]                     |
| Visible: [*] Yes                 |
+----------------------------------+
| [Place] [Select] [Delete]        |
+----------------------------------+
```

### MapTabEntities
```
+----------------------------------+
| Teams:               [+][-]      |
| +------------------------------+ |
| | Team 1 @ (5, 12)             | |
| |   - Rattata Lv.5             | |
| | Team 2 @ (20, 8)             | |
| |   - Zubat Lv.6               | |
| |   - Geodude Lv.5             | |
| +------------------------------+ |
+----------------------------------+
| [Place Team] [Edit Team]         |
+----------------------------------+
```

### MapTabEntrances
```
+----------------------------------+
| Entrances:           [+][-]      |
| +------------------------------+ |
| | Stairs Up @ (1, 1)           | |
| | Stairs Down @ (38, 28)       | |
| +------------------------------+ |
+----------------------------------+
| Type: [Stairs Down v]            |
| Destination: [Next Floor v]      |
+----------------------------------+
| [Place] [Move] [Delete]          |
+----------------------------------+
```

### MapTabEffects
```
+----------------------------------+
| Weather: [Clear v]               |
+----------------------------------+
| Map Status Effects:              |
| [ ] Darkness                     |
| [ ] Sandstorm                    |
| [*] Monster House Active         |
+----------------------------------+
| Visibility Range: [4] tiles      |
+----------------------------------+
| Time Limit: [0] (0 = none)       |
+----------------------------------+
```

## Usage

The map editor is opened from DevForm:

```csharp
// In DevForm
public MapEditForm MapEditForm;

public void OpenMapEditor(string mapPath)
{
    if (MapEditForm == null)
    {
        MapEditForm = new MapEditForm();
        MapEditForm.DataContext = new MapEditViewModel();
    }

    MapEditForm.LoadMap(mapPath);
    MapEditForm.Show();
}
```

### Canvas Interaction

```csharp
// In MapEditForm.axaml.cs
private void Canvas_PointerPressed(object sender, PointerEventArgs e)
{
    var pos = e.GetPosition(MapCanvas);
    var tilePos = ScreenToTile(pos);

    switch (CurrentTab)
    {
        case "Terrain":
            TerrainTab.PaintTerrain(tilePos);
            break;
        case "Entities":
            EntitiesTab.HandleClick(tilePos);
            break;
        // etc.
    }
}
```
