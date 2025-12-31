# DataEditor/Editors/System

## Description

Contains editors for .NET framework collection and system types. These editors handle arrays, lists, dictionaries, enums, and other standard library types. They provide aggregate editing functionality and recursively use the DataEditor system for their element types.

## Key Files

| File | Description |
|------|-------------|
| `ArrayEditor.cs` | Editor for fixed-size arrays; creates indexed collection UI |
| `ListEditor.cs` | Editor for `List<T>` with add/remove/reorder operations |
| `DictionaryEditor.cs` | Editor for `Dictionary<K,V>` with key-value pair editing |
| `HashSetEditor.cs` | Editor for `HashSet<T>` with unique element constraints |
| `NoDupeListEditor.cs` | Editor for lists that enforce unique elements |
| `EnumEditor.cs` | Editor for enum types using ComboBox dropdown |
| `TypeEditor.cs` | Editor for `System.Type` values with type picker |
| `GuidEditor.cs` | Editor for GUID values with generation support |
| `ObjectEditor.cs` | Fallback editor for `object` type using polymorphic ClassBox |

## Relationships

- **CollectionBox**: `ListEditor` and similar use `CollectionBox` control for list management
- **DictionaryBox**: `DictionaryEditor` uses `DictionaryBox` for key-value editing
- **DataEditor**: Collection editors recursively call `DataEditor.LoadClassControls` for elements
- **EnumEditor**: Reads enum values via reflection; supports `[Flags]` enums

## Collection Editor Pattern

Collection editors delegate to specialized controls:

```csharp
public class ListEditor : Editor<IList>
{
    public override void LoadWindowControls(StackPanel control, ..., IList member, ...)
    {
        CollectionBox listBox = new CollectionBox();
        // Configure element type from generic argument
        // Set up add/remove/edit callbacks
        listBox.DataContext = new CollectionBoxViewModel(member, elementType);
        control.Children.Add(listBox);
    }
}
```

## EnumEditor

The enum editor automatically generates a dropdown with all enum values:

```csharp
public class EnumEditor : Editor<Enum>
{
    public override void LoadWindowControls(...)
    {
        ComboBox cbValue = new ComboBox();
        // Populate with Enum.GetValues(type)
        // Handle [Flags] enums with multi-select
    }
}
```

## Usage

System editors handle standard .NET types found in game data:

```csharp
public class DungeonConfig
{
    public List<FloorPlan> Floors;           // -> ListEditor
    public Dictionary<string, int> Stats;    // -> DictionaryEditor
    public DungeonType Type;                  // -> EnumEditor (enum)
    public Type SpawnerClass;                 // -> TypeEditor
}
```

### Supported Collection Types

- `T[]` - Fixed arrays
- `List<T>` - Dynamic lists
- `Dictionary<K,V>` - Key-value maps
- `HashSet<T>` - Unique element sets
- Any `IList` implementation
