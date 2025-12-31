# Views/Testing

## Description

Contains Views for testing and debugging utilities within the editor. These tools help developers verify game functionality without requiring full gameplay sessions, particularly for testing visual elements like text rendering.

## Key Files

| File | Description |
|------|-------------|
| `TextTestForm.axaml` / `.cs` | Text rendering test dialog |

## Relationships

- **ViewModels/Testing/TextTestViewModel**: Text testing ViewModel
- **GraphicsManager**: Uses game's text rendering system
- **DevForm**: May open testing tools from development menu

## TextTestForm Layout

```
+--------------------------------------------------+
| Text Test                                 [X]    |
+--------------------------------------------------+
| Input Text:                                      |
| +----------------------------------------------+ |
| | Hello, {color:yellow}{name:player}{/color}!  | |
| |                                              | |
| | Welcome to the dungeon.                      | |
| +----------------------------------------------+ |
+--------------------------------------------------+
| Font Settings:                                   |
| Font: [default v]    Size: [16 v]                |
| [ ] Bold    [ ] Italic                           |
| Color: [White v]                                 |
+--------------------------------------------------+
| Preview:                                         |
| +----------------------------------------------+ |
| |  Hello, Adventurer!                          | |
| |                                              | |
| |  Welcome to the dungeon.                     | |
| +----------------------------------------------+ |
+--------------------------------------------------+
|                     [Close]                      |
+--------------------------------------------------+
```

## AXAML Structure

```xml
<!-- TextTestForm.axaml -->
<Window Title="Text Test" Width="500" Height="400">
    <DockPanel Margin="10">
        <!-- Input area -->
        <StackPanel DockPanel.Dock="Top">
            <TextBlock Text="Input Text:"/>
            <TextBox Text="{Binding InputText}"
                     AcceptsReturn="True"
                     Height="100"/>
        </StackPanel>

        <!-- Font settings -->
        <Grid DockPanel.Dock="Top" ColumnDefinitions="*,*">
            <StackPanel Grid.Column="0">
                <ComboBox Items="{Binding Fonts}"
                          SelectedItem="{Binding SelectedFont}"/>
            </StackPanel>
            <StackPanel Grid.Column="1">
                <NumericUpDown Value="{Binding FontSize}"
                               Minimum="8" Maximum="72"/>
            </StackPanel>
        </Grid>

        <StackPanel DockPanel.Dock="Top" Orientation="Horizontal">
            <CheckBox IsChecked="{Binding Bold}" Content="Bold"/>
            <CheckBox IsChecked="{Binding Italic}" Content="Italic"/>
            <ComboBox Items="{Binding Colors}"
                      SelectedItem="{Binding TextColor}"/>
        </StackPanel>

        <!-- Preview area -->
        <Border DockPanel.Dock="Top" BorderBrush="Gray" BorderThickness="1">
            <Canvas x:Name="PreviewCanvas" Height="150"/>
        </Border>

        <!-- Close button -->
        <Button DockPanel.Dock="Bottom" Content="Close"
                HorizontalAlignment="Center"
                Click="CloseButton_Click"/>
    </DockPanel>
</Window>
```

## Code-Behind

```csharp
public class TextTestForm : Window
{
    public TextTestForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
```

## Features

### Format Code Testing
Test text formatting codes used in the game:
- `{color:red}Text{/color}` - Colored text
- `{name:player}` - Variable substitution
- `{icon:item_potion}` - Inline icons
- `{pause}` - Text pause markers

### Font Rendering
Verify how text appears with different:
- Font families available in game
- Font sizes
- Bold/italic styles
- Text colors

### Localization Verification
Test strings before adding to localization files:
- Special character support
- Text wrapping behavior
- Variable substitution

## Usage

```csharp
// Open text test from development menu
private void OpenTextTest_Click(object sender, EventArgs e)
{
    var vm = new TextTestViewModel();
    var form = new TextTestForm { DataContext = vm };
    form.Show();
}

// Or with preset text
private void TestString_Click(string textToTest)
{
    var vm = new TextTestViewModel { InputText = textToTest };
    var form = new TextTestForm { DataContext = vm };
    form.Show();
}
```
