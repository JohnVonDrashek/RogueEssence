# Content

Graphics, audio, and asset management systems for RogueEssence. This module handles loading, caching, and rendering of all visual and audio assets including sprites, tiles, fonts, music, and sound effects.

## Key Files

| File | Description |
|------|-------------|
| `GraphicsManager.cs` | Central graphics manager - loads assets, manages caches, handles zoom and window settings |
| `SoundManager.cs` | Audio playback system for music and sound effects with fading support |
| `BaseSheet.cs` | Base sprite sheet class for texture management and rendering |
| `CharSheet.cs` | Character sprite sheet with directional animations and frame data |
| `PortraitSheet.cs` | Portrait sprite sheet for character dialogue portraits |
| `TileSheet.cs` | Tile-based sprite sheet for UI elements and map tiles |
| `DirSheet.cs` | Directional sprite sheet supporting 8-direction animations |
| `BeamSheet.cs` | Beam/projectile sprite sheet for attack effects |
| `FontSheet.cs` | Bitmap font rendering with kerning and color support |
| `LRUCache.cs` | Least Recently Used cache for efficient asset memory management |
| `SpriteSheet.cs` | Generic sprite sheet utilities |
| `LoopedSong.cs` | Music playback with loop point support |
| `BattleFX.cs` | Battle effect visual configurations |
| `EmoteFX.cs` | Emote/emotion visual effect definitions |
| `CharID.cs` | Character identification for sprite lookups |
| `AnimMath.cs` | Animation math utilities |

## Subdirectories

| Directory | Purpose |
|-----------|---------|
| `Animation/` | Animation system with emitters, particles, and visual effects |
| `Indices/` | Index structures for efficient character and tile lookups |
| `RectPacker/` | Rectangle packing algorithm for texture atlases |

## Relationships

- **GraphicsManager** is a singleton accessed throughout the engine for all asset loading
- **Dungeon/** and **Ground/** scenes use Content for rendering characters and maps
- **Menu/** uses fonts and UI sheets from this module
- **Data/** references asset identifiers that Content resolves to actual textures

## Usage

```csharp
// Get a character sprite
CharSheet sprite = GraphicsManager.GetChara(new CharID(species, form, skin, gender));

// Get a portrait
PortraitSheet portrait = GraphicsManager.GetPortrait(charID);

// Get a tile
BaseSheet tile = GraphicsManager.GetTile(tileFrame);

// Play a sound effect
GameManager.Instance.SE("Sound/attack");
```
