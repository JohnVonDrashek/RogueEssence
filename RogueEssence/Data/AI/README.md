# AI

Artificial intelligence system for NPC and enemy behavior. Defines AI tactics and decision-making logic for dungeon encounters.

## Key Files

| File | Description |
|------|-------------|
| `AI.cs` | AITactic class defining behavior patterns and decision priorities |
| `BasePlan.cs` | Base class for AI action plans |
| `ScriptPlan.cs` | Lua script-driven AI plan for custom behaviors |

## Relationships

- Referenced by **Dungeon/Characters/** for enemy decision-making
- **MonsterData** can specify default AI tactics
- **Lua/** can define custom AI behaviors via ScriptPlan
- Loaded and cached by **DataManager**

## Usage

```csharp
// Get AI tactic for an enemy
AITactic tactic = DataManager.Instance.GetAITactic("wander");

// AI makes decisions during turn processing
GameAction action = tactic.GetAction(character, map);
```
