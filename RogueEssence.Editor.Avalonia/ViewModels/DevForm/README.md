# ViewModels/DevForm

## Description

Contains ViewModels for the main development form and its tabbed interface. The DevForm is the primary editor window with tabs for different aspects of game development: runtime control, player management, data editing, travel/debugging, sprite tools, scripting, mods, and constants.

## Key Files

| File | Description |
|------|-------------|
| `DevFormViewModel.cs` | Root ViewModel aggregating all tab ViewModels |
| `DevTabGameViewModel.cs` | Game runtime controls (pause, speed, debug flags) |
| `DevTabPlayerViewModel.cs` | Player/party management (stats, inventory, skills) |
| `DevTabDataViewModel.cs` | Game data browser and editor launcher for all data types |
| `DevTabTravelViewModel.cs` | Zone/map navigation and teleportation tools |
| `DevTabSpritesViewModel.cs` | Sprite sheet management and operations |
| `DevTabScriptViewModel.cs` | Lua script execution and debugging |
| `DevTabModsViewModel.cs` | Mod loading, configuration, and management |
| `DevTabConstantsViewModel.cs` | Game constant editing (balance values, limits) |

## Relationships

- **Views/DevForm/**: Corresponding tab Views (`DevTabGame.axaml`, etc.)
- **DevForm.axaml.cs**: The main window that hosts these tabs
- **DataManager**: Data tab accesses all game data indices
- **ZoneManager**: Travel tab uses zone/map data for navigation

## DevFormViewModel

The root ViewModel that composes all tabs:

```csharp
public class DevFormViewModel : ViewModelBase
{
    public DevFormViewModel()
    {
        Game = new DevTabGameViewModel();
        Player = new DevTabPlayerViewModel();
        Data = new DevTabDataViewModel();
        Travel = new DevTabTravelViewModel();
        Sprites = new DevTabSpritesViewModel();
        Script = new DevTabScriptViewModel();
        Mods = new DevTabModsViewModel();
        Constants = new DevTabConstantsViewModel();
    }

    public DevTabGameViewModel Game { get; }
    public DevTabPlayerViewModel Player { get; }
    public DevTabDataViewModel Data { get; }
    // ... etc
}
```

## Tab Descriptions

### Game Tab
- Pause/resume game execution
- Adjust game speed multiplier
- Toggle debug rendering flags
- Frame stepping for debugging

### Player Tab
- Edit player stats (HP, level, etc.)
- Manage inventory items
- Add/remove skills
- Party member management

### Data Tab
- Browse all data types (Monsters, Items, Skills, etc.)
- Open data entries in DataEditor
- Create new entries
- Search and filter

### Travel Tab
- Zone/dungeon selection
- Floor/map teleportation
- Coordinate input for precise positioning
- Bookmark system for quick access

### Sprites Tab
- Character sheet operations
- Batch sprite processing
- Import/export sprite data

### Script Tab
- Lua console for script execution
- Script file management
- Debug output viewing

### Mods Tab
- List installed mods
- Enable/disable mods
- Mod load order management
- Create new mod packages

### Constants Tab
- Edit game balance constants
- Numeric limits and caps
- Feature toggles

## Usage

The DevFormViewModel is created at application startup:

```csharp
// App.axaml.cs
public override void OnFrameworkInitializationCompleted()
{
    desktop.MainWindow = new DevForm
    {
        DataContext = new DevFormViewModel(),
    };
}
```

Each tab binds to its sub-ViewModel:

```xml
<!-- DevForm.axaml -->
<TabControl>
    <TabItem Header="Game" Content="{Binding Game}"/>
    <TabItem Header="Player" Content="{Binding Player}"/>
    <TabItem Header="Data" Content="{Binding Data}"/>
    <!-- ... -->
</TabControl>
```
