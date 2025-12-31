-- =============================================================================
-- EXAMPLE: Lua Cutscene with Dialogue
-- =============================================================================
-- This file demonstrates how to create cutscenes in RogueEssence using Lua.
-- Cutscenes can include dialogue, character movement, camera control,
-- animations, sound effects, and player choices.
--
-- Cutscenes are typically triggered from:
-- - Ground map NPC interactions
-- - Zone events (entering a floor, etc.)
-- - Story progression triggers
-- =============================================================================

-- Create the cutscene table
local ExampleCutscenes = {}

-- =============================================================================
-- BASIC DIALOGUE CUTSCENE
-- =============================================================================
--- A simple dialogue sequence with one speaker.
-- Demonstrates basic dialogue box usage.
function ExampleCutscenes.SimpleDialogue()
    return function()
        -- =====================================================================
        -- STEP 1: Set up the speaker
        -- =====================================================================
        -- SetSpeaker configures who is talking.
        -- Parameters: (name, key_sound, species_id, form, skin, gender)

        UI:SetSpeaker("Village Elder", true, "npc_elder", 0, "", RogueEssence.Data.Gender.Male)

        -- Set speaker emotion (optional)
        -- Emotions: Normal, Happy, Pain, Angry, Worried, Sad, Crying, Shouting,
        --           Teary-Eyed, Determined, Joyous, Inspired, Surprised, Dizzy,
        --           Special0, Special1, Special2, Special3
        UI:SetSpeakerEmotion("Normal")

        -- =====================================================================
        -- STEP 2: Show dialogue boxes
        -- =====================================================================
        -- WaitShowDialogue shows a textbox and waits for player input.
        -- Use [pause] for dramatic pauses within text.

        TASK:WaitTask(UI:WaitShowDialogue("Welcome, young adventurer."))

        TASK:WaitTask(UI:WaitShowDialogue("I have been expecting you.[pause=30] There is a task I need your help with."))

        -- Change emotion for different lines
        UI:SetSpeakerEmotion("Worried")
        TASK:WaitTask(UI:WaitShowDialogue("The ancient dungeon to the north has become dangerous once again..."))

        UI:SetSpeakerEmotion("Determined")
        TASK:WaitTask(UI:WaitShowDialogue("Will you investigate it for us?"))

        -- =====================================================================
        -- STEP 3: Clean up speaker
        -- =====================================================================
        -- ResetSpeaker clears the speaker info (no portrait next time)
        UI:ResetSpeaker()
    end
end

-- =============================================================================
-- DIALOGUE WITH PLAYER CHOICE
-- =============================================================================
--- A dialogue that asks the player a question and responds based on answer.
function ExampleCutscenes.DialogueWithChoice()
    return function()
        -- Set up the speaker
        UI:SetSpeaker("Merchant", true, "npc_merchant", 0, "", RogueEssence.Data.Gender.Female)
        UI:SetSpeakerEmotion("Happy")

        -- Introduction
        TASK:WaitTask(UI:WaitShowDialogue("Welcome to my shop!"))
        TASK:WaitTask(UI:WaitShowDialogue("Would you like to see my wares?"))

        -- =====================================================================
        -- YES/NO CHOICE
        -- =====================================================================
        -- ChoiceMenuYesNo creates a Yes/No dialogue choice.
        -- Parameters: (message, default_to_no)

        UI:ChoiceMenuYesNo("Would you like to browse?", false)
        UI:WaitForChoice()

        local choice = UI:ChoiceResult()  -- true = Yes, false = No

        if choice then
            -- Player chose Yes
            UI:SetSpeakerEmotion("Joyous")
            TASK:WaitTask(UI:WaitShowDialogue("Wonderful! Take your time browsing."))

            -- You could open a shop menu here
            -- UI:ShopMenu(goodsTable)
            -- UI:WaitForChoice()
        else
            -- Player chose No
            UI:SetSpeakerEmotion("Sad")
            TASK:WaitTask(UI:WaitShowDialogue("Oh, that's too bad..."))
            TASK:WaitTask(UI:WaitShowDialogue("Come back anytime if you change your mind!"))
        end

        UI:ResetSpeaker()
    end
end

-- =============================================================================
-- MULTI-CHOICE DIALOGUE
-- =============================================================================
--- A dialogue with multiple choices (more than Yes/No).
function ExampleCutscenes.MultipleChoiceDialogue()
    return function()
        UI:SetSpeaker("Guide", true, "npc_guide", 0, "", RogueEssence.Data.Gender.Unknown)

        TASK:WaitTask(UI:WaitShowDialogue("Where would you like to go?"))

        -- =====================================================================
        -- MULTI-CHOICE MENU
        -- =====================================================================
        -- BeginChoiceMenu creates a menu with custom choices.
        -- Parameters: (message, choices_table, default_choice, cancel_choice)

        local choices = {
            [1] = "The Forest",
            [2] = "The Mountain",
            [3] = "The Beach",
            [4] = "Nevermind"
        }

        UI:BeginChoiceMenu("Select a destination:", choices, 1, 4)
        UI:WaitForChoice()

        local choice = UI:ChoiceResult()

        if choice == 1 then
            TASK:WaitTask(UI:WaitShowDialogue("The forest is peaceful this time of year."))
            -- Could warp player here
        elseif choice == 2 then
            TASK:WaitTask(UI:WaitShowDialogue("The mountain path is treacherous. Be careful!"))
        elseif choice == 3 then
            TASK:WaitTask(UI:WaitShowDialogue("The beach is lovely. Enjoy your swim!"))
        else -- choice == 4 or cancelled
            TASK:WaitTask(UI:WaitShowDialogue("Take your time deciding."))
        end

        UI:ResetSpeaker()
    end
end

-- =============================================================================
-- CONVERSATION BETWEEN MULTIPLE CHARACTERS
-- =============================================================================
--- A cutscene with dialogue between two characters.
function ExampleCutscenes.TwoCharacterConversation()
    return function()
        -- Get references to characters on the ground map
        -- (These would be spawned NPCs on the map)
        local hero = CH("Hero")      -- Player character
        local rival = CH("Rival")    -- NPC named "Rival"

        -- =====================================================================
        -- CHARACTER MOVEMENT
        -- =====================================================================
        -- Make characters face each other

        if hero and rival then
            -- Calculate direction from hero to rival
            GROUND:CharTurnToChar(hero, rival)
            GROUND:CharTurnToChar(rival, hero)

            -- Wait for turn animation
            coroutine.yield(TASK:WaitFrames(20))
        end

        -- =====================================================================
        -- ALTERNATING DIALOGUE
        -- =====================================================================

        -- Hero speaks
        UI:SetSpeaker(hero)  -- Can pass character object directly
        UI:SetSpeakerEmotion("Surprised")
        TASK:WaitTask(UI:WaitShowDialogue("You! What are you doing here?"))

        -- Rival speaks
        UI:SetSpeaker(rival)
        UI:SetSpeakerEmotion("Determined")
        TASK:WaitTask(UI:WaitShowDialogue("I could ask you the same thing."))

        -- Continue conversation
        UI:SetSpeaker(hero)
        UI:SetSpeakerEmotion("Angry")
        TASK:WaitTask(UI:WaitShowDialogue("I won't let you get in my way!"))

        UI:SetSpeaker(rival)
        UI:SetSpeakerEmotion("Happy")
        TASK:WaitTask(UI:WaitShowDialogue("We'll see about that. May the best explorer win!"))

        -- =====================================================================
        -- CHARACTER EXIT
        -- =====================================================================
        -- Make the rival walk away

        if rival then
            -- Turn and walk
            GROUND:CharTurnToCharAnimated(rival, hero, 4)

            -- Move to a position (animate walk)
            -- CharAnimateTurnTo(char, direction, frame_duration)
            GROUND:CharAnimateTurnTo(rival, RogueElements.Dir8.Down, 4)

            -- Walk animation to position
            local exitPos = RogueElements.Loc(rival.Position.X, rival.Position.Y + 48)
            coroutine.yield(TASK:WaitTask(GROUND:CharMoveToTask(rival, exitPos, false, 2)))

            -- Remove or hide the character
            GROUND:RemoveCharacter(rival.EntName)
        end

        UI:ResetSpeaker()
    end
end

-- =============================================================================
-- CUTSCENE WITH EFFECTS
-- =============================================================================
--- A dramatic cutscene with screen effects, sounds, and animations.
function ExampleCutscenes.DramaticCutscene()
    return function()
        -- =====================================================================
        -- SCREEN EFFECTS
        -- =====================================================================

        -- Fade to black
        GAME:FadeOut(false, 30)  -- (white_fade, frame_duration)
        coroutine.yield(TASK:WaitFrames(30))

        -- Show a title/location text
        TASK:WaitTask(UI:WaitShowTitle("The Ancient Ruins", 60))
        coroutine.yield(TASK:WaitFrames(90))
        TASK:WaitTask(UI:WaitHideTitle(60))

        -- Fade back in
        GAME:FadeIn(30)
        coroutine.yield(TASK:WaitFrames(30))

        -- =====================================================================
        -- BACKGROUND IMAGE
        -- =====================================================================

        -- Show a background image (for CG scenes)
        -- Images should be in Content/BG/
        TASK:WaitTask(UI:WaitShowBG("ruins_interior", 3, 30))

        -- =====================================================================
        -- VOICE OVER (No speaker portrait)
        -- =====================================================================

        UI:ResetSpeaker(false)  -- Clear speaker but keep sound
        UI:SetCenter(true)      -- Center the text

        TASK:WaitTask(UI:WaitShowVoiceOver("Long ago, this place was home to a great civilization...", -1))

        TASK:WaitTask(UI:WaitShowVoiceOver("But now, only ruins remain.", -1))

        UI:SetCenter(false)

        -- =====================================================================
        -- SOUND EFFECTS AND MUSIC
        -- =====================================================================

        -- Play a sound effect
        SOUND:PlaySE("EVT_Rumble")
        coroutine.yield(TASK:WaitFrames(30))

        -- Change background music
        SOUND:PlayBGM("Mystery", true)  -- (track_name, fade_previous)

        -- =====================================================================
        -- SCREEN SHAKE
        -- =====================================================================

        -- Screen shake for drama
        GAME:MoveCamera(5, 5, 5, false)
        coroutine.yield(TASK:WaitFrames(5))
        GAME:MoveCamera(-5, -5, 5, false)
        coroutine.yield(TASK:WaitFrames(5))
        GAME:MoveCamera(0, 0, 5, false)

        -- Voice over continues
        UI:SetCenter(true)
        TASK:WaitTask(UI:WaitShowVoiceOver("What secrets lie within?", -1))
        UI:SetCenter(false)

        -- =====================================================================
        -- CLEANUP
        -- =====================================================================

        -- Hide background
        TASK:WaitTask(UI:WaitHideBG(30))

        -- Reset speaker settings
        UI:ResetSpeaker()
    end
end

-- =============================================================================
-- DUNGEON CUTSCENE (During dungeon exploration)
-- =============================================================================
--- A cutscene that plays during dungeon gameplay.
function ExampleCutscenes.DungeonEventCutscene()
    return function()
        -- Get the player leader
        local player = _DUNGEON:GetPlayerLeader()

        if not player then
            return
        end

        -- =====================================================================
        -- PAUSE GAMEPLAY
        -- =====================================================================

        -- The cutscene automatically pauses normal gameplay

        -- =====================================================================
        -- PLAYER REACTION
        -- =====================================================================

        UI:SetSpeaker(player)
        UI:SetSpeakerEmotion("Surprised")
        TASK:WaitTask(UI:WaitShowDialogue("What was that sound?!"))

        -- Play ambient sound
        SOUND:PlaySE("EVT_Footsteps")
        coroutine.yield(TASK:WaitFrames(30))

        -- =====================================================================
        -- SPAWN AN ENEMY FOR DRAMATIC EFFECT
        -- =====================================================================

        -- Find a location near the player
        local spawnLoc = RogueElements.Loc(player.CharLoc.X + 3, player.CharLoc.Y)

        -- Check if location is valid
        local map = _ZONE.CurrentMap
        if not map:TileBlocked(spawnLoc, player.Mobility) then
            -- Create and spawn an enemy
            local enemyData = RogueEssence.Dungeon.CharData()
            enemyData.BaseForm = RogueEssence.Dungeon.MonsterID("ghost_type", 0, "", RogueEssence.Data.Gender.Genderless)
            enemyData.Level = 20

            -- Spawn with animation
            SOUND:PlayBattleSE("DUN_Spawn")
            -- Note: Actual spawn code depends on context
        end

        UI:SetSpeakerEmotion("Determined")
        TASK:WaitTask(UI:WaitShowDialogue("Here it comes!"))

        UI:ResetSpeaker()

        -- Gameplay resumes automatically after cutscene ends
    end
end

-- =============================================================================
-- ANIMATED CHARACTER MOVEMENT CUTSCENE
-- =============================================================================
--- Shows how to animate character movement paths.
function ExampleCutscenes.CharacterMovementDemo()
    return function()
        local npc = CH("WanderingNPC")

        if not npc then
            PrintInfo("NPC not found!")
            return
        end

        -- =====================================================================
        -- WALK TO POSITION
        -- =====================================================================

        -- Define a path of positions
        local path = {
            RogueElements.Loc(100, 100),
            RogueElements.Loc(150, 100),
            RogueElements.Loc(150, 150),
            RogueElements.Loc(100, 150),
        }

        -- Walk along the path
        for _, pos in ipairs(path) do
            -- Turn toward destination first
            GROUND:CharTurnToLoc(npc, pos)
            coroutine.yield(TASK:WaitFrames(10))

            -- Walk to position
            -- Parameters: (character, destination, run, speed)
            coroutine.yield(TASK:WaitTask(GROUND:CharMoveToTask(npc, pos, false, 2)))

            -- Small pause at each point
            coroutine.yield(TASK:WaitFrames(30))
        end

        -- =====================================================================
        -- PLAY ANIMATION
        -- =====================================================================

        -- Play a specific character animation
        -- Animation names: Walk, Attack, Hurt, Sleep, etc.
        GROUND:CharSetAnim(npc, "Hurt", true)
        coroutine.yield(TASK:WaitFrames(60))
        GROUND:CharSetAnim(npc, "None", true)  -- Return to idle

        -- =====================================================================
        -- EMOTE
        -- =====================================================================

        -- Show an emote above the character
        -- Emote IDs match emotion names
        GROUND:CharSetEmote(npc, "notice", 2)  -- (char, emote, duration_seconds)
        coroutine.yield(TASK:WaitFrames(120))
    end
end

-- =============================================================================
-- NAME INPUT CUTSCENE
-- =============================================================================
--- Shows how to get text input from the player.
function ExampleCutscenes.NameInputExample()
    return function()
        UI:SetSpeaker("Registrar", true, "npc_registrar", 0, "", RogueEssence.Data.Gender.Male)

        TASK:WaitTask(UI:WaitShowDialogue("Welcome! I'll need to register your name."))

        -- =====================================================================
        -- TEXT INPUT
        -- =====================================================================

        -- NameMenu creates a text input dialog
        -- Parameters: (title, description, max_length, default_text)
        UI:NameMenu("Enter your name:", "This will be recorded.", 10, "")
        UI:WaitForChoice()

        local enteredName = UI:ChoiceResult()

        if enteredName and #enteredName > 0 then
            TASK:WaitTask(UI:WaitShowDialogue("Welcome, " .. enteredName .. "!"))

            -- Store the name
            SV.PlayerCustomName = enteredName
        else
            TASK:WaitTask(UI:WaitShowDialogue("No name entered. I'll call you 'Adventurer'."))
            SV.PlayerCustomName = "Adventurer"
        end

        UI:ResetSpeaker()
    end
end

-- =============================================================================
-- HELPER FUNCTION: Get character reference
-- =============================================================================
--- Gets a character from the current ground map by name.
-- @param name The entity name of the character
-- @return The GroundChar object or nil
function CH(name)
    if _GROUND then
        return _GROUND:GetCharacter(name)
    end
    return nil
end

-- =============================================================================
-- UI FUNCTION REFERENCE
-- =============================================================================
--[[
SPEAKER SETUP:
    UI:SetSpeaker(name, keysound, species, form, skin, gender)
    UI:SetSpeaker(character)           -- From Character object
    UI:SetSpeakerEmotion(emotion)      -- "Happy", "Sad", "Angry", etc.
    UI:SetSpeakerReverse(bool)         -- Face left instead of right
    UI:ResetSpeaker(keysound)          -- Clear speaker

DIALOGUE:
    UI:WaitShowDialogue(text)          -- Show and wait for input
    UI:WaitShowTimedDialogue(text, frames) -- Auto-advance after time
    UI:WaitShowVoiceOver(text, frames) -- No portrait, centered

TEXT FORMATTING:
    UI:SetCenter(bool)                 -- Center text
    UI:SetBounds(x, y, width, height)  -- Custom text box position
    UI:SetAutoFinish(bool)             -- Auto-advance text

CHOICES:
    UI:ChoiceMenuYesNo(message, defaultNo)
    UI:BeginChoiceMenu(message, choices, default, cancel)
    UI:WaitForChoice()
    UI:ChoiceResult()                  -- Get the result

SPECIAL:
    UI:NameMenu(title, desc, maxLen, default)
    UI:WaitShowTitle(text, frames)     -- Big centered title
    UI:WaitHideTitle(frames)
    UI:WaitShowBG(bg, frameTime, fadeTime) -- Show background image
    UI:WaitHideBG(fadeTime)

SCREENS:
    GAME:FadeOut(white, frames)
    GAME:FadeIn(frames)
    GAME:MoveCamera(x, y, frames, local)

SOUND:
    SOUND:PlayBGM(track, fade)
    SOUND:StopBGM()
    SOUND:PlaySE(sound)
    SOUND:PlayBattleSE(sound)

GROUND CHARACTERS:
    GROUND:CharMoveToTask(char, pos, run, speed)
    GROUND:CharTurnToChar(char, target)
    GROUND:CharTurnToLoc(char, loc)
    GROUND:CharSetAnim(char, anim, loop)
    GROUND:CharSetEmote(char, emote, seconds)
--]]

-- =============================================================================
-- EXPORT
-- =============================================================================
return ExampleCutscenes
