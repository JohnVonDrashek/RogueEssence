# Mods

Mod management menus for enabling, disabling, and configuring game modifications.

## Key Files

| File | Description |
|------|-------------|
| `ModsMenu.cs` | Mod list and toggle menu |
| `QuestsMenu.cs` | Quest mod selection |
| `ModMiniSummary.cs` | Mod information summary |
| `ModLogMenu.cs` | Mod loading log display |

## Relationships

- Uses **PathMod** for mod discovery and loading
- Modifies **DiagManager** mod settings
- Requires game restart for some changes

## Usage

```csharp
// Open mod manager
ModsMenu menu = new ModsMenu();
MenuManager.Instance.AddMenu(menu, false);
```
