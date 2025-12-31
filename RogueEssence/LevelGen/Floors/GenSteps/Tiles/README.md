# Tiles

Tile placement and terrain manipulation steps for floor generation. Handles special terrain features and post-processing of tile data.

## Key Files

| File | Description |
|------|-------------|
| `LoadBlobStep.cs` | Loads blob-shaped terrain features from templates |
| `TerrainBorderStencil.cs` | Creates terrain borders with stencil patterns |
| `UnbreakableBorderStep.cs` | Adds unbreakable walls at map edges |

## Relationships

- Executed during tile phase of generation
- Uses **Dungeon/Tiles/** for tile types
- Applies **Data/TerrainData** definitions

## Usage

```csharp
// Add unbreakable border walls
floorGen.GenSteps.Add(new UnbreakableBorderStep<MapGenContext>(thickness));

// Load terrain blob template
floorGen.GenSteps.Add(new LoadBlobStep<MapGenContext>(blobPath));
```
