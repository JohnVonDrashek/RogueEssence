# Converters

## Description

Contains Avalonia `IValueConverter` implementations for transforming data between ViewModels and Views. These converters handle common UI patterns like comparing values for radio buttons, converting game data to display formats, and transforming images for rendering in the editor.

## Key Files

| File | Description |
|------|-------------|
| `ComparisonConverter.cs` | Compares a value against a parameter, used for radio button binding |
| `ComparisonXConverter.cs` | Extended comparison converter with additional comparison modes |
| `DataEntryConverter.cs` | Converts data entries to display strings with formatting |
| `ElementIconConverter.cs` | Converts element type IDs to their corresponding icon images |
| `SkillCategoryIconConverter.cs` | Converts skill categories (Physical/Magical/Status) to icons |
| `FileToTitleConverter.cs` | Extracts display titles from file paths |
| `FrameConverter.cs` | Converts animation frame data to displayable images |
| `FrameTypeConverter.cs` | Converts frame type enums to their visual representations |
| `TileConverter.cs` | Converts tile frame data to Avalonia bitmaps for display |
| `TilesetConverter.cs` | Converts tileset identifiers to full tileset images |
| `TileSizedConverter.cs` | Scales values based on tile dimensions |
| `TileToThicknessConverter.cs` | Converts tile coordinates to Avalonia Thickness for margins |
| `IntInSetConverter.cs` | Checks if an integer value exists in a set |
| `IsNoneOrEmptyConverter.cs` | Checks for null, empty, or "none" values |
| `ListNotEmptyConverter.cs` | Returns true if a collection has items |
| `StringNotEmptyConverter.cs` | Returns true if a string is not null or empty |
| `ValidIdxConverter.cs` | Validates that an index is within valid range |
| `MapScriptPathConverter.cs` | Converts map names to script file paths |
| `MultiSelectConverter.cs` | Handles multi-selection binding for list controls |
| `NullableToStringConverter.cs` | Safely converts nullable values to strings |
| `OXConverter.cs` | Converts booleans to O/X icons (checkmark/cross) |
| `PercentConverter.cs` | Formats decimal values as percentages |

## Relationships

- **App.axaml**: Converters are registered as application-level static resources
- **Views/**: XAML files reference converters via `{StaticResource ConverterName}`
- **DevDataManager**: Some converters (like `ElementIconConverter`) use `DevDataManager` to fetch cached icons

## Usage

Converters are declared in `App.axaml`:

```xml
<Application.Resources>
    <converters:ComparisonConverter x:Key="ComparisonConverter"/>
    <converters:TileConverter x:Key="TileConverter"/>
</Application.Resources>
```

Then used in View XAML:

```xml
<RadioButton IsChecked="{Binding Mode, Converter={StaticResource ComparisonConverter},
             ConverterParameter={x:Static local:TileEditMode.Draw}}"/>

<Image Source="{Binding TileFrame, Converter={StaticResource TileConverter}}"/>
```

### Adding New Converters

1. Create a class implementing `IValueConverter` (or `IMultiValueConverter`)
2. Implement `Convert` and optionally `ConvertBack` methods
3. Add to `RogueEssence.Dev.Converters` namespace
4. Register in `App.axaml` resources
