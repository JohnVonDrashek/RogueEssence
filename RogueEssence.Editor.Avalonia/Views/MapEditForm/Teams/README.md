# Views/MapEditForm/Teams

## Description

Contains Views for monster team configuration in the dungeon map editor. The TeamWindow dialog allows developers to configure monster teams that will be placed on dungeon maps, including team composition, individual monster settings, and AI behavior.

## Key Files

| File | Description |
|------|-------------|
| `TeamWindow.axaml` / `.cs` | Monster team configuration dialog |

## Relationships

- **ViewModels/MapEditForm/Teams/TeamViewModel**: Team configuration ViewModel
- **MapTabEntitiesViewModel**: Creates and manages teams on the map
- **MonsterIDEditor**: Used for species selection within team members
- **DataEditor**: Used for editing complex member properties

## TeamWindow Layout

```
+--------------------------------------------------+
| Configure Team                            [X]    |
+--------------------------------------------------+
| Team Settings:                                   |
| Name: [Boss Squad_________________________]      |
| AI Type: [Coordinated v]                         |
+--------------------------------------------------+
| Team Members:                     [+] [-] [^][v] |
| +----------------------------------------------+ |
| | 1. Charizard Lv.50  [HP:200]                 | |
| | 2. Dragonite Lv.45  [HP:180]    <- Selected  | |
| | 3. Salamence Lv.45  [HP:175]                 | |
| +----------------------------------------------+ |
+--------------------------------------------------+
| Selected Member:                                 |
| +----------------------------------------------+ |
| | Species: [Dragonite v] Form: [Normal v]      | |
| | Level: [45]     Gender: [Male v]             | |
| | [*] Shiny                                    | |
| +----------------------------------------------+ |
| | Held Item: [Sitrus Berry v]                  | |
| +----------------------------------------------+ |
| | Skills:                                      | |
| | 1. [Dragon Claw v]                           | |
| | 2. [Fly v]                                   | |
| | 3. [Thunder v]                               | |
| | 4. [Fire Punch v]                            | |
| +----------------------------------------------+ |
| | AI Tactic: [Go After Target v]               | |
| +----------------------------------------------+ |
+--------------------------------------------------+
|                    [OK] [Cancel]                 |
+--------------------------------------------------+
```

## AXAML Structure

```xml
<!-- TeamWindow.axaml -->
<Window Title="Configure Team" Width="500" Height="600">
    <DockPanel Margin="10">
        <!-- Team name and AI -->
        <StackPanel DockPanel.Dock="Top">
            <TextBox Text="{Binding TeamName}"
                     Watermark="Team Name"/>
            <ComboBox Items="{Binding AITypes}"
                      SelectedItem="{Binding AIBehavior}"/>
        </StackPanel>

        <!-- Member list -->
        <ListBox DockPanel.Dock="Top" Height="150"
                 Items="{Binding Members}"
                 SelectedItem="{Binding SelectedMember}">
            <ListBox.ItemTemplate>
                <DataTemplate>
                    <TextBlock Text="{Binding DisplayName}"/>
                </DataTemplate>
            </ListBox.ItemTemplate>
        </ListBox>

        <!-- Add/Remove buttons -->
        <StackPanel DockPanel.Dock="Top" Orientation="Horizontal">
            <Button Content="+" Command="{Binding AddMemberCommand}"/>
            <Button Content="-" Command="{Binding RemoveMemberCommand}"/>
        </StackPanel>

        <!-- Selected member details -->
        <ScrollViewer>
            <StackPanel DataContext="{Binding SelectedMember}">
                <!-- Species, level, item, skills... -->
            </StackPanel>
        </ScrollViewer>

        <!-- Dialog buttons -->
        <StackPanel DockPanel.Dock="Bottom"
                    Orientation="Horizontal"
                    HorizontalAlignment="Right">
            <Button Content="OK" Click="OkButton_Click"/>
            <Button Content="Cancel" Click="CancelButton_Click"/>
        </StackPanel>
    </DockPanel>
</Window>
```

## Code-Behind

```csharp
public class TeamWindow : Window
{
    public TeamWindow()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        // Validate team before closing
        var vm = (TeamViewModel)DataContext;
        if (vm.Members.Count == 0)
        {
            MessageBox.Show(this, "Team must have at least one member.",
                           "Invalid Team", MessageBoxButtons.Ok);
            return;
        }

        Close(true);
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        Close(false);
    }
}
```

## Usage

The TeamWindow is opened from the Entities tab:

```csharp
// In MapTabEntitiesViewModel
public async Task PlaceNewTeam(Loc position)
{
    var teamVM = new TeamViewModel();

    var window = new TeamWindow { DataContext = teamVM };
    var parent = GetParentWindow();

    if (await window.ShowDialog<bool>(parent))
    {
        // Convert to game spawn data
        var spawn = teamVM.ToMobSpawn();
        PlaceTeamOnMap(spawn, position);
    }
}

public async Task EditExistingTeam(TeamSpawn spawn)
{
    var teamVM = new TeamViewModel(spawn);

    var window = new TeamWindow { DataContext = teamVM };
    var parent = GetParentWindow();

    if (await window.ShowDialog<bool>(parent))
    {
        teamVM.UpdateSpawn(spawn);
        RefreshDisplay();
    }
}
```
