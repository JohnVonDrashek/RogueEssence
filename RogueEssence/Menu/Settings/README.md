# Settings

Game settings and configuration menus for audio, controls, display, and preferences.

## Key Files

| File | Description |
|------|-------------|
| `SettingsMenu.cs` | Main settings menu |
| `SettingsPageMenu.cs` | Paginated settings display |
| `SettingsTitleMenu.cs` | Settings page header |
| `OptionsMenu.cs` | General options |
| `LanguageMenu.cs` | Language selection |
| `KeyControlsMenu.cs` | Keyboard control remapping |
| `GamepadControlsMenu.cs` | Gamepad button remapping |
| `GetKeyMenu.cs` | Key input capture |
| `GetButtonMenu.cs` | Button input capture |

## Relationships

- Modifies **DiagManager.CurSettings**
- Uses **Settings** class for configuration
- Saves to config files via **DiagManager**

## Usage

```csharp
// Open settings
SettingsMenu menu = new SettingsMenu();
MenuManager.Instance.AddMenu(menu, false);
```
