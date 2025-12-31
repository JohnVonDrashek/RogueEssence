# Views/GroundEditForm

## Description

Contains Views for the ground map editor, used for creating non-procedural overworld areas like towns, story locations, and hub areas. The editor provides a tabbed interface for editing textures, decorations, collision, entities, properties, strings, and scripts associated with ground maps.

## Key Files

| File | Description |
|------|-------------|
| `GroundEditForm.axaml` / `.cs` | Main ground editor window with canvas and tabs |
| `GroundTabTextures.axaml` / `.cs` | Texture layer painting interface |
| `GroundTabDecorations.axaml` / `.cs` | Decoration placement interface |
| `GroundTabWalls.axaml` / `.cs` | Collision/wall painting interface |
| `GroundTabEntities.axaml` / `.cs` | Entity (NPC, object) management |
| `GroundTabProperties.axaml` / `.cs` | Map properties and settings |
| `GroundTabStrings.axaml` / `.cs` | Map-specific string editing |
| `GroundTabScript.axaml` / `.cs` | Lua script association |
| `EntityBrowser.axaml` / `.cs` | Entity type browser and property panel |

## Subdirectory

| Directory | Description |
|-----------|-------------|
| `Layers/` | Layer management windows for texture/anim/entity layers |

## Relationships

- **ViewModels/GroundEditForm/**: Corresponding ViewModels
- **DevForm**: Opens GroundEditForm for ground map editing
- **GroundMap**: Core game type being edited
- **DataEditor/Editors/RogueEssence/Tiles/**: Tile browser controls

## GroundEditForm Layout

```
+------------------------------------------------------------------+
| Ground Editor: town_square.pokemon        [-][O][X]              |
+------------------------------------------------------------------+
| File  Edit  View  Tools                                          |
+------------------------------------------------------------------+
| [Tex][Deco][Wall][Ent][Props][Str][Script]                       |
+------------------------------------------------------------------+
|                              |                                    |
|     (Active Tab Panel)       |        (Map Canvas)                |
|                              |                                    |
|     - Tile Browser           |        [Visual map display         |
|     - Layer controls         |         with grid overlay          |
|     - Tool options           |         and entity markers]        |
|                              |                                    |
+------------------------------+------------------------------------+
| Tool: [Draw v] | Pos: (12, 8) | Zoom: [100%]                     |
+------------------------------------------------------------------+
```

## Tab Views

### GroundTabTextures
```
+----------------------------------+
| Layers:            [+][-][^][v]  |
| +------------------------------+ |
| | [*] Floor                    | |
| | [*] Details                  | |
| +------------------------------+ |
+----------------------------------+
| Tileset: [forest_ground v]       |
| +------------------------------+ |
| | [Tile browser grid]          | |
| +------------------------------+ |
+----------------------------------+
| Tool: ( )Draw (*)Rect ( )Fill   |
+----------------------------------+
```

### GroundTabEntities
```
+----------------------------------+
| Entities:          [+][-]        |
| +------------------------------+ |
| | NPC_Shopkeeper (12, 5)       | |
| | Sign_Welcome (3, 2)          | |
| | Marker_Spawn (8, 8)          | |
| +------------------------------+ |
+----------------------------------+
| Entity Type: [NPC v]             |
| [Place Mode] [Select Mode]       |
+----------------------------------+
| Properties:                      |
| [Entity Browser Panel]           |
+----------------------------------+
```

### GroundTabScript
```
+----------------------------------+
| Script File:                     |
| [town_square.lua]    [Browse]    |
+----------------------------------+
| Entry Points:                    |
| - OnEnter                        |
| - OnExit                         |
| - OnInteract_NPC_Shopkeeper      |
+----------------------------------+
| [Open in Editor] [Reload]        |
+----------------------------------+
```

## Entity Browser

Detailed entity configuration panel:

```
+----------------------------------+
| Entity: NPC_Shopkeeper           |
+----------------------------------+
| Type: [NPC v]                    |
| Species: [Kecleon v]             |
| Direction: [Down v]              |
+----------------------------------+
| Position:                        |
| X: [12]  Y: [5]                  |
+----------------------------------+
| Behavior:                        |
| [ ] Wandering                    |
| [*] Stationary                   |
| Interaction: [Shop v]            |
+----------------------------------+
| Script Events:                   |
| OnTalk: [ShopkeeperDialog]       |
+----------------------------------+
```

## Usage

The ground editor is opened from DevForm:

```csharp
// In DevForm
public GroundEditForm GroundEditForm;

public void OpenGroundEditor(string mapPath)
{
    if (GroundEditForm == null)
    {
        GroundEditForm = new GroundEditForm();
        GroundEditForm.DataContext = new GroundEditViewModel();
    }

    GroundEditForm.LoadMap(mapPath);
    GroundEditForm.Show();
}
```
