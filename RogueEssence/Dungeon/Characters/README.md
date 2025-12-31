# Characters

Character entity system including the main Character class, actions, animations, stats, and skills. Handles all aspects of playable characters and enemies in dungeons.

## Key Files

| File | Description |
|------|-------------|
| `Character.cs` | Main character class with stats, skills, status, position, and AI |
| `CharAction.cs` | Character action definitions (attack, walk, use item, etc.) |
| `CharAnimation.cs` | Character animation state machine and rendering |
| `CharData.cs` | Serializable character data (base stats, species, moves) |
| `CharState.cs` | Transient character state flags |
| `Hitbox.cs` | Attack hitbox system for area-of-effect skills |
| `ExplosionData.cs` | Explosion/area effect data structure |
| `MonsterID.cs` | Monster identifier (species, form, skin, gender) |
| `Skill.cs` | Individual skill instance with PP and state |
| `SlotSkill.cs` | Skill in a moveslot with enable/disable state |
| `InvSlot.cs` | Inventory slot reference |
| `StatusEffect.cs` | Active status effect instance on a character |
| `BackReference.cs` | Weak reference to character for event callbacks |

## Relationships

- Uses **Data/MonsterData** for base stats and abilities
- Uses **Data/SkillData** for move mechanics
- **DungeonScene** manages character turn order
- **GameEffects/** processes battle mechanics
- **Content/** renders character sprites via CharSheet

## Usage

```csharp
// Create a character from monster ID
Character enemy = new Character(monsterID, level);

// Execute an attack action
CharAction action = new CharAnimAttack(skill, dir);
yield return character.StartAction(action);

// Apply damage
character.HP -= damage;
```
