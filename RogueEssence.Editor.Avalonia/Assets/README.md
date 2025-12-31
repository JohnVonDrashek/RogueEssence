# Assets

## Description

Contains image resources and icons used throughout the Avalonia editor interface. These assets are embedded as Avalonia resources and referenced in XAML for toolbar buttons, layer management controls, and application branding.

## Key Files

| File | Description |
|------|-------------|
| `app-logo.ico` | Application icon for the editor window and taskbar |
| `LayerAdd.png` | Icon for adding new layers in map/ground editors |
| `LayerDelete.png` | Icon for removing layers from the layer stack |
| `LayerUp.png` | Icon for moving a layer up in the z-order |
| `LayerDown.png` | Icon for moving a layer down in the z-order |
| `LayerDupe.png` | Icon for duplicating an existing layer |
| `LayerMerge.png` | Icon for merging layers together |
| `AssetOrder.png` | Icon for asset ordering/arrangement operations |

## Relationships

- **Views/UserControls/LayerBox**: Uses layer management icons for the layer panel toolbar
- **RogueEssence.Editor.Avalonia.csproj**: Includes all files via `<AvaloniaResource Include="Assets\**" />`
- **App.axaml**: May reference icons for application-wide resources

## Usage

Assets are referenced in XAML using the Avalonia resource path syntax:

```xml
<Image Source="avares://RogueEssence.Editor.Avalonia/Assets/LayerAdd.png" />
```

To add new assets:
1. Add the image file to this directory
2. The `.csproj` automatically includes all files in `Assets/` as Avalonia resources
3. Reference in XAML using the `avares://` protocol with the assembly name
