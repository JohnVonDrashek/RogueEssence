# Emitters

Particle emitter system for spawning and controlling visual effects. Emitters define how, when, and where particle animations are created, providing flexible effect composition.

## Key Files

| File | Description |
|------|-------------|
| `Emitter.cs` | Base emitter class and common emitter interfaces |
| `SingleEmitter.cs` | Emits a single animation at a point |
| `AreaEmitter.cs` | Emits particles across a rectangular area |
| `SqueezedAreaEmitter.cs` | Area emitter with squeezed/distorted shape |
| `CircleSquareEmitter.cs` | Emits in circular or square patterns |
| `FountainEmitter.cs` | Fountain-style upward particle spray |
| `StreamEmitter.cs` | Continuous stream of particles in a direction |
| `SpinEmitter.cs` | Particles emitted with spinning motion |
| `VortexEmitter.cs` | Swirling vortex particle pattern |
| `SprinkleEmitter.cs` | Random scatter of particles across an area |
| `ReleaseEmitter.cs` | Releases particles from a source outward |
| `GatherEmitter.cs` | Particles converge toward a central point |
| `BetweenEmitter.cs` | Emits particles along a line between two points |
| `MoveToEmitter.cs` | Particles that travel to a destination |
| `FlashEmitter.cs` | Emits screen flash effects |
| `OverlayEmitter.cs` | Full-screen overlay effect emitter |
| `RepeatEmitter.cs` | Repeatedly emits from child emitter |
| `ListEmitter.cs` | Emits from a list of sub-emitters |
| `ClampEmitter.cs` | Clamps emitter to specific bounds |
| `ExpandableEmitter.cs` | Emitter that can scale with area size |
| `ScreenRainEmitter.cs` | Full-screen rain particle effect |
| `WindEmitter.cs` | Wind-based directional particles |
| `AfterImageEmitter.cs` | Emits character after-image trails |
| `SwingSwitchEmitter.cs` | Alternating swing pattern emitter |

## Relationships

- Used by **BattleFX** to define skill visual effects
- Referenced from **Data/SkillData** for move animations
- Spawns animations defined in parent `Animation/` directory
- **DungeonScene** manages active emitters during battle

## Usage

```csharp
// Create an area emitter for explosion effect
AreaEmitter emitter = new AreaEmitter(particleAnim);
emitter.Range = 48;
emitter.ParticlesPerBurst = 10;
emitter.SetupEmit(centerPos, dir, origin);
```
