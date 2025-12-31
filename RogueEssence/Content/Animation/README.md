# Animation

Visual animation system providing sprite animations, particle effects, screen effects, and emitters. This module handles all dynamic visual content that plays during gameplay including attack effects, character animations, and environmental effects.

## Key Files

| File | Description |
|------|-------------|
| `AnimData.cs` | Animation data structure defining frame sequences and timing |
| `Sprites.cs` | Sprite animation base classes and interfaces |
| `ParticleAnim.cs` | Particle animation with physics-based movement |
| `LoopingAnim.cs` | Continuously looping sprite animation |
| `StaticAnim.cs` | Single-frame static animation display |
| `FlashAnim.cs` | Screen flash effect animation |
| `OverlayAnim.cs` | Full-screen overlay animation effects |
| `ColumnAnim.cs` | Column-based animation for vertical effects |
| `HelixAnim.cs` | Helix/spiral pattern animation |
| `SwingAnim.cs` | Swinging motion animation |
| `ItemAnim.cs` | Item throw and bounce animation |
| `MoveToAnim.cs` | Animation that moves between two points |
| `MultiAnim.cs` | Composite animation containing multiple sub-animations |
| `TextAnim.cs` | Floating text animation (damage numbers, etc.) |
| `WrappedRainAnim.cs` | Screen-wrapped rain particle effect |
| `ScreenMover.cs` | Screen shake and movement effects |
| `CharAfterImage.cs` | Character after-image trail effect |
| `Emote.cs` | Emote bubble animation above characters |
| `EmittingAnim.cs` | Animation that spawns particles via emitters |

## Subdirectories

| Directory | Purpose |
|-----------|---------|
| `Backgrounds/` | Background layer animations for scenes |
| `Emitters/` | Particle emitter systems for spawning effects |

## Relationships

- Used by **Dungeon/** for battle effects and character actions
- **BattleFX** in Content references these animations for skill effects
- **LuaEngine** can trigger animations via scripting
- Emitters in `Emitters/` spawn animations defined here

## Usage

```csharp
// Create a particle animation at a position
ParticleAnim anim = new ParticleAnim(animData, position, direction);

// Add to scene for rendering
DungeonScene.Instance.Anims[(int)DrawLayer.Normal].Add(anim);
```
