# Notices

Notification overlays and brief information displays. Non-blocking UI elements that inform without interrupting gameplay.

## Key Files

| File | Description |
|------|-------------|
| `HotkeyMenu.cs` | Hotkey/shortcut display overlay |
| `TeamModeNotice.cs` | Team mode toggle notification |

## Relationships

- Displayed by **MenuManager** as overlays
- Triggered by **Dungeon/** gameplay events

## Usage

```csharp
// Show team mode notice
TeamModeNotice notice = new TeamModeNotice(enabled);
MenuManager.Instance.AddMenu(notice, false);
```
