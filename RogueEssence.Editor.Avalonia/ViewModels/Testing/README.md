# ViewModels/Testing

## Description

Contains ViewModels for testing and debugging tools within the editor. These utilities help developers verify game functionality, test text rendering, and debug various game systems without requiring full gameplay sessions.

## Key Files

| File | Description |
|------|-------------|
| `TextTestViewModel.cs` | ViewModel for testing text rendering and formatting |

## Relationships

- **Views/Testing/TextTestForm**: Corresponding View for text testing
- **GraphicsManager**: Uses text rendering system for preview
- **StringsEditViewModel**: May use text testing for localization verification

## TextTestViewModel

Tests the game's text rendering system:

```csharp
public class TextTestViewModel : ViewModelBase
{
    private string inputText;
    public string InputText
    {
        get => inputText;
        set => this.RaiseAndSetIfChanged(ref inputText, value);
    }

    // Font selection
    public string SelectedFont { get; set; }

    // Text formatting options
    public bool Bold { get; set; }
    public bool Italic { get; set; }

    // Color options
    public Color TextColor { get; set; }

    // Preview rendering
    public void RenderPreview()
    {
        // Renders InputText using game's text system
        // Displays in preview area
    }
}
```

## Use Cases

### Text Rendering Verification
Test how strings will appear in-game:
- Special characters and symbols
- Multi-line text wrapping
- Color codes and formatting
- Font rendering at different sizes

### Localization Testing
Verify translated strings:
- Character support for different languages
- Text length and overflow
- Right-to-left language support

### Format Code Testing
Test text formatting codes:
- `{color:red}Text{/color}` - Color changes
- `{icon:item}` - Inline icons
- `{name:player}` - Variable substitution

## Usage

```csharp
// Open text test form
var textTestVM = new TextTestViewModel
{
    InputText = "Hello, {name:player}!",
    SelectedFont = "default"
};

var form = new TextTestForm { DataContext = textTestVM };
form.Show();

// User modifies input, preview updates automatically
```

### Text Test Window

```
+----------------------------------+
| Text Test                         |
+----------------------------------+
| Input:                           |
| [Hello, {name:player}!          ]|
|                                  |
| Font: [default v] Size: [16]     |
| [ ] Bold  [ ] Italic             |
| Color: [White v]                 |
+----------------------------------+
| Preview:                         |
| +------------------------------+ |
| | Hello, Adventurer!           | |
| +------------------------------+ |
+----------------------------------+
|              [Close]             |
+----------------------------------+
```
