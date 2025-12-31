# Views/DevForm

## Description

Contains Views for the main development form and its tabbed interface. The DevForm is the primary editor window that provides access to all development tools through a tabbed layout. Each tab has its own View file for the specific functionality it provides.

## Key Files

| File | Description |
|------|-------------|
| `DevForm.axaml` / `.cs` | Main window with tab container and menu bar |
| `DevTabGame.axaml` / `.cs` | Game runtime controls (pause, speed, debug) |
| `DevTabPlayer.axaml` / `.cs` | Player/party management interface |
| `DevTabData.axaml` / `.cs` | Game data browser with type selection |
| `DevTabTravel.axaml` / `.cs` | Zone/map navigation and teleport |
| `DevTabSprites.axaml` / `.cs` | Sprite sheet management tools |
| `DevTabScript.axaml` / `.cs` | Lua script console and execution |
| `DevTabMods.axaml` / `.cs` | Mod management interface |
| `DevTabConstants.axaml` / `.cs` | Game constants editor |

## Relationships

- **ViewModels/DevForm/**: Corresponding ViewModels for each tab
- **App.axaml.cs**: Creates DevForm as the main window
- **MapEditForm/GroundEditForm**: Opened from DevForm for map editing
- **IRootEditor**: DevForm implements editor interface for game integration

## DevForm Layout

```
+----------------------------------------------------------+
| RogueEssence Dev Editor                          [-][O][X]|
+----------------------------------------------------------+
| File  Edit  View  Tools  Help                             |
+----------------------------------------------------------+
| [Game][Player][Data][Travel][Sprites][Script][Mods][Const]|
+----------------------------------------------------------+
|                                                           |
|                                                           |
|                   (Active Tab Content)                    |
|                                                           |
|                                                           |
+----------------------------------------------------------+
| Status: Ready                                             |
+----------------------------------------------------------+
```

## Tab Views

### DevTabGame
```
+----------------------------------+
| Game Control                      |
+----------------------------------+
| [ ] Pause Game                    |
| Speed: [1.0x v]                   |
+----------------------------------+
| Debug Options:                    |
| [ ] Show Hitboxes                 |
| [ ] Show Grid                     |
| [ ] Show FPS                      |
+----------------------------------+
| [Step Frame] [Reset]              |
+----------------------------------+
```

### DevTabPlayer
```
+----------------------------------+
| Player Stats                      |
+----------------------------------+
| Name: [Adventurer]                |
| Level: [50]  HP: [100/100]        |
+----------------------------------+
| Inventory:                        |
| [List of items with +/-]          |
+----------------------------------+
| Skills:                           |
| [List of skills with +/-]         |
+----------------------------------+
```

### DevTabData
```
+----------------------------------+
| Data Type: [Monster v]            |
+----------------------------------+
| Search: [________]                |
| +------------------------------+ |
| | 001 - Bulbasaur              | |
| | 002 - Ivysaur                | |
| | 003 - Venusaur               | |
| +------------------------------+ |
+----------------------------------+
| [New] [Edit] [Delete] [Reindex]  |
+----------------------------------+
```

### DevTabTravel
```
+----------------------------------+
| Zone: [Thunderwave Cave v]        |
| Floor: [5]                        |
+----------------------------------+
| Position:                         |
| X: [12]  Y: [8]                   |
+----------------------------------+
| [Teleport] [Reload Map]           |
+----------------------------------+
| Bookmarks:                        |
| [List of saved locations]         |
+----------------------------------+
```

## DevForm Code-Behind

The code-behind handles game thread synchronization:

```csharp
public class DevForm : Window, IRootEditor
{
    public MapEditForm MapEditForm;
    public GroundEditForm GroundEditForm;

    // Thread-safe execution for game operations
    public static void ExecuteOrPend(Action action)
    {
        // Marshal to game thread if needed
    }

    // Data reload when game data changes
    void IRootEditor.ReloadData(DataType dataType)
    {
        ExecuteOrInvoke(() => reload(dataType));
    }
}
```

## Usage

DevForm is created at application startup:

```csharp
// In App.axaml.cs
public override void OnFrameworkInitializationCompleted()
{
    if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
    {
        desktop.MainWindow = new DevForm
        {
            DataContext = new DevFormViewModel(),
        };
    }
}
```
