# ViewModels/DialogBoxes

## Description

Contains ViewModels for modal dialog windows used throughout the editor. These dialogs handle user input for operations like renaming, resizing maps, configuring mods, and selecting animations. Each ViewModel manages dialog state and validation logic.

## Key Files

| File | Description |
|------|-------------|
| `RenameViewModel.cs` | Simple text input dialog for renaming items |
| `MapResizeViewModel.cs` | Dialog for changing map dimensions with anchor point selection |
| `MapRetileViewModel.cs` | Dialog for retiling maps with different tilesets |
| `AnimChoiceViewModel.cs` | Animation selection dialog with preview |
| `ModConfigViewModel.cs` | Mod configuration and metadata editing dialog |

## Relationships

- **Views/DialogBoxes/**: Corresponding dialog Views (`RenameWindow.axaml`, etc.)
- **MapEditForm/GroundEditForm**: Map dialogs are invoked from map editors
- **DevTabModsViewModel**: Mod config dialog is opened from Mods tab

## Dialog Patterns

### Simple Input Dialog (RenameViewModel)

```csharp
public class RenameViewModel : ViewModelBase
{
    private string name;
    public string Name
    {
        get => name;
        set => this.RaiseAndSetIfChanged(ref name, value);
    }

    public bool IsValid => !string.IsNullOrWhiteSpace(Name);
}
```

### Complex Configuration (MapResizeViewModel)

```csharp
public class MapResizeViewModel : ViewModelBase
{
    public int NewWidth { get; set; }
    public int NewHeight { get; set; }
    public int AnchorX { get; set; }  // -1 = left, 0 = center, 1 = right
    public int AnchorY { get; set; }  // -1 = top, 0 = center, 1 = bottom

    // Preview of how map will be cropped/expanded
    public string PreviewDescription =>
        $"Map will be {NewWidth}x{NewHeight}, anchored {GetAnchorDescription()}";
}
```

### With Asset Selection (AnimChoiceViewModel)

```csharp
public class AnimChoiceViewModel : ViewModelBase
{
    public ObservableCollection<string> AvailableAnims { get; }
    public string SelectedAnim { get; set; }

    // Preview image updates when selection changes
    public Bitmap PreviewImage => LoadAnimPreview(SelectedAnim);
}
```

## Usage

Dialogs are shown modally and return results:

```csharp
// Show rename dialog
var renameVM = new RenameViewModel { Name = currentName };
var dialog = new RenameWindow { DataContext = renameVM };

var result = await dialog.ShowDialog<bool>(parentWindow);

if (result && renameVM.IsValid)
{
    ApplyRename(renameVM.Name);
}
```

### Map Resize Example

```csharp
var resizeVM = new MapResizeViewModel
{
    NewWidth = map.Width,
    NewHeight = map.Height,
    AnchorX = 0,
    AnchorY = 0
};

var dialog = new MapResizeWindow { DataContext = resizeVM };
if (await dialog.ShowDialog<bool>(this))
{
    map.Resize(resizeVM.NewWidth, resizeVM.NewHeight,
               resizeVM.AnchorX, resizeVM.AnchorY);
}
```

### Mod Configuration

```csharp
var modVM = new ModConfigViewModel(existingMod);
var dialog = new ModConfigWindow { DataContext = modVM };

if (await dialog.ShowDialog<bool>(this))
{
    modVM.SaveToMod(existingMod);
}
```
