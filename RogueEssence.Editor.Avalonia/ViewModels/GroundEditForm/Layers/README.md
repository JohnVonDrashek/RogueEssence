# ViewModels/GroundEditForm/Layers

## Description

Contains ViewModels for layer management in the ground map editor. Ground maps support multiple layer types: texture layers for the background, animation layers for animated decorations, and entity layers for grouping interactive objects. These ViewModels handle layer selection, ordering, visibility, and properties.

## Key Files

| File | Description |
|------|-------------|
| `TextureLayerBoxViewModel.cs` | ViewModel for texture layer list with add/remove/reorder |
| `MapLayerViewModel.cs` | ViewModel for individual map layer properties |
| `AnimLayerBoxViewModel.cs` | ViewModel for animation layer list management |
| `AnimLayerViewModel.cs` | ViewModel for individual animation layer properties |
| `EntityLayerBoxViewModel.cs` | ViewModel for entity layer list management |
| `EntityLayerViewModel.cs` | ViewModel for individual entity layer properties |

## Relationships

- **Views/GroundEditForm/Layers/**: Corresponding layer window Views
- **UserControls/LayerBoxViewModel**: Base layer box functionality
- **GroundTabTexturesViewModel**: Uses texture layers for painting
- **GroundTabEntitiesViewModel**: Uses entity layers for organization

## Layer Types

### Texture Layers (TextureLayerBoxViewModel)
- Background tile layers
- Support for transparency and blending
- Z-order determines render order
- Each layer can use different tilesets

### Animation Layers (AnimLayerBoxViewModel)
- Animated decorations (waterfalls, torches, etc.)
- Frame-based animation playback
- Positioned objects with animation data

### Entity Layers (EntityLayerBoxViewModel)
- Logical grouping of entities
- NPCs, objects, markers
- Layer visibility for editing

## Layer Box Pattern

Each layer type follows a similar pattern:

```csharp
public class TextureLayerBoxViewModel : ViewModelBase
{
    public ObservableCollection<MapLayerViewModel> Layers { get; }

    private MapLayerViewModel selectedLayer;
    public MapLayerViewModel SelectedLayer
    {
        get => selectedLayer;
        set => this.RaiseAndSetIfChanged(ref selectedLayer, value);
    }

    public void AddLayer()
    {
        var layer = new MapLayerViewModel { Name = $"Layer {Layers.Count}" };
        Layers.Add(layer);
        SelectedLayer = layer;
    }

    public void RemoveLayer()
    {
        if (SelectedLayer != null)
            Layers.Remove(SelectedLayer);
    }

    public void MoveLayerUp()
    {
        int idx = Layers.IndexOf(SelectedLayer);
        if (idx > 0)
            Layers.Move(idx, idx - 1);
    }

    public void MoveLayerDown()
    {
        int idx = Layers.IndexOf(SelectedLayer);
        if (idx < Layers.Count - 1)
            Layers.Move(idx, idx + 1);
    }
}
```

## Individual Layer ViewModels

```csharp
public class MapLayerViewModel : ViewModelBase
{
    public string Name { get; set; }
    public bool Visible { get; set; }
    public bool Locked { get; set; }
    public float Opacity { get; set; }

    // For texture layers
    public string Tileset { get; set; }
}

public class AnimLayerViewModel : ViewModelBase
{
    public string Name { get; set; }
    public bool Visible { get; set; }
    public string AnimationAsset { get; set; }
    public int FrameRate { get; set; }
}
```

## Usage

Layer management is accessed from the editor tabs:

```csharp
// In GroundTabTexturesViewModel
public TextureLayerBoxViewModel LayerBox { get; }

// Add new layer
LayerBox.AddLayer();

// Select layer for painting
var activeLayer = LayerBox.SelectedLayer;
PaintTileOnLayer(activeLayer, position, tile);
```

### Layer Windows

Layer properties are edited in modal windows:

```csharp
var layerVM = new MapLayerViewModel(existingLayer);
var window = new MapLayerWindow { DataContext = layerVM };

if (await window.ShowDialog<bool>(parent))
{
    layerVM.ApplyTo(existingLayer);
}
```
