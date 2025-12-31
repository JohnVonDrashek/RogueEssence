# Rescue

Rescue mail system data structures for the SOS/AOK mail feature. Allows players to request help and send rescue completions.

## Key Files

| File | Description |
|------|-------------|
| `BaseRescueMail.cs` | Base class for rescue mail with common fields |
| `SOSMail.cs` | SOS (Save Our Souls) mail - sent when player needs rescue |
| `AOKMail.cs` | AOK (A-OK) mail - sent after successful rescue |

## Relationships

- Used by **Network/** for online rescue features
- **Menu/Rescue/** provides UI for managing rescue mail
- **DataManager** handles saving/loading rescue mail files
- **GameProgress** tracks rescue-related player data

## Usage

```csharp
// Create an SOS mail when player faints
SOSMail mail = new SOSMail(team, zone, seed);

// Load rescue mail from file
SOSMail loaded = DataManager.LoadRescueMail<SOSMail>(path);
```
