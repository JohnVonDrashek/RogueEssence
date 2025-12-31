# RogueEssence.Editor.Avalonia

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/)
[![Avalonia](https://img.shields.io/badge/Avalonia-0.10.0-8B5CF6)](https://avaloniaui.net/)
[![ReactiveUI](https://img.shields.io/badge/ReactiveUI-Enabled-61DAFB)](https://reactiveui.net/)

## Description

The Avalonia-based development editor for RogueEssence, a roguelike dungeon crawler engine. This editor provides a comprehensive GUI for creating and editing game content including dungeon maps, ground maps, sprites, tilesets, and game data. It follows the MVVM (Model-View-ViewModel) pattern using ReactiveUI for data binding and reactive programming.

## Key Files

| File | Description |
|------|-------------|
| `App.axaml` / `App.axaml.cs` | Application entry point and XAML loader, initializes the DevForm as main window |
| `Program.cs` | Main program entry with Avalonia configuration and ReactiveUI setup |
| `ViewLocator.cs` | Convention-based View-ViewModel resolver that maps ViewModels to Views automatically |
| `DevDataManager.cs` | Central manager for editor data including tile caches, icon loading, documentation, and settings |
| `EditModes.cs` | Enums defining editing modes (Draw, Rectangle, Fill, Eyedrop for tiles; Select/Place for entities) |
| `ReactiveExt.cs` | Extension methods for ReactiveUI including `SetIfChanged` and form navigation helpers |
| `RogueEssence.Editor.Avalonia.csproj` | Project file with dependencies on Avalonia, ReactiveUI, and RogueEssence core |

## Relationships

- **RogueEssence Core**: References the main `RogueEssence.csproj` for game data types, dungeon structures, and content management
- **MVVM Architecture**: Uses `ViewModels/` for logic and state, `Views/` for UI, with automatic binding via `ViewLocator`
- **Data Editing**: `DataEditor/` provides a flexible system for editing any serializable game object using reflection
- **Converters**: `Converters/` provides value converters for XAML data binding transformations

## Directory Structure

```
RogueEssence.Editor.Avalonia/
├── Assets/          # Icons and images for the editor UI
├── Converters/      # IValueConverter implementations for XAML bindings
├── DataEditor/      # Reflection-based property editor system
│   └── Editors/     # Type-specific editors (Primitive, System, RogueEssence)
├── ViewModels/      # MVVM ViewModels with ReactiveUI
│   ├── Content/     # Asset editing (sprites, tilesets, strings)
│   ├── DevForm/     # Main development form tabs
│   ├── DialogBoxes/ # Modal dialog ViewModels
│   ├── GroundEditForm/  # Ground map editor
│   ├── MapEditForm/     # Dungeon map editor
│   └── UserControls/    # Reusable component ViewModels
└── Views/           # Avalonia XAML Views
    ├── Content/     # Asset editor windows
    ├── DevForm/     # Main form and tab views
    ├── DialogBoxes/ # Modal dialog windows
    ├── GroundEditForm/  # Ground editor views
    ├── MapEditForm/     # Map editor views
    └── UserControls/    # Reusable UI components
```

## Usage

The editor is launched as part of the RogueEssence development build. The main window (`DevForm`) provides tabbed access to:

- **Game**: Runtime game controls and debugging
- **Player**: Character and party management
- **Data**: Game data type editing (monsters, items, skills, etc.)
- **Travel**: Zone and map navigation
- **Sprites**: Character sprite management
- **Script**: Lua script execution
- **Mods**: Mod management
- **Constants**: Game constant configuration

### Extending the Editor

1. **New Data Type**: Add an editor in `DataEditor/Editors/` implementing `IEditor`
2. **New View**: Create matching View/ViewModel pairs; ViewLocator handles binding automatically
3. **New Converter**: Add to `Converters/` and register in `App.axaml` resources

![Repobeats analytics](https://repobeats.axiom.co/api/embed/placeholder.svg "Repobeats analytics image")
