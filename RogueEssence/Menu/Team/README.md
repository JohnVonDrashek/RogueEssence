# Team

Team and party management menus for viewing members, managing assembly, and party configuration.

## Key Files

| File | Description |
|------|-------------|
| `TeamMenu.cs` | Main party team menu |
| `TeamChosenMenu.cs` | Actions for team member |
| `TeamMiniSummary.cs` | Member preview panel |
| `TeamNameMenu.cs` | Team name editing |
| `AssemblyMenu.cs` | Full roster assembly menu |
| `AssemblyChosenMenu.cs` | Assembly member actions |
| `AddToTeamMenu.cs` | Add member to party |
| `MemberInfoMenu.cs` | Detailed member info |
| `MemberStatsMenu.cs` | Stats display |
| `MemberFeaturesMenu.cs` | Abilities and features |
| `MemberLearnsetMenu.cs` | Learnable moves |
| `LevelUpMenu.cs` | Level up notification |
| `PromoteMenu.cs` | Evolution/promotion menu |
| `StatusMenu.cs` | Status effects display |
| `TacticsMenu.cs` | AI tactics selection |
| `NicknameMenu.cs` | Nickname editing |
| `OfferFeaturesMenu.cs` | Offered features display |
| `TradeTeamMenu.cs` | Multiplayer team trading |

## Relationships

- Manages **Dungeon/Team** party
- Uses **Data/MonsterData** for species info
- Integrates with **Network/** for trading

## Usage

```csharp
// Open team menu
TeamMenu menu = new TeamMenu(team);
MenuManager.Instance.AddMenu(menu, false);
```
