# DataEditor/Editors/RogueEssence/Tiles

## Description

Contains editors and browser controls for tile-based graphics editing. This includes single tile selection, autotile configuration, and complete tileset browsing. These editors provide visual tile pickers that display the actual game graphics, allowing developers to select tiles by clicking on a visual grid rather than entering numeric IDs.

## Key Files

| File | Description |
|------|-------------|
| `TileLayerEditor.cs` | Editor for TileLayer objects with full tile configuration |
| `AutoTileEditor.cs` | Editor for autotile references with pattern preview |
| `AutoTileBaseEditor.cs` | Base editor for autotile systems with terrain matching |
| `TileBrowser.axaml` / `.cs` / `ViewModel.cs` | Visual browser for selecting individual tiles from tilesets |
| `AutotileBrowser.axaml` / `.cs` / `ViewModel.cs` | Visual browser for autotile pattern selection |
| `TileBox.axaml` / `.cs` / `ViewModel.cs` | Compact tile display/selection control |
| `TileEditForm.axaml` / `.cs` | Modal dialog for detailed tile editing |
| `TileEditViewModel.cs` | ViewModel for tile editing operations |

## Relationships

- **GraphicsManager**: Loads tileset textures from game assets
- **DevDataManager**: Caches tile bitmaps for efficient display
- **TileConverter**: Converts tile data to Avalonia bitmaps for display
- **MapEditForm/GroundEditForm**: Uses tile browsers for texture layer editing

## TileBrowser

The tile browser provides a visual grid of all tiles in a tileset:

```
+------------------------------------------+
| Tileset: [ground_forest  v]              |
+------------------------------------------+
| +--+--+--+--+--+--+--+--+--+--+          |
| |  |  |  |  |  |  |  |  |  |  |          |
| +--+--+--+--+--+--+--+--+--+--+          |
| |  |XX|  |  |  |  |  |  |  |  |  <- Selected
| +--+--+--+--+--+--+--+--+--+--+          |
| |  |  |  |  |  |  |  |  |  |  |          |
| +--+--+--+--+--+--+--+--+--+--+          |
+------------------------------------------+
| Selected: (1, 1)  Sheet: ground_forest   |
+------------------------------------------+
```

## AutotileBrowser

Autotile browser shows available autotile patterns:

```
+------------------------------------------+
| Autotile Set: [water  v]                 |
+------------------------------------------+
| [Pattern 1] [Pattern 2] [Pattern 3]      |
| [Pattern 4] [Pattern 5] [Pattern 6]      |
|                                          |
| Preview: [Large tile preview area]       |
+------------------------------------------+
```

## TileEditViewModel

```csharp
public class TileEditViewModel : ViewModelBase
{
    public string TileSheet { get; set; }      // Current tileset name
    public Loc TileLoc { get; set; }           // Selected tile coordinates
    public TileFrame CurrentTile { get; set; } // Full tile frame data

    public void SelectTile(Loc location)
    {
        TileLoc = location;
        CurrentTile = new TileFrame(TileSheet, location);
        this.RaisePropertyChanged(nameof(CurrentTile));
    }
}
```

## Usage

Tile editors are used in map/ground editing tabs:

```csharp
// In texture editing tab
TileBrowser browser = new TileBrowser();
browser.DataContext = new TileBrowserViewModel(currentTileset);

browser.TileSelected += (tile) => {
    currentBrush = tile;
    // Now clicking on map paints this tile
};
```

### Integration with Map Editors

The tile browsers integrate with the painting system:

1. Developer selects tile in browser
2. Selected tile becomes the current brush
3. Click/drag on map canvas applies the tile
4. Autotiles automatically connect with neighbors
