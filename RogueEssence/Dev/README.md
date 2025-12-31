# Dev

Development tools, editor support, and asset import utilities. This module provides functionality for creating and modifying game content, including map editors and asset conversion.

## Key Files

| File | Description |
|------|-------------|
| `DevHelper.cs` | Development utilities for data indexing and file operations |
| `ImportHelper.cs` | Asset import and conversion for sprites, tiles, and fonts |
| `DtefImportHelper.cs` | DTEF (Dungeon Tile Exchange Format) autotile importer |
| `DungeonEditScene.cs` | In-game dungeon map editor scene |
| `GroundEditScene.cs` | In-game ground/overworld map editor scene |
| `ReflectionExt.cs` | Reflection utilities for serialization and editor UI |
| `UndoStack.cs` | Undo/redo system for editor operations |
| `CanvasStroke.cs` | Drawing stroke data for map painting |
| `IRootEditor.cs` | Interface for the main editor window |
| `IMapEditor.cs` | Interface for dungeon map editor |
| `IGroundEditor.cs` | Interface for ground map editor |
| `EmptyEditor.cs` | Null editor implementation for non-dev builds |
| `CharSheetOp.cs` | Character sheet operations interface |
| `CharSheetDummyOp.cs` | Dummy character sheet operations |
| `PartialType.cs` | Partial type information for serialization |

## Subdirectories

| Directory | Purpose |
|-----------|---------|
| `Converters/` | Data format upgrade converters for version migration |
| `CustomAttributes/` | Custom attributes for editor UI generation |

## Relationships

- **DiagManager** enables dev mode and loads the editor
- **GraphicsManager** uses ImportHelper for asset conversion
- Editor scenes extend base scenes from **Scene/**
- **Lua/** scripts can be edited and hot-reloaded in dev mode

## Usage

```csharp
// Convert raw sprites to game format
ImportHelper.ImportAllChars(sourcePath, destPattern);

// Build character sprite index
ImportHelper.BuildCharIndex(charPattern);

// Index data files
DevHelper.IndexNamedData(dataPath);
```
