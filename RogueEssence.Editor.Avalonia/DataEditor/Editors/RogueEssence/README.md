# DataEditor/Editors/RogueEssence

## Description

Contains editors for RogueEssence game-specific data types. These editors provide specialized UI for game concepts like monsters, items, animations, sounds, and dungeon generation configurations. Many include game-aware features like asset previews, dropdown menus populated from game data, and integration with the game's content systems.

## Key Files

| File | Description |
|------|-------------|
| `MonsterIDEditor.cs` | Editor for monster species selection with sprite preview |
| `InvItemEditor.cs` | Editor for inventory items with item type and count |
| `MapItemEditor.cs` | Editor for map-placed items (ground items, money, traps) |
| `AnimDataEditor.cs` | Editor for animation references with playback preview |
| `SoundEditor.cs` | Editor for sound effect selection with audio playback |
| `MusicEditor.cs` | Editor for background music track selection |
| `ColorEditor.cs` | Editor for RGBA color values with color picker |
| `StatusEffectEditor.cs` | Editor for status effect references |
| `StringKeyEditor.cs` | Editor for localized string keys with translation preview |
| `EntryDataEditor.cs` | Editor for data entry references (skills, items, etc.) |
| `AliasDataEditor.cs` | Editor for aliased data references |
| `FlagTypeEditor.cs` | Editor for flag/boolean type configurations |
| `FrameTypeEditor.cs` | Editor for animation frame type selection |
| `MobSpawnEditor.cs` | Editor for monster spawn configurations |
| `MobSpawnExtraEditor.cs` | Extended mob spawn with additional parameters |
| `TeamMemberSpawnEditor.cs` | Editor for team member spawn settings |
| `CategorySpawnEditor.cs` | Editor for category-based spawn systems |
| `SpreadPlanBaseEditor.cs` | Editor for dungeon spread plan configurations |
| `SegLocEditor.cs` | Editor for segment location references |
| `MultiplierEditor.cs` | Editor for multiplier/ratio values |
| `PromoteBranchEditor.cs` | Editor for evolution/promotion branch selection |
| `ZoneDataEditor.cs` | Editor for zone/dungeon configuration |
| `ItemDataEditor.cs` | Specialized editor for item definition data |
| `*SpawnZoneStepEditor.cs` | Editors for zone-based spawn step configurations |

## Subdirectories

| Directory | Description |
|-----------|-------------|
| `Testable/` | Editors with live preview/testing capability in the game engine |
| `Tiles/` | Editors for tile and autotile selection with visual browsers |

## Relationships

- **DataManager**: Accesses game data indices for dropdown population
- **GraphicsManager**: Loads sprites and animations for previews
- **DevDataManager**: Uses cached icons and tile images
- **Content/**: Works with the content system for asset loading

## Game-Aware Features

These editors provide game-specific functionality:

```csharp
public class MonsterIDEditor : Editor<MonsterID>
{
    public override void LoadWindowControls(...)
    {
        // Species dropdown populated from DataManager
        ComboBox cbSpecies = new ComboBox();
        foreach (var entry in DataManager.Instance.DataIndices[DataType.Monster])
            cbSpecies.Items.Add(entry);

        // Form selection based on species
        ComboBox cbForm = new ComboBox();
        // Updates when species changes

        // Gender selection
        // Shiny toggle
        // Sprite preview image
    }
}
```

## Usage

These editors are automatically used for game types:

```csharp
public class TeamMember
{
    public MonsterID Species;    // -> MonsterIDEditor with sprite preview
    public InvItem HeldItem;     // -> InvItemEditor with item icon
    public string Nickname;      // -> StringEditor
}
```

### Preview Integration

Many editors include preview functionality:

```csharp
// Animation preview
animEditor.TestButton.Click += (s, e) => {
    var anim = GetAnimationData();
    DungeonScene.Instance.PlayAnimation(anim);
};

// Sound preview
soundEditor.PlayButton.Click += (s, e) => {
    SoundManager.PlaySound(selectedSound);
};
```
