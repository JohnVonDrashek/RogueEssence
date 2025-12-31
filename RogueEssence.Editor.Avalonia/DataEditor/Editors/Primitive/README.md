# DataEditor/Editors/Primitive

## Description

Contains editors for C# primitive and value types. These are the most basic building blocks of the DataEditor system, providing simple single-control editors for fundamental types like integers, strings, booleans, and floating-point numbers.

## Key Files

| File | Description |
|------|-------------|
| `IntEditor.cs` | Editor for `Int32` using NumericUpDown; supports `[NumberRange]` and `[IntRange]` attributes |
| `ByteEditor.cs` | Editor for `Byte` values with 0-255 range constraint |
| `StringEditor.cs` | Editor for strings using TextBox; handles null and multiline options |
| `BooleanEditor.cs` | Editor for booleans using CheckBox control |
| `CharEditor.cs` | Editor for single characters using TextBox with length limit |
| `SingleEditor.cs` | Editor for `float` values using NumericUpDown with decimal support |
| `DoubleEditor.cs` | Editor for `double` values using NumericUpDown with decimal support |

## Relationships

- **Editor<T>**: All primitive editors extend `Editor<T>` where T is the primitive type
- **DataEditor**: These are among the first editors registered; handle leaf nodes of object graphs
- **NumberRangeAttribute**: `IntEditor` respects min/max constraints from this attribute
- **IntRangeAttribute**: `IntEditor` supports 1-indexed display for user-friendly editing

## Editor Pattern

Each primitive editor follows this pattern:

```csharp
public class IntEditor : Editor<Int32>
{
    public override bool DefaultSubgroup => true;      // Display inline
    public override bool DefaultDecoration => false;   // No border box

    public override void LoadWindowControls(StackPanel control, ..., Int32 member, ...)
    {
        NumericUpDown nudValue = new NumericUpDown();
        nudValue.Minimum = /* from attributes or Int32.MinValue */;
        nudValue.Maximum = /* from attributes or Int32.MaxValue */;
        nudValue.Value = member;
        control.Children.Add(nudValue);
    }

    public override Int32 SaveWindowControls(StackPanel control, ...)
    {
        NumericUpDown nudValue = (NumericUpDown)control.Children[0];
        return (Int32)nudValue.Value;
    }
}
```

## Usage

Primitive editors are automatically selected when the DataEditor encounters primitive types:

```csharp
// When editing a class with primitive fields:
public class MonsterData
{
    public string Name;              // -> StringEditor
    public int BaseHP;               // -> IntEditor
    public float CritRate;           // -> SingleEditor
    public bool CanFly;              // -> BooleanEditor
}
```

### Attribute Support

```csharp
// Constrain integer range
[NumberRange(1, 100)]
public int Level;

// 1-indexed display (shows 1-100 but stores 0-99)
[IntRange(true)]
public int FloorNumber;
```
