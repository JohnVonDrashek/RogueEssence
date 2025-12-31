# Views/DialogBoxes

## Description

Contains Views for modal dialog windows used throughout the editor. These dialogs collect user input for specific operations like renaming items, resizing maps, configuring mods, and displaying messages. All dialogs are shown modally and return results to the calling code.

## Key Files

| File | Description |
|------|-------------|
| `RenameWindow.axaml` / `.cs` | Simple text input dialog for renaming |
| `MapResizeWindow.axaml` / `.cs` | Map dimension change dialog with anchor selection |
| `MapRetileWindow.axaml` / `.cs` | Tileset replacement dialog |
| `AnimChoiceWindow.axaml` / `.cs` | Animation selection dialog with preview |
| `ModConfigWindow.axaml` / `.cs` | Mod metadata and configuration dialog |
| `MessageBox.axaml` / `.cs` | Alert and confirmation message dialog |

## Relationships

- **ViewModels/DialogBoxes/**: Corresponding ViewModels for dialogs with state
- **MapEditForm/GroundEditForm**: Invoke resize and retile dialogs
- **DevTabModsViewModel**: Invokes mod config dialog

## MessageBox

Static utility for showing alerts:

```csharp
public class MessageBox : Window
{
    public static async Task<MessageBoxResult> Show(
        Window parent,
        string message,
        string title,
        MessageBoxButtons buttons)
    {
        var dialog = new MessageBox();
        dialog.SetMessage(message, title, buttons);
        return await dialog.ShowDialog<MessageBoxResult>(parent);
    }
}

// Usage
var result = await MessageBox.Show(
    this,
    "Save changes before closing?",
    "Unsaved Changes",
    MessageBoxButtons.YesNoCancel
);
```

## RenameWindow

```
+----------------------------------+
| Rename                    [X]    |
+----------------------------------+
| Enter new name:                  |
| [Current Name______________]     |
+----------------------------------+
|            [OK] [Cancel]         |
+----------------------------------+
```

## MapResizeWindow

```
+----------------------------------+
| Resize Map                [X]    |
+----------------------------------+
| Current: 40 x 30                 |
+----------------------------------+
| New Width:  [50]                 |
| New Height: [40]                 |
+----------------------------------+
| Anchor:                          |
| [TL][TC][TR]                     |
| [ML][MC][MR]   <- Center selected|
| [BL][BC][BR]                     |
+----------------------------------+
| Preview: Expand 10 right, 10 down|
+----------------------------------+
|            [OK] [Cancel]         |
+----------------------------------+
```

## ModConfigWindow

```
+--------------------------------------------+
| Mod Configuration                   [X]    |
+--------------------------------------------+
| Mod Name: [My Custom Mod_____________]     |
| Author:   [Developer Name____________]     |
| Version:  [1.0.0]                          |
+--------------------------------------------+
| Description:                               |
| [Multi-line text area for mod             ]|
| [description...                           ]|
+--------------------------------------------+
| Dependencies:                              |
| [ ] Base Game                              |
| [ ] Expansion Pack 1                       |
+--------------------------------------------+
|            [Save] [Cancel]                 |
+--------------------------------------------+
```

## Dialog Patterns

### ShowDialog Usage

```csharp
// Simple input
var renameVM = new RenameViewModel { Name = currentName };
var dialog = new RenameWindow { DataContext = renameVM };

if (await dialog.ShowDialog<bool>(parentWindow))
{
    // User clicked OK
    ApplyRename(renameVM.Name);
}

// Confirmation
var result = await MessageBox.Show(
    parentWindow,
    "Delete this item?",
    "Confirm Delete",
    MessageBoxButtons.YesNo
);

if (result == MessageBoxResult.Yes)
{
    DeleteItem();
}
```

### Dialog Result Handling

```csharp
// In dialog code-behind
private void OkButton_Click(object sender, RoutedEventArgs e)
{
    Close(true);  // Returns true to ShowDialog
}

private void CancelButton_Click(object sender, RoutedEventArgs e)
{
    Close(false);  // Returns false to ShowDialog
}
```

## Usage

Dialogs are typically invoked from ViewModels or Views:

```csharp
// In MapEditViewModel
public async Task ResizeMap()
{
    var resizeVM = new MapResizeViewModel
    {
        NewWidth = CurrentMap.Width,
        NewHeight = CurrentMap.Height
    };

    var dialog = new MapResizeWindow { DataContext = resizeVM };
    var parent = Application.Current.MainWindow;

    if (await dialog.ShowDialog<bool>(parent))
    {
        CurrentMap.Resize(resizeVM.NewWidth, resizeVM.NewHeight,
                         resizeVM.AnchorX, resizeVM.AnchorY);
        RefreshMapDisplay();
    }
}
```
