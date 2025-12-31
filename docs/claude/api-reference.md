# RogueEssence API Quick Reference

A condensed reference for AI assistants showing the most important classes and their key methods.

---

## 1. Singleton Managers

### DataManager (RogueEssence.Data.DataManager)

| Property/Method | Description |
|-----------------|-------------|
| `Instance` | Singleton instance |
| `Save` | Current GameProgress save data |
| `DataIndices[DataType]` | Access data indices by type |
| `GetMonster(string id)` | Get MonsterData by ID |
| `GetSkill(string id)` | Get SkillData by ID |
| `GetItem(string id)` | Get ItemData by ID |
| `GetStatus(string id)` | Get StatusData by ID |
| `GetIntrinsic(string id)` | Get IntrinsicData by ID |
| `GetZone(string id)` | Get ZoneData by ID |
| `GetTerrain(string id)` | Get TerrainData by ID |
| `GetTile(string id)` | Get TileData (effect tiles) by ID |
| `GetEmote(string id)` | Get EmoteData by ID |
| `SaveData(string path)` | Save current game progress |
| `LoadData(string path)` | Load game progress |

### GameManager (RogueEssence.Scene.GameManager)

| Property/Method | Description |
|-----------------|-------------|
| `Instance` | Singleton instance |
| `CurrentScene` | Currently active scene |
| `SceneOutcome` | Set to trigger scene transition |
| `GameSpeed` | Current game speed setting |
| `SE(string name)` | Play sound effect |
| `BattleSE(string name)` | Play battle sound effect |
| `LoopSE(string name)` | Play looping sound effect |
| `SetFade(bool fadeIn, bool white)` | Control screen fade |
| `FadeOut(bool white, int duration)` | Fade out coroutine |
| `FadeIn(int duration)` | Fade in coroutine |
| `MoveToZone(ZoneLoc dest)` | Navigate to zone location |
| `MoveToGround(string zone, string map, string entry)` | Navigate to ground map |
| `BeginGameInSegment(ZoneLoc dest, DungeonStakes, bool, bool)` | Start dungeon adventure |

### ZoneManager (RogueEssence.Dungeon.ZoneManager)

| Property/Method | Description |
|-----------------|-------------|
| `Instance` | Singleton instance |
| `CurrentZone` | Currently loaded Zone |
| `CurrentZoneID` | ID of current zone |
| `CurrentMap` | Current dungeon Map |
| `CurrentMapID` | Current map's SegLoc |
| `CurrentGround` | Current GroundMap |

### GraphicsManager (RogueEssence.Content.GraphicsManager)

| Property/Method | Description |
|-----------------|-------------|
| `WindowZoom` | Current window zoom level |
| `TotalFrameTick` | Total elapsed ticks |
| `GetAnimIndex(string name)` | Get animation index by name |
| `GetChara(MonsterID)` | Get character sprite sheet |
| `GetPortrait(MonsterID, bool shiny)` | Get portrait sprite |
| `GetAttackSheet(string name)` | Get attack animation sheet |
| `GetObject(string name)` | Get object sprite |
| `GetTile(TileFrame)` | Get tile texture |

### MenuManager (RogueEssence.Menu.MenuManager)

| Property/Method | Description |
|-----------------|-------------|
| `Instance` | Singleton instance |
| `Menus` | Stack of active menus |
| `AddMenu(IInteractable, bool)` | Push menu onto stack |
| `RemoveMenu()` | Pop topmost menu |
| `ReplaceMenu(IInteractable)` | Replace current menu |
| `ClearMenus()` | Clear all menus |
| `SetDialogue(...)` | Show dialogue box |

### SoundManager (RogueEssence.Content.SoundManager)

| Property/Method | Description |
|-----------------|-------------|
| `PlayBGM(string, bool loop)` | Play background music |
| `StopBGM()` | Stop current BGM |
| `SetBGMVolume(float)` | Set BGM volume (0-1) |
| `PlaySE(string)` | Play sound effect |
| `SetSEVolume(float)` | Set SE volume (0-1) |

### DiagManager (RogueEssence.DiagManager)

| Property/Method | Description |
|-----------------|-------------|
| `Instance` | Singleton instance |
| `DevMode` | Whether dev mode is active |
| `LogError(Exception, bool show)` | Log error with optional display |
| `LogInfo(string)` | Log informational message |

---

## 2. Scene Classes

### DungeonScene (RogueEssence.Dungeon.DungeonScene)

| Property/Method | Description |
|-----------------|-------------|
| `Instance` | Singleton instance |
| `ActiveTeam` | The player's team |
| `FocusedCharacter` | Currently controlled character |
| `PendingDevEvent` | Queue coroutine for next turn |
| `LogMsg(string)` | Add message to dungeon log |
| `SetCharAnimation(Character, anim)` | Set character animation |
| `MoveCamera(Loc, int duration)` | Pan camera to location |
| `CreateAnim(BaseAnim, DrawLayer)` | Spawn visual effect |
| `SetScreenShake(ScreenMover)` | Apply screen shake |
| `AddMapStatus(MapStatus)` | Add status to floor |
| `RemoveMapStatus(string id)` | Remove floor status |
| `SaveGame()` | Save in dungeon |

### GroundScene (RogueEssence.Ground.GroundScene)

| Property/Method | Description |
|-----------------|-------------|
| `Instance` | Singleton instance |
| `FocusedCharacter` | Currently controlled GroundChar |
| `ZoomScale` | Current camera zoom |
| `MoveCamera(Loc, int duration, bool toPlayer)` | Pan camera |
| `SaveGame()` | Save in ground mode |
| `AddMapStatus(MapStatus)` | Add status to map |

### TitleScene (RogueEssence.Scene.TitleScene)

| Property/Method | Description |
|-----------------|-------------|
| `Instance` | Singleton instance |
| `TitlePhase` | Current title screen phase |

---

## 3. Data Types

### MonsterData (RogueEssence.Data.MonsterData)

| Property | Type | Description |
|----------|------|-------------|
| `Name` | LocalText | Localized species name |
| `Title` | LocalText | Species title/category |
| `Forms` | List<MonsterForm> | Available forms |
| `EvoFrom` | string | Pre-evolution ID |
| `PromoteFrom` | string | Promotion source ID |
| `JoinRate` | int | Base recruitment rate |
| `Comment` | string | Developer comment |

### MonsterForm (RogueEssence.Data.MonsterForm)

| Property | Type | Description |
|----------|------|-------------|
| `FormName` | LocalText | Form name |
| `Element1`, `Element2` | string | Type IDs |
| `Intrinsic1-3` | string | Ability IDs |
| `BaseHP`, `BaseAtk`, etc. | int | Base stats |
| `ExpYield` | int | Experience yield |
| `LevelSkills` | List<LevelUpSkill> | Learnable skills |
| `SharedSkills`, `SecretSkills` | List<LearnableSkill> | Other skills |

### SkillData (RogueEssence.Data.SkillData)

| Property | Type | Description |
|----------|------|-------------|
| `Name` | LocalText | Skill name |
| `Desc` | LocalText | Description |
| `BaseCharges` | int | Max PP |
| `Strikes` | int | Number of hits |
| `Data` | BattleData | Combat parameters |
| `HitRate` | int | Accuracy (0-100, -1 = never miss) |
| `HitboxAction` | CombatAction | Attack pattern |

### ItemData (RogueEssence.Data.ItemData)

| Property | Type | Description |
|----------|------|-------------|
| `Name` | LocalText | Item name |
| `Desc` | LocalText | Description |
| `Icon` | int | Sprite index |
| `Price` | int | Shop price |
| `MaxStack` | int | Max stack size |
| `UsageType` | UseType | How item is used |
| `ItemStates` | StateCollection | Item properties |
| `UseEvent` | BattleData | Use effect data |

### ZoneData (RogueEssence.Data.ZoneData)

| Property | Type | Description |
|----------|------|-------------|
| `Name` | LocalText | Zone/dungeon name |
| `Grounds` | List<GroundSegment> | Ground map segments |
| `Segments` | List<ZoneSegment> | Dungeon segments |
| `Level` | int | Recommended level |
| `LevelCap` | bool | Enforce level cap |
| `BagRestrict` | int | Inventory limit |
| `TeamRestrict` | bool | Solo dungeon |
| `Rescues` | int | Allowed rescue count |
| `Rogue` | RogueStatus | Roguelike type |

---

## 4. Dungeon Classes

### Character (RogueEssence.Dungeon.Character)

| Property/Method | Description |
|-----------------|-------------|
| `CharLoc` | Tile position (Loc) |
| `CharDir` | Facing direction (Dir8) |
| `CurrentForm` | MonsterID appearance |
| `Nickname` | Display name |
| `Level` | Current level |
| `EXP` | Current experience |
| `MaxHP`, `HP` | Health values |
| `BaseAtk`, `Atk` | Attack stat (base/effective) |
| `BaseDef`, `Def` | Defense stat |
| `BaseMAtk`, `MAtk` | Sp. Attack stat |
| `BaseMDef`, `MDef` | Sp. Defense stat |
| `BaseSpeed`, `Speed` | Speed stat |
| `Skills` | BackReference<Skill>[] (4 slots) |
| `EquippedItem` | Held InvItem |
| `StatusEffects` | Dictionary<string, StatusEffect> |
| `Dead` | Whether character is KO'd |
| `MemberTeam` | Owning Team reference |
| `StartEmote(Emote)` | Play emote animation |
| `StartAnim(CharAnimation)` | Play character animation |
| `RefreshTraits()` | Recalculate stats |

### Map (RogueEssence.Dungeon.Map)

| Property/Method | Description |
|-----------------|-------------|
| `Width`, `Height` | Map dimensions in tiles |
| `Tiles[x, y]` | Access Tile at position |
| `Name` | LocalText map name |
| `MapTeams` | List of active teams |
| `AllyTeams` | List of ally teams |
| `Items` | List of MapItem on ground |
| `EntryPoints` | List of LocRay8 entries |
| `Status` | Dictionary<string, MapStatus> |
| `GetCharAtLoc(Loc)` | Get character at tile |
| `GetItem(Loc)` | Get item at tile |
| `TileBlocked(Loc, bool)` | Check if tile impassable |
| `WrapLoc(Loc)` | Wrap coords for looped maps |

### Tile (RogueEssence.Dungeon.Tile)

| Property | Type | Description |
|----------|------|-------------|
| `Data` | TerrainTile | Terrain type |
| `Effect` | EffectTile | Trap/stairs/etc. |
| `ID` | string | Terrain type ID |

### Team (RogueEssence.Dungeon.Team)

| Property/Method | Description |
|-----------------|-------------|
| `Name` | Team display name |
| `Players` | EventedList<Character> members |
| `Guests` | EventedList<Character> guests |
| `Leader` | Team leader Character |
| `LeaderIndex` | Index of leader in Players |
| `MapFaction` | Faction enum value |
| `GetInvCount()` | Inventory count |
| `GetInv(int slot)` | Get InvItem at slot |
| `AddToInv(InvItem)` | Add item to inventory |
| `RemoveFromInv(int)` | Remove item by index |
| `SortItems()` | Sort inventory |
| `EnumerateChars()` | Iterate all members |

### ExplorerTeam : Team

| Property/Method | Description |
|-----------------|-------------|
| `Money` | Current money |
| `Bank` | Stored money |
| `Assembly` | List<Character> in storage |
| `MaxInv` | Max inventory size |
| `GetStorageCount()` | Storage item count |

---

## 5. Ground Classes

### GroundChar (RogueEssence.Ground.GroundChar)

| Property/Method | Description |
|-----------------|-------------|
| `Data` | CharData character data |
| `CurrentForm` | MonsterID appearance |
| `Nickname` | Display name |
| `MapLoc` | Pixel position |
| `CharDir` | Facing direction (Dir8) |
| `Bounds` | Collision Rect |
| `EntEnabled` | Whether visible/active |
| `AIEnabled` | Whether AI runs |
| `StartEmote(Emote)` | Play emote |
| `StartAction(GroundAction)` | Play movement action |
| `GetCurrentAction()` | Get active action |
| `Move(int x, int y, filter)` | Move with collision |
| `GetFront()` | Get front pixel position |

### GroundMap (RogueEssence.Ground.Maps.GroundMap)

| Property/Method | Description |
|-----------------|-------------|
| `Name` | LocalText map name |
| `Width`, `Height` | Map dimensions in tiles |
| `TexWidth`, `TexHeight` | Map dimensions in pixels |
| `Layers` | List<MapLayer> tile layers |
| `Decorations` | List<AnimLayer> animated decor |
| `IterateCharacters()` | Iterate all GroundChars |
| `GetMapChar(string name)` | Get char by instance name |
| `AddMapChar(GroundChar)` | Add character to map |
| `RemoveMapChar(GroundChar)` | Remove character |
| `FindObject(string name)` | Find object by name |
| `FindEntity(string name)` | Find any entity by name |
| `GetLocInDir(Loc, Dir8, int dist)` | Get location in direction |

### GroundObject (RogueEssence.Ground.GroundObject)

| Property/Method | Description |
|-----------------|-------------|
| `EntName` | Instance name |
| `ObjectAnim` | ObjAnimData animation |
| `Bounds` | Collision Rect |
| `EntEnabled` | Whether visible/active |
| `TriggerType` | Trigger interaction type |

---

## 6. Menu Classes

### MenuBase (RogueEssence.Menu.MenuBase)

| Property/Method | Description |
|-----------------|-------------|
| `Bounds` | Rect position and size |
| `Visible` | Whether rendered |
| `Label` | Identifier string |
| `Elements` | List<IMenuElement> contents |
| `Draw(SpriteBatch)` | Render the menu |
| `GetElementIndexByLabel(string)` | Find element by label |

### ChoiceMenu : InteractableMenu

| Property/Method | Description |
|-----------------|-------------|
| `Choices` | List<IChoosable> options |
| `NonChoices` | Non-selectable elements |
| `Hovered` | Currently highlighted choice |
| `GetChoiceIndexByLabel(string)` | Find choice by label |
| `ImportChoices(IChoosable[])` | Replace choices |
| `ExportChoices()` | Clone choices list |

### DialogueBox (RogueEssence.Menu.DialogueBox)

| Property/Method | Description |
|-----------------|-------------|
| `DefaultBounds` | Static default Rect |
| `SOUND_EFFECT` | Default speak sound |
| `SPEAK_FRAMES` | Default char delay |
| `Speaker` | MonsterID of speaker |
| `SpeakerName` | Speaker's name |
| `Message` | Text being displayed |
| `CreateScripts(LuaTable)` | Create callback scripts |

---

## 7. LevelGen Classes

### IGenStep Interface

| Method | Description |
|--------|-------------|
| `CanApply(IGenContext)` | Check if step applies |
| `Apply(IGenContext)` | Execute generation step |

### Common GenStep Types

| Step Class | Description |
|------------|-------------|
| `MobSpawnStep<T>` | Populates enemy spawn table |
| `ItemSpawnStep<T>` | Populates item spawn table |
| `TileSpawnStep<T>` | Populates trap spawn table |
| `MoneySpawnStep<T>` | Configures money spawning |
| `MapTextureStep<T>` | Sets floor tileset |
| `MapNameIDStep<T>` | Sets floor name |
| `DetourStep<T>` | Adds secondary destinations |

### TeamSpawner (RogueEssence.LevelGen.TeamSpawner)

| Property/Method | Description |
|-----------------|-------------|
| `Explorer` | Is ally team (bool) |
| `GetPossibleSpawns()` | Get SpawnList<MobSpawn> |
| `ChooseSpawns(IRandom)` | Select mobs to spawn |
| `Spawn(IMobSpawnMap)` | Create and return Team |
| `Clone()` | Duplicate spawner |

### SpawnList<T>

| Property/Method | Description |
|-----------------|-------------|
| `Count` | Number of entries |
| `SpawnTotal` | Sum of all weights |
| `Add(T, int rate)` | Add spawn with weight |
| `GetSpawn(int index)` | Get spawn at index |
| `GetSpawnRate(int index)` | Get weight at index |
| `Pick(IRandom)` | Randomly select spawn |
| `CanPick` | Has at least one entry |

---

## 8. Lua Bindings

### GAME (ScriptGame)

| Function | Description |
|----------|-------------|
| `Rand` | IRandom instance |
| `GetCurrentGround()` | Get current GroundMap |
| `GetCurrentFloor()` | Get current dungeon Map |
| `GetCurrentDungeon()` | Get current Zone |
| `EnterGroundMap(...)` | Navigate to ground map |
| `EnterDungeon(...)` | Start dungeon run (coroutine) |
| `ContinueDungeon(...)` | Continue dungeon (coroutine) |
| `EndDungeonRun(...)` | End adventure (coroutine) |
| `EnterZone(...)` | Enter zone directly |
| `RestartToTitle()` | Return to title screen |
| `FadeOut(white, duration)` | Fade screen (coroutine) |
| `FadeIn(duration)` | Fade in (coroutine) |
| `GroundSave()` | Save game (coroutine) |
| `GetPlayerPartyCount()` | Get team size |
| `GetPlayerPartyMember(i)` | Get team member |
| `GetPlayerBagCount()` | Get inventory count |
| `GetPlayerBagItem(i)` | Get inventory item |
| `GetPlayerMoney()` | Get current money |

### DUNGEON (ScriptDungeon)

| Function | Description |
|----------|-------------|
| `LastDungeonResult()` | Get ResultType of last run |
| `DungeonCurrentFloor()` | Get current floor number |
| `DungeonDisplayName()` | Get localized dungeon name |
| `SetMinimapVisible(bool)` | Show/hide minimap |
| `CharTurnToChar(ch1, ch2)` | Turn character to face another |
| `CharSetEmote(ch, id, cycles)` | Set character emote |
| `CharStartAnim(ch, anim, loop)` | Start animation (coroutine) |
| `CharEndAnim(ch)` | End animation (coroutine) |
| `CharWaitAnim(ch, anim)` | Play and wait (coroutine) |
| `CharSetAction(ch, anim)` | Set action animation (coroutine) |
| `PlayVFX(emitter, x, y, dir)` | Spawn visual effect |
| `PlayVFXAnim(anim, layer)` | Play BaseAnim |
| `MoveScreen(mover)` | Apply screen shake |
| `AddMapStatus(id)` | Add floor status (coroutine) |
| `RemoveMapStatus(id)` | Remove floor status (coroutine) |

### GROUND (ScriptGround)

| Function | Description |
|----------|-------------|
| `Hide(entityname)` | Hide entity by name |
| `Unhide(entityname)` | Show entity by name |
| `RemoveObject(name)` | Delete object |
| `RemoveCharacter(name)` | Delete character |
| `CreateCharacterFromCharData(...)` | Spawn GroundChar |
| `RefreshPlayer()` | Reload player character |
| `SetPlayer(CharData)` | Set player character |
| `SpawnerDoSpawn(name)` | Trigger spawner |
| `CharTurnToChar(ch1, ch2)` | Face character toward another |
| `CharSetEmote(ch, id, cycles)` | Set emote |
| `CharAnimateTurn(ch, dir, time)` | Animated turn (coroutine) |
| `CharAnimateTurnTo(ch1, ch2, time)` | Turn to face (coroutine) |
| `MoveInDirection(ch, dir, dist)` | Walk in direction (coroutine) |
| `MoveToPosition(ch, x, y)` | Walk to position (coroutine) |
| `AnimateJump(ch, height, dur)` | Jump in place (coroutine) |
| `MoveCamera(x, y, dur, toPlayer)` | Pan camera (coroutine) |
| `TeleportTo(ch, x, y, dir)` | Instantly move character |

### UI (ScriptUI)

| Function | Description |
|----------|-------------|
| `SetSpeaker(MonsterID)` | Set dialogue speaker |
| `SetSpeakerName(string)` | Set speaker name |
| `SetSpeakerEmotion(EmoteStyle)` | Set portrait emotion |
| `SetSpeakerLoc(Loc)` | Set portrait position |
| `SetBounds(Rect)` | Set text box bounds |
| `ResetSpeaker()` | Reset to defaults |
| `WaitShowDialogue(text)` | Show dialogue (coroutine) |
| `WaitShowTimedDialogue(text, time)` | Timed dialogue (coroutine) |
| `WaitShowVoiceOver(text, ...)` | Voice over box (coroutine) |
| `WaitShowTitle(text, time)` | Show title text (coroutine) |
| `TextPopUp(text, time, ...)` | Show popup text |
| `WaitInput(anyInput)` | Wait for button (coroutine) |
| `ChoiceMenuYesNo(question)` | Yes/No choice (coroutine) |
| `ShowMonologue(speaker, text)` | Monologue box (coroutine) |

### SOUND (ScriptSound)

| Function | Description |
|----------|-------------|
| `PlaySE(name)` | Play sound effect |
| `PlayBattleSE(name)` | Play battle sound |
| `LoopSE(name)` | Loop sound effect |
| `StopSE(name)` | Stop looping SE |
| `FadeInSE(name, time)` | Fade in SE |
| `FadeOutSE(name, time)` | Fade out SE |
| `WaitSE()` | Wait for SE (coroutine) |
| `PlayBGM(name, fade)` | Play background music |
| `StopBGM()` | Stop BGM |
| `FadeOutBGM(time)` | Fade out BGM |
| `PlayFanfare(name)` | Play victory fanfare |

---

## 9. Common Enums

### Dir8 (RogueElements)

```csharp
None = -1, Down = 0, DownLeft = 1, Left = 2, UpLeft = 3,
Up = 4, UpRight = 5, Right = 6, DownRight = 7
```

### Dir4 (RogueElements)

```csharp
None = -1, Down = 0, Left = 1, Up = 2, Right = 3
```

### Faction (RogueEssence.Dungeon)

```csharp
None = -1, Player = 0, Friend = 1, Foe = 2
```

### BattleActionType (RogueEssence.Dungeon)

```csharp
None, Skill, Item, Throw, Trap
```

### GameAction.ActionType (RogueEssence.Dungeon)

```csharp
None = -1, Dir = 0, Move, Attack, Pickup, Tile, UseItem,
Give, Take, Drop, Throw, UseSkill, Wait, TeamMode,
ShiftTeam, SetLeader, SendHome, Tactics, ShiftSkill,
SetSkill, SortItems, GiveUp, Rescue, Option
```

### GameProgress.ResultType (RogueEssence.Data)

```csharp
Unknown, Downed, Failed, TimedOut, Cleared, Escaped, Rescued, GaveUp
```

### GameProgress.DungeonStakes (RogueEssence.Data)

```csharp
None, Risk, Loss
```

### DataManager.DataType (RogueEssence.Data)

```csharp
Monster, Skill, Item, Intrinsic, Status, MapStatus,
Terrain, Tile, Zone, Emote, Element, AI, Rank, Skin, AutoTile
```

### Gender (RogueEssence.Data)

```csharp
Unknown = -1, Genderless = 0, Male = 1, Female = 2
```

### Stat (RogueEssence.Data)

```csharp
None = -1, HP = 0, Attack, Defense, MAtk, MDef, Speed, HitRate, DodgeRate
```

### DrawLayer (RogueEssence.Content)

```csharp
Bottom, Back, Normal, Front, Top, NoDraw
```

### Map.SightRange (RogueEssence.Dungeon)

```csharp
Any = 0, Dark = 1, Murky = 2, Clear = 3
```

### ItemData.UseType (RogueEssence.Data)

```csharp
None, Use, UseOther, Throw, Learn, Box, Treasure
```

---

## 10. Utility Classes

### Loc (RogueElements)

| Property/Method | Description |
|-----------------|-------------|
| `X`, `Y` | Coordinates |
| `Zero`, `One` | Static constants |
| `UnitX`, `UnitY` | Unit vectors |
| Operators | `+`, `-`, `*`, `/`, `==`, `!=` |
| `Dist8(Loc)` | Chebyshev distance |
| `DistSquared(Loc)` | Euclidean dist squared |
| `ApproximateDir8()` | Get Dir8 from vector |

### Rect (RogueElements)

| Property/Method | Description |
|-----------------|-------------|
| `X`, `Y` | Position |
| `Width`, `Height` | Size |
| `Start`, `End` | Corner Loc values |
| `Left`, `Right`, `Top`, `Bottom` | Edge values |
| `Center` | Center Loc |
| `Area`, `Perimeter` | Calculated values |
| `Contains(Loc)` | Point inside rect |
| `Contains(Rect)` | Rect inside rect |
| `Intersects(Rect)` | Rects overlap |

### FrameTick (RogueEssence)

| Property/Method | Description |
|-----------------|-------------|
| `Ticks` | Raw tick count (120/frame) |
| `Zero` | Static zero value |
| `FromFrames(long)` | Create from frame count |
| `ToFrames()` | Convert to frames |
| `FractionOf(int time)` | Get progress fraction |
| `DivOf(long time)` | Get divisions elapsed |
| Operators | `+`, `-`, `*`, `/`, `%`, comparisons |

### MonsterID (RogueEssence.Dungeon)

| Property | Description |
|----------|-------------|
| `Species` | Monster species ID (string) |
| `Form` | Form index (int) |
| `Skin` | Skin ID (string) |
| `Gender` | Gender enum |
| `Invalid` | Static invalid instance |
| `ToCharID()` | Convert to CharID for sprites |

### ZoneLoc (RogueEssence.Dungeon)

| Property | Description |
|----------|-------------|
| `ID` | Zone ID (string) |
| `StructID` | SegLoc (segment + map) |
| `EntryPoint` | Entry point index |

### SegLoc (RogueEssence.Dungeon)

| Property | Description |
|----------|-------------|
| `Segment` | Segment index (-1 = ground) |
| `ID` | Map index within segment |

### InvItem (RogueEssence.Dungeon)

| Property | Description |
|----------|-------------|
| `ID` | Item data ID (string) |
| `Amount` | Stack count |
| `Cursed` | Is cursed |
| `Price` | Shop price (0 = not for sale) |
| `HiddenValue` | Internal state |
| `GetDisplayName()` | Get formatted name |

### CharIndex (RogueEssence.Dungeon)

| Property | Description |
|----------|-------------|
| `Faction` | Faction enum |
| `Team` | Team index |
| `Guest` | Is guest member |
| `Char` | Character index in team |
| `Invalid` | Static invalid instance |

---

## Quick Tips

1. **Accessing current game state:**
   ```csharp
   var save = DataManager.Instance.Save;
   var team = save.ActiveTeam;
   var leader = team.Leader;
   var map = ZoneManager.Instance.CurrentMap; // dungeon
   var ground = ZoneManager.Instance.CurrentGround; // ground mode
   ```

2. **Spawning VFX in dungeons:**
   ```lua
   DUNGEON:PlayVFX(emitter, x * 24, y * 24, RogueElements.Dir8.Down)
   ```

3. **Showing dialogue in Lua:**
   ```lua
   UI:SetSpeaker(speaker_id)
   UI:WaitShowDialogue("Hello, world!")
   ```

4. **Getting data by ID:**
   ```csharp
   var monster = DataManager.Instance.GetMonster("bulbasaur");
   var skill = DataManager.Instance.GetSkill("tackle");
   ```

5. **Tile coordinates vs pixel coordinates:**
   - Dungeon uses tile coordinates (1 tile = 24 pixels)
   - Ground mode uses pixel coordinates directly
