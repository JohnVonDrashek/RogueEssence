# Rogue

Roguelike mode menus for character selection, quicksaves, and rogue-specific options.

## Key Files

| File | Description |
|------|-------------|
| `RogueMenu.cs` | Main rogue mode menu |
| `RogueDestMenu.cs` | Destination selection |
| `RogueInfoMenu.cs` | Rogue mode information |
| `CharaChoiceMenu.cs` | Character/starter selection |
| `CharaDetailMenu.cs` | Character detail view |
| `CharaSummary.cs` | Character preview summary |
| `QuicksaveMenu.cs` | Quicksave slot selection |
| `QuicksaveChosenMenu.cs` | Quicksave actions |
| `RogueTeamInputMenu.cs` | Team name input |
| `SeedInputMenu.cs` | Random seed input |
| `SeedSummary.cs` | Seed information display |

## Relationships

- Starts new games via **Scene/GameManager**
- Uses **Data/GameProgress** for saves
- Configures **Dungeon/** entry parameters

## Usage

```csharp
// Open rogue mode
RogueMenu menu = new RogueMenu();
MenuManager.Instance.AddMenu(menu, false);
```
