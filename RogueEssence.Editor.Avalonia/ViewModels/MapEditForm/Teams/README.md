# ViewModels/MapEditForm/Teams

## Description

Contains ViewModels for monster team configuration in the dungeon map editor. Monster teams represent groups of enemies that spawn together, with shared AI behavior and coordinated actions. The TeamViewModel handles individual team configuration including member composition and team-level settings.

## Key Files

| File | Description |
|------|-------------|
| `TeamViewModel.cs` | ViewModel for configuring a monster team (members, AI, behavior) |

## Relationships

- **Views/MapEditForm/Teams/TeamWindow**: Dialog for team editing
- **MapTabEntitiesViewModel**: Creates and manages teams on the map
- **MonsterIDEditor**: Used for selecting team member species
- **MobSpawn**: Core game type for monster spawn configuration

## TeamViewModel

Represents a placeable monster team:

```csharp
public class TeamViewModel : ViewModelBase
{
    // Team members (monsters)
    public ObservableCollection<TeamMemberViewModel> Members { get; }

    // Team-level settings
    public string TeamName { get; set; }
    public AIType AIBehavior { get; set; }

    // Selected member for editing
    public TeamMemberViewModel SelectedMember { get; set; }

    public void AddMember()
    {
        Members.Add(new TeamMemberViewModel());
    }

    public void RemoveMember()
    {
        if (SelectedMember != null)
            Members.Remove(SelectedMember);
    }
}

public class TeamMemberViewModel : ViewModelBase
{
    public MonsterID Species { get; set; }      // Species, form, gender, shiny
    public int Level { get; set; }
    public string Nickname { get; set; }
    public InvItem HeldItem { get; set; }
    public List<string> Skills { get; set; }
    public AITactic Tactic { get; set; }
}
```

## Team Configuration

Teams support various configurations:

### Single Monster
```csharp
var team = new TeamViewModel();
team.AddMember(new TeamMemberViewModel
{
    Species = new MonsterID("rattata"),
    Level = 5
});
```

### Monster Group
```csharp
var team = new TeamViewModel { TeamName = "Pikachu Family" };
team.AddMember(/* Pikachu */);
team.AddMember(/* Pichu */);
team.AddMember(/* Raichu */);
team.AIBehavior = AIType.Coordinated;
```

### Boss with Minions
```csharp
var team = new TeamViewModel { TeamName = "Boss Fight" };
team.AddMember(/* Boss monster with high level */);
team.AddMember(/* Minion 1 */);
team.AddMember(/* Minion 2 */);
```

## Usage

Teams are created from the Entities tab:

```csharp
// In MapTabEntitiesViewModel
public void PlaceNewTeam(Loc position)
{
    var teamVM = new TeamViewModel();

    // Show team editor dialog
    var window = new TeamWindow { DataContext = teamVM };
    if (await window.ShowDialog<bool>(parent))
    {
        // Create actual team spawn
        var spawn = teamVM.ToMobSpawn();
        PlaceTeamOnMap(spawn, position);
    }
}
```

### Team Window Dialog

```
+----------------------------------+
| Team Editor                       |
+----------------------------------+
| Team Name: [Boss Squad]           |
| AI Type: [Coordinated v]          |
+----------------------------------+
| Members:                          |
| [+] [-]                          |
| - Charizard Lv.50                |
| - Dragonite Lv.45   <- Selected  |
| - Salamence Lv.45                |
+----------------------------------+
| Member Details:                   |
| Species: [Dragonite]              |
| Level: [45]                       |
| Held Item: [None]                 |
+----------------------------------+
|           [OK] [Cancel]           |
+----------------------------------+
```
