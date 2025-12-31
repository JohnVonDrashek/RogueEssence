# Tiles

Tile generation utilities for terrain manipulation and special tile placement during level generation.

## Key Files

| File | Description |
|------|-------------|
| `AddTunnelStep.cs` | Adds additional tunnel connections between rooms |
| `DetectItemStep.cs` | Detects and marks item locations |
| `DetectTileStep.cs` | Detects specific tile types for processing |
| `FillImpassableStep.cs` | Fills impassable gaps in terrain |
| `MinimizeBarrierStep.cs` | Minimizes barrier walls for better flow |
| `StairsStencil.cs` | Stencil pattern for stair placement |
| `TileEffectStencil.cs` | Stencil for effect tile placement |

## Relationships

- Used by **Floors/GenSteps/** for terrain post-processing
- Manipulates **MapGenContext** tile data
- Creates connections using **Dungeon/Tiles/** types

## Usage

```csharp
// Add extra tunnels for connectivity
var tunnelStep = new AddTunnelStep<MapGenContext>();
tunnelStep.TunnelCount = 3;

// Fill gaps with impassable terrain
floorGen.GenSteps.Add(new FillImpassableStep<MapGenContext>());
```
