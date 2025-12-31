# CustomAttributes

Custom attributes for controlling editor UI generation and data validation. These attributes are applied to data class properties to customize how they appear in the development editor.

## Key Files

| File | Description |
|------|-------------|
| `DataTypeAttribute.cs` | Specifies data type category for editor dropdowns |
| `MonsterIDAttribute.cs` | Marks fields as monster ID references |
| `StringKeyAttribute.cs` | Marks fields as localization string keys |
| `SoundAttribute.cs` | Marks fields as sound effect references |
| `AnimAttribute.cs` | Marks fields as animation references |
| `FrameTypeAttribute.cs` | Specifies animation frame type |
| `NumberRangeAttribute.cs` | Defines valid numeric range |
| `FractionLimitAttribute.cs` | Limits fraction precision |
| `EditorHeightAttribute.cs` | Sets editor UI height |
| `MultilineAttribute.cs` | Enables multiline text editing |
| `CollectionAttribute.cs` | Customizes collection editing |
| `RankedListAttribute.cs` | Marks list as priority-ranked |
| `SubGroupAttribute.cs` | Groups related properties |
| `TypeConstraintAttribute.cs` | Limits allowed types in polymorphic fields |
| `MapItemAttribute.cs` | Marks fields as map item references |
| `PassableAttribute.cs` | Marks terrain as passable/impassable |
| `RangeBorderAttribute.cs` | Defines range with border constraints |
| `NonEditedAttribute.cs` | Hides field from editor |
| `NonNullAttribute.cs` | Requires non-null value |
| `NoDupeAttribute.cs` | Prevents duplicate entries |
| `SanitizeAttribute.cs` | Sanitizes string input |
| `SharedRowAttribute.cs` | Shares row with adjacent field |
| `ListCollapseAttribute.cs` | Collapses list in editor |
| `AliasAttribute.cs` | Provides display alias for field |

## Relationships

- Read by **ReflectionExt** for editor UI generation
- Applied to data classes in **Data/** module
- Used by external editor (RogueEssence.Dev) for UI

## Usage

```csharp
// Example attribute usage on a data property
[DataType(0, DataManager.DataType.Monster, false)]
public string MonsterID { get; set; }

[NumberRange(0, 100)]
public int Percentage { get; set; }

[StringKey("SKILL_NAME")]
public LocalText Name { get; set; }
```
