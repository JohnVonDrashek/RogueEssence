# Lua

Lua scripting engine integration using MoonSharp. Provides scripting support for cutscenes, events, AI, custom game logic, and mod extensibility.

## Key Files

| File | Description |
|------|-------------|
| `LuaEngine.cs` | Main Lua engine singleton - initializes MoonSharp, loads scripts, exposes APIs |
| `ScriptGame.cs` | Game-related script functions (save, load, transitions) |
| `ScriptGround.cs` | Ground scene script functions (NPC control, movement) |
| `ScriptDungeon.cs` | Dungeon scene script functions (spawn, map manipulation) |
| `ScriptUI.cs` | UI script functions (menus, dialogue, text) |
| `ScriptSound.cs` | Sound script functions (music, SFX, fades) |
| `ScriptStrings.cs` | String and localization script functions |
| `ScriptServices.cs` | Utility services exposed to Lua |
| `ScriptEvent.cs` | Event system for Lua callbacks |
| `ScriptTask.cs` | Task-based async operations in Lua |
| `ScriptAI.cs` | AI behavior scripting |
| `ScriptXML.cs` | XML parsing utilities for scripts |
| `LuaCoroutineIterator.cs` | Coroutine support for async Lua code |
| `LuaCoroutineWrap.cs` | Wrapper for Lua coroutines |
| `EntityWithLuaData.cs` | Entity base class with Lua data storage |
| `TemplateManager.cs` | Template system for reusable script patterns |
| `TriggerResult.cs` | Result type for triggered events |

## Relationships

- **GameBase** initializes LuaEngine during startup
- All game modules expose APIs through Script* classes
- **Ground/** and **Dungeon/** scenes call Lua event handlers
- **Data/AI/ScriptPlan** uses Lua for custom AI
- Mod scripts loaded from mod directories

## Usage

```lua
-- Example Lua script
function OnMapInit(map)
    GAME:FadeIn(20)
    UI:WaitShowDialogue("Welcome to the dungeon!")
end

-- Control ground NPCs
function OnTalk(npc)
    GROUND:CharTurnToChar(npc, CH("PLAYER"))
    UI:WaitShowDialogue("Hello, adventurer!")
end

-- Custom dungeon event
function OnPickupItem(owner, item)
    if item.ID == "rare_gem" then
        SOUND:PlaySE("Fanfare")
    end
end
```
