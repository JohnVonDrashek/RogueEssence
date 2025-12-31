# Tiles

Tile system for dungeon floors including terrain, effects, and autotiling. Manages both gameplay properties (walkability, damage) and visual representation.

## Key Files

| File | Description |
|------|-------------|
| `Tile.cs` | Basic tile class combining terrain and effect |
| `TerrainTile.cs` | Terrain component with type, blocking, and visuals |
| `EffectTile.cs` | Effect component for traps, stairs, and interactive tiles |
| `TileLayer.cs` | Visual layer of animated tile frames |
| `TileFrame.cs` | Single tile frame reference (sheet + coordinates) |
| `AutoTile.cs` | Autotile wrapper managing tile connections |
| `AutoTileBase.cs` | Base class for autotile logic |
| `AutoTileAdjacent.cs` | Standard 47-tile autotile for 8-directional connections |
| `AutoTileAdjacentLite.cs` | Simplified autotile with fewer variants |
| `AutoTileBlob.cs` | Blob-style autotile for organic shapes |
| `AutoTileStacked.cs` | Multi-layer stacked autotile |
| `AutoTileRandom.cs` | Randomly selected tile variants |
| `TerrainState.cs` | Terrain state flags |
| `TileState.cs` | Effect tile state data |

## Relationships

- **Maps/** contains grids of Tiles
- **Data/TerrainData** defines terrain types
- **Data/TileData** defines effect tiles
- **LevelGen/** places tiles during generation
- **Content/** renders tile visuals

## Usage

```csharp
// Create a terrain tile
TerrainTile terrain = new TerrainTile("ground", autoTile);

// Create an effect tile (trap)
EffectTile trap = new EffectTile("spike_trap", true);

// Combine into a tile
Tile tile = new Tile(terrain, effect);
```
