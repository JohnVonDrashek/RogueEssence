# RectPacker

Rectangle packing algorithm for creating texture atlases. This module efficiently arranges multiple smaller images into larger atlas textures to minimize memory usage and draw calls.

## Key Files

| File | Description |
|------|-------------|
| `OptimalMapper.cs` | Main packing algorithm that finds optimal placement for rectangles |
| `Canvas.cs` | Represents the target canvas/texture being packed into |
| `Atlas.cs` | Resulting texture atlas with all packed images |
| `DynamicTwoDimensionalArray.cs` | Dynamic 2D array for tracking available space |
| `ImageInfo.cs` | Input image metadata (dimensions, source path) |
| `MappedImageInfo.cs` | Output mapping of image to atlas position |
| `CanvasStats.cs` | Statistics about packing efficiency |

## Relationships

- Used by **Dev/ImportHelper** during asset compilation
- Generates packed textures loaded by **GraphicsManager**
- Produces index files used by **Content/Indices/**

## Usage

```csharp
// Pack a list of images into an atlas
OptimalMapper mapper = new OptimalMapper();
List<ImageInfo> images = LoadImages(inputPath);
Canvas result = mapper.Mapping(images, maxWidth, maxHeight);
```
