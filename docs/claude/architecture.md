# RogueEssence Architecture Guide

This document provides a comprehensive overview of the RogueEssence codebase architecture for AI assistants. RogueEssence is a Pokemon Mystery Dungeon-style roguelike game engine built on MonoGame/FNA.

## Table of Contents

1. [System Overview](#system-overview)
2. [Core Subsystems](#core-subsystems)
3. [Data Flow](#data-flow)
4. [Render Pipeline](#render-pipeline)
5. [Input Handling](#input-handling)
6. [Mod System](#mod-system)
7. [Key Abstractions](#key-abstractions)

---

## System Overview

### High-Level Architecture

```
+------------------------------------------------------------------+
|                           GameBase                                |
|  (MonoGame entry point - initializes all managers)               |
+------------------------------------------------------------------+
                                |
        +-----------------------+-----------------------+
        |                       |                       |
        v                       v                       v
+----------------+    +------------------+    +------------------+
|  GameManager   |    |   DataManager    |    | GraphicsManager  |
|  (Scene loop)  |    |  (Data loading)  |    |   (Rendering)    |
+----------------+    +------------------+    +------------------+
        |                       |                       |
        v                       v                       v
+----------------+    +------------------+    +------------------+
| CoroutineManager|   |   ZoneManager    |    |  SoundManager    |
| (Async actions) |   |  (Map/zone mgmt) |    |  (Audio)         |
+----------------+    +------------------+    +------------------+
        |
        +------------------------------------------+
        |                    |                     |
        v                    v                     v
+----------------+    +----------------+    +----------------+
|  DungeonScene  |    |  GroundScene   |    |   TitleScene   |
|  (Turn-based)  |    |  (Real-time)   |    |   (Menus)      |
+----------------+    +----------------+    +----------------+
        |                    |
        v                    v
+----------------+    +----------------+
|   LuaEngine    |    |  MenuManager   |
|  (Scripting)   |    |   (UI menus)   |
+----------------+    +----------------+
```

### Project Structure

```
RogueEssence/
|-- RogueEssence/                 # Main game library
|   |-- Content/                  # Asset management (GraphicsManager, SoundManager)
|   |-- Data/                     # Data loading and serialization (DataManager, Serializer)
|   |-- Dungeon/                  # Dungeon mode (DungeonScene, Characters, Combat)
|   |-- Ground/                   # Ground/overworld mode (GroundScene, NPCs)
|   |-- Menu/                     # UI system (MenuManager, various menu types)
|   |-- Scene/                    # Scene infrastructure (BaseScene, CoroutineManager)
|   |-- Lua/                      # Scripting engine (LuaEngine)
|   |-- LevelGen/                 # Procedural dungeon generation
|-- RogueEssence.Editor.Avalonia/ # Editor tools (Avalonia-based)
|-- WaypointServer/               # Multiplayer server component
|-- RogueElements/                # Core algorithms library (separate repo)
```

---

## Core Subsystems

### GameManager and Scene Lifecycle

**File:** `/RogueEssence/Scene/GameManager.cs`

GameManager is the central orchestrator for game state and scene transitions. It is a singleton accessed via `GameManager.Instance`.

#### Key Responsibilities

1. **Scene Management**: Holds `CurrentScene` reference, handles transitions via `MoveToScene()`
2. **Main Game Loop**: `ScreenMainCoroutine()` runs the core game loop
3. **Input Routing**: Maintains `InputManager` and `MetaInputManager` for game/debug input
4. **Audio Control**: Manages BGM, SFX, fanfares via `BGM()`, `SE()`, `Fanfare()` methods
5. **Screen Effects**: Fade transitions via `FadeIn()`, `FadeOut()`, screen shake

#### Scene Lifecycle

```
Begin() -> ProcessInput() [loop] -> Exit()
             |
             v
        SceneOutcome != null?
             |
             +---> Execute outcome coroutine
             |
             +---> Continue loop
```

**Scene Transitions:**
```csharp
// Change scenes
GameManager.Instance.MoveToScene(newScene);

// Scene outcomes trigger transitions
GameManager.Instance.SceneOutcome = MoveToZone(destId);
```

#### Important Methods

| Method | Purpose |
|--------|---------|
| `Begin()` | Initializes game, starts main coroutine |
| `MoveToScene(BaseScene)` | Switches active scene |
| `MoveToZone(ZoneLoc)` | Transitions to dungeon/ground map |
| `MoveToGround(zone, map, entry)` | Enters specific ground map |
| `EndSegment(ResultType)` | Handles dungeon completion/failure |
| `Update(FrameTick)` | Per-frame updates for music, scenes |

### DataManager and Data Loading

**File:** `/RogueEssence/Data/DataManager.cs`

DataManager handles all game data: monsters, items, skills, zones, and save files. Singleton accessed via `DataManager.Instance`.

#### Data Types

The `DataType` enum defines all loadable data categories:

```csharp
public enum DataType {
    None = 0,
    Monster = 1,      // MonsterData - Pokemon species
    Skill = 2,        // SkillData - Moves/attacks
    Item = 4,         // ItemData - Usable items
    Intrinsic = 8,    // IntrinsicData - Abilities
    Status = 16,      // StatusData - Status effects
    MapStatus = 32,   // MapStatusData - Weather/floor effects
    Terrain = 64,     // TerrainData - Tile behaviors
    Tile = 128,       // TileData - Tile appearances
    Zone = 256,       // ZoneData - Dungeon definitions
    Emote = 512,      // EmoteData - Character emotions
    AutoTile = 1024,  // AutoTileData - Tileset rules
    Element = 2048,   // ElementData - Type effectiveness
    GrowthGroup = 4096,
    SkillGroup = 8192,
    AI = 16384,       // AITactic - AI behaviors
    Rank = 32768,     // RankData - Team ranks
    Skin = 65536,     // SkinData - Shiny/form variants
    All = 131071
}
```

#### Caching Strategy

DataManager uses LRU caches for frequently accessed data:

```csharp
// LRU caches for large/frequently accessed data
private LRUCache<string, MonsterData> monsterCache;  // 50 entries
private LRUCache<string, SkillData> skillCache;      // 100 entries
private LRUCache<string, ItemData> itemCache;        // 100 entries

// Full dictionaries for small/always-needed data
private Dictionary<string, TileData> tileCache;
private Dictionary<string, TerrainData> terrainCache;
private Dictionary<string, ElementData> elementCache;
```

#### Loading Data

```csharp
// Get cached/load monster data
MonsterData monster = DataManager.Instance.GetMonster("bulbasaur");

// Get cached/load skill data
SkillData skill = DataManager.Instance.GetSkill("tackle");

// Get cached/load item data
ItemData item = DataManager.Instance.GetItem("oran_berry");

// Load zone data
ZoneData zone = DataManager.Instance.GetZone("test_dungeon");
```

#### Save/Load System

```csharp
// Current save file
GameProgress Save;

// Save game state
DataManager.Instance.SaveGameState(gameState);

// Load game state
GameState state = DataManager.Instance.LoadMainGameState(includeRecording);
```

### DungeonScene vs GroundScene

These are the two primary gameplay modes, both inheriting from `BaseScene`.

#### DungeonScene

**File:** `/RogueEssence/Dungeon/DungeonScene.cs`

Turn-based dungeon exploration with grid-based movement.

**Characteristics:**
- Lock-step turn system: all entities act in order
- Grid-based movement and collision
- Combat with skills, items, status effects
- Fog of war and exploration
- Minimap display

**Key Members:**
```csharp
public ExplorerTeam ActiveTeam;     // Player's team
public Character FocusedCharacter;   // Currently controlled character
public Character CurrentCharacter;   // Character whose turn it is

public List<Hitbox> Hitboxes;       // Active attack hitboxes
public List<PickupItem> PickupItems; // Items to pick up

public MinimapState ShowMap;        // Minimap display state
```

**Turn Processing:**
```csharp
public override IEnumerator<YieldInstruction> ProcessInput() {
    if (IsGameOver()) {
        yield return EndSegment(ResultType.Downed);
        yield break;
    }

    if (!IsPlayerTurn()) {
        yield return ProcessAI();  // AI characters act
    } else {
        yield return ProcessInput(InputManager);  // Player acts
    }
}
```

#### GroundScene

**File:** `/RogueEssence/Ground/GroundScene.cs`

Real-time overworld exploration with free movement.

**Characteristics:**
- Real-time movement (not turn-based)
- Free-form movement with collision
- NPC interactions via scripts
- Cutscene support
- Menu access

**Key Members:**
```csharp
public GroundChar FocusedCharacter;  // Player character
public Loc? FreeCamCenter;           // Free camera mode position
```

**Update Loop:**
```csharp
public override void Update(FrameTick elapsedTime) {
    // Update all entities
    foreach (GroundEntity ent in ZoneManager.Instance.CurrentGround.IterateEntities()) {
        if (ent.EntEnabled && ent is GroundAIUser)
            ((GroundAIUser)ent).Think();
    }

    // Update character movement and collision
    foreach (GroundChar character in ZoneManager.Instance.CurrentGround.IterateCharacters()) {
        character.Update(elapsedTime);
        character.Collide();
    }
}
```

### Coroutine System (YieldInstruction)

**File:** `/RogueEssence/Scene/YieldInstruction.cs`

RogueEssence uses a custom coroutine system for asynchronous game logic, similar to Unity's coroutines.

#### YieldInstruction Base Class

```csharp
public abstract class YieldInstruction {
    // Returns true when the instruction is complete
    public abstract bool FinishedYield();

    // Called each frame while waiting
    public virtual void Update() { }
}
```

#### Built-in Yield Types

| Type | Purpose | Example |
|------|---------|---------|
| `WaitForFrames` | Wait N frames | `yield return new WaitForFrames(30);` |
| `WaitUntil` | Wait until predicate true | `yield return new WaitUntil(() => animDone);` |
| `WaitWhile` | Wait while predicate true | `yield return new WaitWhile(() => isMoving);` |
| `Coroutine` | Wait for sub-coroutine | `yield return StartCoroutine(OtherMethod());` |

#### CoroutineManager

**File:** `/RogueEssence/Scene/CoroutineManager.cs`

Manages execution of multiple coroutine stacks.

```csharp
// Start a coroutine
CoroutineManager.Instance.StartCoroutine(MyCoroutine());

// Within a coroutine, start a sub-coroutine
yield return CoroutineManager.Instance.StartCoroutine(SubTask());
```

#### Writing Coroutines

```csharp
public IEnumerator<YieldInstruction> DoAnimation() {
    // Play sound effect
    GameManager.Instance.SE("attack");

    // Wait 30 frames
    yield return new WaitForFrames(30);

    // Run sub-coroutine
    yield return CoroutineManager.Instance.StartCoroutine(ShowEffect());

    // Wait until animation complete
    yield return new WaitUntil(() => animation.Finished);
}
```

### Event System (BattleEvent, GameEvent)

**Files:**
- `/RogueEssence/Dungeon/GameEffects/GameEvent.cs`
- `/RogueEssence/Dungeon/GameEffects/BattleEvent.cs`
- `/RogueEssence/Dungeon/GameEffects/GameEventPriority.cs`

The event system handles all game mechanics through composable event handlers.

#### GameEvent Base

```csharp
[Serializable]
public abstract class GameEvent {
    public abstract GameEvent Clone();
}
```

#### BattleEvent

Primary event type for combat interactions:

```csharp
[Serializable]
public abstract class BattleEvent : GameEvent {
    public abstract IEnumerator<YieldInstruction> Apply(
        GameEventOwner owner,      // Status, item, etc. that owns this event
        Character ownerChar,        // Character with the effect
        BattleContext context       // Current battle state
    );
}
```

#### Event Priority System

Events are sorted by multiple criteria for deterministic ordering:

```csharp
public class GameEventPriority : IComparable<GameEventPriority> {
    public Priority Priority;           // Developer-set priority
    public Priority PortPriority;       // User vs Target vs Other
    public EventCause TypeID;           // Skill/Status/Equip/Intrinsic/etc.
    public string ID;                   // Effect ID
    public int ListIndex;               // Position in list
}

public enum EventCause {
    None, Skill, MapState, Tile, Terrain, Status, Equip, Intrinsic
}
```

#### Script Events

Lua-powered events for modding:

```csharp
[Serializable]
public class BattleScriptEvent : BattleEvent {
    public string Script;      // Lua function name
    public string ArgTable;    // Lua table arguments

    public override IEnumerator<YieldInstruction> Apply(...) {
        LuaTable args = LuaEngine.Instance.RunString("return " + ArgTable);
        LuaFunction func = LuaEngine.Instance.CreateCoroutineIterator(
            LuaEngine.EVENT_BATTLE_NAME + "." + Script,
            new object[] { owner, ownerChar, context, args }
        );
        yield return StartCoroutine(ScriptEvent.ApplyFunc(name, func));
    }
}
```

---

## Data Flow

### JSON Files to Runtime Objects

```
+----------------+     +-------------+     +---------------+     +--------------+
|  JSON Files    | --> | Serializer  | --> | DataManager   | --> | Runtime      |
|  (.json)       |     | (Newtonsoft)|     | (Caching)     |     | Objects      |
+----------------+     +-------------+     +---------------+     +--------------+
        |                     |                   |
        v                     v                   v
  Data/Monster/         DeserializeData()   GetMonster()
  bulbasaur.json        with version        returns cached
                        conversion          MonsterData
```

### Serialization

**File:** `/RogueEssence/Data/Serializer.cs`

Uses Newtonsoft.Json with custom settings:

```csharp
public static JsonSerializerSettings Settings = new JsonSerializerSettings() {
    ContractResolver = resolver,
    SerializationBinder = binder,
    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
    TypeNameHandling = TypeNameHandling.Auto,
};
```

#### Versioned Serialization

Data files include version information for migration:

```csharp
[Serializable]
public class SerializationContainer {
    public Version Version;  // Data format version
    public object Object;    // Actual data
}
```

#### Diff-Based Patching

Mods can provide patches instead of full replacements:

```csharp
// Apply diff patches from mods
public static object DeserializeDataWithDiffs(string path, params string[] diffpaths) {
    JToken containerToken = JToken.Parse(baseJson);
    foreach (string diffPath in diffpaths) {
        JToken patch = JToken.Parse(diffJson);
        containerToken = jdp.Patch(containerToken, patch);
    }
    return Deserialize(containerToken);
}
```

### Data Path Resolution

```
Base Game:     ASSET_PATH/Data/Monster/bulbasaur.json
Quest Mod:     MODS/QuestMod/Data/Monster/bulbasaur.json (override)
Patch Mod:     MODS/PatchMod/Data/Monster/bulbasaur.jsonpatch (diff)
```

---

## Render Pipeline

### GraphicsManager

**File:** `/RogueEssence/Content/GraphicsManager.cs`

Central manager for all rendering resources.

#### Asset Types

```csharp
[Flags]
public enum AssetType {
    Font = 1,
    Chara = 2,        // Character sprites
    Portrait = 4,     // Character portraits
    Tile = 8,         // Map tiles
    Item = 16,        // Item icons
    Particle = 32,    // Particle effects
    Beam = 64,        // Beam attacks
    Icon = 128,       // UI icons
    Object = 256,     // Map objects
    BG = 512,         // Backgrounds
    Autotile = 1024,  // Auto-tiling textures
}
```

#### Asset Caches

```csharp
private static LRUCache<CharID, CharSheet> spriteCache;     // 200 entries
private static LRUCache<CharID, PortraitSheet> portraitCache; // 50 entries
private static LRUCache<string, DirSheet> objectCache;      // 500 entries
private static LRUCache<TileAddr, BaseSheet> tileCache;     // Pixel-based limit
```

#### Zoom System

```csharp
public enum GameZoom {
    x8Near = -3,  // 8x zoom in
    x4Near = -2,  // 4x zoom in
    x2Near = -1,  // 2x zoom in
    x1 = 0,       // Normal
    x2Far = 1,    // 2x zoom out
    x4Far = 2,    // 4x zoom out
    x8Far = 3,    // 8x zoom out
}

// Window zoom affects final render size
public static int WindowZoom { get; }  // 1, 2, 3, or fullscreen
```

### Drawing Process

#### GameManager.Draw()

```csharp
public void Draw(SpriteBatch spriteBatch, double updateTime) {
    // 1. Set render target
    GraphicsDevice.SetRenderTarget(GameScreen);
    GraphicsDevice.Clear(Color.Black);

    // 2. Draw current scene
    CurrentScene.Draw(spriteBatch);

    // 3. Draw screen effects (fades, transitions)
    spriteBatch.Begin(...);
    fadeScreen.Draw(spriteBatch);
    fadeBG.Draw(spriteBatch);
    fadeTitle.Draw(spriteBatch);

    // 4. Draw menus
    MenuManager.Instance.DrawMenus(spriteBatch);

    // 5. Draw debug info
    if (ShowDebug)
        DrawDebug(spriteBatch, updateTime);

    spriteBatch.End();

    // 6. Render to screen with letterboxing
    GraphicsDevice.SetRenderTarget(null);
    spriteBatch.Draw(GameScreen, screenOffset, Color.White);
}
```

#### Scene Drawing

Scenes maintain sorted draw lists for proper layering:

```csharp
// BaseScene provides layer management
public List<IFinishableSprite>[] Anims;  // 6 layers

public enum DrawLayer {
    Bottom = 0,
    Back = 1,
    Normal = 2,
    Front = 3,
    Top = 4,
    NoDraw = 5
}

// Add sprites to draw lists sorted by Y coordinate
public void AddToDraw(List<(IDrawableSprite, Loc)> sprites, IDrawableSprite sprite) {
    CollectionExt.AddToSortedList(sprites, (sprite, viewOffset), CompareSpriteCoords);
}
```

---

## Input Handling

### FrameInput

**File:** `/RogueEssence/FrameInput.cs`

Captures all input state for a single frame.

```csharp
public class FrameInput {
    public enum InputType {
        // Game actions
        Confirm, Cancel, Attack, Run, Skills, Turn, Diagonal,
        TeamMode, Minimap, Menu, MsgLog,
        SkillMenu, ItemMenu, TacticMenu, TeamMenu,
        LeaderSwap1, LeaderSwap2, LeaderSwap3, LeaderSwap4,
        LeaderSwapBack, LeaderSwapForth,
        Skill1, Skill2, Skill3, Skill4,
        SortItems, SelectItems, SkillPreview, Wait,
        LeftMouse, RightMouse,

        // Meta/debug inputs
        MuteMusic, ShowDebug, Ctrl, Pause,
        AdvanceFrame, Screenshot,
        SpeedDown, SpeedUp, SeeAll, Restart, Test,
    }

    public bool this[InputType i];        // Check if input is pressed
    public Dir8 Direction;                 // 8-directional input
    public Loc MouseLoc;                   // Mouse position
    public int MouseWheel;                 // Scroll wheel delta
    public KeyboardState BaseKeyState;     // Raw keyboard
    public GamePadState BaseGamepadState;  // Raw gamepad
}
```

### InputManager

**File:** `/RogueEssence/InputManager.cs`

Tracks input state changes between frames.

```csharp
public class InputManager {
    private FrameInput PrevInput;
    private FrameInput CurrentInput;

    public long InputTime;        // Frames input held
    public bool this[InputType];  // Current frame state
    public Dir8 Direction;        // Current direction
    public Dir8 PrevDirection;    // Previous direction

    // Edge detection
    public bool JustPressed(InputType input);
    public bool JustReleased(InputType input);
    public bool OnlyPressed(InputType input);
}
```

### Input Flow

```
Hardware Input
      |
      v
+-------------+
| FrameInput  |  <- Captured once per frame
+-------------+
      |
      +---> GameManager.MetaInputManager (debug controls, always active)
      |
      +---> GameManager.InputManager (game controls, can be paused)
                  |
                  v
            CurrentScene.ProcessInput()
                  |
                  v
            GameAction created and executed
```

### GameAction

Actions are serializable commands for replays:

```csharp
public class GameAction {
    public enum ActionType {
        None, Dir, Move, Attack, UseSkill, UseItem,
        Throw, Drop, Give, Take, SetLeader, SendHome,
        ShiftTeam, SetSkill, ShiftSkill, SortItems, Wait,
        Interact, TeamMode, GiveUp, Rescue
    }

    public ActionType Type;
    public Dir8 Dir;
    public List<int> Arg;  // Action-specific arguments
}
```

---

## Mod System

### PathMod

**File:** `/RogueEssence/PathMod.cs`

Handles file path resolution with mod support.

#### Mod Types

```csharp
public enum ModType {
    None = -1,
    Mod,    // Regular mod extending base game
    Quest,  // Total conversion/campaign
    Count
}
```

#### ModHeader

```csharp
public struct ModHeader {
    public string Path;           // Relative path to mod folder
    public string Name;           // Display name
    public string Author;         // Creator
    public string Description;    // Mod description
    public string Namespace;      // Unique namespace identifier
    public Guid UUID;             // Unique ID
    public Version Version;       // Mod version
    public Version GameVersion;   // Required game version
    public ModType ModType;       // Mod or Quest
    public RelatedMod[] Relationships;  // Dependencies, conflicts
}
```

#### Path Resolution

```csharp
// Get path with mod fallback (tries mods first, then base)
string path = PathMod.ModPath("Data/Monster/pikachu.json");

// Iterate all paths for a file (newest first)
foreach (string path in PathMod.FallbackPaths("Data/Monster/pikachu.json")) {
    // path from highest-priority mod first, base game last
}

// Get path in current quest's save folder
string savePath = PathMod.ModSavePath("SAVE/");
```

#### Mod Load Order

Mods specify relationships to control load order:

```csharp
public enum ModRelationship {
    Incompatible,  // Cannot load together
    LoadAfter,     // This mod loads after related mod
    LoadBefore,    // This mod loads before related mod
    DependsOn      // Requires related mod
}

public struct RelatedMod {
    public Guid UUID;
    public string Namespace;
    public ModRelationship Relationship;
}
```

### How Mods Work

1. **Full Replacement**: Mod provides complete file, replaces base
2. **Diff Patching**: Mod provides `.jsonpatch` file, modifies base
3. **New Content**: Mod adds new files not in base game

```
Load Order Resolution:
1. Base game files loaded first
2. Mods sorted by dependencies
3. For each mod in order:
   - Full files override previous
   - Diff patches applied to current state
```

### Creating Mod Content

```
MODS/MyMod/
|-- Mod.xml           # Required: mod metadata
|-- Data/
|   |-- Monster/
|   |   |-- new_mon.json      # New monster
|   |   |-- pikachu.json      # Override existing
|   |   |-- bulbasaur.jsonpatch  # Patch existing
|   |-- Script/
|       |-- my_script.lua
|-- Content/
    |-- Chara/
    |-- Portrait/
```

---

## Key Abstractions

### Singleton Managers

Most managers use singleton pattern with `InitInstance()`:

| Manager | Purpose | Access |
|---------|---------|--------|
| `GameManager` | Scene/game loop | `GameManager.Instance` |
| `DataManager` | Data loading | `DataManager.Instance` |
| `ZoneManager` | Current zone/map | `ZoneManager.Instance` |
| `MenuManager` | UI menus | `MenuManager.Instance` |
| `DungeonScene` | Dungeon gameplay | `DungeonScene.Instance` |
| `GroundScene` | Ground gameplay | `GroundScene.Instance` |
| `LuaEngine` | Lua scripting | `LuaEngine.Instance` |
| `DiagManager` | Diagnostics/settings | `DiagManager.Instance` |

### BaseScene Hierarchy

```
BaseScene (abstract)
|-- BaseDungeonScene
|   |-- DungeonScene (gameplay)
|   |-- DungeonEditScene (editor)
|-- BaseGroundScene
|   |-- GroundScene (gameplay)
|   |-- GroundEditScene (editor)
|-- TitleScene
|-- SplashScene
```

### Character/Entity Hierarchy

**Dungeon Mode:**
```
CharData (base stats)
|-- Character (dungeon character with AI/status)
```

**Ground Mode:**
```
GroundEntity (base)
|-- GroundChar (character)
|-- GroundObject (interactable object)
|-- GroundMarker (invisible trigger)
```

### Map/Zone Structure

```
ZoneData (definition)
|-- Zone (active instance)
    |-- Segment (dungeon segment)
    |   |-- Map (dungeon floor)
    |-- GroundMap (ground map)
```

### IEntryData Interface

All data entries implement this interface:

```csharp
public interface IEntryData {
    LocalText Name { get; }
    string Comment { get; }
    EntrySummary GenerateEntrySummary();
}
```

### Important File Paths

| Constant | Path | Purpose |
|----------|------|---------|
| `DATA_PATH` | `Data/` | Game data files |
| `MAP_PATH` | `Data/Map/` | Dungeon map files |
| `GROUND_PATH` | `Data/Ground/` | Ground map files |
| `CONTENT_PATH` | `Content/` | Graphics/audio assets |
| `SAVE_PATH` | `SAVE/` | Save files |
| `REPLAY_PATH` | `REPLAY/` | Replay recordings |
| `MODS_PATH` | `MODS/` | Mod folders |

### Common Patterns

#### Coroutine-Based Async

```csharp
public IEnumerator<YieldInstruction> DoThing() {
    // Do work
    yield return new WaitForFrames(30);

    // Chain to another coroutine
    yield return CoroutineManager.Instance.StartCoroutine(SubThing());

    // Conditional wait
    yield return new WaitUntil(() => condition);
}
```

#### Event Application

```csharp
// Collect and sort events
foreach (var tuple in IterateEvents(priority)) {
    GameEvent evt = tuple.Item2;
    if (evt is BattleEvent battleEvt) {
        yield return CoroutineManager.Instance.StartCoroutine(
            battleEvt.Apply(owner, ownerChar, context)
        );
    }
}
```

#### Data Access with Caching

```csharp
// Gets from cache or loads from disk
MonsterData data = DataManager.Instance.GetMonster(speciesId);

// Data is automatically cached for future access
```

---

## Tips for Modifying the Codebase

1. **Follow the Coroutine Pattern**: Any game logic that takes time should be a coroutine yielding `YieldInstruction` objects.

2. **Use Existing Events**: Add behavior by creating new `BattleEvent` or `GameEvent` subclasses rather than modifying core logic.

3. **Respect the Mod System**: When adding data, use `PathMod.ModPath()` to allow mod overrides.

4. **Maintain Serialization Compatibility**: When changing data classes, implement version converters in `UpgradeConverters.cs`.

5. **Use the Caching System**: Access data through `DataManager.Instance.Get*()` methods rather than loading directly.

6. **Scene Transitions**: Always go through `GameManager.Instance.SceneOutcome` for scene changes to ensure proper cleanup.

7. **Input Handling**: Use `InputManager.JustPressed()` for one-shot actions, direct `[InputType]` for held actions.
