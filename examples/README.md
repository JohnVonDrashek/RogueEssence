# RogueEssence Examples

This folder contains annotated example files demonstrating how to create various game data
and custom behaviors in RogueEssence. Each file is heavily commented to explain what each
part does.

## Contents

### Data Definition Examples

These examples show how to define new game content:

| File | Description |
|------|-------------|
| `add-monster.cs` | Complete MonsterData with forms, stats, and evolutions |
| `add-skill.cs` | SkillData with targeting, power, and battle events |
| `add-item.cs` | ItemData with use effects and states |
| `add-status.cs` | StatusData with states and passive events |

### Custom Behavior Examples

These examples show how to extend the engine with custom logic:

| File | Description |
|------|-------------|
| `floor-gen-step.cs` | Custom floor generation step for dungeon floors |
| `battle-event.cs` | Custom BattleEvent for skill/item effects |
| `menu-example.cs` | Custom menu with choices and input handling |

### Lua Scripting Examples

These examples demonstrate the Lua scripting system:

| File | Description |
|------|-------------|
| `lua-event.lua` | Zone and map script callbacks |
| `lua-cutscene.lua` | Cutscene with dialogue, movement, and choices |

## Key Concepts

### Priority Lists

Many event systems use `PriorityList<T>` which allows events to be ordered by priority.
Lower priority numbers execute first. Use `Priority.Zero` for default timing.

```csharp
// Adding an event with default priority
myData.OnHits.Add(Priority.Zero, new MyBattleEvent());

// Adding an event that runs before normal events
myData.OnHits.Add(new Priority(-1), new EarlyEvent());
```

### State Collections

Items, skills, and statuses can have state objects that store data or act as markers:

```csharp
// Adding a power state to a skill
skill.Data.SkillStates.Set(new BasePowerState(100));

// Checking for a state
if (item.ItemStates.Contains<FoodState>())
    // Handle food item
```

### LocalText

Localized text uses the `LocalText` class for multi-language support:

```csharp
// Simple text (will use DefaultText for all languages)
Name = new LocalText("Fire Blast");

// Adding translations
Name = new LocalText("Fire Blast");
Name.Strings["ja"] = "Fire Blast JP";
Name.Strings["fr"] = "Explosion de feu";
```

### Coroutines and Yield Instructions

Battle events and scripts use C# iterator methods that yield control:

```csharp
public override IEnumerator<YieldInstruction> Apply(...)
{
    // Do something
    yield return new WaitForFrames(30);  // Wait 30 frames

    // Start another coroutine and wait for it
    yield return CoroutineManager.Instance.StartCoroutine(OtherMethod());
}
```

## Lua Global Objects

The following objects are available in Lua scripts:

| Global | Description |
|--------|-------------|
| `GAME` | Game management functions |
| `GROUND` | Ground map operations |
| `DUNGEON` | Dungeon operations |
| `UI` | User interface (dialogues, menus) |
| `SOUND` | Sound effects and music |
| `STRINGS` | Localized string access |
| `TASK` | Task scheduling |
| `AI` | AI control functions |
| `SV` | Script variables (saved with the game) |
| `_ZONE` | Current ZoneManager instance |
| `_DATA` | DataManager instance |
| `_GAME` | GameManager instance |

## Common Patterns

### Adding Events to Data

```csharp
// For skills - add to BattleData
skill.Data.OnHits.Add(new DealDamageEvent());

// For items/statuses - add to passive lists
item.BeforeHittings.Add(new ModifyDamageEvent());
status.OnTurnEnds.Add(new HealOverTimeEvent());
```

### Creating Hitboxes

```csharp
// Melee attack
skill.HitboxAction = new AttackAction();
((AttackAction)skill.HitboxAction).CharAnimData = new CharAnimFrameType(GraphicsManager.ChargeAction);

// Projectile
skill.HitboxAction = new ProjectileAction();
((ProjectileAction)skill.HitboxAction).Range = 8;

// Area of effect
skill.HitboxAction = new AreaAction();
((AreaAction)skill.HitboxAction).Range = 3;
```

## Further Reading

- Check the `RogueEssence/Data/` folder for data class definitions
- Check `RogueEssence/Dungeon/GameEffects/` for battle event implementations
- Check `RogueEssence/Menu/` for menu implementations
- Check `RogueEssence/Lua/` for script API implementations
