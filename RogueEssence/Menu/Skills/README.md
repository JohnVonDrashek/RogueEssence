# Skills

Skill and move management menus for viewing, learning, forgetting, and configuring moves.

## Key Files

| File | Description |
|------|-------------|
| `SkillMenu.cs` | Main skill list menu |
| `SkillChosenMenu.cs` | Actions for selected skill |
| `SkillSummary.cs` | Skill detail summary |
| `SkillForgetMenu.cs` | Forget move selection |
| `SkillReplaceMenu.cs` | Replace move selection |
| `SkillRecallMenu.cs` | Recall forgotten moves |
| `IntrinsicForgetMenu.cs` | Forget ability |
| `IntrinsicRecallMenu.cs` | Recall ability |
| `TutorTeamMenu.cs` | Move tutor team selection |
| `FacilityTeamChosenMenu.cs` | Facility team options |

## Relationships

- Uses **Data/SkillData** for move info
- Modifies **Dungeon/Characters/** movesets
- Integrates with **Ground/** facilities

## Usage

```csharp
// Open skill menu for character
SkillMenu menu = new SkillMenu(character);
MenuManager.Instance.AddMenu(menu, false);
```
