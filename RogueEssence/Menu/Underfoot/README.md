# Underfoot

Menus for interacting with items and tiles at the player's feet. Quick access to ground items and tile effects.

## Key Files

| File | Description |
|------|-------------|
| `UnderfootMenu.cs` | Main underfoot interaction menu |
| `ItemUnderfootMenu.cs` | Item on ground actions |
| `TileUnderfootMenu.cs` | Tile effect actions |
| `TileSummary.cs` | Tile information panel |

## Relationships

- Triggered by **Dungeon/** when standing on items/tiles
- Uses **Dungeon/Maps/MapItem** for items
- Uses **Dungeon/Tiles/EffectTile** for traps

## Usage

```csharp
// Open underfoot menu
UnderfootMenu menu = new UnderfootMenu(itemsAtFeet, tileAtFeet);
MenuManager.Instance.AddMenu(menu, false);
```
