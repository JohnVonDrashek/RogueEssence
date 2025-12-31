# ViewModels/UserControls

## Description

Contains ViewModels for reusable UI components used across multiple editor forms. These controls provide common functionality like layer management panels and team member lists that appear in both ground and dungeon map editors.

## Key Files

| File | Description |
|------|-------------|
| `LayerBoxViewModel.cs` | ViewModel for the layer management panel with add/remove/reorder |
| `TeamBoxViewModel.cs` | ViewModel for team member list with add/remove/edit |

## Relationships

- **Views/UserControls/**: Corresponding UserControl Views
- **GroundEditForm/Layers/**: Inherits layer patterns from this base
- **MapEditForm/Teams/**: Uses team management patterns
- **Assets/**: Layer icons used by LayerBox

## LayerBoxViewModel

Generic layer management panel:

```csharp
public class LayerBoxViewModel : ViewModelBase
{
    public ObservableCollection<ILayerViewModel> Layers { get; }

    private ILayerViewModel selectedLayer;
    public ILayerViewModel SelectedLayer
    {
        get => selectedLayer;
        set => this.RaiseAndSetIfChanged(ref selectedLayer, value);
    }

    // Layer operations
    public void AddLayer() { ... }
    public void DeleteLayer() { ... }
    public void DuplicateLayer() { ... }
    public void MergeLayerDown() { ... }
    public void MoveLayerUp() { ... }
    public void MoveLayerDown() { ... }

    // Visibility toggle
    public void ToggleLayerVisibility(ILayerViewModel layer)
    {
        layer.Visible = !layer.Visible;
    }
}
```

## TeamBoxViewModel

Team member management panel:

```csharp
public class TeamBoxViewModel : ViewModelBase
{
    public ObservableCollection<TeamMemberViewModel> Members { get; }

    private TeamMemberViewModel selectedMember;
    public TeamMemberViewModel SelectedMember
    {
        get => selectedMember;
        set => this.RaiseAndSetIfChanged(ref selectedMember, value);
    }

    // Member operations
    public void AddMember() { ... }
    public void RemoveMember() { ... }
    public void EditMember() { ... }
    public void MoveMemberUp() { ... }
    public void MoveMemberDown() { ... }

    // Team validation
    public bool IsValidTeam => Members.Count > 0 && Members.Count <= MaxTeamSize;
}
```

## LayerBox UI Pattern

```
+----------------------------------+
| Layers                    [+][-] |
+----------------------------------+
| [^][v]                          |
| +------------------------------+|
| | [*] Background      -------- ||
| | [*] Midground       ======== ||
| | [ ] Foreground      -------- ||  <- Hidden
| +------------------------------+|
+----------------------------------+
| [Merge] [Dupe] [Order]          |
+----------------------------------+

[*] = Visible toggle
[^][v] = Move up/down
[+][-] = Add/Delete
```

## Usage

UserControls are embedded in larger editor forms:

```xml
<!-- In GroundTabTextures.axaml -->
<UserControl>
    <DockPanel>
        <!-- Main editing area -->
        <Canvas DockPanel.Dock="Left" ... />

        <!-- Layer panel on right -->
        <local:LayerBox DockPanel.Dock="Right"
                        DataContext="{Binding LayerBox}"/>
    </DockPanel>
</UserControl>
```

### Reusing in Different Contexts

```csharp
// Ground map textures
var textureLayerBox = new LayerBoxViewModel();
textureLayerBox.Layers.Add(new TextureLayerViewModel { Name = "Floor" });

// Dungeon map decorations
var decoLayerBox = new LayerBoxViewModel();
decoLayerBox.Layers.Add(new DecorationLayerViewModel { Name = "Objects" });

// Both use same LayerBox control, different layer types
```
