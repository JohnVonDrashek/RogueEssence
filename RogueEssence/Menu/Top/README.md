# Top

Title screen and top-level menus displayed before entering the game.

## Key Files

| File | Description |
|------|-------------|
| `TopMenu.cs` | Main title screen menu |
| `ModDiffDialog.cs` | Mod version difference dialog |
| `ModDiffSummary.cs` | Mod difference summary panel |

## Relationships

- Displayed by **Scene/TitleScene**
- Leads to **Rogue/** or save loading
- Shows **Mods/** if mod changes detected

## Usage

```csharp
// The top menu is shown by TitleScene
TopMenu menu = new TopMenu();
MenuManager.Instance.AddMenu(menu, false);
```
