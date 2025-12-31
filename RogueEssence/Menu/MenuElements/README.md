# MenuElements

Reusable menu UI components for building menu interfaces. Provides text, graphics, dividers, and interactive elements.

## Key Files

| File | Description |
|------|-------------|
| `IMenuElement.cs` | Interface for menu elements |
| `BaseMenuElement.cs` | Base class for elements with position |
| `MenuText.cs` | Text label element with formatting |
| `DialogueText.cs` | Animated text with typewriter effect |
| `MenuChoice.cs` | Selectable menu option with callback |
| `IChoosable.cs` | Interface for selectable elements |
| `MenuCursor.cs` | Animated selection cursor |
| `MenuDivider.cs` | Horizontal divider line |
| `MenuGraphic.cs` | Static image/icon element |
| `MenuPortrait.cs` | Character portrait display |
| `MenuDigits.cs` | Numeric digit display |
| `MenuDirTex.cs` | Directional texture element |
| `MenuStatBar.cs` | Stat/HP bar display |
| `MenuSetting.cs` | Settings toggle/slider element |

## Relationships

- Used by all menu classes for UI composition
- Renders using **Content/** fonts and textures
- **MenuChoice** enables selection in **InteractableMenu**

## Usage

```csharp
// Create text element
MenuText title = new MenuText("Inventory", new Loc(8, 8));

// Create selectable choice
MenuChoice choice = new MenuChoice("Use Item", () => UseSelectedItem());

// Create divider
MenuDivider div = new MenuDivider(new Loc(0, 24), menuWidth);
```
