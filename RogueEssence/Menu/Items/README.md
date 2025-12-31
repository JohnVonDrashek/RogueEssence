# Items

Item management menus for inventory, shops, banking, and item transactions. Comprehensive item handling UI for all item-related actions.

## Key Files

| File | Description |
|------|-------------|
| `ItemMenu.cs` | Main inventory item list |
| `ItemChosenMenu.cs` | Actions for a selected item |
| `ItemTargetMenu.cs` | Target selection for item use |
| `ItemSummary.cs` | Item detail summary panel |
| `ItemAmountMenu.cs` | Amount selection for stackable items |
| `ShopMenu.cs` | Shop browsing interface |
| `BuyChosenMenu.cs` | Purchase confirmation |
| `SellMenu.cs` | Item selling interface |
| `SellChosenMenu.cs` | Sell confirmation |
| `BankMenu.cs` | Item storage bank interface |
| `DepositMenu.cs` | Deposit items to storage |
| `DepositChosenMenu.cs` | Deposit confirmation |
| `WithdrawMenu.cs` | Withdraw from storage |
| `WithdrawChosenMenu.cs` | Withdraw confirmation |
| `SwapShopMenu.cs` | Item swap/trade shop |
| `SwapGiveMenu.cs` | Give item in swap |
| `TradeItemMenu.cs` | Multiplayer item trading |
| `TradeSummary.cs` | Trade preview panel |
| `AppraiseMenu.cs` | Unidentified item appraisal |
| `TeachMenu.cs` | TM/move teaching menu |
| `TeachWhomMenu.cs` | Target selection for teaching |
| `TeachInfoMenu.cs` | Move teaching info |
| `SpoilsMenu.cs` | Post-dungeon loot menu |
| `OfferItemsMenu.cs` | Item offering menu |
| `MoneySummary.cs` | Money display panel |

## Relationships

- Uses **Data/ItemData** for item information
- Modifies **Dungeon/Team** inventory
- Integrates with **Network/** for trading

## Usage

```csharp
// Open inventory
ItemMenu menu = new ItemMenu(team.Bag);
MenuManager.Instance.AddMenu(menu, false);
```
