// =============================================================================
// EXAMPLE: Adding a Status (StatusData)
// =============================================================================
// This file demonstrates how to create a complete StatusData entry in
// RogueEssence. StatusData defines a status condition/effect with its
// states, visual indicators, and event handlers.
//
// Statuses can be:
// - Temporary conditions (like poison, burn, sleep)
// - Stat modifiers (like Attack Up, Defense Down)
// - Stacking counters (like Stockpile charges)
// - Passive effects (like Reflect, Light Screen)
// =============================================================================

using System;
using System.Collections.Generic;
using RogueEssence.Data;
using RogueEssence.Dungeon;
using RogueEssence.Content;
using RogueElements;

namespace RogueEssence.Examples
{
    public class AddStatusExample
    {
        /// <summary>
        /// Creates a damage-over-time status (like Poison).
        /// Demonstrates periodic effects via OnTurnEnds.
        /// </summary>
        public static StatusData CreateDamageOverTimeStatus()
        {
            // -----------------------------------------------------------------
            // STEP 1: Create the StatusData container
            // -----------------------------------------------------------------
            StatusData status = new StatusData();

            // -----------------------------------------------------------------
            // STEP 2: Set basic metadata
            // -----------------------------------------------------------------

            // Name - The status name shown to players
            status.Name = new LocalText("Venom");

            // Desc - The status description
            status.Desc = new LocalText("Loses HP each turn from poisoning.");

            // Comment - Developer notes (not shown to players)
            status.Comment = "Example damage-over-time status";

            // Released - If false, won't appear in the game
            status.Released = true;

            // IndexNum - Internal ordering number
            status.IndexNum = 9999;

            // -----------------------------------------------------------------
            // STEP 3: Configure duration and behavior
            // -----------------------------------------------------------------

            // DefaultTurns - How long the status lasts (-1 = indefinite)
            // Note: Actual duration is set by the CountDownState
            status.DefaultTurns = 10;

            // MenuName - If true, status appears in the status menu
            status.MenuName = true;

            // CarryOver - If true, status persists when leaving floor
            status.CarryOver = false;

            // -----------------------------------------------------------------
            // STEP 4: Add status states
            // -----------------------------------------------------------------
            // StatusStates store data about the status and act as markers.

            // BadStatusState marks this as a negative status
            // (affects certain abilities that react to bad statuses)
            status.StatusStates.Set(new BadStatusState());

            // CountDownState tracks remaining duration
            // The status will automatically expire when the counter reaches 0
            status.StatusStates.Set(new CountDownState(10));  // 10 turns

            // -----------------------------------------------------------------
            // STEP 5: Configure visual representation
            // -----------------------------------------------------------------

            // Emoticon - The status icon shown above the character
            // References an emote in the Graphics/Emotes folder
            status.Emoticon = "Poison";

            // DrawEffect - Particle effect drawn on the character
            // status.DrawEffect = new AnimData("Poison_Bubbles", 3);

            // -----------------------------------------------------------------
            // STEP 6: Configure passive events
            // -----------------------------------------------------------------
            // These events trigger during various game phases.
            // The priority determines execution order within that phase.

            // OnTurnEnds - Triggers at the end of each turn
            // This is where we apply damage
            status.PassiveData.OnTurnEnds.Add(Priority.Zero, new StatusDamageEvent("Venom", false));

            // The StatusDamageEvent deals fractional HP damage each turn
            // Parameters: (message_key, removes_on_damage)

            // OnStatusAdds - When this status is first applied
            // Can be used for effects when status begins
            status.PassiveData.OnStatusAdds.Add(Priority.Zero, new StatusLogEvent("MSG_POISONED"));

            // OnStatusRemoves - When this status is removed
            // Can be used for cleanup effects
            status.PassiveData.OnStatusRemoves.Add(Priority.Zero, new StatusLogEvent("MSG_CURED"));

            // -----------------------------------------------------------------
            // Status is complete!
            // -----------------------------------------------------------------
            return status;
        }

        /// <summary>
        /// Creates a stat modifier status (like Attack Up).
        /// Demonstrates OnRefresh for stat modifications.
        /// </summary>
        public static StatusData CreateStatModifierStatus()
        {
            StatusData status = new StatusData();

            status.Name = new LocalText("Power Boost");
            status.Desc = new LocalText("Attack is sharply increased.");
            status.Released = true;
            status.MenuName = true;
            status.CarryOver = false;

            // This is a beneficial status
            // (No BadStatusState means it's good)

            // Duration tracking
            status.StatusStates.Set(new CountDownState(10));

            // Stack counter for stat stages (how much boost)
            // StackState tracks integer values that can be increased/decreased
            status.StatusStates.Set(new StackState(2));  // +2 stages

            status.Emoticon = "Attack_Up";

            // -----------------------------------------------------------------
            // STAT MODIFICATION
            // -----------------------------------------------------------------
            // OnRefresh is called whenever stats need recalculating.
            // This is the place to apply stat modifiers.

            // StatBoostEvent modifies a stat based on StackState
            // Each stack typically represents one stage of boost
            // In Pokemon terms: +1 stage = 1.5x, +2 = 2x, etc.
            status.PassiveData.OnRefresh.Add(Priority.Zero, new StatBuffEvent(Stat.Attack));

            // The StatBuffEvent reads from StackState and applies
            // the appropriate multiplier to the specified stat

            return status;
        }

        /// <summary>
        /// Creates a protective status (like Reflect).
        /// Demonstrates BeforeBeingHits to modify incoming damage.
        /// </summary>
        public static StatusData CreateProtectiveStatus()
        {
            StatusData status = new StatusData();

            status.Name = new LocalText("Barrier");
            status.Desc = new LocalText("Reduces damage from attacks.");
            status.Released = true;
            status.MenuName = true;
            status.CarryOver = false;

            // Duration tracking
            status.StatusStates.Set(new CountDownState(5));

            status.Emoticon = "Shield";

            // -----------------------------------------------------------------
            // DAMAGE REDUCTION
            // -----------------------------------------------------------------
            // BeforeBeingHits triggers before the character takes damage.
            // We can modify the damage here.

            // DamageReductionEvent reduces incoming damage by a percentage
            status.PassiveData.BeforeBeingHits.Add(Priority.Zero, new MultiplyDamageEvent(50));

            // The 50 means 50% damage (halved)
            // 0 would mean immune, 200 would mean double damage

            // Alternative: Block damage entirely under certain conditions
            // status.PassiveData.BeforeBeingHits.Add(Priority.Zero, new ImmuneToEvent());

            return status;
        }

        /// <summary>
        /// Creates a counter/retaliation status (like Counter).
        /// Demonstrates AfterBeingHits to react to damage.
        /// </summary>
        public static StatusData CreateRetaliationStatus()
        {
            StatusData status = new StatusData();

            status.Name = new LocalText("Revenge Aura");
            status.Desc = new LocalText("Deals damage to attackers when hit.");
            status.Released = true;
            status.MenuName = true;
            status.CarryOver = false;

            status.StatusStates.Set(new CountDownState(5));
            status.Emoticon = "Counter";

            // -----------------------------------------------------------------
            // RETALIATION
            // -----------------------------------------------------------------
            // AfterBeingHits triggers after taking damage.
            // We can deal damage back to the attacker here.

            // CounterDamageEvent deals damage back to attackers
            // Parameters depend on implementation - commonly:
            // (percent_of_damage_taken, fixed_damage, element)
            status.PassiveData.AfterBeingHits.Add(Priority.Zero, new CounterDamageEvent(50));

            // This deals 50% of received damage back to the attacker

            return status;
        }

        /// <summary>
        /// Creates an action-modifying status (like Taunt).
        /// Demonstrates BeforeTryActions to prevent certain actions.
        /// </summary>
        public static StatusData CreateActionRestrictionStatus()
        {
            StatusData status = new StatusData();

            status.Name = new LocalText("Taunt");
            status.Desc = new LocalText("Can only use damaging moves.");
            status.Released = true;
            status.MenuName = true;
            status.CarryOver = false;

            status.StatusStates.Set(new BadStatusState());
            status.StatusStates.Set(new CountDownState(3));

            status.Emoticon = "Taunt";

            // -----------------------------------------------------------------
            // ACTION RESTRICTION
            // -----------------------------------------------------------------
            // BeforeTryActions triggers before a character attempts an action.
            // We can cancel non-damaging moves here.

            // PreventStatusEvent prevents using Status category moves
            status.PassiveData.BeforeTryActions.Add(Priority.Zero, new PreventStatusMoveEvent("MSG_TAUNTED"));

            return status;
        }

        /// <summary>
        /// Creates a sleep/incapacitation status.
        /// Demonstrates complete action prevention and wake conditions.
        /// </summary>
        public static StatusData CreateSleepStatus()
        {
            StatusData status = new StatusData();

            status.Name = new LocalText("Sleep");
            status.Desc = new LocalText("Cannot take actions while asleep.");
            status.Released = true;
            status.MenuName = true;
            status.CarryOver = false;

            status.StatusStates.Set(new BadStatusState());
            status.StatusStates.Set(new CountDownState(3));

            // TransferStatusState allows this status to be transferred
            // (for moves like Psycho Shift)
            status.StatusStates.Set(new TransferStatusState());

            status.Emoticon = "Sleep";

            // -----------------------------------------------------------------
            // INCAPACITATION
            // -----------------------------------------------------------------

            // BeforeTryActions - Prevent all actions
            status.PassiveData.BeforeTryActions.Add(Priority.Zero, new SleepEvent());

            // AfterBeingHits - Wake up when hit
            status.PassiveData.AfterBeingHits.Add(Priority.Zero, new WakeUpEvent("MSG_WOKE_UP"));

            // OnTurnEnds - Natural wake check
            status.PassiveData.OnTurnEnds.Add(Priority.Zero, new CountDownRemoveEvent());

            return status;
        }

        /// <summary>
        /// Creates a type-adding status (like Electrify).
        /// Demonstrates type modification.
        /// </summary>
        public static StatusData CreateTypeModifyStatus()
        {
            StatusData status = new StatusData();

            status.Name = new LocalText("Electrified");
            status.Desc = new LocalText("Moves become Electric-type.");
            status.Released = true;
            status.MenuName = true;
            status.CarryOver = false;

            status.StatusStates.Set(new CountDownState(1));  // Lasts 1 turn/action

            status.Emoticon = "Electric";

            // -----------------------------------------------------------------
            // TYPE MODIFICATION
            // -----------------------------------------------------------------
            // BeforeActions can modify move properties before they execute

            // ChangeToElementEvent changes the move's type
            status.PassiveData.BeforeActions.Add(Priority.Zero, new ChangeToElementEvent("electric"));

            return status;
        }

        /// <summary>
        /// Creates a stacking status with charges (like Stockpile).
        /// Demonstrates StackState for multiple levels.
        /// </summary>
        public static StatusData CreateStackingStatus()
        {
            StatusData status = new StatusData();

            status.Name = new LocalText("Charged");
            status.Desc = new LocalText("Building up power. Stacks up to 3 times.");
            status.Released = true;
            status.MenuName = true;
            status.CarryOver = false;

            // StackState tracks the stack count
            // This can be read by other effects to determine power
            status.StatusStates.Set(new StackState(1));  // Starts at 1 stack

            // No countdown - stays until consumed
            // status.StatusStates.Set(new CountDownState(-1));

            status.Emoticon = "Charge";

            // -----------------------------------------------------------------
            // STACKING BONUSES
            // -----------------------------------------------------------------
            // OnRefresh can read the stack count and apply scaling bonuses

            // Each stack increases defense
            status.PassiveData.OnRefresh.Add(Priority.Zero, new StatBuffEvent(Stat.Defense));

            // Note: The stacking behavior (adding more stacks) is handled
            // by the move/effect that applies this status, not the status itself

            return status;
        }

        /// <summary>
        /// Creates a proximity aura status (affects nearby characters).
        /// Demonstrates ProximityData for area effects.
        /// </summary>
        public static StatusData CreateAuraStatus()
        {
            StatusData status = new StatusData();

            status.Name = new LocalText("Intimidate Aura");
            status.Desc = new LocalText("Lowers Attack of nearby enemies.");
            status.Released = true;
            status.MenuName = true;
            status.CarryOver = true;  // Persists between floors

            status.Emoticon = "Intimidate";

            // -----------------------------------------------------------------
            // PROXIMITY EFFECT
            // -----------------------------------------------------------------
            // ProximityEvent defines effects that affect characters within range

            // Radius - How far the aura reaches (DO NOT SET OVER 5)
            status.ProximityEvent.Radius = 2;  // 2 tile radius

            // TargetAlignments - Who is affected
            status.ProximityEvent.TargetAlignments = Alignment.Foe;

            // OnRefresh - Apply stat debuff to enemies in range
            status.ProximityEvent.OnRefresh.Add(Priority.Zero, new AuraStatDebuffEvent(Stat.Attack, -1));

            // Note: The actual implementation of AuraStatDebuffEvent
            // would need to check distance and apply appropriately

            return status;
        }

        // =====================================================================
        // COMMON STATUS STATES REFERENCE
        // =====================================================================
        // The following are commonly used StatusState types:
        //
        // CountDownState(turns) - Tracks remaining duration
        // StackState(count) - Tracks stack/stage count
        // BadStatusState - Marks as a negative status
        // TransferStatusState - Can be transferred by certain moves
        // StickyState - Cannot be removed by certain effects
        // RemovedOnActionState - Removed when taking action
        // SkillChargeState(skill_id) - Stores a charged skill
        //
        // Check RogueEssence/Dungeon/GameEffects/ for more state types

        // =====================================================================
        // COMMON STATUS EVENTS REFERENCE
        // =====================================================================
        // The following are commonly used status event types:
        //
        // StatusDamageEvent(msg, removes) - Deals HP damage each turn
        // StatusHealEvent(amount, percent) - Heals HP each turn
        // StatBuffEvent(stat) - Applies stat buff from StackState
        // MultiplyDamageEvent(percent) - Modifies incoming damage
        // CounterDamageEvent(percent) - Returns damage to attacker
        // PreventStatusMoveEvent(msg) - Blocks status moves
        // SleepEvent() - Prevents all actions
        // WakeUpEvent(msg) - Removes status when hit
        // CountDownRemoveEvent() - Decrements and removes at 0
        // ChangeToElementEvent(element) - Changes move type
        //
        // Check RogueEssence/Dungeon/GameEffects/ for more event types

        // =====================================================================
        // EVENT HOOK REFERENCE
        // =====================================================================
        // The following event hooks are available in PassiveData:
        //
        // OnEquips - When equipping an item
        // OnPickups - When picking up an item
        // BeforeStatusAdds - Before this status is added (can prevent)
        // BeforeStatusAddings - Before adding status to others
        // OnStatusAdds - After status is added
        // OnStatusRemoves - When status is removed
        // OnMapStatusAdds - When map status is added
        // OnMapStatusRemoves - When map status is removed
        // OnMapStarts - When map starts or character spawns
        // OnTurnStarts - At start of character's turn
        // OnTurnEnds - At end of character's turn
        // OnMapTurnEnds - At end of global turn
        // OnWalks - When character walks
        // OnDeaths - When character dies
        // OnRefresh - When stats are recalculated
        // BeforeTryActions - Before attempting action (can prevent)
        // BeforeActions - Before executing action
        // OnActions - During action execution
        // BeforeHittings - Before hitting a target
        // BeforeBeingHits - Before being hit
        // AfterHittings - After hitting a target
        // AfterBeingHits - After being hit
        // OnHitTiles - When hitting a tile
        // AfterActions - After action completes
        // UserElementEffects - Modify element effectiveness (attacking)
        // TargetElementEffects - Modify element effectiveness (defending)
        // ModifyHPs - Modify HP regeneration
        // RestoreHPs - Modify healing effects
    }
}
