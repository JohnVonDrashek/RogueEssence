# DataEditor/Editors

## Description

Contains `IEditor` implementations that define how specific types are rendered and edited in the DataEditor system. Each editor knows how to create appropriate UI controls for its type and how to extract edited values back. Editors are organized by category: primitive types, .NET system types, RogueElements library types, and RogueEssence game-specific types.

## Key Files

| File | Description |
|------|-------------|
| `IEditor.cs` | Interface defining the editor contract: type identification, UI generation, value extraction |
| `Editor.cs` | Abstract base class with common functionality for reflection-based member editing |
| `DataFolderEditor.cs` | Editor for selecting data folders/directories |
| `StringConv.cs` | String conversion utilities for editor display |

## Subdirectories

| Directory | Description |
|-----------|-------------|
| `Primitive/` | Editors for C# primitive types (int, bool, string, etc.) |
| `System/` | Editors for .NET framework types (arrays, lists, dictionaries, enums) |
| `RogueElements/` | Editors for RogueElements library types (Loc, IntRange, random pickers) |
| `RogueEssence/` | Editors for game-specific types (monsters, items, skills, animations) |

## Relationships

- **DataEditor.cs**: Maintains registry of all editors; calls `AddEditor()` for each
- **Primitive/**: Simple single-control editors (NumericUpDown, TextBox, CheckBox)
- **System/**: Collection editors that recursively use DataEditor for elements
- **RogueEssence/**: Complex editors with game-aware dropdowns and previews

## IEditor Interface

```csharp
public interface IEditor
{
    bool SimpleEditor { get; }                    // Opens inline by default
    Type GetAttributeType();                       // Optional attribute for specialization
    Type GetConvertingType();                      // The type this editor handles

    void LoadClassControls(...);                   // Create UI as embedded controls
    void LoadWindowControls(...);                  // Create UI for dedicated window
    void LoadMemberControl(...);                   // Create UI for a single member

    object SaveClassControls(...);                 // Extract value from embedded UI
    object SaveWindowControls(...);                // Extract value from window UI
    object SaveMemberControl(...);                 // Extract single member value

    string GetString(object obj, ...);             // Display string for object
    string GetTypeString();                        // Friendly type name
}
```

## Editor<T> Base Class

The `Editor<T>` base class provides:
- Automatic member enumeration via reflection
- Support for `[SubGroup]` attribute for inline editing
- Support for `[SharedRow]` attribute for horizontal layouts
- Label generation with documentation tooltips
- Type hierarchy handling for polymorphic editing

## Usage

Editors are registered during application startup:

```csharp
// Primitive types
DataEditor.AddEditor(new IntEditor());
DataEditor.AddEditor(new StringEditor());
DataEditor.AddEditor(new BooleanEditor());

// Game types
DataEditor.AddEditor(new MonsterIDEditor());
DataEditor.AddEditor(new TileLayerEditor());
```

The DataEditor selects the most specific editor based on type inheritance.
