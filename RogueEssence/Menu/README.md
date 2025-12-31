# Menu

UI menu system for all game interfaces. Provides a comprehensive framework for dialogue boxes, selection menus, settings, and game UI overlays.

## Key Files

| File | Description |
|------|-------------|
| `MenuManager.cs` | Menu stack manager handling display and input routing |
| `MenuBase.cs` | Base class for all menus with common functionality |
| `MainMenu.cs` | In-game main menu (items, team, settings, etc.) |
| `InteractableMenu.cs` | Base for menus with selectable choices |
| `ChoiceMenu.cs` | Simple multiple choice menu |
| `SingleStripMenu.cs` | Single column scrolling menu |
| `MultiPageMenu.cs` | Multi-page menu with page navigation |
| `SideScrollMenu.cs` | Side-scrolling menu layout |
| `TitledStripMenu.cs` | Strip menu with title header |
| `ChooseAmountMenu.cs` | Numeric amount selection menu |
| `TextInputMenu.cs` | Text input with virtual keyboard |
| `SummaryMenu.cs` | Base class for summary display panels |
| `ScriptableMenu.cs` | Lua-scriptable menu base |
| `BaseSettingsMenu.cs` | Base for settings pages |
| `IInteractable.cs` | Interface for interactive menus |
| `ILabeled.cs` | Interface for labeled menu items |

## Subdirectories

| Directory | Purpose |
|-----------|---------|
| `Dialogue/` | Dialogue boxes and text display |
| `Dungeons/` | Dungeon selection and info |
| `Items/` | Item management menus |
| `MenuElements/` | Reusable menu UI components |
| `Mods/` | Mod management menus |
| `Network/` | Multiplayer and contact menus |
| `Notices/` | Notification overlays |
| `Others/` | Miscellaneous menus |
| `Records/` | Score and replay menus |
| `Rescue/` | Rescue mail menus |
| `Rogue/` | Roguelike mode menus |
| `Settings/` | Game settings menus |
| `Skills/` | Skill management menus |
| `Team/` | Team management menus |
| `Top/` | Title screen menus |
| `Underfoot/` | Ground item/tile interaction |

## Relationships

- **MenuManager** is called by all scenes to show menus
- Uses **Content/** for fonts and UI textures
- **Lua/** can create scriptable menus
- Accesses **Data/** for display information

## Usage

```csharp
// Show a simple choice menu
ChoiceMenu menu = new ChoiceMenu("Select option", choices, defaultChoice, action);
MenuManager.Instance.AddMenu(menu, false);

// Show dialogue
yield return MenuManager.Instance.SetDialogue(speaker, "Hello!");

// Access current menu
IInteractable current = MenuManager.Instance.CurrentMenu;
```
