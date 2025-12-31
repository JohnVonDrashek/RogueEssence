# Network

Multiplayer networking menus for connecting to servers, managing contacts, and peer-to-peer interaction.

## Key Files

| File | Description |
|------|-------------|
| `ServersMenu.cs` | Server list menu |
| `ServerChosenMenu.cs` | Server connection actions |
| `ContactsMenu.cs` | Contact/friend list |
| `ContactChosenMenu.cs` | Contact interaction options |
| `ContactMiniSummary.cs` | Contact info summary |
| `ContactInputMenu.cs` | Add contact by code |
| `PeersMenu.cs` | Connected peers list |
| `HostInputMenu.cs` | Host server configuration |
| `ConnectingMenu.cs` | Connection in progress display |
| `SelfChosenMenu.cs` | Own connection info |
| `MailChosenMenu.cs` | Mail action menu |

## Relationships

- Uses **Network/NetworkManager** for connections
- Stores contacts in **DiagManager** settings
- Enables multiplayer features like trading

## Usage

```csharp
// Open servers menu
ServersMenu menu = new ServersMenu();
MenuManager.Instance.AddMenu(menu, false);
```
