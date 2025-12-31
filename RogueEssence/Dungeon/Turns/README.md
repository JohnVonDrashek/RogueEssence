# Turns

Turn order and character turn management system. Handles the queue of characters waiting to act and determines when each character gets their turn.

## Key Files

| File | Description |
|------|-------------|
| `TurnState.cs` | Turn state machine managing the current turn phase |
| `TurnOrder.cs` | Turn order queue and priority calculation |
| `CharIndex.cs` | Index for efficiently finding characters by position |
| `ITurnChar.cs` | Interface for characters participating in turns |

## Relationships

- Used by **DungeonScene** for turn processing
- **Characters/** implement `ITurnChar` for turn participation
- Speed stats affect turn order calculations

## Usage

```csharp
// Get next character to act
Character next = turnState.GetNextTurn();

// End current character's turn
turnState.EndTurn(character);

// Calculate when character acts next
int turnCount = TurnOrder.CalculateTurnMod(character.Speed);
```
