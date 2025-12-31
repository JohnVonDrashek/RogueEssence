# Views/Content

## Description

Contains Views for asset and content editing forms. These windows provide specialized interfaces for editing game assets like character sprites, animations, tilesets, and localized strings. Each form includes preview capabilities and import/export functionality.

## Key Files

| File | Description |
|------|-------------|
| `SpeciesEditForm.axaml` / `.cs` | Character/monster sprite sheet editor with form variants |
| `AnimEditForm.axaml` / `.cs` | Animation editor with frame timeline and playback preview |
| `TilesetEditForm.axaml` / `.cs` | Tileset import/export and tile arrangement editor |
| `StringsEditForm.axaml` / `.cs` | Localization string editor with language switching |

## Relationships

- **ViewModels/Content/**: Corresponding ViewModels for each editor
- **DevTabSpritesViewModel**: Opens species editor for sprite management
- **DevTabDataViewModel**: Opens content editors from data browser
- **GraphicsManager**: Asset loading and saving

## SpeciesEditForm

Character sprite sheet editor:

```
+----------------------------------------------------+
| Species Editor: Pikachu                    [X]     |
+----------------------------------------------------+
| Forms:          | Animation:       | Preview:      |
| [+][-]         |                  |               |
| - Normal       | State: [Idle v]  | +----------+  |
| - Shiny        | Frame: [1/4]     | | Animated |  |
| - Female       | [<] [>] [Play]   | | Sprite   |  |
|                |                  | | Preview  |  |
|                | Timing: [100]ms  | +----------+  |
+----------------+------------------+---------------+
| Sprite Sheet:                                      |
| +------------------------------------------------+|
| | [Grid of animation frames]                     ||
| +------------------------------------------------+|
+----------------------------------------------------+
| [Import Sheet] [Export Sheet] [Save] [Cancel]      |
+----------------------------------------------------+
```

## AnimEditForm

General animation editor:

```
+----------------------------------------------------+
| Animation Editor                           [X]     |
+----------------------------------------------------+
| Timeline:                                          |
| [Frame 1][Frame 2][Frame 3][Frame 4] [+][-]       |
+----------------------------------------------------+
| Frame Properties:    | Preview:                    |
| Duration: [100] ms   | +------------------------+ |
| Offset X: [0]        | |                        | |
| Offset Y: [0]        | |    [Animation          | |
| Flip H: [ ]          | |     Preview]           | |
| Flip V: [ ]          | |                        | |
|                      | +------------------------+ |
+----------------------------------------------------+
|                [Play] [Stop] [Loop]                |
+----------------------------------------------------+
| [Import] [Export] [Save] [Cancel]                  |
+----------------------------------------------------+
```

## TilesetEditForm

Tileset management:

```
+----------------------------------------------------+
| Tileset Editor: forest_ground              [X]     |
+----------------------------------------------------+
| Tile Grid:                                         |
| +------------------------------------------------+|
| | [Visual grid of all tiles in tileset]         ||
| +------------------------------------------------+|
+----------------------------------------------------+
| Selected Tile: (3, 2)                              |
| Autotile Pattern: [None v]                         |
+----------------------------------------------------+
| [Import PNG] [Export PNG] [Rebuild] [Save]         |
+----------------------------------------------------+
```

## StringsEditForm

Localization editor:

```
+----------------------------------------------------+
| Strings Editor                             [X]     |
+----------------------------------------------------+
| Language: [English v]                              |
+----------------------------------------------------+
| Key                    | Value                     |
+------------------------+---------------------------+
| menu.start            | Start Game                |
| menu.continue         | Continue                  |
| menu.options          | Options                   |
| dialog.greeting       | Hello, {name}!            |
+------------------------+---------------------------+
| [Add Key] [Remove] [Import CSV] [Export CSV]       |
+----------------------------------------------------+
| [Save] [Cancel]                                    |
+----------------------------------------------------+
```

## Usage

Content editors are opened from the development form:

```csharp
// From Sprites tab
private async void OpenSpeciesEditor(string speciesId)
{
    var vm = new SpeciesEditViewModel();
    vm.LoadSpecies(speciesId);

    var form = new SpeciesEditForm { DataContext = vm };
    await form.ShowDialog(this);
}

// From Data tab for strings
private async void OpenStringsEditor()
{
    var vm = new StringsEditViewModel();
    vm.LoadStrings();

    var form = new StringsEditForm { DataContext = vm };
    await form.ShowDialog(this);
}
```
