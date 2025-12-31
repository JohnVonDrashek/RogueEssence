# ZoneSteps

Zone-wide generation steps that configure settings across multiple floors. These steps set up spawn tables, naming, and special features that vary by floor range.

## Key Files

| File | Description |
|------|-------------|
| `ZoneStep.cs` | Base class for zone steps |
| `GenPriority.cs` | Priority wrapper for ordering steps |
| `FloorNameIDZoneStep.cs` | Sets floor names by range |
| `ItemSpawnZoneStep.cs` | Configures item spawn tables per floor range |
| `MoneySpawnZoneStep.cs` | Configures money spawn amounts |
| `TeamSpawnZoneStep.cs` | Configures enemy spawn tables |
| `TileSpawnZoneStep.cs` | Configures trap/tile spawn tables |
| `ScriptZoneStep.cs` | Executes Lua scripts for zone setup |
| `SpreadZoneStep.cs` | Spreads special features across floors |
| `SpreadStepZoneStep.cs` | Spreads generation steps across floors |
| `SpreadRoomZoneStep.cs` | Spreads special rooms across floors |
| `SpreadCombinedZoneStep.cs` | Combined spread of multiple features |
| `SpreadPlanBase.cs` | Base class for spread planning |
| `RangeStepZoneStep.cs` | Applies steps to floor ranges |

## Relationships

- Added to **ZoneSegment** in zone configuration
- Applies settings to **MapGenContext** during generation
- Uses **Rand/** spawn dictionaries

## Usage

```csharp
// Configure items for floors 1-5
var itemStep = new ItemSpawnZoneStep();
itemStep.SpawnTable.Add("potion", new IntRange(1, 6), 100);
zoneData.ZoneSteps.Add(itemStep);

// Spread special rooms across the zone
var spreadStep = new SpreadRoomZoneStep(specialRoom, spreadPlan);
zoneData.ZoneSteps.Add(spreadStep);
```
