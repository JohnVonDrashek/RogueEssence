# Rescue

Rescue mail system menus for sending and receiving rescue requests.

## Key Files

| File | Description |
|------|-------------|
| `RescueMenu.cs` | Main rescue menu |
| `MailMenu.cs` | Mail list display |
| `MailMiniSummary.cs` | Mail preview summary |
| `RescueCardMenu.cs` | Rescue code display |
| `GetHelpMenu.cs` | Request rescue help |
| `SendHelpMenu.cs` | Send rescue completion |

## Relationships

- Uses **Data/Rescue/** mail structures
- Integrates with **Network/** for online rescue
- Accesses **Data/GameProgress** for status

## Usage

```csharp
// Open rescue menu
RescueMenu menu = new RescueMenu();
MenuManager.Instance.AddMenu(menu, false);
```
