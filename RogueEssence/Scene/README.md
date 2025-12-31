# Scene

Scene management, game state machine, and coroutine system. Controls transitions between different game states and provides async execution support.

## Key Files

| File | Description |
|------|-------------|
| `GameManager.cs` | Central game state manager - handles scene transitions, save/load, game flow |
| `BaseScene.cs` | Base class for all game scenes with common lifecycle methods |
| `TitleScene.cs` | Title screen scene |
| `SplashScene.cs` | Initial splash screen scene |
| `CoroutineManager.cs` | Coroutine execution manager for async game logic |
| `YieldInstruction.cs` | Base class for coroutine yield instructions |
| `FadeEffect.cs` | Screen fade transition effects |
| `MusicEffect.cs` | Music transition and crossfade effects |

## Relationships

- **GameManager** is the central controller accessed by all systems
- Manages **Dungeon/DungeonScene** and **Ground/GroundScene**
- Uses **Content/** for fade graphics and audio
- **Lua/** can trigger scene transitions

## Usage

```csharp
// Switch to ground scene
yield return GameManager.Instance.MoveToZone(zoneLoc, true);

// Start a coroutine
yield return CoroutineManager.Instance.StartCoroutine(myCoroutine);

// Fade out and in
yield return new FadeOut(20);
// ... change state ...
yield return new FadeIn(20);

// Play music with fade
yield return new MusicFadeIn("theme", 40);
```
