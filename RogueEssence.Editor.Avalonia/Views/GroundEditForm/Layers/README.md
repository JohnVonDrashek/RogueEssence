# Views/GroundEditForm/Layers

## Description

Contains Views for layer configuration dialogs in the ground map editor. These windows allow detailed configuration of texture layers, animation layers, and entity layers including properties like visibility, opacity, and layer-specific settings.

## Key Files

| File | Description |
|------|-------------|
| `GroundLayerWindow.axaml` / `.cs` | Generic ground layer properties dialog |
| `MapLayerWindow.axaml` / `.cs` | Texture/tile layer configuration dialog |
| `AnimLayerWindow.axaml` / `.cs` | Animation layer configuration dialog |
| `EntityLayerWindow.axaml` / `.cs` | Entity layer grouping configuration |

## Relationships

- **ViewModels/GroundEditForm/Layers/**: Corresponding layer ViewModels
- **GroundTabTexturesViewModel**: Opens MapLayerWindow for texture layers
- **GroundTabDecorationsViewModel**: Opens AnimLayerWindow for decoration layers
- **GroundTabEntitiesViewModel**: Opens EntityLayerWindow for entity organization

## MapLayerWindow

Texture layer configuration:

```
+----------------------------------+
| Layer Properties          [X]    |
+----------------------------------+
| Name: [Floor Tiles__________]    |
+----------------------------------+
| [*] Visible                      |
| [ ] Locked                       |
+----------------------------------+
| Opacity: [====|======] 100%      |
+----------------------------------+
| Tileset: [forest_ground v]       |
+----------------------------------+
|            [OK] [Cancel]         |
+----------------------------------+
```

## AnimLayerWindow

Animation layer configuration:

```
+----------------------------------+
| Animation Layer           [X]    |
+----------------------------------+
| Name: [Waterfall Effects____]    |
+----------------------------------+
| [*] Visible                      |
| [ ] Locked                       |
+----------------------------------+
| Animation: [water_flow v]        |
| Frame Rate: [12] fps             |
+----------------------------------+
| Blend Mode: [Normal v]           |
+----------------------------------+
|            [OK] [Cancel]         |
+----------------------------------+
```

## EntityLayerWindow

Entity layer grouping:

```
+----------------------------------+
| Entity Layer              [X]    |
+----------------------------------+
| Name: [NPCs_________________]    |
+----------------------------------+
| [*] Visible in Editor            |
| [*] Active in Game               |
+----------------------------------+
| Entity Count: 5                  |
+----------------------------------+
|            [OK] [Cancel]         |
+----------------------------------+
```

## Layer Window Pattern

All layer windows follow a similar pattern:

```csharp
// Opening a layer configuration dialog
public async Task EditLayer(MapLayerViewModel layer)
{
    var window = new MapLayerWindow
    {
        DataContext = layer
    };

    if (await window.ShowDialog<bool>(parentWindow))
    {
        // Layer properties updated via binding
        RefreshLayerDisplay();
    }
}
```

## AXAML Structure

```xml
<!-- MapLayerWindow.axaml -->
<Window xmlns="https://github.com/avaloniaui"
        Title="Layer Properties"
        Width="300" Height="250">
    <StackPanel Margin="10">
        <TextBlock Text="Name:"/>
        <TextBox Text="{Binding Name}"/>

        <CheckBox IsChecked="{Binding Visible}"
                  Content="Visible"/>

        <CheckBox IsChecked="{Binding Locked}"
                  Content="Locked"/>

        <TextBlock Text="Opacity:"/>
        <Slider Value="{Binding Opacity}"
                Minimum="0" Maximum="1"/>

        <StackPanel Orientation="Horizontal"
                    HorizontalAlignment="Right">
            <Button Content="OK" Click="OkButton_Click"/>
            <Button Content="Cancel" Click="CancelButton_Click"/>
        </StackPanel>
    </StackPanel>
</Window>
```

## Usage

Layer windows are typically accessed through the layer panel:

```csharp
// Double-click on layer to edit
private async void Layer_DoubleClick(object sender, EventArgs e)
{
    var layer = (MapLayerViewModel)((Control)sender).DataContext;

    var window = new MapLayerWindow { DataContext = layer };
    await window.ShowDialog(this);
}

// Or via edit button
private async void EditLayerButton_Click(object sender, EventArgs e)
{
    if (SelectedLayer != null)
    {
        var window = new MapLayerWindow { DataContext = SelectedLayer };
        await window.ShowDialog(this);
    }
}
```
