# ViewModels

## Description

Contains MVVM ViewModels for the Avalonia editor using ReactiveUI. ViewModels encapsulate the presentation logic and state for each View, implementing property change notifications and commands. They follow the naming convention `*ViewModel.cs` and are automatically matched to Views by the `ViewLocator`.

## Key Files

| File | Description |
|------|-------------|
| `ViewModelBase.cs` | Base class extending `ReactiveObject` for all ViewModels |
| `DataListFormViewModel.cs` | ViewModel for the data list browser showing game data entries |
| `SearchListBoxViewModel.cs` | ViewModel for searchable/filterable list controls |
| `ReversedObservableCollection.cs` | Observable collection that maintains reverse order (newest first) |
| `WrappedObservableCollection.cs` | Wrapper for observable collections with additional features |

## Subdirectories

| Directory | Description |
|-----------|-------------|
| `Content/` | ViewModels for asset editing (sprites, tilesets, animations, strings) |
| `DevForm/` | ViewModels for the main development form and its tabs |
| `DialogBoxes/` | ViewModels for modal dialog windows |
| `GroundEditForm/` | ViewModels for the ground map editor |
| `MapEditForm/` | ViewModels for the dungeon map editor |
| `Testing/` | ViewModels for testing/debugging tools |
| `UserControls/` | ViewModels for reusable UI components |

## Relationships

- **Views/**: Each ViewModel has a corresponding View with the same name (minus "Model")
- **ViewLocator**: Automatically resolves `FooViewModel` to `FooView`
- **ReactiveUI**: All ViewModels extend `ReactiveObject` for reactive property binding
- **RogueEssence Core**: ViewModels interact with game data managers and scene objects

## ReactiveUI Pattern

```csharp
public class ExampleViewModel : ViewModelBase
{
    private string name;
    public string Name
    {
        get => name;
        set => this.RaiseAndSetIfChanged(ref name, value);
    }

    // Using ReactiveExt helper
    public string Title
    {
        get => title;
        set => this.SetIfChanged(ref title, value);  // Returns bool if changed
    }

    // Computed property
    public string DisplayName => $"{Name} - {Title}";
}
```

## View-ViewModel Binding

The `ViewLocator` automatically matches ViewModels to Views:

```csharp
// ViewLocator.cs
public IControl Build(object data)
{
    // "RogueEssence.Dev.ViewModels.DevFormViewModel"
    // becomes "RogueEssence.Dev.Views.DevFormView"
    var name = data.GetType().FullName.Replace("ViewModel", "View");
    var type = Type.GetType(name);
    return (Control)Activator.CreateInstance(type);
}
```

## Usage

ViewModels are set as DataContext in Views:

```csharp
// App.axaml.cs
desktop.MainWindow = new DevForm
{
    DataContext = new DevFormViewModel(),
};

// Or in XAML
<Window DataContext="{Binding DevFormViewModel}">
    <TextBlock Text="{Binding Name}"/>
</Window>
```

### Observable Collections

For list-based UI, use observable collections:

```csharp
public ObservableCollection<ItemViewModel> Items { get; }
    = new ObservableCollection<ItemViewModel>();

// Adding items automatically updates bound ListBox
Items.Add(new ItemViewModel { Name = "Sword" });
```

### Commands

Use ReactiveCommand for button actions:

```csharp
public ReactiveCommand<Unit, Unit> SaveCommand { get; }

public ExampleViewModel()
{
    SaveCommand = ReactiveCommand.Create(Save);
}

private void Save()
{
    // Save logic here
}
```
