# Others

Miscellaneous menus that don't fit other categories. Includes logs, music, rest options, and utility menus.

## Key Files

| File | Description |
|------|-------------|
| `OthersMenu.cs` | "Others" submenu from main menu |
| `LogMenu.cs` | Message log viewer |
| `MsgLogMenu.cs` | Detailed message log |
| `LiveMsgLog.cs` | Real-time message overlay |
| `MusicMenu.cs` | Music jukebox menu |
| `SongSummary.cs` | Song information panel |
| `ReplayMenu.cs` | Replay playback menu |
| `ReplayInfoMenu.cs` | Replay details |
| `RestMenu.cs` | Rest/wait options |
| `WaitMenu.cs` | Wait for turns menu |
| `ChooseMonsterMenu.cs` | Monster selection menu |

## Relationships

- Accessed from **MainMenu** "Others" option
- **MusicMenu** uses **Content/SoundManager**
- **ReplayMenu** uses **DiagManager** replay system

## Usage

```csharp
// Open message log
MsgLogMenu menu = new MsgLogMenu();
MenuManager.Instance.AddMenu(menu, false);
```
