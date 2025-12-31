# Dungeons

Dungeon selection and information menus. Allows players to choose which dungeon to enter and view dungeon details.

## Key Files

| File | Description |
|------|-------------|
| `DungeonsMenu.cs` | Dungeon selection list menu |
| `DungeonSummary.cs` | Dungeon information summary panel |
| `DungeonEnterDialog.cs` | Confirmation dialog for entering dungeon |

## Relationships

- Accesses **Data/ZoneData** for dungeon information
- Triggers dungeon entry via **Scene/GameManager**
- Displayed from ground areas

## Usage

```csharp
// Show dungeon selection
DungeonsMenu menu = new DungeonsMenu(availableDungeons);
MenuManager.Instance.AddMenu(menu, false);
```
