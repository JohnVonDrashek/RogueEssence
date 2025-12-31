# Common Tasks Guide for RogueEssence

This guide shows AI assistants how to accomplish common tasks in RogueEssence. Each section includes files to modify, step-by-step instructions, code examples, and common pitfalls.

---

## Table of Contents

1. [Adding Game Content](#adding-game-content)
   - [Add a New Monster Species](#1-add-a-new-monster-species)
   - [Add a New Skill/Move](#2-add-a-new-skillmove)
   - [Add a New Item](#3-add-a-new-item)
   - [Add a New Status Effect](#4-add-a-new-status-effect)
   - [Add a New Intrinsic Ability](#5-add-a-new-intrinsic-ability)
2. [Map/Level Design](#maplevel-design)
   - [Create a New Zone (Dungeon)](#6-create-a-new-zone-dungeon)
   - [Create Floor Generation Steps](#7-create-floor-generation-steps)
   - [Create a Ground Map](#8-create-a-ground-map)
   - [Add Spawn Tables](#9-add-spawn-tables)
3. [Scripting](#scripting)
   - [Write a Lua Event Script](#10-write-a-lua-event-script)
   - [Create a Cutscene](#11-create-a-cutscene)
   - [Add Custom Dialogue](#12-add-custom-dialogue)
4. [Engine Work](#engine-work)
   - [Add a New Menu](#13-add-a-new-menu)
   - [Add a New BattleEvent Type](#14-add-a-new-battleevent-type)
   - [Extend the Editor](#15-extend-the-editor)

---

## Adding Game Content

### 1. Add a New Monster Species

**Files to Modify:**
- Data file: Create `POKEMON_DIR/{monster_id}.bin` via editor or script
- Reference: `/RogueEssence/Data/MonsterData.cs`, `/RogueEssence/Data/MonsterForm.cs`

**Data Structure:**
```csharp
// MonsterData.cs - Top-level monster definition
public class MonsterData : IEntryData
{
    public LocalText Name;           // Monster name (localized)
    public bool Released;            // Whether available in game
    public LocalText Title;          // Title/species description
    public int IndexNum;             // Dex number
    public int JoinRate;             // Recruitment rate
    public List<MonsterForm> Forms;  // List of forms (normal, shiny, etc.)
}

// MonsterForm.cs - Form-specific data
public class MonsterForm
{
    public bool Released;
    public LocalText FormName;
    public int Generation;
    public bool GenderlessWeight;    // Gender distribution
    public int BaseHP, BaseAtk, BaseDef, BaseMAtk, BaseMDef, BaseSpeed;
    public string Element1, Element2;
    public string Intrinsic1, Intrinsic2, Intrinsic3;
    public List<LevelUpSkill> LevelSkills;
    public List<string> TeachSkills;
    public List<PromoteBranch> Promotions;  // Evolution branches
}
```

**Step-by-Step:**
1. Open the RogueEssence Editor
2. Navigate to Data > Monster
3. Create new entry or duplicate existing
4. Fill in basic info: Name, Title, IndexNum
5. Add at least one form with:
   - Base stats (HP, Atk, Def, MAtk, MDef, Speed)
   - Element types (use element IDs from Element data)
   - Intrinsic abilities (use intrinsic IDs)
   - Level-up skills
6. Set `Released = true` when ready
7. Save the data file

**Common Pitfalls:**
- Forgetting to set `Released = true` - monster won't appear
- Invalid element/intrinsic/skill IDs cause runtime errors
- FormData list must have at least one form
- IndexNum should be unique across all monsters

---

### 2. Add a New Skill/Move

**Files to Modify:**
- Data file: `SKILL_DIR/{skill_id}.bin`
- Reference: `/RogueEssence/Data/SkillData.cs`, `/RogueEssence/Data/BattleData.cs`

**Data Structure:**
```csharp
// SkillData.cs
public class SkillData : IEntryData
{
    public LocalText Name;           // Skill name
    public bool Released;
    public LocalText Desc;           // Description
    public int BaseCharges;          // PP/uses
    public int Strikes;              // Number of hits (1 = single hit)
    public BattleData Data;          // Battle mechanics
    public ExplosionData Explosion;  // Area of effect settings
    public CombatAction HitboxAction; // Attack hitbox type
}

// BattleData.cs - Core battle mechanics
public class BattleData
{
    public string Element;           // Element type ID
    public SkillCategory Category;   // Physical, Magical, or Status
    public int HitRate;              // Accuracy (-1 = always hits)
    public StateCollection<SkillState> SkillStates;  // Power, contact, etc.

    // Event hooks for battle flow
    public PriorityList<BattleEvent> BeforeTryActions;
    public PriorityList<BattleEvent> BeforeActions;
    public PriorityList<BattleEvent> OnActions;
    public PriorityList<BattleEvent> BeforeExplosions;
    public PriorityList<BattleEvent> BeforeHits;
    public PriorityList<BattleEvent> OnHits;
    public PriorityList<BattleEvent> OnHitTiles;
    public PriorityList<BattleEvent> AfterActions;
}
```

**Step-by-Step:**
1. Open the Editor, navigate to Data > Skill
2. Create new skill entry
3. Set basic properties:
   - Name, Description
   - BaseCharges (typically 10-40)
   - Strikes (usually 1)
4. Configure BattleData:
   - Element (fire, water, etc.)
   - Category (Physical/Magical/Status)
   - HitRate (use 100 for standard, -1 for always-hit)
5. Add SkillStates:
   - `BasePowerState` for damage skills (set Power value)
   - `ContactState` if skill makes contact
   - Custom states as needed
6. Add BattleEvents to OnHits for effects:
   - `DamageFormulaEvent` for damage calculation
   - `StatusBattleEvent` to inflict status
   - `StatChangeEvent` for stat modifications
7. Configure HitboxAction for targeting:
   - `AttackAction` for melee
   - `ProjectileAction` for ranged
   - `AreaAction` for AoE
8. Set Released = true

**Example - Creating a Damage Skill:**
```csharp
// In BattleData.SkillStates:
SkillStates.Set(new BasePowerState(80));  // 80 base power
SkillStates.Set(new ContactState());       // Makes contact

// In BattleData.OnHits:
OnHits.Add(new Priority(0), new DamageFormulaEvent());
```

**Common Pitfalls:**
- Missing `DamageFormulaEvent` in OnHits = no damage dealt
- Forgetting BasePowerState for damaging moves
- HitRate of 0 means the move always misses
- Wrong Category affects which stats are used for damage

---

### 3. Add a New Item

**Files to Modify:**
- Data file: `ITEM_DIR/{item_id}.bin`
- Reference: `/RogueEssence/Data/ItemData.cs`

**Data Structure:**
```csharp
public class ItemData : IEntryData
{
    public LocalText Name;
    public bool Released;
    public LocalText Desc;
    public int Price;                // Shop price
    public int MaxStack;             // Max stack size (1 for unique)
    public int Icon;                 // Sprite index
    public string Sprite;            // Sprite sheet

    public StateCollection<ItemState> ItemStates;  // Item properties

    // Use effects (when consumed/thrown)
    public BattleData UseEvent;      // Effect when used
    public ItemData.UseType UsageType; // Throw, Use, UseOther, etc.

    // Held item effects (PassiveData)
    public ProximityPassive ItemEffect;  // Passive effects when held
}
```

**Step-by-Step:**
1. Open Editor, navigate to Data > Item
2. Create new item entry
3. Set basic properties:
   - Name, Description
   - Price, MaxStack
   - Icon and Sprite
4. Set UsageType:
   - `None` - Cannot be used
   - `Throw` - Thrown only
   - `Use` - Used on self
   - `UseOther` - Used on another
5. Configure UseEvent (BattleData) for use effects:
   - Add BattleEvents to OnHits
6. For held items, configure ItemEffect (ProximityPassive):
   - Add events to OnRefresh, BeforeHittings, etc.
7. Add ItemStates for special properties:
   - `EdibleState` for food items
   - `HeldPowerState` for held damage modifiers

**Example - Healing Berry:**
```csharp
// ItemStates
ItemStates.Set(new EdibleState());

// UseEvent.OnHits - heal when eaten
UseEvent.OnHits.Add(new Priority(0), new RestoreHPEvent(100, true));
```

**Example - Held Item (Scope Lens):**
```csharp
// ItemEffect.BeforeHittings - boost crit rate
ItemEffect.BeforeHittings.Add(new Priority(0),
    new AddContextStateEvent(new CritRateLevelState(1)));
```

**Common Pitfalls:**
- Missing EdibleState for food = can't eat it
- UseEvent without proper BattleEvents = nothing happens
- MaxStack of 0 = cannot be picked up
- Wrong UsageType = confusing UI behavior

---

### 4. Add a New Status Effect

**Files to Modify:**
- Data file: `STATUS_DIR/{status_id}.bin`
- Reference: `/RogueEssence/Data/StatusData.cs`, `/RogueEssence/Data/PassiveData.cs`

**Data Structure:**
```csharp
public class StatusData : IEntryData
{
    public LocalText Name;
    public bool Released;
    public LocalText Desc;

    public bool Targeted;            // Does this status track a target?
    public int StatusStates;         // Status-specific state data
    public DrawEffect DrawEffect;    // Visual indicator

    // Passive effects (from PassiveData)
    public PriorityList<SingleCharEvent> OnMapStarts;
    public PriorityList<SingleCharEvent> OnTurnStarts;
    public PriorityList<SingleCharEvent> OnTurnEnds;
    public PriorityList<SingleCharEvent> OnWalks;
    public PriorityList<SingleCharEvent> OnDeaths;
    public PriorityList<BattleEvent> BeforeTryActions;
    public PriorityList<BattleEvent> BeforeActions;
    public PriorityList<BattleEvent> BeforeHittings;
    public PriorityList<BattleEvent> BeforeBeingHits;
    public PriorityList<BattleEvent> AfterHittings;
    public PriorityList<BattleEvent> AfterBeingHits;
    public PriorityList<RefreshEvent> OnRefresh;
    public PriorityList<StatusGivenEvent> BeforeStatusAdds;
    public PriorityList<StatusGivenEvent> OnStatusAdds;
    public PriorityList<StatusGivenEvent> OnStatusRemoves;
    // ... and more from PassiveData
}
```

**Step-by-Step:**
1. Open Editor, navigate to Data > Status
2. Create new status entry
3. Set Name, Description
4. Add StatusStates for tracking data:
   - `CountDownState` for turn-limited effects
   - `StackState` for stackable effects
   - `BadStatusState` to mark as negative
5. Add event handlers:
   - `OnTurnEnds` for damage-over-time
   - `OnRefresh` for stat modifications
   - `BeforeHittings` for attack modifications
6. Set DrawEffect for visual representation
7. Set Released = true

**Example - Poison Status:**
```csharp
// StatusStates
StatusStates.Set(new BadStatusState());  // Marks as negative
StatusStates.Set(new CountDownState(8)); // Lasts 8 turns

// OnTurnEnds - deal damage each turn
OnTurnEnds.Add(new Priority(0), new ChipDamageEvent(12));

// OnRefresh - show poison indicator
OnRefresh.Add(new Priority(0), new StatusDrawEffect(PoisonEmitter));
```

**Example - Stat Boost Status:**
```csharp
// OnRefresh - modify attack stat
OnRefresh.Add(new Priority(0), new StatChangeRefresh(Stat.Attack, 1));
```

**Common Pitfalls:**
- Missing CountDownState = status never expires
- Forgetting BadStatusState = won't be cleared by healing
- OnRefresh fires constantly - don't put heavy logic there
- Status ID conflicts cause overwriting

---

### 5. Add a New Intrinsic Ability

**Files to Modify:**
- Data file: `INTRINSIC_DIR/{intrinsic_id}.bin`
- Reference: `/RogueEssence/Data/IntrinsicData.cs`, `/RogueEssence/Data/PassiveData.cs`

**Data Structure:**
```csharp
public class IntrinsicData : IEntryData
{
    public LocalText Name;
    public bool Released;
    public LocalText Desc;

    // All PassiveData event handlers available:
    public PriorityList<ItemGivenEvent> OnEquips;
    public PriorityList<ItemGivenEvent> OnPickups;
    public PriorityList<StatusGivenEvent> BeforeStatusAdds;
    public PriorityList<SingleCharEvent> OnMapStarts;
    public PriorityList<SingleCharEvent> OnTurnStarts;
    public PriorityList<SingleCharEvent> OnTurnEnds;
    public PriorityList<RefreshEvent> OnRefresh;
    public PriorityList<BattleEvent> BeforeTryActions;
    public PriorityList<BattleEvent> BeforeActions;
    public PriorityList<BattleEvent> BeforeHittings;
    public PriorityList<BattleEvent> BeforeBeingHits;
    public PriorityList<BattleEvent> AfterHittings;
    public PriorityList<BattleEvent> AfterBeingHits;
    public PriorityList<ElementEffectEvent> UserElementEffects;
    public PriorityList<ElementEffectEvent> TargetElementEffects;
    public PriorityList<HPChangeEvent> ModifyHPs;
    public PriorityList<HPChangeEvent> RestoreHPs;

    // Proximity effects (affect nearby allies/enemies)
    public ProximityData ProximityEvent;
}
```

**Step-by-Step:**
1. Open Editor, navigate to Data > Intrinsic
2. Create new intrinsic entry
3. Set Name and Description
4. Add event handlers based on ability effect:
   - `OnRefresh` for stat modifications
   - `BeforeBeingHits` for defensive abilities
   - `AfterHittings` for offensive abilities
   - `TargetElementEffects` for type immunities
5. For aura effects, use ProximityEvent:
   - Set Radius (max 5)
   - Set TargetAlignments
   - Add events to ProximityEvent's handlers
6. Set Released = true

**Example - Stat Boost Intrinsic (Like Huge Power):**
```csharp
// OnRefresh - double attack stat
OnRefresh.Add(new Priority(0), new MultStatEvent(Stat.Attack, 2, 1));
```

**Example - Type Immunity (Like Levitate):**
```csharp
// TargetElementEffects - immune to Ground
TargetElementEffects.Add(new Priority(0),
    new TypeImmuneEvent("ground", new StringKey("MSG_LEVITATE")));
```

**Example - Aura Ability (Like Friend Guard):**
```csharp
// ProximityEvent settings
ProximityEvent.Radius = 3;
ProximityEvent.TargetAlignments = Alignment.Friend;

// ProximityEvent.BeforeBeingHits - reduce damage to allies
ProximityEvent.BeforeBeingHits.Add(new Priority(0),
    new MultDamageEvent(3, 4));  // 75% damage
```

**Common Pitfalls:**
- OnRefresh events must be idempotent (run repeatedly)
- ProximityEvent Radius > 5 causes performance issues
- Element immunity needs proper message key
- Priority values affect event ordering

---

## Map/Level Design

### 6. Create a New Zone (Dungeon)

**Files to Modify:**
- Data file: `ZONE_DIR/{zone_id}.bin`
- Reference: `/RogueEssence/Data/ZoneData.cs`, `/RogueEssence/LevelGen/ZoneSegment.cs`

**Data Structure:**
```csharp
public class ZoneData : IEntryData
{
    public LocalText Name;
    public bool Released;
    public int Level;                // Difficulty level
    public int LevelCap;             // Max level allowed
    public int TeamSize;             // Max party size
    public int TeamCount;            // Teams allowed
    public int BagRestrict;          // Item restrictions
    public int BagSize;              // Inventory size
    public int MoneyRestrict;        // Money restrictions

    public List<ZoneSegment> Segments;  // Floor groups
    public List<GroundMap> Grounds;     // Hub/town maps

    public List<ZoneStep> ZoneSteps;    // Zone-wide generation steps
}

public class ZoneSegment
{
    public int FloorCount;           // Number of floors in segment
    public IFloorGen FloorGen;       // Floor generator type
    public List<ZoneStep> ZoneSteps; // Segment-specific steps
}
```

**Step-by-Step:**
1. Open Editor, navigate to Data > Zone
2. Create new zone entry
3. Set basic properties:
   - Name
   - Level (base difficulty)
   - LevelCap, TeamSize, TeamCount
   - BagRestrict, BagSize, MoneyRestrict
4. Add at least one ZoneSegment:
   - Set FloorCount
   - Choose FloorGen type (GridFloorGen recommended)
   - Add ZoneSteps for spawning, items, etc.
5. Configure ZoneSteps for the segment:
   - `TeamSpawnZoneStep` for enemies
   - `SpreadVaultZoneStep` for treasure
   - `FloorNameDropZoneStep` for floor names
6. Optionally add GroundMaps for hub areas
7. Set Released = true

**Zone Segment Types:**
- `GridFloorGen` - Grid-based rooms connected by halls
- `RoomFloorGen` - Free-form room placement
- `StairsFloorGen` - Simple rectangular floors
- `LoadGen` - Load pre-made maps from editor

**Common Pitfalls:**
- Empty Segments list = zone has no floors
- FloorCount of 0 = segment skipped
- Missing spawn steps = empty floors
- Level too high without proper scaling

---

### 7. Create Floor Generation Steps

**Files to Modify:**
- Zone data files
- Reference: `/RogueEssence/LevelGen/IFloorGen.cs`, GenSteps in RogueElements

**Floor Generation Types:**
```csharp
// Grid-based generation (most common)
public class GridFloorGen : FloorMapGen<MapGenContext>

// Room-based generation
public class RoomFloorGen : FloorMapGen<ListMapGenContext>

// Simple stairs-based
public class StairsFloorGen : FloorMapGen<StairsMapGenContext>

// Load pre-made maps
public class LoadGen : FloorMapGen<MapLoadContext>
```

**Key GenSteps:**
```csharp
// Initialize the grid
InitGridPlanStep<T> - Sets up grid dimensions

// Room generation
GridPathBranch<T> - Creates branching paths
GridPathGrid<T> - Creates full grid connections

// Room types
RoomGen<T> - Base room generator
RoomGenCross<T> - Cross-shaped rooms
RoomGenRound<T> - Circular rooms

// Hallway types
GridHallBranch<T> - Branching hallways
GridDefaultHall<T> - Standard hallways

// Tile and terrain
FloorStairsStep<T> - Add entrance/exit stairs
WaterStep<T> - Add water terrain
PerlinWaterStep<T> - Perlin noise water
```

**Step-by-Step (GridFloorGen):**
1. In zone segment, set FloorGen to GridFloorGen
2. Add GenSteps in order:
   ```
   Priority 1: InitGridPlanStep (set CellX, CellY, CellWidth, CellHeight)
   Priority 2: GridPathBranch (create room connections)
   Priority 3: SetGridDefaultsStep (fill in room types)
   Priority 4: DrawGridToFloorStep (convert grid to tiles)
   Priority 5: FloorStairsStep (add entrance/exit)
   ```
3. Add spawning steps:
   ```
   Priority 6: MobSpawnStep (enemy spawns)
   Priority 7: ItemSpawnStep (item spawns)
   ```

**Example - Basic Dungeon Floor:**
```csharp
// In FloorGen.GenSteps:

// Step 1: Initialize 4x4 grid with 8x8 cell size
GenSteps.Add(new Priority(1), new InitGridPlanStep<MapGenContext>()
{
    CellX = 4, CellY = 4,
    CellWidth = 8, CellHeight = 8
});

// Step 2: Create branching path
GenSteps.Add(new Priority(2), new GridPathBranch<MapGenContext>()
{
    RoomRatio = 50,
    BranchRatio = 30
});

// Step 3: Default room types
GenSteps.Add(new Priority(3), new SetGridDefaultsStep<MapGenContext>()
{
    DefaultRoom = new RoomGenSquare<MapGenContext>(4, 8, 4, 8)
});

// Step 4: Draw to floor
GenSteps.Add(new Priority(4), new DrawGridToFloorStep<MapGenContext>());

// Step 5: Add stairs
GenSteps.Add(new Priority(5), new FloorStairsStep<MapGenContext>(
    new MapGenEntrance(), new MapGenExit()
));
```

**Common Pitfalls:**
- Wrong step order = generation fails
- Missing InitGridPlanStep = null reference
- CellWidth/Height too small = rooms overlap
- No FloorStairsStep = no way to exit

---

### 8. Create a Ground Map

**Files to Modify:**
- Ground map files in `GROUND_DIR/`
- Reference: `/RogueEssence/Ground/GroundMap.cs`

**Data Structure:**
```csharp
public class GroundMap
{
    public LocalText Name;
    public int Width, Height;
    public Tile[,] Tiles;            // Terrain tiles
    public AutoTile[,] TextureMap;   // Visual textures

    public List<GroundSpawner> Spawners;  // NPC/object spawners
    public List<GroundMarker> Markers;    // Trigger points
    public List<GroundChar> Chars;        // Placed characters
    public List<GroundObject> Objects;    // Placed objects

    public string Music;             // Background music
    public string Entry;             // Entry script
}
```

**Step-by-Step:**
1. Open Editor, navigate to Edit > Ground Editor
2. Create new ground map or load existing
3. Set dimensions (Width, Height)
4. Paint terrain using tile palette:
   - Walkable tiles
   - Blocking tiles (walls)
   - Water/hazard tiles
5. Add textures/decorations
6. Place spawn points and markers:
   - `GroundSpawner` for NPCs
   - `GroundMarker` for trigger zones
7. Add objects:
   - Signs, chests, doors
   - Interactive objects
8. Set Music and Entry script
9. Save to GROUND_DIR

**Script Integration:**
```lua
-- In SCRIPT_DIR/{zone}/init.lua
function ZONE_SCRIPT.Zone_Init(zone)
    -- Called when zone loads
end

-- Ground map entry script
function GROUND_SCRIPT.{MapName}_Init(map)
    -- Called when map initializes
end

function GROUND_SCRIPT.{MapName}_Enter(map)
    -- Called when player enters
end
```

**Common Pitfalls:**
- Missing entry script = no events fire
- Unconnected spawn points = stuck characters
- No exit marker = player trapped
- Texture layer misaligned with terrain

---

### 9. Add Spawn Tables

**Files to Modify:**
- Zone/segment data files
- Reference: `/RogueEssence/LevelGen/Spawning/MobSpawn.cs`, `/RogueEssence/LevelGen/Zones/TeamSpawnZoneStep.cs`

**Data Structure:**
```csharp
// MobSpawn.cs - Individual spawn entry
public class MobSpawn
{
    public string BaseForm;          // Monster ID
    public int Level;                // Spawn level
    public string Intrinsic;         // Override intrinsic
    public List<string> Skills;      // Override skills
    public MobSpawnExtra SpawnExtra; // Extra spawn data
}

// TeamSpawnZoneStep - Zone-level spawning
public class TeamSpawnZoneStep : ZoneStep
{
    public SpawnList<TeamSpawner> Spawns;  // Spawn table
    public int Priority;                    // Step priority
}
```

**Step-by-Step:**
1. In zone segment, add TeamSpawnZoneStep to ZoneSteps
2. Configure the SpawnList with entries:
   ```csharp
   // Add spawn with weight
   Spawns.Add(new TeamSpawner<MobSpawn>(
       new MobSpawn() { BaseForm = "pikachu", Level = 10 }
   ), 100);  // Weight of 100
   ```
3. Set spawn rates per floor range
4. Configure spawn density in MobSpawnStep

**Example - Multi-monster Spawn Table:**
```csharp
var spawnStep = new TeamSpawnZoneStep();

// Common spawn (high weight)
spawnStep.Spawns.Add(new TeamSpawner<MobSpawn>(
    new MobSpawn() { BaseForm = "rattata", Level = 5 }
), 200);

// Uncommon spawn (medium weight)
spawnStep.Spawns.Add(new TeamSpawner<MobSpawn>(
    new MobSpawn() { BaseForm = "pidgey", Level = 6 }
), 100);

// Rare spawn (low weight)
spawnStep.Spawns.Add(new TeamSpawner<MobSpawn>(
    new MobSpawn() { BaseForm = "pikachu", Level = 8 }
), 20);

// Add to zone segment
Segment.ZoneSteps.Add(spawnStep);
```

**Spawn Density (in MobSpawnStep):**
```csharp
// In FloorGen.GenSteps:
GenSteps.Add(new Priority(10), new MobSpawnStep<MapGenContext>()
{
    Spawns = spawnList,
    RespawnRate = 30,    // Turns between respawns
    MaxTeams = 8         // Max enemies at once
});
```

**Common Pitfalls:**
- Weight of 0 = never spawns
- Missing TeamSpawnZoneStep = empty floors
- Level too high relative to zone = difficulty spike
- Forgetting to add MobSpawnStep in FloorGen

---

## Scripting

### 10. Write a Lua Event Script

**Files to Modify:**
- Script files in `SCRIPT_DIR/{zone}/`
- Reference: `/RogueEssence/Lua/LuaEngine.cs`

**Script Structure:**
```lua
-- init.lua - Main entry point for zone scripts
require 'common'  -- Common utilities

-- Zone-level callbacks
ZONE_SCRIPT = {}

function ZONE_SCRIPT.Zone_Init(zone)
    -- Called when zone is loaded
end

function ZONE_SCRIPT.Zone_EnterSegment(zone, segmentID, mapID)
    -- Called when entering a dungeon segment
end

function ZONE_SCRIPT.Zone_ExitSegment(zone, result, rescue, segmentID, mapID)
    -- Called when exiting a dungeon segment
end
```

**Available Global Objects:**
```lua
-- Core game systems
GAME           -- Game manager (save, load, party access)
GROUND         -- Ground map utilities
DUNGEON        -- Dungeon-specific utilities
UI             -- UI and dialogue system
SOUND          -- Sound/music playback
SV             -- Script variables (persistent storage)

-- Game data access
DATA           -- Data manager
ZONE           -- Current zone
MAP            -- Current map
FLOOR          -- Current floor

-- Character/entity access
CH(name)       -- Get character by name
PARTY          -- Access party members
```

**Step-by-Step:**
1. Create `SCRIPT_DIR/{zone_id}/` directory
2. Create `init.lua` with zone callbacks
3. Define ZONE_SCRIPT table with callbacks:
   - `Zone_Init` - One-time setup
   - `Zone_EnterSegment` - Dungeon entry
   - `Zone_ExitSegment` - Dungeon completion
4. For ground maps, add callbacks:
   - `{MapName}_Init` - Map setup
   - `{MapName}_Enter` - Player enters
   - `{MapName}_Exit` - Player leaves
5. Link zone to script in zone data

**Example - Zone Entry Script:**
```lua
-- SCRIPT_DIR/test_dungeon/init.lua
require 'common'

ZONE_SCRIPT = {}

function ZONE_SCRIPT.Zone_Init(zone)
    PrintInfo("Test Dungeon initialized!")
end

function ZONE_SCRIPT.Zone_EnterSegment(zone, segmentID, mapID)
    -- Show message when entering
    UI:WaitShowDialogue("Welcome to Test Dungeon!")
end

function ZONE_SCRIPT.Zone_ExitSegment(zone, result, rescue, segmentID, mapID)
    if result == RogueEssence.Data.GameProgress.ResultType.Cleared then
        UI:WaitShowDialogue("Congratulations! You cleared the dungeon!")
    end
end
```

**Common Pitfalls:**
- Missing `require 'common'` = missing utilities
- Wrong callback name = never called
- Forgetting to yield in coroutines
- SV changes not persisting (use GAME:SetSV)

---

### 11. Create a Cutscene

**Files to Modify:**
- Script files in `SCRIPT_DIR/`
- Reference: `/RogueEssence/Lua/ScriptUI.cs`, `/RogueEssence/Lua/ScriptGround.cs`

**Cutscene Functions:**
```lua
-- Dialogue and UI
UI:SetSpeaker(name, emotion)           -- Set dialogue speaker
UI:WaitShowDialogue(text)              -- Show and wait for dialogue
UI:SetSpeakerEmotion(emotion)          -- Change speaker emotion
UI:WaitShowChoice(text, choices)       -- Show choice menu

-- Character control
GROUND:CharSetEmote(char, emote)       -- Set character emote
GROUND:CharAnimateTurn(char, dir)      -- Turn character
GROUND:CharAnimateMoveTo(char, x, y)   -- Move character
GROUND:CharWaitAnim(char, anim)        -- Play animation

-- Camera
GAME:MoveCamera(x, y, duration)        -- Pan camera
GAME:FadeIn(duration)                  -- Fade from black
GAME:FadeOut(duration)                 -- Fade to black

-- Music/Sound
SOUND:PlayBGM(track, fade)             -- Play background music
SOUND:PlaySE(effect)                   -- Play sound effect
```

**Step-by-Step:**
1. Create cutscene function in script file
2. Use GAME:FadeOut() to start
3. Position characters off-screen or hidden
4. Use GAME:FadeIn() to reveal scene
5. Control dialogue flow with UI functions
6. Move characters with GROUND functions
7. End with transition (fade out, teleport, etc.)

**Example - Story Cutscene:**
```lua
function ZONE_SCRIPT.Story_Intro()
    -- Fade out to set up scene
    GAME:FadeOut(false, 20)
    GAME:WaitFrames(20)

    -- Position characters
    local hero = CH("HERO")
    local partner = CH("PARTNER")

    GROUND:CharSetPosition(hero, 100, 150)
    GROUND:CharSetPosition(partner, 150, 150)

    -- Fade in
    GAME:FadeIn(20)
    GAME:WaitFrames(20)

    -- Dialogue sequence
    UI:SetSpeaker(hero, "Normal")
    UI:WaitShowDialogue("Where are we?")

    UI:SetSpeaker(partner, "Worried")
    UI:WaitShowDialogue("I don't know... but look over there!")

    -- Partner points
    GROUND:CharAnimateTurn(partner, Direction.Right)
    GROUND:CharSetEmote(partner, "Exclaim")
    GAME:WaitFrames(30)

    -- Choice for player
    local result = UI:WaitShowChoice("What do you do?", {
        "Investigate",
        "Stay here",
        "Run away"
    })

    if result == 1 then
        UI:WaitShowDialogue("Let's check it out!")
        -- Continue to investigation scene
    elseif result == 2 then
        UI:WaitShowDialogue("Maybe we should wait...")
    else
        UI:WaitShowDialogue("Let's get out of here!")
    end
end
```

**Common Pitfalls:**
- Forgetting GAME:WaitFrames() after animations
- Not yielding (UI functions that start with Wait automatically yield)
- Character positions off-map = invisible
- Wrong Direction enum values

---

### 12. Add Custom Dialogue

**Files to Modify:**
- String files in `STRINGS_DIR/`
- Script files calling dialogue
- Reference: UI system in LuaEngine

**Dialogue Methods:**
```lua
-- Simple dialogue
UI:WaitShowDialogue("Hello there!")

-- Dialogue with speaker
UI:SetSpeaker(character, "Normal")  -- Set speaker first
UI:WaitShowDialogue("This is my line.")

-- Dialogue with portrait
UI:SetSpeaker("Speaker Name", "Happy", true)  -- true = show portrait
UI:WaitShowDialogue("I'm so happy!")

-- Multiple lines (auto-continues)
UI:WaitShowDialogue("Line 1[br]Line 2[br]Line 3")

-- Pause within dialogue
UI:WaitShowDialogue("Wait for it...[pause=60]Surprise!")
```

**Text Formatting:**
```lua
-- Line breaks
"[br]"                    -- New line

-- Pauses
"[pause=N]"               -- Pause for N frames

-- Text speed
"[speed=N]"               -- Set text speed

-- Colors
"[color=#FF0000]Red text[color]"  -- Colored text

-- Variables
"[hero]"                  -- Player name
"[partner]"               -- Partner name
```

**Step-by-Step:**
1. Decide where dialogue triggers (event, NPC interaction, etc.)
2. Use UI:SetSpeaker() to set who's talking
3. Use UI:WaitShowDialogue() for the text
4. For localization, use string keys from STRINGS_DIR
5. For choices, use UI:WaitShowChoice()

**Example - NPC Conversation:**
```lua
function NPC_Shopkeeper_Action(npc, activator)
    -- Set speaker with portrait
    UI:SetSpeaker("Shopkeeper", "Happy")
    UI:WaitShowDialogue("Welcome to my shop!")

    local choice = UI:WaitShowChoice("What would you like?", {
        "Buy items",
        "Sell items",
        "Just browsing"
    })

    if choice == 1 then
        -- Open buy menu
        UI:WaitShowDialogue("Here's what I have!")
        -- ... open shop interface
    elseif choice == 2 then
        UI:WaitShowDialogue("Let me see what you've got!")
        -- ... open sell interface
    else
        UI:SetSpeakerEmotion("Normal")
        UI:WaitShowDialogue("Take your time looking around!")
    end
end
```

**Localization:**
```lua
-- In STRINGS_DIR, define keys:
-- SHOP_WELCOME = "Welcome to my shop!"

-- In script, reference by key:
UI:WaitShowDialogue(STRINGS:FormatKey("SHOP_WELCOME"))
```

**Common Pitfalls:**
- Missing closing tags for [color]
- Pause value too high = feels frozen
- Long lines without [br] = text overflow
- SetSpeaker after WaitShowDialogue = wrong portrait

---

## Engine Work

### 13. Add a New Menu

**Files to Modify:**
- New menu class in `/RogueEssence/Menu/`
- Reference: `/RogueEssence/Menu/MenuBase.cs`, `/RogueEssence/Menu/InteractableMenu.cs`

**Menu Hierarchy:**
```csharp
MenuBase              // Base for all menus
├── InteractableMenu  // Menus with input handling
│   ├── SingleStripMenu   // Single column of choices
│   ├── MultiPageMenu     // Paged menu
│   └── SideScrollMenu    // Horizontal scrolling
└── InfoMenu          // Display-only menus
```

**Key Classes:**
```csharp
// MenuBase.cs - Base class
public abstract class MenuBase
{
    public Loc Bounds { get; protected set; }
    public abstract IEnumerable<IMenuElement> GetElements();
    public virtual void Update(InputManager input);
}

// InteractableMenu.cs - Interactive menus
public abstract class InteractableMenu : MenuBase
{
    protected List<IInteractable> choices;
    protected virtual void UpdateMenu();
    protected virtual void Choose(int choiceIndex);
    protected virtual void Cancel();
}
```

**Step-by-Step:**
1. Create new class in `/RogueEssence/Menu/`
2. Inherit from appropriate base (usually `InteractableMenu`)
3. Define menu layout in constructor:
   ```csharp
   public MyMenu() : base()
   {
       Bounds = new Rect(0, 0, 200, 150);
       choices = new List<IInteractable>();
       // Add menu choices
   }
   ```
4. Override `GetElements()` for visual elements
5. Override `Choose()` for selection handling
6. Override `Cancel()` for back/cancel behavior
7. Register in menu manager if needed

**Example - Simple Choice Menu:**
```csharp
public class DifficultyMenu : InteractableMenu
{
    private List<MenuTextChoice> choices;

    public DifficultyMenu()
    {
        // Set bounds
        Bounds = new Rect(
            GraphicsManager.ScreenWidth / 2 - 80,
            GraphicsManager.ScreenHeight / 2 - 60,
            160, 120
        );

        // Create choices
        choices = new List<MenuTextChoice>
        {
            new MenuTextChoice("Easy", () => SetDifficulty(0)),
            new MenuTextChoice("Normal", () => SetDifficulty(1)),
            new MenuTextChoice("Hard", () => SetDifficulty(2))
        };

        // Add to interactables
        for (int i = 0; i < choices.Count; i++)
        {
            choices[i].Bounds = new Rect(8, 8 + i * 14, 144, 14);
            AddChoice(choices[i]);
        }
    }

    private void SetDifficulty(int level)
    {
        GameManager.Instance.SetDifficulty(level);
        MenuManager.Instance.RemoveMenu();
    }

    public override IEnumerable<IMenuElement> GetElements()
    {
        yield return new MenuFrame(Bounds);
        foreach (var choice in choices)
            yield return choice;
    }

    protected override void Cancel()
    {
        MenuManager.Instance.RemoveMenu();
    }
}
```

**Opening the Menu:**
```csharp
// From game code:
MenuManager.Instance.AddMenu(new DifficultyMenu(), false);

// From Lua:
UI:ChoiceMenu("Select Difficulty", {"Easy", "Normal", "Hard"})
```

**Common Pitfalls:**
- Bounds outside screen = menu not visible
- Not calling base constructor = null refs
- Forgetting to remove menu on cancel = stuck
- Choice callbacks not handling all cases

---

### 14. Add a New BattleEvent Type

**Files to Modify:**
- New event class in `/RogueEssence/Dungeon/GameEffects/`
- Reference: `/RogueEssence/Dungeon/GameEffects/BattleEvent.cs`

**BattleEvent Base:**
```csharp
// BattleEvent.cs
[Serializable]
public abstract class BattleEvent : GameEvent
{
    // Main application method
    public abstract IEnumerator<YieldInstruction> Apply(
        GameEventOwner owner,
        Character ownerChar,
        BattleContext context
    );

    // Clone for copying events
    public abstract GameEvent Clone();
}
```

**BattleContext Properties:**
```csharp
public class BattleContext
{
    public Character User;           // Attacker
    public Character Target;         // Target
    public BattleData Data;          // Skill data
    public int BaseDamage;           // Pre-modified damage
    public int ActionType;           // Type of action

    // State bags for passing data between events
    public StateCollection<ContextState> GlobalContextStates;
    public StateCollection<ContextState> UserContextStates;
    public StateCollection<ContextState> TargetContextStates;
}
```

**Step-by-Step:**
1. Create new class in `/RogueEssence/Dungeon/GameEffects/`
2. Inherit from `BattleEvent`
3. Add serializable fields for configuration
4. Implement `Apply()` method:
   ```csharp
   public override IEnumerator<YieldInstruction> Apply(
       GameEventOwner owner,
       Character ownerChar,
       BattleContext context)
   {
       // Your logic here
       yield break;
   }
   ```
5. Implement `Clone()` method
6. Add to appropriate event list (OnHits, BeforeHittings, etc.)

**Example - Lifesteal Event:**
```csharp
[Serializable]
public class LifestealEvent : BattleEvent
{
    /// <summary>
    /// Percentage of damage to heal (0-100)
    /// </summary>
    public int HealPercent;

    public LifestealEvent() { }

    public LifestealEvent(int healPercent)
    {
        HealPercent = healPercent;
    }

    public override GameEvent Clone()
    {
        return new LifestealEvent(HealPercent);
    }

    public override IEnumerator<YieldInstruction> Apply(
        GameEventOwner owner,
        Character ownerChar,
        BattleContext context)
    {
        // Only apply if we dealt damage
        if (context.BaseDamage > 0 && context.Target != null)
        {
            int healAmount = context.BaseDamage * HealPercent / 100;

            if (healAmount > 0)
            {
                // Show message
                DungeonScene.Instance.LogMsg(
                    String.Format("{0} drained energy!",
                        context.User.GetDisplayName(false)));

                // Heal the user
                yield return CoroutineManager.Instance.StartCoroutine(
                    context.User.RestoreHP(healAmount));
            }
        }

        yield break;
    }
}
```

**Using the Event:**
```csharp
// In skill's OnHits:
skill.Data.OnHits.Add(new Priority(10), new LifestealEvent(50));

// In intrinsic's AfterHittings:
intrinsic.AfterHittings.Add(new Priority(0), new LifestealEvent(25));
```

**Event Hook Points (from PassiveData):**
| Hook | When it fires |
|------|---------------|
| BeforeTryActions | Before action attempt (can cancel) |
| BeforeActions | Before action executes |
| OnActions | After action initiates |
| BeforeHittings | Before attacker hits |
| BeforeBeingHits | Before target is hit |
| AfterHittings | After attacker hits |
| AfterBeingHits | After target is hit |
| OnHitTiles | When hitting tiles |
| AfterActions | After action completes |

**Common Pitfalls:**
- Forgetting `[Serializable]` = save/load fails
- Not implementing Clone() properly = shared state bugs
- yield break missing = coroutine never ends
- Accessing null Target in area attacks

---

### 15. Extend the Editor

**Files to Modify:**
- Editor classes in `/RogueEssence.Editor.Avalonia/`
- Reference: Avalonia UI framework

**Editor Architecture:**
```
RogueEssence.Editor.Avalonia/
├── ViewModels/           # MVVM ViewModels
│   ├── DataEditor/       # Data type editors
│   └── Dialogs/          # Dialog ViewModels
├── Views/                # Avalonia XAML views
│   ├── DataEditor/       # Data editor views
│   └── Dialogs/          # Dialog views
└── Controls/             # Custom UI controls
```

**Key Patterns:**
```csharp
// ViewModel base
public class ViewModelBase : INotifyPropertyChanged
{
    protected void OnPropertyChanged(string propertyName);
}

// Data editor pattern
public class MonsterDataEditor : ViewModelBase
{
    private MonsterData _data;

    public string Name
    {
        get => _data.Name.DefaultText;
        set { _data.Name.DefaultText = value; OnPropertyChanged(nameof(Name)); }
    }
}
```

**Step-by-Step (Add Field to Existing Editor):**
1. Find existing editor in ViewModels/DataEditor/
2. Add property for new field:
   ```csharp
   public int NewField
   {
       get => _data.NewField;
       set { _data.NewField = value; OnPropertyChanged(nameof(NewField)); }
   }
   ```
3. Update corresponding View in Views/DataEditor/
4. Add UI element bound to property:
   ```xml
   <TextBox Text="{Binding NewField}" />
   ```

**Step-by-Step (Add New Editor Dialog):**
1. Create ViewModel in ViewModels/Dialogs/:
   ```csharp
   public class MyDialogViewModel : ViewModelBase
   {
       public string InputText { get; set; }
       public ICommand OkCommand { get; }
       public ICommand CancelCommand { get; }

       public MyDialogViewModel()
       {
           OkCommand = new RelayCommand(OnOk);
           CancelCommand = new RelayCommand(OnCancel);
       }
   }
   ```
2. Create View in Views/Dialogs/:
   ```xml
   <Window xmlns="https://github.com/avaloniaui"
           x:Class="RogueEssence.Editor.Avalonia.Views.MyDialog">
       <StackPanel>
           <TextBox Text="{Binding InputText}" />
           <Button Command="{Binding OkCommand}" Content="OK" />
           <Button Command="{Binding CancelCommand}" Content="Cancel" />
       </StackPanel>
   </Window>
   ```
3. Create code-behind in Views/:
   ```csharp
   public partial class MyDialog : Window
   {
       public MyDialog()
       {
           InitializeComponent();
       }
   }
   ```
4. Open dialog from other code:
   ```csharp
   var dialog = new MyDialog();
   dialog.DataContext = new MyDialogViewModel();
   var result = await dialog.ShowDialog<bool>(parentWindow);
   ```

**Adding Custom Property Editor:**
```csharp
// For complex types, create dedicated editor
public class CustomTypeEditor : UserControl
{
    public static readonly StyledProperty<CustomType> ValueProperty =
        AvaloniaProperty.Register<CustomTypeEditor, CustomType>(nameof(Value));

    public CustomType Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }
}
```

**Common Pitfalls:**
- Missing INotifyPropertyChanged = UI doesn't update
- XAML binding typos fail silently
- Not handling null data gracefully
- Forgetting to register new views/controls

---

## Quick Reference

### Event Priority Guidelines

| Priority | Use Case |
|----------|----------|
| -100 to -50 | Early modifiers (element changes) |
| -49 to 0 | Pre-processing (setup states) |
| 1 to 50 | Main effects (damage, status) |
| 51 to 100 | Post-processing (reactions) |

### Common Data IDs

Access data by ID strings. Check DataManager for loaded data:
```csharp
// Get monster data
MonsterData mon = DataManager.Instance.GetMonster("pikachu");

// Get skill data
SkillData skill = DataManager.Instance.GetSkill("thunderbolt");

// Get item data
ItemData item = DataManager.Instance.GetItem("oran_berry");
```

### Lua Quick Reference

```lua
-- Get party leader
local leader = GAME:GetPlayerPartyMember(0)

-- Get character by name
local npc = CH("ShopNPC")

-- Teleport player
GROUND:TeleportTo(leader, "new_map", "spawn_marker")

-- Play sound
SOUND:PlaySE("Menu/confirm")

-- Save script variable
SV.MyVar = "value"
GAME:SetSV()  -- Persist to save
```

### Build and Test

```bash
# Build the solution
dotnet build RogueEssence.sln

# Run the game
dotnet run --project RogueEssence

# Run the editor
dotnet run --project RogueEssence.Editor.Avalonia
```
