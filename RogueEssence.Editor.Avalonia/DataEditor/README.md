# DataEditor

## Description

A reflection-based property editing system that can generate UI controls for any serializable game object. The DataEditor uses a registry of type-specific editors to create appropriate input controls (textboxes, dropdowns, nested editors) based on the type and attributes of each property. This enables editing of complex nested data structures like dungeon configurations, monster definitions, and skill data.

## Key Files

| File | Description |
|------|-------------|
| `DataEditor.cs` | Static class managing the editor registry; routes types to appropriate editors |
| `DataEditForm.axaml` / `.cs` | Modal dialog window for editing a single object |
| `DataEditRootForm.axaml` / `.cs` | Root form wrapper for top-level data editing |
| `ParentForm.cs` | Base class for editor forms providing common functionality |
| `ClassBox.axaml` / `.cs` / `ViewModel.cs` | Collapsible box for editing class instances with subtype selection |
| `CollectionBox.axaml` / `.cs` / `ViewModel.cs` | Editor for lists/arrays with add/remove/reorder controls |
| `DictionaryBox.axaml` / `.cs` / `ViewModel.cs` | Editor for key-value dictionary types |
| `PriorityListBox.axaml` / `.cs` / `ViewModel.cs` | Editor for priority-ordered lists (used in dungeon generation) |
| `SpawnListBox.axaml` / `.cs` / `ViewModel.cs` | Editor for weighted spawn lists |
| `SpawnRangeListBox.axaml` / `.cs` / `ViewModel.cs` | Editor for spawn lists with floor range restrictions |
| `CategorySpawnBox.axaml` / `.cs` / `ViewModel.cs` | Editor for categorized spawn configurations |
| `RangeDictBox.axaml` / `.cs` / `ViewModel.cs` | Editor for range-keyed dictionaries |
| `RankedCollectionBox.axaml` / `.cs` | Editor for ranked/weighted collections |
| `SpawnListViewBox.axaml` / `.cs` | Read-only view of spawn list contents |

## Relationships

- **Editors/**: Contains `IEditor` implementations for specific types (primitives, game types, etc.)
- **ViewModels/**: Box ViewModels follow MVVM pattern and use ReactiveUI
- **DevDataManager**: Provides documentation tooltips and type size persistence
- **RogueEssence Core**: Edits data types defined in the main game project

## Architecture

```
DataEditor (static registry)
    ├── findEditor(Type) -> IEditor
    ├── LoadClassControls() -> Creates UI from object
    └── SaveClassControls() -> Extracts object from UI

IEditor (interface)
    ├── GetConvertingType() -> Type this editor handles
    ├── GetAttributeType() -> Optional attribute for specialization
    ├── LoadWindowControls() -> Generate editing UI
    └── SaveWindowControls() -> Extract edited values

Editor<T> (base class)
    ├── Handles member iteration via reflection
    ├── Supports SubGroupAttribute for nested editing
    └── Creates ClassBox for polymorphic types
```

## Usage

The DataEditor is used throughout the editor to create property editors:

```csharp
// Load controls for an object
DataEditor.LoadDataControls("Monster", monsterData, editForm);

// Save edited values back
DataEditor.SaveDataControls(ref monsterData, editForm.ControlPanel, new Type[0]);
```

### Custom Attributes

- `[NonEdited]` - Skip this property in the editor
- `[SubGroup]` - Display inline instead of in a collapsible box
- `[SharedRow]` - Display on the same row as previous property
- `[NumberRange(min, max)]` - Constrain numeric input range
- `[DataType(index)]` - Specify which data index to use for lookups

### Adding New Type Editors

1. Create a class in `Editors/` extending `Editor<T>` or implementing `IEditor`
2. Override `LoadWindowControls` to generate UI for your type
3. Override `SaveWindowControls` to extract values from UI
4. Register with `DataEditor.AddEditor(new YourEditor())` during initialization
