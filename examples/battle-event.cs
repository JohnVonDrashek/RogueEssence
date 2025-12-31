// =============================================================================
// EXAMPLE: Custom Battle Event
// =============================================================================
// This file demonstrates how to create custom BattleEvent classes in
// RogueEssence. BattleEvents are the building blocks of skill, item,
// and status effects. They execute at specific phases of battle.
//
// BattleEvents receive a BattleContext containing:
// - The user (attacker)
// - The target (if applicable)
// - The action being performed
// - Damage values and modifiers
// =============================================================================

using System;
using System.Collections.Generic;
using RogueEssence.Data;
using RogueEssence.Dungeon;
using RogueEssence.Content;
using RogueElements;
using RogueEssence.Dev;

namespace RogueEssence.Examples
{
    // =========================================================================
    // EXAMPLE 1: Simple Damage Modifier Event
    // =========================================================================
    /// <summary>
    /// A BattleEvent that multiplies damage by a percentage.
    /// Demonstrates basic damage modification in BeforeHits.
    /// </summary>
    [Serializable]
    public class DamageMultiplierEvent : BattleEvent
    {
        // ---------------------------------------------------------------------
        // STEP 1: Define properties
        // ---------------------------------------------------------------------
        // These can be configured in the data editor.

        /// <summary>
        /// Damage multiplier as a percentage (100 = normal, 150 = 1.5x, etc.)
        /// </summary>
        public int Multiplier;

        /// <summary>
        /// If set, only applies to moves of this element.
        /// Leave empty to apply to all moves.
        /// </summary>
        [DataType(0, DataManager.DataType.Element, false)]
        public string RequiredElement;

        // ---------------------------------------------------------------------
        // STEP 2: Implement constructors
        // ---------------------------------------------------------------------

        /// <summary>
        /// Default constructor for serialization.
        /// </summary>
        public DamageMultiplierEvent()
        {
            Multiplier = 100;
            RequiredElement = "";
        }

        /// <summary>
        /// Convenience constructor.
        /// </summary>
        public DamageMultiplierEvent(int multiplier, string element = "")
        {
            Multiplier = multiplier;
            RequiredElement = element;
        }

        // ---------------------------------------------------------------------
        // STEP 3: Implement Clone method
        // ---------------------------------------------------------------------
        // Clone is used when copying event data. Always implement this!

        public override GameEvent Clone()
        {
            return new DamageMultiplierEvent(Multiplier, RequiredElement);
        }

        // ---------------------------------------------------------------------
        // STEP 4: Implement Apply method
        // ---------------------------------------------------------------------
        // This is the main logic that runs when the event triggers.
        // It's a coroutine that can yield for animations/delays.

        public override IEnumerator<YieldInstruction> Apply(
            GameEventOwner owner,      // The skill/item/status that owns this event
            Character ownerChar,       // The character that has this effect
            BattleContext context)     // The battle context with all combat info
        {
            // Check element requirement
            if (!string.IsNullOrEmpty(RequiredElement))
            {
                // Get the move's element from the context
                string moveElement = context.Data.Element;

                // Skip if element doesn't match
                if (moveElement != RequiredElement)
                    yield break;  // Exit early, no effect
            }

            // Modify the damage
            // context.AddContextStateMult<T>(multiplier) is common pattern
            // For direct damage modification:
            context.AddContextStateMult<DmgMult>(new Fraction(Multiplier, 100));

            // Note: The actual damage calculation happens later.
            // We're just setting up the multiplier here.

            yield break;  // No wait needed, instant effect
        }
    }

    // =========================================================================
    // EXAMPLE 2: Conditional Effect Event
    // =========================================================================
    /// <summary>
    /// A BattleEvent that applies a status effect only if a condition is met.
    /// Demonstrates context checking and conditional logic.
    /// </summary>
    [Serializable]
    public class ConditionalStatusEvent : BattleEvent
    {
        /// <summary>
        /// Status to apply if condition is met.
        /// </summary>
        [DataType(0, DataManager.DataType.Status, false)]
        public string StatusID;

        /// <summary>
        /// Only apply if target is below this HP percentage.
        /// </summary>
        public int HPThreshold;

        /// <summary>
        /// Chance of applying (0-100).
        /// </summary>
        public int Chance;

        public ConditionalStatusEvent()
        {
            StatusID = "";
            HPThreshold = 50;
            Chance = 100;
        }

        public ConditionalStatusEvent(string statusId, int hpThreshold, int chance)
        {
            StatusID = statusId;
            HPThreshold = hpThreshold;
            Chance = chance;
        }

        public override GameEvent Clone()
        {
            return new ConditionalStatusEvent(StatusID, HPThreshold, Chance);
        }

        public override IEnumerator<YieldInstruction> Apply(
            GameEventOwner owner,
            Character ownerChar,
            BattleContext context)
        {
            // Get the target character
            Character target = context.Target;

            // Null check - target might not exist in some contexts
            if (target == null)
                yield break;

            // Check HP threshold
            int hpPercent = (target.HP * 100) / target.MaxHP;
            if (hpPercent > HPThreshold)
                yield break;  // Target HP too high, skip

            // Check random chance
            if (DataManager.Instance.Save.Rand.Next(100) >= Chance)
                yield break;  // Failed chance roll

            // Apply the status
            // Use the StatusCheckContext to properly apply status effects
            yield return CoroutineManager.Instance.StartCoroutine(
                target.AddStatusEffect(null, StatusID, null, null, true, true)
            );

            // Optional: Show a message
            DungeonScene.Instance.LogMsg(
                Text.FormatKey("MSG_STATUS_INFLICTED",
                    target.GetDisplayName(false),
                    DataManager.Instance.GetStatus(StatusID).Name.ToLocal())
            );
        }
    }

    // =========================================================================
    // EXAMPLE 3: Recoil/Self-Damage Event
    // =========================================================================
    /// <summary>
    /// A BattleEvent that deals damage to the user after attacking.
    /// Demonstrates accessing user vs target and dealing damage.
    /// Best used in AfterActions or AfterHittings.
    /// </summary>
    [Serializable]
    public class RecoilDamageEvent : BattleEvent
    {
        /// <summary>
        /// Percentage of max HP to deal as recoil.
        /// </summary>
        public int HPPercent;

        /// <summary>
        /// Fixed damage amount (added to percentage damage).
        /// </summary>
        public int FixedDamage;

        /// <summary>
        /// If true, only triggers if the attack actually hit.
        /// </summary>
        public bool OnlyOnHit;

        public RecoilDamageEvent()
        {
            HPPercent = 10;
            FixedDamage = 0;
            OnlyOnHit = true;
        }

        public RecoilDamageEvent(int hpPercent, int fixedDamage = 0, bool onlyOnHit = true)
        {
            HPPercent = hpPercent;
            FixedDamage = fixedDamage;
            OnlyOnHit = onlyOnHit;
        }

        public override GameEvent Clone()
        {
            return new RecoilDamageEvent(HPPercent, FixedDamage, OnlyOnHit);
        }

        public override IEnumerator<YieldInstruction> Apply(
            GameEventOwner owner,
            Character ownerChar,
            BattleContext context)
        {
            // Get the user (attacker)
            Character user = context.User;

            if (user == null || user.Dead)
                yield break;

            // Check if we need a successful hit
            if (OnlyOnHit)
            {
                // Check the hit result from context
                // context.Hit indicates if the attack connected
                if (!context.Hit)
                    yield break;
            }

            // Calculate recoil damage
            int damage = (user.MaxHP * HPPercent) / 100 + FixedDamage;

            // Minimum 1 damage
            if (damage < 1)
                damage = 1;

            // Show message
            DungeonScene.Instance.LogMsg(
                Text.FormatKey("MSG_RECOIL", user.GetDisplayName(false))
            );

            // Deal the damage
            // Using ProcessBattleDamage properly handles damage and death
            yield return CoroutineManager.Instance.StartCoroutine(
                user.InflictDamage(damage)
            );
        }
    }

    // =========================================================================
    // EXAMPLE 4: Healing Event with Animation
    // =========================================================================
    /// <summary>
    /// A BattleEvent that heals the target with visual effects.
    /// Demonstrates healing mechanics and visual feedback.
    /// </summary>
    [Serializable]
    public class HealWithEffectEvent : BattleEvent
    {
        /// <summary>
        /// Amount to heal (flat value).
        /// </summary>
        public int HealAmount;

        /// <summary>
        /// Percentage of max HP to heal.
        /// </summary>
        public int HealPercent;

        /// <summary>
        /// Sound effect to play.
        /// </summary>
        public string SoundEffect;

        public HealWithEffectEvent()
        {
            HealAmount = 0;
            HealPercent = 0;
            SoundEffect = "DUN_Heal";
        }

        public HealWithEffectEvent(int amount, int percent, string sfx = "DUN_Heal")
        {
            HealAmount = amount;
            HealPercent = percent;
            SoundEffect = sfx;
        }

        public override GameEvent Clone()
        {
            return new HealWithEffectEvent(HealAmount, HealPercent, SoundEffect);
        }

        public override IEnumerator<YieldInstruction> Apply(
            GameEventOwner owner,
            Character ownerChar,
            BattleContext context)
        {
            Character target = context.Target;

            if (target == null || target.Dead)
                yield break;

            // Calculate total healing
            int healing = HealAmount + (target.MaxHP * HealPercent) / 100;

            if (healing <= 0)
                yield break;

            // Play sound effect
            if (!string.IsNullOrEmpty(SoundEffect))
            {
                GameManager.Instance.BattleSE(SoundEffect);
            }

            // Play visual effect (if you have one configured)
            // SingleEmitter emitter = new SingleEmitter(new AnimData("Heal_Green", 3));
            // emitter.SetupEmit(target.MapLoc, target.MapLoc, target.CharDir);
            // DungeonScene.Instance.CreateAnim(emitter, DrawLayer.NoDraw);

            // Wait a moment for effect
            yield return new WaitForFrames(GameManager.Instance.ModifyBattleSpeed(30));

            // Apply healing
            yield return CoroutineManager.Instance.StartCoroutine(
                target.RestoreHP(healing, true)
            );
        }
    }

    // =========================================================================
    // EXAMPLE 5: Type Effectiveness Modifier
    // =========================================================================
    /// <summary>
    /// A BattleEvent that modifies type effectiveness.
    /// Use in TargetElementEffects to change how types interact.
    /// </summary>
    [Serializable]
    public class TypeEffectivenessModEvent : BattleEvent
    {
        /// <summary>
        /// Element to modify (leave empty for all).
        /// </summary>
        [DataType(0, DataManager.DataType.Element, false)]
        public string AffectedElement;

        /// <summary>
        /// New effectiveness multiplier (100 = normal).
        /// </summary>
        public int EffectivenessMultiplier;

        public TypeEffectivenessModEvent()
        {
            AffectedElement = "";
            EffectivenessMultiplier = 100;
        }

        public TypeEffectivenessModEvent(string element, int effectiveness)
        {
            AffectedElement = element;
            EffectivenessMultiplier = effectiveness;
        }

        public override GameEvent Clone()
        {
            return new TypeEffectivenessModEvent(AffectedElement, EffectivenessMultiplier);
        }

        public override IEnumerator<YieldInstruction> Apply(
            GameEventOwner owner,
            Character ownerChar,
            BattleContext context)
        {
            // Check if the incoming move matches our element
            if (!string.IsNullOrEmpty(AffectedElement))
            {
                if (context.Data.Element != AffectedElement)
                    yield break;
            }

            // Modify the type multiplier
            context.AddContextStateMult<DmgMult>(new Fraction(EffectivenessMultiplier, 100));

            yield break;
        }
    }

    // =========================================================================
    // EXAMPLE 6: Move User Event (Knockback/Pull)
    // =========================================================================
    /// <summary>
    /// A BattleEvent that moves the target.
    /// Demonstrates position manipulation and movement.
    /// </summary>
    [Serializable]
    public class KnockbackEvent : BattleEvent
    {
        /// <summary>
        /// Number of tiles to push back.
        /// </summary>
        public int Distance;

        /// <summary>
        /// If true, pulls toward user instead of pushing away.
        /// </summary>
        public bool Pull;

        public KnockbackEvent()
        {
            Distance = 1;
            Pull = false;
        }

        public KnockbackEvent(int distance, bool pull = false)
        {
            Distance = distance;
            Pull = pull;
        }

        public override GameEvent Clone()
        {
            return new KnockbackEvent(Distance, Pull);
        }

        public override IEnumerator<YieldInstruction> Apply(
            GameEventOwner owner,
            Character ownerChar,
            BattleContext context)
        {
            Character user = context.User;
            Character target = context.Target;

            if (user == null || target == null || target.Dead)
                yield break;

            // Calculate direction from user to target
            Dir8 dir = DirExt.GetDir(user.CharLoc, target.CharLoc);

            // If pulling, reverse the direction
            if (Pull)
                dir = dir.Reverse();

            // Move the target
            for (int i = 0; i < Distance; i++)
            {
                // Calculate destination
                Loc destLoc = target.CharLoc + dir.GetLoc();

                // Check if destination is valid (walkable and not occupied)
                if (ZoneManager.Instance.CurrentMap.TileBlocked(destLoc, target.Mobility))
                    break;

                if (ZoneManager.Instance.CurrentMap.GetCharAtLoc(destLoc) != null)
                    break;

                // Move the character
                yield return CoroutineManager.Instance.StartCoroutine(
                    DungeonScene.Instance.ProcessCharacterMove(target, destLoc, false, false)
                );
            }
        }
    }

    // =========================================================================
    // USAGE NOTES
    // =========================================================================
    //
    // BattleEvents are added to various lists depending on when they trigger:
    //
    // ON SKILLS (BattleData):
    // - BeforeTryActions: Before skill is attempted (can cancel)
    // - BeforeActions: Before skill executes (turn still used if canceled)
    // - OnActions: When skill activates
    // - BeforeExplosions: Before AoE hitbox expands
    // - BeforeHits: Before damage calculation
    // - OnHits: When damage is dealt
    // - OnHitTiles: When hitting terrain
    // - AfterActions: After skill completes
    //
    // ON ITEMS/STATUSES/INTRINSICS (PassiveData):
    // - BeforeTryActions: Before any action attempt
    // - BeforeActions: Before action execution
    // - OnActions: During action
    // - BeforeHittings: Before user hits something
    // - BeforeBeingHits: Before being hit
    // - AfterHittings: After hitting something
    // - AfterBeingHits: After being hit
    // - AfterActions: After any action
    // - OnTurnStarts/OnTurnEnds: Turn phases
    // - OnRefresh: Stat recalculation
    //
    // =========================================================================
    // BATTLECONTEXT REFERENCE
    // =========================================================================
    //
    // Useful BattleContext properties:
    //
    // context.User - The attacking character
    // context.Target - The target character (may be null)
    // context.Data - The BattleData (skill info)
    // context.Hit - Whether the attack connected
    // context.GetContextStateInt<T>() - Get int context state
    // context.AddContextStateMult<T>(Fraction) - Add multiplier
    //
    // =========================================================================
    // COROUTINE HELPERS
    // =========================================================================
    //
    // yield break; - Exit immediately
    // yield return new WaitForFrames(n); - Wait n frames
    // yield return CoroutineManager.Instance.StartCoroutine(coroutine);
    //   - Start another coroutine and wait for it
    //
}
