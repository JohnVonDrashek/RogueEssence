# Backgrounds

Background rendering system for dungeon and ground scenes. Provides scrolling, layered, and tiled background effects that create visual depth and atmosphere.

## Key Files

| File | Description |
|------|-------------|
| `MapBG.cs` | Main map background class managing parallax layers and scrolling |
| `LayeredBG.cs` | Multi-layer background with configurable parallax depths |
| `TileBG.cs` | Repeating tile-based background pattern |

## Relationships

- Used by **Dungeon/Maps/** for dungeon floor backgrounds
- Used by **Ground/Maps/** for overworld area backgrounds
- References textures loaded through **GraphicsManager**

## Usage

```csharp
// Create a layered background with parallax scrolling
MapBG background = new MapBG();
background.AddLayer(bgSheet, parallaxSpeed);
```
