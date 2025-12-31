# Indices

Index data structures for efficient lookups of character sprites and tile assets. These indices enable fast access to specific sprites within large asset files without loading everything into memory.

## Key Files

| File | Description |
|------|-------------|
| `CharaIndex.cs` | Hierarchical index for character sprites (species -> form -> skin -> gender) |
| `TileIndex.cs` | Index for tile sheets with position lookups by sheet name and coordinates |

## Relationships

- Built by **Dev/ImportHelper** during asset conversion
- Used by **GraphicsManager** to locate sprites in packed asset files
- Enables **LRUCache** to load individual sprites on demand

## Usage

```csharp
// Get character sprite position from index
long position = CharaIndex.GetPosition(species, form, skin, gender);

// Get tile position from index
long position = TileIndex.GetPosition(sheetName, texLoc);
```
