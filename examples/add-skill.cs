// =============================================================================
// EXAMPLE: Adding a Skill (SkillData)
// =============================================================================
// This file demonstrates how to create a complete SkillData entry in
// RogueEssence. SkillData defines a skill/move with its targeting,
// power, effects, and visual feedback.
//
// A skill consists of three main parts:
// 1. SkillData - Container with name, description, and charges
// 2. BattleData - The combat mechanics (element, power, events)
// 3. CombatAction - The targeting and hitbox behavior
// =============================================================================

using System;
using System.Collections.Generic;
using RogueEssence.Data;
using RogueEssence.Dungeon;
using RogueEssence.Content;
using RogueElements;

namespace RogueEssence.Examples
{
    public class AddSkillExample
    {
        /// <summary>
        /// Creates a complete example skill - a fire-type attack called "Blaze Cannon"
        /// that has high power, can cause burns, and uses a projectile hitbox.
        /// </summary>
        public static SkillData CreateFireProjectileSkill()
        {
            // -----------------------------------------------------------------
            // STEP 1: Create the SkillData container
            // -----------------------------------------------------------------
            SkillData skill = new SkillData();

            // -----------------------------------------------------------------
            // STEP 2: Set basic metadata
            // -----------------------------------------------------------------

            // Name - The skill name shown to players
            skill.Name = new LocalText("Blaze Cannon");

            // Desc - The skill description shown in menus
            skill.Desc = new LocalText("Fires a powerful blast of flames. May cause a burn.");

            // Comment - Developer notes (not shown to players)
            skill.Comment = "Example skill for documentation";

            // Released - If false, this skill won't be available in the game
            skill.Released = true;

            // IndexNum - Internal ordering number
            skill.IndexNum = 9999;

            // -----------------------------------------------------------------
            // STEP 3: Set skill charges (PP in Pokemon terms)
            // -----------------------------------------------------------------

            // BaseCharges - How many times this skill can be used before needing restoration
            skill.BaseCharges = 10;

            // -----------------------------------------------------------------
            // STEP 4: Configure the BattleData (combat mechanics)
            // -----------------------------------------------------------------
            // BattleData holds the element, power, accuracy, and battle events.

            // Element - The elemental type of the skill
            // Must match an ID in your Element data files
            skill.Data.Element = "fire";

            // Category - Determines which stats are used for damage
            // - SkillCategory.Physical: Uses Atk vs Def
            // - SkillCategory.Magical: Uses MAtk vs MDef
            // - SkillCategory.Status: Non-damaging (like stat buffs)
            skill.Data.Category = BattleData.SkillCategory.Magical;

            // HitRate - Accuracy percentage (0-100, or -1 for never miss)
            skill.Data.HitRate = 90;  // 90% accuracy

            // -----------------------------------------------------------------
            // STEP 4a: Add SkillStates to BattleData
            // -----------------------------------------------------------------
            // SkillStates store data that battle events can check against.
            // Common states include base power, contact flag, sound-based, etc.

            // BasePowerState - The base power of the skill for damage calculation
            skill.Data.SkillStates.Set(new BasePowerState(120));  // 120 base power

            // CategoryState - Can override the default category check
            // (Usually not needed if Category is already set correctly)

            // ContactState - Marks if this skill makes contact (triggers abilities)
            // skill.Data.SkillStates.Set(new ContactState());

            // SoundState - Marks if this skill is sound-based
            // skill.Data.SkillStates.Set(new SoundState());

            // -----------------------------------------------------------------
            // STEP 4b: Add battle events to BattleData
            // -----------------------------------------------------------------
            // Events run at different phases of the attack. Use Priority to
            // control execution order (lower numbers run first).

            // OnHits - Events that trigger when the attack successfully hits
            // This is where we add the burn effect

            // DamageEvent deals damage based on power, category, and element
            skill.Data.OnHits.Add(Priority.Zero, new DamageEvent());

            // Add a chance to inflict burn status (30% chance)
            // StatusBattleEvent(status_id, affects_user, affects_foe, percent_chance)
            skill.Data.OnHits.Add(Priority.Zero, new StatusBattleEvent("burn", false, true, false, 30));

            // -----------------------------------------------------------------
            // STEP 4c: Configure visual effects (BattleFX)
            // -----------------------------------------------------------------

            // IntroFX - VFX that play before the hit (even on miss)
            // Can have multiple IntroFX that play in sequence
            BattleFX introEffect = new BattleFX();
            introEffect.Sound = "DUN_Fire_Blast";  // Sound effect ID
            // introEffect.Emitter = new SingleEmitter(new AnimData("Fire_Column", 3));
            skill.Data.IntroFX.Add(introEffect);

            // HitFX - VFX that play when the target is hit
            skill.Data.HitFX.Sound = "DUN_Hit_Super_Effective";
            // skill.Data.HitFX.Emitter = new SingleEmitter(new AnimData("Hit_Fire", 2));

            // HitCharAction - Character animation when hit
            // Uses CharAnimFrameType to specify the animation
            skill.Data.HitCharAction = new CharAnimFrameType(GraphicsManager.HurtAction);

            // -----------------------------------------------------------------
            // STEP 5: Configure the CombatAction (Hitbox/Targeting)
            // -----------------------------------------------------------------
            // The HitboxAction determines how the skill targets enemies.
            // Different action types have different behaviors.

            // -- PROJECTILE ACTION (Fires in a line) --
            ProjectileAction projectile = new ProjectileAction();

            // CharAnimData - What animation the user plays when attacking
            projectile.CharAnimData = new CharAnimFrameType(GraphicsManager.ChargeAction);

            // Range - How far the projectile travels (in tiles)
            projectile.Range = 8;

            // Speed - How fast the projectile moves (pixels per frame)
            projectile.Speed = 16;

            // StopAtWall - Whether the projectile stops when hitting a wall
            projectile.StopAtWall = true;

            // StopAtHit - Whether the projectile stops after hitting one target
            projectile.StopAtHit = true;

            // HitTiles - Whether the projectile triggers tile effects
            projectile.HitTiles = true;

            // Anim - The visual animation of the projectile
            // projectile.Anim = new AnimData("Flamethrower", 3);

            // Set the hitbox action
            skill.HitboxAction = projectile;

            // -----------------------------------------------------------------
            // STEP 6: Configure the explosion/AOE (if any)
            // -----------------------------------------------------------------
            // ExplosionData defines what happens when the hitbox hits something.
            // For a simple projectile, you might not need an explosion.

            skill.Explosion.Range = 0;  // No explosion radius
            skill.Explosion.Speed = 0;
            skill.Explosion.TargetAlignments = Alignment.Foe;  // Only hits foes

            // For an AoE explosion:
            // skill.Explosion.Range = 2;  // 2 tile radius
            // skill.Explosion.HitTiles = true;

            // -----------------------------------------------------------------
            // STEP 7: Configure strike data
            // -----------------------------------------------------------------
            // Strikes determines how many times the skill hits

            skill.Strikes = 1;  // Single hit

            // For multi-hit moves:
            // skill.Strikes = 3;  // Hits 3 times

            // -----------------------------------------------------------------
            // Complete skill is ready!
            // -----------------------------------------------------------------
            return skill;
        }

        /// <summary>
        /// Creates a melee attack skill with contact and recoil damage.
        /// Demonstrates AttackAction and additional battle events.
        /// </summary>
        public static SkillData CreateMeleeRecoilSkill()
        {
            SkillData skill = new SkillData();
            skill.Name = new LocalText("Wild Tackle");
            skill.Desc = new LocalText("A reckless charge that also damages the user.");
            skill.Released = true;
            skill.BaseCharges = 15;

            // Physical attack
            skill.Data.Element = "normal";
            skill.Data.Category = BattleData.SkillCategory.Physical;
            skill.Data.HitRate = 100;
            skill.Data.SkillStates.Set(new BasePowerState(90));
            skill.Data.SkillStates.Set(new ContactState());  // Makes contact

            // Deal damage
            skill.Data.OnHits.Add(Priority.Zero, new DamageEvent());

            // Recoil damage - user takes 25% of damage dealt
            // RecoilEvent(percent, hpFraction, element, affectsUser, affectsTarget)
            // Note: Actual implementation may vary - check BattleEvents for exact API
            skill.Data.AfterActions.Add(Priority.Zero, new RecoilPercentEvent(25, new List<string>()));

            // Hit effects
            skill.Data.HitFX.Sound = "DUN_Hit_Neutral";
            skill.Data.HitCharAction = new CharAnimFrameType(GraphicsManager.HurtAction);

            // -- ATTACK ACTION (Melee hit in front) --
            AttackAction attack = new AttackAction();

            // LagBehindTime - Frames of delay before acting
            attack.LagBehindTime = 5;

            // CharAnimData - Animation the user plays
            attack.CharAnimData = new CharAnimFrameType(GraphicsManager.ChargeAction);

            // WideAngle - Attack angle
            // AttackAction.AttackRange.Any = 360 degrees around
            // AttackAction.AttackRange.Front = 90 degree cone
            attack.WideAngle = AttackAction.AttackRange.Front;

            // HitTiles - Whether this hits tiles (for breaking walls, etc.)
            attack.HitTiles = true;

            skill.HitboxAction = attack;

            // No explosion for melee
            skill.Explosion.Range = 0;
            skill.Explosion.TargetAlignments = Alignment.Foe;

            skill.Strikes = 1;

            return skill;
        }

        /// <summary>
        /// Creates an area-of-effect status skill.
        /// Demonstrates AreaAction and status effects on multiple targets.
        /// </summary>
        public static SkillData CreateAoeStatusSkill()
        {
            SkillData skill = new SkillData();
            skill.Name = new LocalText("Sleepy Fog");
            skill.Desc = new LocalText("Releases a mist that may put nearby enemies to sleep.");
            skill.Released = true;
            skill.BaseCharges = 10;

            // Status move (no damage)
            skill.Data.Element = "psychic";
            skill.Data.Category = BattleData.SkillCategory.Status;
            skill.Data.HitRate = 75;  // Can miss

            // Apply sleep status (100% if the move hits)
            skill.Data.OnHits.Add(Priority.Zero, new StatusBattleEvent("sleep", false, true, false, 100));

            // Sound effect
            skill.Data.IntroFX.Add(new BattleFX { Sound = "DUN_Sleep_Powder" });

            // -- AREA ACTION (Affects tiles around user) --
            AreaAction area = new AreaAction();

            // CharAnimData - Animation for the user
            area.CharAnimData = new CharAnimFrameType(GraphicsManager.ChargeAction);

            // Range - Radius of effect (in tiles)
            area.Range = 2;  // 2 tile radius around user

            // Speed - How fast the effect expands (0 = instant)
            area.Speed = 0;

            // HitSelf - Whether the user is affected
            area.HitSelf = false;

            skill.HitboxAction = area;

            // Explosion that targets foes only
            skill.Explosion.Range = 0;
            skill.Explosion.TargetAlignments = Alignment.Foe;

            skill.Strikes = 1;

            return skill;
        }

        /// <summary>
        /// Creates a self-targeting buff skill.
        /// Demonstrates SelfAction and stat modification.
        /// </summary>
        public static SkillData CreateSelfBuffSkill()
        {
            SkillData skill = new SkillData();
            skill.Name = new LocalText("Power Up");
            skill.Desc = new LocalText("Focuses energy to boost Attack and Special Attack.");
            skill.Released = true;
            skill.BaseCharges = 20;

            // Status move that targets self
            skill.Data.Element = "normal";
            skill.Data.Category = BattleData.SkillCategory.Status;
            skill.Data.HitRate = -1;  // Never miss

            // Stat boost events
            // StatChangeEvent(stat, stages, affectsUser, affectsTarget)
            // Positive stages = buff, negative = debuff
            skill.Data.OnHits.Add(Priority.Zero, new StatusStackBattleEvent("mod_attack", true, false, 1));
            skill.Data.OnHits.Add(Priority.Zero, new StatusStackBattleEvent("mod_special_attack", true, false, 1));

            // Sound effect
            skill.Data.IntroFX.Add(new BattleFX { Sound = "DUN_Stat_Up" });

            // -- SELF ACTION (Targets the user) --
            SelfAction selfAction = new SelfAction();

            // CharAnimData - Animation for the user
            selfAction.CharAnimData = new CharAnimFrameType(GraphicsManager.ChargeAction);

            // TargetAlignments - Typically Self for self-targeting
            selfAction.TargetAlignments = Alignment.Self;

            skill.HitboxAction = selfAction;

            // Explosion targets self
            skill.Explosion.Range = 0;
            skill.Explosion.TargetAlignments = Alignment.Self;

            skill.Strikes = 1;

            return skill;
        }

        /// <summary>
        /// Creates a beam/wave attack that pierces through targets.
        /// Demonstrates OffsetAction for line attacks.
        /// </summary>
        public static SkillData CreatePiercingBeamSkill()
        {
            SkillData skill = new SkillData();
            skill.Name = new LocalText("Prismatic Beam");
            skill.Desc = new LocalText("Fires a piercing ray of light.");
            skill.Released = true;
            skill.BaseCharges = 5;

            skill.Data.Element = "psychic";
            skill.Data.Category = BattleData.SkillCategory.Magical;
            skill.Data.HitRate = 100;
            skill.Data.SkillStates.Set(new BasePowerState(130));

            skill.Data.OnHits.Add(Priority.Zero, new DamageEvent());

            // -- OFFSET ACTION (Creates effect at positions relative to user) --
            // Good for beam attacks that hit multiple tiles in a line
            OffsetAction offset = new OffsetAction();

            offset.CharAnimData = new CharAnimFrameType(GraphicsManager.ChargeAction);

            // HitArea specifies the shape of the attack
            // For a beam, you'd typically use multiple offsets in a line
            offset.HitArea = OffsetAction.OffsetArea.Beam;

            // Range - How far the beam extends
            offset.Range = 10;

            // HitTiles - Whether it hits tiles
            offset.HitTiles = true;

            skill.HitboxAction = offset;

            skill.Explosion.Range = 0;
            skill.Explosion.TargetAlignments = Alignment.Foe;

            skill.Strikes = 1;

            return skill;
        }

        // =====================================================================
        // COMMON BATTLE EVENTS REFERENCE
        // =====================================================================
        // The following are commonly used BattleEvent types:
        //
        // DamageEvent() - Deals damage based on power/category
        // StatusBattleEvent(status, onUser, onFoe, stack, percent) - Applies status
        // StatusStackBattleEvent(status, onUser, onFoe, stacks) - Stat mod status
        // RecoilPercentEvent(percent, elements) - Recoil damage to user
        // ChangeToElementEvent(element) - Changes move's element
        // MultiplyEffectEvent(multiplier) - Damage multiplier
        // HPDrainEvent(percent) - Heals user based on damage dealt
        // MoveCharEvent(direction, distance) - Moves target
        // RemoveStatusBattleEvent(status) - Removes a status
        // WeatherNeededEvent(weather, action) - Requires weather condition
        //
        // Check RogueEssence/Dungeon/GameEffects/ for more event types
    }
}
