# Views/UserControls

## Description

Contains reusable UI components used across multiple editor forms. These UserControls provide common functionality like layer management panels, team member lists, and searchable combo boxes that appear in both ground and dungeon map editors.

## Key Files

| File | Description |
|------|-------------|
| `LayerBox.axaml` / `.cs` | Layer management panel with add/remove/reorder |
| `TeamBox.axaml` / `.cs` | Team member list with add/remove/edit |
| `SearchComboBox.cs` | ComboBox with search/filter functionality |

## Relationships

- **ViewModels/UserControls/**: Corresponding ViewModels
- **GroundEditForm/GroundTabTextures**: Uses LayerBox for texture layers
- **MapEditForm/MapTabEntities**: Uses TeamBox for monster teams
- **Assets/**: Layer icons referenced by LayerBox

## LayerBox

Layer management panel used in texture and decoration tabs:

```
+------------------------------------------+
| Layers                                    |
+------------------------------------------+
| [+] [-] [Dupe] [Merge]     [^] [v]       |
+------------------------------------------+
| +--------------------------------------+ |
| | [*] Background Layer      ========   | |
| | [*] Floor Details         --------   | |
| | [ ] Foreground (hidden)   --------   | |  <- Selected
| | [*] Overlay               ========   | |
| +--------------------------------------+ |
+------------------------------------------+

[*] = Visibility toggle
[^][v] = Move up/down
[+] = Add layer
[-] = Delete layer
```

### LayerBox.axaml

```xml
<UserControl x:Class="RogueEssence.Dev.Views.LayerBox">
    <DockPanel>
        <!-- Toolbar -->
        <StackPanel DockPanel.Dock="Top" Orientation="Horizontal">
            <Button Command="{Binding AddLayerCommand}">
                <Image Source="/Assets/LayerAdd.png"/>
            </Button>
            <Button Command="{Binding DeleteLayerCommand}">
                <Image Source="/Assets/LayerDelete.png"/>
            </Button>
            <Button Command="{Binding DuplicateLayerCommand}">
                <Image Source="/Assets/LayerDupe.png"/>
            </Button>
            <Button Command="{Binding MergeLayerCommand}">
                <Image Source="/Assets/LayerMerge.png"/>
            </Button>
            <Separator/>
            <Button Command="{Binding MoveUpCommand}">
                <Image Source="/Assets/LayerUp.png"/>
            </Button>
            <Button Command="{Binding MoveDownCommand}">
                <Image Source="/Assets/LayerDown.png"/>
            </Button>
        </StackPanel>

        <!-- Layer list -->
        <ListBox Items="{Binding Layers}"
                 SelectedItem="{Binding SelectedLayer}">
            <ListBox.ItemTemplate>
                <DataTemplate>
                    <StackPanel Orientation="Horizontal">
                        <CheckBox IsChecked="{Binding Visible}"/>
                        <TextBlock Text="{Binding Name}"/>
                    </StackPanel>
                </DataTemplate>
            </ListBox.ItemTemplate>
        </ListBox>
    </DockPanel>
</UserControl>
```

## TeamBox

Team member management panel:

```
+------------------------------------------+
| Team Members                              |
+------------------------------------------+
| [+] [-]                       [^] [v]    |
+------------------------------------------+
| +--------------------------------------+ |
| | Pikachu Lv.25         [HP: 80]       | |
| | Eevee Lv.22           [HP: 65]       | |  <- Selected
| | Jigglypuff Lv.20      [HP: 70]       | |
| +--------------------------------------+ |
+------------------------------------------+
| [Edit Member]                             |
+------------------------------------------+
```

## SearchComboBox

A ComboBox with built-in search/filter:

```csharp
public class SearchComboBox : ComboBox
{
    private TextBox searchBox;
    private string filterText = "";

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        searchBox = e.NameScope.Find<TextBox>("PART_SearchBox");
        if (searchBox != null)
        {
            searchBox.TextChanged += (s, args) =>
            {
                filterText = searchBox.Text;
                RefreshFilter();
            };
        }
    }

    private void RefreshFilter()
    {
        // Filter items based on filterText
        var filtered = AllItems.Where(i =>
            i.ToString().Contains(filterText, StringComparison.OrdinalIgnoreCase));
        Items = filtered.ToList();
    }
}
```

## Usage

UserControls are embedded in larger forms:

```xml
<!-- In GroundTabTextures.axaml -->
<UserControl>
    <Grid ColumnDefinitions="*,200">
        <!-- Main editing canvas -->
        <Canvas Grid.Column="0" ... />

        <!-- Layer panel -->
        <local:LayerBox Grid.Column="1"
                        DataContext="{Binding LayerBox}"/>
    </Grid>
</UserControl>

<!-- In MapTabEntities.axaml -->
<UserControl>
    <Grid ColumnDefinitions="200,*">
        <!-- Team list -->
        <local:TeamBox Grid.Column="0"
                       DataContext="{Binding TeamBox}"/>

        <!-- Map canvas -->
        <Canvas Grid.Column="1" ... />
    </Grid>
</UserControl>
```

### Instantiation

```csharp
// Programmatic creation
var layerBox = new LayerBox();
layerBox.DataContext = new LayerBoxViewModel();
parentPanel.Children.Add(layerBox);

// Or in XAML with binding
<local:LayerBox DataContext="{Binding TextureLayerBox}"/>
```
