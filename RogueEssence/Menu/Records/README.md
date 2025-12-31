# Records

Score, replay, and record viewing menus. Displays adventure history and allows replay review.

## Key Files

| File | Description |
|------|-------------|
| `RecordsMenu.cs` | Main records menu |
| `ReplaysMenu.cs` | Saved replays list |
| `ReplayChosenMenu.cs` | Replay action options |
| `ReplayMiniSummary.cs` | Replay info preview |
| `ScoreMenu.cs` | High score display |
| `DexMenu.cs` | Monster dex/collection |
| `FinalResultsMenu.cs` | End-of-run results |
| `TeamResultsMenu.cs` | Team performance summary |
| `InvResultsMenu.cs` | Inventory results |
| `TrailResultsMenu.cs` | Adventure trail/path results |
| `VersionResultsMenu.cs` | Version info in results |
| `VersionDiffMenu.cs` | Version difference display |

## Relationships

- Reads from **Data/GameProgress** for records
- Uses **DiagManager** for replay files
- Displays results from **Dungeon/** runs

## Usage

```csharp
// Show final results after dungeon
FinalResultsMenu menu = new FinalResultsMenu(result, team);
MenuManager.Instance.AddMenu(menu, false);
```
