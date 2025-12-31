# ViewModels/Content

## Description

Contains ViewModels for asset and content editing forms. These handle editing of game assets like character sprites, animations, tilesets, and localized strings. Each ViewModel manages the state and logic for its corresponding editor window.

## Key Files

| File | Description |
|------|-------------|
| `SpeciesEditViewModel.cs` | ViewModel for editing character/monster sprite sheets with form variants |
| `AnimEditViewModel.cs` | ViewModel for animation editing with frame management and playback |
| `BeamEditViewModel.cs` | ViewModel for beam/projectile animation editing |
| `TilesetEditViewModel.cs` | ViewModel for tileset import/export and tile arrangement |
| `StringsEditViewModel.cs` | ViewModel for localization string editing and translation management |

## Relationships

- **Views/Content/**: Corresponding View files (`SpeciesEditForm.axaml`, etc.)
- **GraphicsManager**: Loads and saves sprite/animation assets
- **DataManager**: Accesses game data for species and asset indices
- **DevDataManager**: Provides cached image resources for display

## SpeciesEditViewModel

Manages character sprite editing with support for:
- Multiple forms (normal, shiny, gender variants)
- Animation states (idle, walk, attack, etc.)
- Frame timing and offsets
- Sprite sheet import/export

```csharp
public class SpeciesEditViewModel : ViewModelBase
{
    public ObservableCollection<FormViewModel> Forms { get; }
    public FormViewModel SelectedForm { get; set; }
    public ObservableCollection<AnimViewModel> Animations { get; }

    public void ImportSheet(string path) { ... }
    public void ExportSheet(string path) { ... }
    public void AddForm() { ... }
    public void RemoveForm() { ... }
}
```

## AnimEditViewModel

Handles general animation editing:
- Frame sequence management
- Timing/duration per frame
- Loop and playback settings
- Preview with actual game rendering

## TilesetEditViewModel

Manages tileset assets:
- Import from image files
- Tile grid configuration
- Autotile pattern definition
- Export to game format

## StringsEditViewModel

Handles localization:
- Language selection
- String key management
- Translation editing
- Missing translation detection

## Usage

Content ViewModels are created when opening asset editors:

```csharp
// Opening species editor
var speciesVM = new SpeciesEditViewModel();
speciesVM.LoadSpecies(speciesId);

var form = new SpeciesEditForm { DataContext = speciesVM };
form.ShowDialog(parentWindow);

// Save changes
if (form.DialogResult == true)
{
    speciesVM.SaveSpecies();
}
```

### Data Binding Example

```xml
<!-- SpeciesEditForm.axaml -->
<ListBox Items="{Binding Forms}" SelectedItem="{Binding SelectedForm}">
    <ListBox.ItemTemplate>
        <DataTemplate>
            <TextBlock Text="{Binding FormName}"/>
        </DataTemplate>
    </ListBox.ItemTemplate>
</ListBox>

<Image Source="{Binding SelectedForm.PreviewSprite}"/>
```
