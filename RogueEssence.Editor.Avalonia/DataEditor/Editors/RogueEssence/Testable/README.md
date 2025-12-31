# DataEditor/Editors/RogueEssence/Testable

## Description

Contains editors for game types that support live testing within the game engine. These editors extend the base editing functionality with a "Test" button that executes the configured data in the running game, allowing developers to preview animations, emitters, combat actions, and visual effects in real-time without reloading the game.

## Key Files

| File | Description |
|------|-------------|
| `TestableEditor.cs` | Base class providing the Test button infrastructure and game thread coordination |
| `BaseEmitterEditor.cs` | Editor for particle emitters; tests by spawning emitter at player location |
| `CircleSquareEmitterEditor.cs` | Editor for circular/square area emitters with preview |
| `ShootingEmitterEditor.cs` | Editor for projectile-based particle emitters |
| `BattleDataEditor.cs` | Editor for battle/skill data with combat preview |
| `BattleFXEditor.cs` | Editor for battle visual effects with playback test |
| `CombatActionEditor.cs` | Editor for combat action sequences with animation test |
| `ExplosionDataEditor.cs` | Editor for explosion effects with area preview |
| `SkillDataEditor.cs` | Editor for complete skill definitions with full test |
| `StaticAnimEditor.cs` | Editor for static animation frames with display test |
| `ColumnAnimEditor.cs` | Editor for column-based animations with playback |

## Relationships

- **DungeonScene**: Tests execute in the active dungeon scene context
- **FocusedCharacter**: Many tests use the player's current position/direction
- **DrawLayer**: Animations are created at appropriate render layers
- **DevForm.ExecuteOrPend**: Ensures tests run on the game thread

## TestableEditor Base

```csharp
public abstract class TestableEditor<T> : Editor<T>
{
    protected abstract void RunTest(T data);

    public override void LoadWindowControls(...)
    {
        // Standard editing controls
        base.LoadWindowControls(...);

        // Add Test button
        Button btnTest = new Button { Content = "Test" };
        btnTest.Click += (s, e) => {
            T data = ExtractCurrentData();
            DevForm.ExecuteOrPend(() => RunTest(data));
        };
        control.Children.Add(btnTest);
    }
}
```

## Example: Emitter Testing

```csharp
public class BaseEmitterEditor : TestableEditor<EndingEmitter>
{
    protected override void RunTest(EndingEmitter data)
    {
        Character player = DungeonScene.Instance.FocusedCharacter;

        // Clone to avoid modifying the editor's data
        EndingEmitter emitter = (EndingEmitter)data.Clone();

        // Set up at player's position
        emitter.SetupEmit(player.MapLoc, player.MapLoc, player.CharDir);

        // Add to scene for rendering
        DungeonScene.Instance.CreateAnim(emitter, DrawLayer.NoDraw);
    }
}
```

## Usage

Testable editors appear the same as regular editors but include a Test button:

```
+----------------------------------+
| Emitter Configuration            |
|   Particle Texture: [sparkle]    |
|   Count: [25]                    |
|   Speed: [3.5]                   |
|                                  |
|   [Test] [OK] [Cancel]           |
+----------------------------------+
```

Clicking Test:
1. Extracts current values from editor controls
2. Marshals execution to the game thread
3. Instantiates and runs the effect at the player's location
4. Developer sees immediate visual feedback in the game window

### Requirements for Testing

- Game must be running with an active dungeon scene
- Player character must be present on the map
- Test effects are temporary and do not save to data
