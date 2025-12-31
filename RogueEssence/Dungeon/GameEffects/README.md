# GameEffects

Battle event system and game mechanics. This module implements the event-driven architecture for all combat calculations, status effects, abilities, and game rules.

## Key Files

| File | Description |
|------|-------------|
| `BattleContext.cs` | Complete battle context with attacker, defender, skill, and results |
| `GameContext.cs` | General game context for non-battle events |
| `SingleCharContext.cs` | Context for single-character events |
| `BattleEvent.cs` | Base class for battle event handlers |
| `SingleCharEvent.cs` | Event handlers for single character effects |
| `RefreshEvent.cs` | Events triggered on stat refresh |
| `SkillChangeEvent.cs` | Events for skill modification |
| `StatusGivenEvent.cs` | Events when status is applied |
| `MapStatusGivenEvent.cs` | Events for map-wide status changes |
| `ItemGivenEvent.cs` | Events when items are given/received |
| `HPChangeEvent.cs` | Events for HP changes |
| `ElementEffectEvent.cs` | Type effectiveness events |
| `GameEvent.cs` | Base game event class |
| `GameEventPriority.cs` | Priority system for event ordering |
| `ContextState.cs` | Base class for context state data |
| `StateCollection.cs` | Collection of state objects |
| `StatusState.cs` | State data for status effects |
| `MapStatusState.cs` | State data for map statuses |
| `ItemState.cs` | State data for items |
| `SkillState.cs` | State data for skills |
| `TerrainState.cs` | State data for terrain |
| `UniversalState.cs` | Global state data |
| `Multiplier.cs` | Damage/stat multiplier calculations |
| `StatusCheckContext.cs` | Context for checking status conditions |
| `ItemCheckContext.cs` | Context for checking item conditions |

## Relationships

- Used by **DungeonScene** for all combat resolution
- **Data/SkillData** defines which events trigger
- **Data/StatusData** contains status effect events
- **Data/IntrinsicData** contains ability events
- Priority system ensures correct event ordering

## Usage

```csharp
// Create a battle context
BattleContext context = new BattleContext(user, target, skill);

// Process all battle events
yield return ProcessBattleEvents(context, BattleEvent.Category.Hit);

// Add a status effect
StatusEffect status = new StatusEffect(statusID);
yield return character.AddStatusEffect(status);
```
