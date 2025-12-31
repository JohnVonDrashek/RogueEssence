// =============================================================================
// EXAMPLE: Adding a Monster (MonsterData)
// =============================================================================
// This file demonstrates how to create a complete MonsterData entry in
// RogueEssence. MonsterData defines a species with its forms, stats,
// evolutions, and learn sets.
//
// In practice, monsters are usually created via the Data Editor, which
// serializes to JSON. This code shows what's happening under the hood.
// =============================================================================

using System;
using System.Collections.Generic;
using RogueEssence.Data;
using RogueEssence.Dungeon;
using RogueElements;

namespace RogueEssence.Examples
{
    public class AddMonsterExample
    {
        /// <summary>
        /// Creates a complete example MonsterData.
        /// This monster has two forms (normal and alternate), stats,
        /// level-up skills, and an evolution.
        /// </summary>
        public static MonsterData CreateExampleMonster()
        {
            // -----------------------------------------------------------------
            // STEP 1: Create the MonsterData container
            // -----------------------------------------------------------------
            // MonsterData is the top-level container for a species.
            // It holds metadata like name, title, and a collection of forms.

            MonsterData monster = new MonsterData();

            // -----------------------------------------------------------------
            // STEP 2: Set the basic metadata
            // -----------------------------------------------------------------

            // Name - The species name shown to players
            // LocalText supports multiple languages via its Strings dictionary
            monster.Name = new LocalText("Flamekin");

            // Title - Optional subtitle shown below the name (like "Flame Pokemon")
            // Leave empty LocalText() if no title is needed
            monster.Title = new LocalText("The Fire Spirit");

            // Released - If false, this monster won't appear in the game
            // Useful for work-in-progress content
            monster.Released = true;

            // Comment - Developer notes (not shown to players)
            monster.Comment = "Example monster for documentation";

            // IndexNum - The internal index number for this monster
            // This is typically set automatically by the data system
            monster.IndexNum = 9999;

            // -----------------------------------------------------------------
            // STEP 3: Create the base form (Form 0)
            // -----------------------------------------------------------------
            // Every monster needs at least one form. Forms define the actual
            // gameplay data: stats, types, abilities, skills, etc.

            MonsterForm baseForm = new MonsterForm();

            // FormName - Display name for this form (empty for default form)
            baseForm.FormName = new LocalText("");

            // Released - Form-specific release flag
            baseForm.Released = true;

            // -----------------------------------------------------------------
            // STEP 3a: Set the form's elemental types
            // -----------------------------------------------------------------
            // Element1 is the primary type, Element2 is secondary
            // Use empty string "" for no secondary type
            // Element IDs must match your Element data files

            baseForm.Element1 = "fire";     // Primary type: Fire
            baseForm.Element2 = "";          // No secondary type

            // -----------------------------------------------------------------
            // STEP 3b: Set intrinsic abilities
            // -----------------------------------------------------------------
            // Intrinsic1-3 are the possible abilities
            // Hidden intrinsics are secret abilities (like Hidden Abilities)
            // IDs must match your Intrinsic data files

            baseForm.Intrinsic1 = "blaze";           // Normal ability 1
            baseForm.Intrinsic2 = "";                 // No second normal ability
            baseForm.Intrinsic3 = "";                 // No third normal ability
            baseForm.Intrinsic1Hidden = "flash_fire"; // Hidden ability

            // -----------------------------------------------------------------
            // STEP 3c: Set base stats
            // -----------------------------------------------------------------
            // Stats determine the monster's capabilities
            // These are base values that get modified by level, nature, etc.

            baseForm.BaseHP = 39;        // Hit Points - survivability
            baseForm.BaseAtk = 52;       // Attack - physical damage
            baseForm.BaseDef = 43;       // Defense - physical damage reduction
            baseForm.BaseMAtk = 60;      // Magic Attack - special damage
            baseForm.BaseMDef = 50;      // Magic Defense - special damage reduction
            baseForm.BaseSpeed = 65;     // Speed - turn order and evasion

            // -----------------------------------------------------------------
            // STEP 3d: Set experience yield and group
            // -----------------------------------------------------------------
            // ExpYield determines how much EXP this monster gives when defeated
            // EXPTable is the experience growth rate category

            baseForm.ExpYield = 62;           // EXP given to defeater
            baseForm.EXPTable = GrowthGroup.MediumSlow; // Level-up EXP requirements

            // -----------------------------------------------------------------
            // STEP 3e: Set physical characteristics
            // -----------------------------------------------------------------
            // These are mostly cosmetic or used for specific game mechanics

            baseForm.Height = 6;              // Height in units (1 = 0.1m)
            baseForm.Weight = 85;             // Weight in units (1 = 0.1kg)
            baseForm.Gender = Gender.Genderless; // Gender ratio

            // Gender options:
            // - Gender.Genderless: Cannot be male or female
            // - Gender.Male: Always male
            // - Gender.Female: Always female
            // - Gender.Unknown: Can be either (50/50 ratio typically)

            // -----------------------------------------------------------------
            // STEP 3f: Set base friendship
            // -----------------------------------------------------------------
            // BaseFriendship affects recruiting and other friendship mechanics

            baseForm.BaseFriendship = 70;     // Starting friendship (0-255)

            // -----------------------------------------------------------------
            // STEP 3g: Configure level-up skills (LevelSkills)
            // -----------------------------------------------------------------
            // LevelSkills determine what skills the monster learns as it levels up
            // Each entry is a (level, skill_id) pair

            baseForm.LevelSkills = new List<LevelUpSkill>();

            // Level 1 starting moves (level 0 or 1 = known at start)
            baseForm.LevelSkills.Add(new LevelUpSkill("scratch", 1));    // Basic attack
            baseForm.LevelSkills.Add(new LevelUpSkill("growl", 1));      // Defense debuff

            // Moves learned at later levels
            baseForm.LevelSkills.Add(new LevelUpSkill("ember", 7));      // Fire attack
            baseForm.LevelSkills.Add(new LevelUpSkill("smokescreen", 10)); // Accuracy debuff
            baseForm.LevelSkills.Add(new LevelUpSkill("dragon_rage", 16)); // Fixed damage
            baseForm.LevelSkills.Add(new LevelUpSkill("scary_face", 19)); // Speed debuff
            baseForm.LevelSkills.Add(new LevelUpSkill("fire_fang", 25));  // Fire + flinch
            baseForm.LevelSkills.Add(new LevelUpSkill("flame_burst", 28)); // AoE fire
            baseForm.LevelSkills.Add(new LevelUpSkill("slash", 34));      // High crit
            baseForm.LevelSkills.Add(new LevelUpSkill("flamethrower", 37)); // Strong fire
            baseForm.LevelSkills.Add(new LevelUpSkill("fire_spin", 43));  // Trapping fire
            baseForm.LevelSkills.Add(new LevelUpSkill("inferno", 46));    // Burn + damage
            baseForm.LevelSkills.Add(new LevelUpSkill("flare_blitz", 56)); // Recoil fire

            // -----------------------------------------------------------------
            // STEP 3h: Configure egg moves (SharedSkills)
            // -----------------------------------------------------------------
            // SharedSkills are moves that can be inherited through breeding
            // or learned through special tutors

            baseForm.SharedSkills = new List<LearnableSkill>();
            baseForm.SharedSkills.Add(new LearnableSkill("dragon_dance"));
            baseForm.SharedSkills.Add(new LearnableSkill("outrage"));
            baseForm.SharedSkills.Add(new LearnableSkill("ancient_power"));
            baseForm.SharedSkills.Add(new LearnableSkill("belly_drum"));
            baseForm.SharedSkills.Add(new LearnableSkill("counter"));
            baseForm.SharedSkills.Add(new LearnableSkill("crunch"));

            // -----------------------------------------------------------------
            // STEP 3i: Configure TM/tutor moves (SecretSkills)
            // -----------------------------------------------------------------
            // SecretSkills are moves that can be learned through TMs or tutors

            baseForm.SecretSkills = new List<LearnableSkill>();
            baseForm.SecretSkills.Add(new LearnableSkill("focus_punch"));
            baseForm.SecretSkills.Add(new LearnableSkill("toxic"));
            baseForm.SecretSkills.Add(new LearnableSkill("hidden_power"));
            baseForm.SecretSkills.Add(new LearnableSkill("sunny_day"));
            baseForm.SecretSkills.Add(new LearnableSkill("protect"));
            baseForm.SecretSkills.Add(new LearnableSkill("frustration"));
            baseForm.SecretSkills.Add(new LearnableSkill("return"));
            baseForm.SecretSkills.Add(new LearnableSkill("dig"));
            baseForm.SecretSkills.Add(new LearnableSkill("brick_break"));
            baseForm.SecretSkills.Add(new LearnableSkill("double_team"));
            baseForm.SecretSkills.Add(new LearnableSkill("fire_blast"));
            baseForm.SecretSkills.Add(new LearnableSkill("aerial_ace"));
            baseForm.SecretSkills.Add(new LearnableSkill("facade"));
            baseForm.SecretSkills.Add(new LearnableSkill("rest"));
            baseForm.SecretSkills.Add(new LearnableSkill("attract"));

            // -----------------------------------------------------------------
            // STEP 3j: Configure evolutions
            // -----------------------------------------------------------------
            // Promotions define how this monster can evolve
            // Each PromoteBranch is one possible evolution path

            baseForm.Promotions = new List<PromoteBranch>();

            // Create an evolution to "flameking" at level 16
            PromoteBranch evolution1 = new PromoteBranch();
            evolution1.Result = "flameking";  // The evolved monster's ID
            evolution1.Details = new List<PromoteDetail>();

            // Add the level requirement
            // PromoteDetail can be various types: level, item, location, etc.
            evolution1.Details.Add(new PromoteLevelUp(16));  // Level 16+

            baseForm.Promotions.Add(evolution1);

            // Example: Add a second evolution branch requiring an item
            // PromoteBranch evolution2 = new PromoteBranch();
            // evolution2.Result = "flamedragon";
            // evolution2.Details.Add(new PromoteItem("fire_stone")); // Requires Fire Stone
            // baseForm.Promotions.Add(evolution2);

            // -----------------------------------------------------------------
            // STEP 3k: Configure visuals
            // -----------------------------------------------------------------
            // These determine how the monster looks in-game

            // TerrainType affects special floor tiles the monster can cross
            // 0 = Normal, 1 = Water walker, 2 = Lava walker, etc.
            baseForm.TerrainType = 0;

            // BodySize affects certain hitbox calculations
            baseForm.BodySize = 0;

            // -----------------------------------------------------------------
            // STEP 4: Add the base form to the monster
            // -----------------------------------------------------------------
            monster.Forms.Add(baseForm);

            // -----------------------------------------------------------------
            // STEP 5: Create an alternate form (optional)
            // -----------------------------------------------------------------
            // Alternate forms can have different stats, types, abilities, etc.
            // This is useful for mega evolutions, regional variants, etc.

            MonsterForm alternateForm = new MonsterForm();
            alternateForm.FormName = new LocalText("Solar Form");
            alternateForm.Released = true;

            // Different typing for the alternate form
            alternateForm.Element1 = "fire";
            alternateForm.Element2 = "flying";  // Secondary type: Flying

            // Boosted stats for the alternate form
            alternateForm.BaseHP = 78;
            alternateForm.BaseAtk = 84;
            alternateForm.BaseDef = 78;
            alternateForm.BaseMAtk = 109;
            alternateForm.BaseMDef = 85;
            alternateForm.BaseSpeed = 100;

            // Different ability
            alternateForm.Intrinsic1 = "drought";
            alternateForm.Intrinsic1Hidden = "solar_power";

            // Copy the base form's skills (can also define different ones)
            alternateForm.LevelSkills = new List<LevelUpSkill>(baseForm.LevelSkills);
            alternateForm.SharedSkills = new List<LearnableSkill>(baseForm.SharedSkills);
            alternateForm.SecretSkills = new List<LearnableSkill>(baseForm.SecretSkills);

            // Add a unique move only this form can learn
            alternateForm.LevelSkills.Add(new LevelUpSkill("solar_beam", 50));

            // This form doesn't evolve further
            alternateForm.Promotions = new List<PromoteBranch>();

            alternateForm.Gender = Gender.Genderless;
            alternateForm.EXPTable = GrowthGroup.MediumSlow;
            alternateForm.ExpYield = 240;  // Higher EXP yield for stronger form
            alternateForm.BaseFriendship = 70;
            alternateForm.Height = 17;   // Larger in this form
            alternateForm.Weight = 905;
            alternateForm.TerrainType = 2; // Can walk on lava
            alternateForm.BodySize = 2;    // Larger body

            // Add the alternate form
            monster.Forms.Add(alternateForm);

            // -----------------------------------------------------------------
            // STEP 6: Return the completed monster
            // -----------------------------------------------------------------
            // The MonsterData is now complete and can be saved/loaded

            return monster;
        }

        // =====================================================================
        // HELPER CLASS EXPLANATIONS
        // =====================================================================

        /// <summary>
        /// Example of accessing and using MonsterData after loading.
        /// </summary>
        public static void UseMonsterDataExample(string monsterId)
        {
            // Load a monster from the data system
            MonsterData monster = DataManager.Instance.GetMonster(monsterId);

            // Access the base form (form 0)
            MonsterForm baseForm = monster.Forms[0];

            // Get the localized name
            string name = monster.Name.ToLocal();

            // Get a specific form by index
            if (monster.Forms.Count > 1)
            {
                MonsterForm altForm = monster.Forms[1];
                string formName = altForm.FormName.ToLocal();
            }

            // Check what skills the monster can learn at a given level
            foreach (LevelUpSkill levelSkill in baseForm.LevelSkills)
            {
                if (levelSkill.Level <= 10) // Skills learned by level 10
                {
                    string skillId = levelSkill.Skill;
                    // Use the skill ID
                }
            }

            // Check evolutions
            foreach (PromoteBranch evolution in baseForm.Promotions)
            {
                string evolvesInto = evolution.Result;
                // Check evolution requirements
                foreach (PromoteDetail detail in evolution.Details)
                {
                    if (detail is PromoteLevelUp levelReq)
                    {
                        int requiredLevel = levelReq.Level;
                    }
                }
            }
        }
    }
}
