# Views

## Description

Contains Avalonia XAML Views for the editor UI. Views define the visual structure and layout using AXAML (Avalonia XAML) files paired with C# code-behind files. Each View is automatically bound to its corresponding ViewModel by the `ViewLocator` based on naming convention (`FooView` matches `FooViewModel`).

## Key Files

| File | Description |
|------|-------------|
| `DataListForm.axaml` / `.cs` | List view for browsing game data entries with search |
| `SearchListBox.axaml` / `.cs` | Reusable searchable list control component |

## Subdirectories

| Directory | Description |
|-----------|-------------|
| `Content/` | Asset editor forms (sprites, tilesets, animations, strings) |
| `DevForm/` | Main development form window and tab views |
| `DialogBoxes/` | Modal dialog windows (rename, resize, configure) |
| `GroundEditForm/` | Ground/overworld map editor views |
| `MapEditForm/` | Dungeon map editor views |
| `Testing/` | Testing and debugging tool views |
| `UserControls/` | Reusable UI components (layer box, team box) |

## Relationships

- **ViewModels/**: Each View has a corresponding ViewModel with matching name
- **ViewLocator**: Resolves ViewModels to Views automatically
- **Converters/**: XAML uses converters for data binding transformations
- **Assets/**: References image resources for icons and graphics

## AXAML File Structure

Each View consists of paired files:

### AXAML File (Layout)
```xml
<!-- FooView.axaml -->
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="clr-namespace:RogueEssence.Dev.ViewModels"
             x:Class="RogueEssence.Dev.Views.FooView">

    <Design.DataContext>
        <vm:FooViewModel/>
    </Design.DataContext>

    <StackPanel>
        <TextBlock Text="{Binding Title}"/>
        <Button Content="Click" Command="{Binding ClickCommand}"/>
    </StackPanel>
</UserControl>
```

### Code-Behind File (Logic)
```csharp
// FooView.axaml.cs
public class FooView : UserControl
{
    public FooView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
```

## View Types

### Window Views
Full windows with title bar, can be modal or modeless:
- `DevForm` - Main editor window
- `MapEditForm` - Dungeon editor window
- `GroundEditForm` - Ground map editor window

### UserControl Views
Embeddable components within windows:
- Tab content views (`DevTabGame`, `MapTabTerrain`)
- Reusable controls (`LayerBox`, `SearchListBox`)

### Dialog Views
Modal windows for user input:
- `RenameWindow` - Text input dialog
- `MapResizeWindow` - Map dimension dialog
- `MessageBox` - Alert/confirmation dialog

## Data Binding

Views bind to ViewModel properties using Avalonia binding syntax:

```xml
<!-- Text binding -->
<TextBlock Text="{Binding PlayerName}"/>

<!-- Two-way binding for input -->
<TextBox Text="{Binding SearchText, Mode=TwoWay}"/>

<!-- Collection binding -->
<ListBox Items="{Binding Monsters}" SelectedItem="{Binding SelectedMonster}"/>

<!-- Command binding -->
<Button Command="{Binding SaveCommand}" Content="Save"/>

<!-- Converter binding -->
<Image Source="{Binding TileData, Converter={StaticResource TileConverter}}"/>
```

## Usage

Views are typically instantiated by the ViewLocator or explicitly:

```csharp
// Automatic via ViewLocator (in ContentControl)
<ContentControl Content="{Binding CurrentTabViewModel}"/>

// Explicit creation
var form = new MapEditForm();
form.DataContext = new MapEditViewModel();
form.Show();

// Modal dialog
var dialog = new RenameWindow { DataContext = renameVM };
var result = await dialog.ShowDialog<bool>(parentWindow);
```
