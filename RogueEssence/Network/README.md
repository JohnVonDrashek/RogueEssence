# Network

Multiplayer networking system for peer-to-peer connections, trading, and rescue features. Handles network communication and activity management.

## Key Files

| File | Description |
|------|-------------|
| `NetworkManager.cs` | Main network manager handling connections and message routing |
| `OnlineActivity.cs` | Base class for online activities (trade, rescue) |
| `ActivityTradeItem.cs` | Item trading activity |
| `ActivityTradeTeam.cs` | Team member trading activity |
| `ActivityTradeMail.cs` | Mail exchange activity |
| `ActivityGetHelp.cs` | Request rescue help activity |
| `ActivitySendHelp.cs` | Send rescue completion activity |
| `WrapperPacket.cs` | Network packet wrapper |

## Relationships

- **Menu/Network/** provides UI for connections
- **Data/Rescue/** contains rescue mail structures
- **DiagManager** stores server/peer contacts
- Enables multiplayer features across scenes

## Usage

```csharp
// Connect to server
NetworkManager.Instance.Connect(serverIP, port);

// Start item trade
ActivityTradeItem trade = new ActivityTradeItem(items);
NetworkManager.Instance.StartActivity(trade);

// Send rescue mail
ActivitySendHelp rescue = new ActivitySendHelp(aokMail);
NetworkManager.Instance.StartActivity(rescue);
```
