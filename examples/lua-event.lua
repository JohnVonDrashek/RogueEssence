-- =============================================================================
-- EXAMPLE: Lua Zone/Map Script Events
-- =============================================================================
-- This file demonstrates how to create Lua scripts for zone and map events
-- in RogueEssence. Scripts can handle floor initialization, NPC interactions,
-- tile triggers, and more.
--
-- Zone scripts go in: Content/Zones/[zone_name]/Script/
-- Ground map scripts go in: Content/Ground/[map_name]/Script/
--
-- The script file should match the zone/map name with "_" prefix variations
-- =============================================================================

-- =============================================================================
-- SCRIPT ORGANIZATION
-- =============================================================================
-- Scripts are organized into tables. Create a table for your zone/map and
-- define callback functions within it.

-- Create the script table for this zone
-- The name should match your zone's internal ID
local ExampleZone = {}

-- =============================================================================
-- ZONE LIFECYCLE EVENTS
-- =============================================================================
-- These events trigger at various points during zone/dungeon progression.

--- Called when the zone is first entered (before floor generation)
-- @param zone The ZoneSegment being entered
-- @param rescuing Boolean indicating if this is a rescue operation
-- @param segmentID The segment ID within the zone
-- @param seed The random seed for this zone run
function ExampleZone.OnZoneEnter(zone, rescuing, segmentID, seed)
    -- Log for debugging
    PrintInfo("Entering zone with seed: " .. seed)

    -- Initialize zone-specific variables
    -- SV (Script Variables) persist across floors within a dungeon run
    SV.ExampleZone = SV.ExampleZone or {}
    SV.ExampleZone.FloorsCleared = 0
    SV.ExampleZone.BossDefeated = false
end

--- Called when exiting the zone
-- @param zone The ZoneSegment being exited
-- @param result The exit result (success, failure, etc.)
-- @param rescue Boolean indicating if this was a rescue
-- @param segmentID The segment ID
-- @param mapID The map ID being exited from
function ExampleZone.OnZoneExit(zone, result, rescue, segmentID, mapID)
    PrintInfo("Exiting zone with result: " .. tostring(result))

    -- Clean up zone-specific data if needed
    SV.ExampleZone = nil
end

-- =============================================================================
-- FLOOR/MAP EVENTS
-- =============================================================================
-- These events trigger during floor-specific gameplay.

--- Called when a floor is entered (after generation)
-- @param map The current Map object
-- @param entryPoint The entry point type (how player entered)
function ExampleZone.OnFloorEnter(map, entryPoint)
    local floorNum = _ZONE.CurrentMapID.Segment
    PrintInfo("Entered floor " .. floorNum)

    -- Track floor progression
    if SV.ExampleZone then
        SV.ExampleZone.CurrentFloor = floorNum
    end

    -- Example: Show a message on specific floors
    if floorNum == 5 then
        -- Use TASK for operations that need coroutine yielding
        TASK:WaitTask(_DUNGEON:MessageBox("You feel a strange presence..."))
    end
end

--- Called when the map's turn begins (after all characters have acted)
-- @param map The current Map
function ExampleZone.OnMapTurnEnd(map)
    -- Example: Check for special conditions each turn
    local player = _DUNGEON:GetPlayerLeader()

    if player then
        -- Check HP threshold
        if player.HP < player.MaxHP / 4 then
            -- Low HP warning (only show once per floor)
            if not SV.ExampleZone.LowHPWarned then
                TASK:WaitTask(_DUNGEON:MessageBox("Your HP is critical!"))
                SV.ExampleZone.LowHPWarned = true
            end
        end
    end
end

-- =============================================================================
-- CHARACTER EVENTS
-- =============================================================================
-- Events related to character actions and interactions.

--- Called when the player uses the stairs
-- @param map The current Map
-- @param entryPoint Where the player is going
function ExampleZone.OnStairsUsed(map, entryPoint)
    -- Track floors cleared
    if SV.ExampleZone then
        SV.ExampleZone.FloorsCleared = SV.ExampleZone.FloorsCleared + 1
    end

    PrintInfo("Stairs used. Total floors cleared: " .. SV.ExampleZone.FloorsCleared)
end

--- Called when a character is defeated
-- @param map The current Map
-- @param defeatedChar The character that was defeated
-- @param attackerChar The character that defeated them (may be nil)
function ExampleZone.OnCharacterDeath(map, defeatedChar, attackerChar)
    -- Check if this is a boss
    if defeatedChar and defeatedChar.CharacterName == "BossMonster" then
        SV.ExampleZone.BossDefeated = true

        -- Play victory fanfare
        SOUND:PlayBGM("Fanfare", true)
        TASK:WaitTask(_DUNGEON:MessageBox("You defeated the boss!"))
        SOUND:PlayBGM(ZoneManager.Instance.CurrentMap.Music, true)
    end
end

-- =============================================================================
-- TILE TRIGGER EVENTS
-- =============================================================================
-- Events triggered by stepping on specific tiles.

--- Called when a character steps on a tile with a trigger
-- @param map The current Map
-- @param char The character stepping on the tile
-- @param tile The tile being stepped on
function ExampleZone.OnTileTriggered(map, char, tile)
    -- Check if this is the player
    if not char or char.MemberTeam ~= _DUNGEON:GetPlayerTeam() then
        return
    end

    -- Handle specific tile types by ID
    local tileID = tile.Effect.ID

    if tileID == "example_teleport_trap" then
        -- Custom teleport handling
        TASK:WaitTask(CustomTeleportPlayer(char))

    elseif tileID == "example_treasure_tile" then
        -- Give treasure
        TASK:WaitTask(GiveTreasure(char))
    end
end

-- Helper function for custom teleport
function CustomTeleportPlayer(char)
    return function()
        -- Find a random valid tile
        local map = _ZONE.CurrentMap
        local attempts = 0
        local destLoc = nil

        while attempts < 100 and not destLoc do
            local testX = math.random(0, map.Width - 1)
            local testY = math.random(0, map.Height - 1)
            local testLoc = RogueElements.Loc(testX, testY)

            if not map:TileBlocked(testLoc, char.Mobility) then
                destLoc = testLoc
            end
            attempts = attempts + 1
        end

        if destLoc then
            -- Animate and move
            SOUND:PlayBattleSE("DUN_Warp")
            coroutine.yield(TASK:WaitTask(_DUNGEON:CharWarp(char, destLoc)))
        end
    end
end

-- Helper function for treasure reward
function GiveTreasure(char)
    return function()
        -- Create and give an item
        local item = RogueEssence.Dungeon.InvItem("reviver_seed")
        TASK:WaitTask(_DUNGEON:MessageBox("You found a hidden treasure!"))
        coroutine.yield(TASK:WaitTask(_DUNGEON:GiveItem(char, item)))
    end
end

-- =============================================================================
-- ITEM EVENTS
-- =============================================================================
-- Events related to items.

--- Called when an item is picked up
-- @param map The current Map
-- @param char The character picking up
-- @param item The item being picked up
function ExampleZone.OnItemPickup(map, char, item)
    -- Check for special items
    if item.ID == "example_key_item" then
        SV.ExampleZone.HasKeyItem = true
        TASK:WaitTask(_DUNGEON:MessageBox("You got the Key Item!"))
    end
end

-- =============================================================================
-- UTILITY FUNCTIONS
-- =============================================================================
-- Helpful utility functions for common operations.

--- Get all characters in a radius around a point
-- @param centerLoc The center location
-- @param radius The search radius
-- @return Table of characters within range
function GetCharactersInRadius(centerLoc, radius)
    local chars = {}
    local map = _ZONE.CurrentMap

    for x = centerLoc.X - radius, centerLoc.X + radius do
        for y = centerLoc.Y - radius, centerLoc.Y + radius do
            local loc = RogueElements.Loc(x, y)
            local char = map:GetCharAtLoc(loc)
            if char then
                table.insert(chars, char)
            end
        end
    end

    return chars
end

--- Check if player has a specific item
-- @param itemID The item ID to check for
-- @return Boolean indicating if item is in inventory
function PlayerHasItem(itemID)
    local team = _DATA.Save.ActiveTeam

    -- Check inventory
    for i = 0, team:GetInvCount() - 1 do
        local item = team:GetInv(i)
        if item.ID == itemID then
            return true
        end
    end

    -- Check held items
    for i = 0, team.Players.Count - 1 do
        local member = team.Players[i]
        if member.EquippedItem.ID == itemID then
            return true
        end
    end

    return false
end

--- Show a timed message that auto-closes
-- @param message The message text
-- @param frames How many frames to show (60 = 1 second)
function ShowTimedMessage(message, frames)
    UI:SetAutoFinish(true)
    TASK:WaitTask(UI:WaitShowTimedDialogue(message, frames))
    UI:SetAutoFinish(false)
end

-- =============================================================================
-- GLOBAL REFERENCES
-- =============================================================================
-- These are the main global objects available in scripts:
--
-- _ZONE    - ZoneManager.Instance (current zone/dungeon management)
-- _DATA    - DataManager.Instance (game data access)
-- _GAME    - GameManager.Instance (game state management)
--
-- TASK     - Task scheduling for coroutines
-- UI       - User interface (dialogues, menus)
-- SOUND    - Sound effects and music
-- STRINGS  - Localized strings
-- AI       - AI control functions
-- SV       - Script Variables (persisted with save)
--
-- _DUNGEON - DungeonScene.Instance (dungeon-specific operations)
-- _GROUND  - GroundScene.Instance (ground map operations)

-- =============================================================================
-- COMMON PATTERNS
-- =============================================================================

-- Pattern: Running a coroutine sequence
function ExampleSequence()
    return function()
        -- Step 1
        TASK:WaitTask(_DUNGEON:MessageBox("Step 1"))

        -- Wait between steps
        coroutine.yield(TASK:WaitFrames(30))

        -- Step 2
        TASK:WaitTask(_DUNGEON:MessageBox("Step 2"))

        -- Step 3
        TASK:WaitTask(_DUNGEON:MessageBox("Step 3"))
    end
end

-- Pattern: Yes/No question
function AskQuestion(question)
    return function()
        UI:ChoiceMenuYesNo(question, false)
        UI:WaitForChoice()
        local result = UI:ChoiceResult()
        return result -- true for Yes, false for No
    end
end

-- Pattern: Multi-choice menu
function ShowMultiChoice(message, choices)
    return function()
        -- choices is a table: { "Choice 1", "Choice 2", "Choice 3" }
        local choiceTable = {}
        for i, choice in ipairs(choices) do
            choiceTable[i] = choice
        end

        UI:BeginChoiceMenu(message, choiceTable, 1, #choices)
        UI:WaitForChoice()
        return UI:ChoiceResult() -- Returns the choice index (1-based)
    end
end

-- =============================================================================
-- EXPORT THE SCRIPT TABLE
-- =============================================================================
-- Return the script table so the engine can find the functions

return ExampleZone
