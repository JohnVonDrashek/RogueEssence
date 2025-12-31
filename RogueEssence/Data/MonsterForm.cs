using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using RogueElements;
using RogueEssence.Dev;

namespace RogueEssence.Data
{
    /// <summary>
    /// Abstract base class for monster forms. Each form defines a monster's stats,
    /// types, abilities, and learnable skills for a specific variation of a species.
    /// </summary>
    [Serializable]
    public abstract class BaseMonsterForm
    {
        /// <summary>
        /// Monster's form title
        /// </summary>
        public LocalText FormName;

        /// <summary>
        /// Is it released and allowed to show up in the game?
        /// </summary>
        public bool Released { get; set; }

        /// <summary>
        /// Is it a temporary form?  Cannot be picked in rogue mode if so.
        /// </summary>
        public bool Temporary;

        /// <summary>
        /// Monster FORM this was promoted from
        /// </summary>
        public int PromoteForm;

        /// <summary>
        /// elemental typing 1
        /// </summary>
        [JsonConverter(typeof(ElementConverter))]
        [Dev.DataType(0, DataManager.DataType.Element, false)]
        public string Element1;

        /// <summary>
        /// elemental typing 2
        /// </summary>
        [JsonConverter(typeof(ElementConverter))]
        [Dev.SharedRow, Dev.DataType(0, DataManager.DataType.Element, false)]
        public string Element2;

        /// <summary>
        /// possible intrinsic 1
        /// </summary>
        [JsonConverter(typeof(IntrinsicConverter))]
        [Dev.DataType(0, DataManager.DataType.Intrinsic, false)]
        public string Intrinsic1;

        /// <summary>
        /// possible intrinsic 2
        /// </summary>
        [JsonConverter(typeof(IntrinsicConverter))]
        [Dev.DataType(0, DataManager.DataType.Intrinsic, false)]
        public string Intrinsic2;

        /// <summary>
        /// possible intrinsic 3
        /// </summary>
        [JsonConverter(typeof(IntrinsicConverter))]
        [Dev.DataType(0, DataManager.DataType.Intrinsic, false)]
        public string Intrinsic3;


        /// <summary>
        /// skills learned on level up
        /// </summary>
        public List<LevelUpSkill> LevelSkills;



        /// <summary>
        /// Generates a summary of this form for indexing purposes.
        /// </summary>
        /// <returns>A BaseFormSummary containing the form's metadata.</returns>
        public BaseFormSummary GenerateEntrySummary()
        {
            return new BaseFormSummary(FormName, Released, Temporary);
        }

        /// <summary>
        /// Initializes a new instance of the BaseMonsterForm class with default values.
        /// </summary>
        public BaseMonsterForm()
        {
            FormName = new LocalText();
            LevelSkills = new List<LevelUpSkill>();

            // TODO: Initialize to default element, when we can guarantee that DataManager.Instance.DefaultElement itself is initialized
            Element1 = "";
            Element2 = "";
            
            // TODO: Make invalid intrinsic represent no-ability, not default
            Intrinsic1 = "";
            Intrinsic2 = "";
            Intrinsic3 = "";
        }



        /// <summary>
        /// Returns the form name, or "[EMPTY]" if none is set.
        /// </summary>
        /// <returns>The form name string.</returns>
        public override string ToString()
        {
            if (!String.IsNullOrEmpty(FormName.DefaultText))
                return FormName.DefaultText;
            else
                return "[EMPTY]";
        }

        /// <summary>
        /// Calculates the stat value at a given level with bonus.
        /// </summary>
        /// <param name="level">The monster's level.</param>
        /// <param name="stat">The stat to calculate.</param>
        /// <param name="bonus">Bonus value to add.</param>
        /// <returns>The calculated stat value.</returns>
        public abstract int GetStat(int level, Stat stat, int bonus);

        /// <summary>
        /// Gets the maximum value for a stat at the given level.
        /// </summary>
        /// <param name="stat">The stat to check.</param>
        /// <param name="level">The monster's level.</param>
        /// <returns>The maximum stat value.</returns>
        public abstract int GetMaxStat(Stat stat, int level);

        /// <summary>
        /// Reverse calculates what bonus would produce the given stat value.
        /// </summary>
        /// <param name="stat">The stat type.</param>
        /// <param name="val">The target stat value.</param>
        /// <param name="level">The monster's level.</param>
        /// <returns>The bonus value that would produce the target stat.</returns>
        public abstract int ReverseGetStat(Stat stat, int val, int level);

        /// <summary>
        /// Gets the maximum possible stat bonus for a given stat.
        /// </summary>
        /// <param name="stat">The stat type.</param>
        /// <returns>The maximum bonus value.</returns>
        public abstract int GetMaxStatBonus(Stat stat);

        /// <summary>
        /// Checks if this form can learn the specified skill.
        /// </summary>
        /// <param name="skill">The skill ID to check.</param>
        /// <returns>True if the skill can be learned.</returns>
        public abstract bool CanLearnSkill(string skill);

        /// <summary>
        /// Randomly selects a skin for this form.
        /// </summary>
        /// <param name="rand">Random number generator.</param>
        /// <returns>The selected skin ID.</returns>
        public abstract string RollSkin(IRandom rand);

        /// <summary>
        /// Gets the personality type based on a discriminator value.
        /// </summary>
        /// <param name="discriminator">The discriminator value.</param>
        /// <returns>The personality type index.</returns>
        public abstract int GetPersonalityType(int discriminator);

        /// <summary>
        /// Randomly selects a gender for this form based on gender ratios.
        /// </summary>
        /// <param name="rand">Random number generator.</param>
        /// <returns>The selected gender.</returns>
        public abstract Gender RollGender(IRandom rand);

        /// <summary>
        /// Randomly selects an intrinsic ability within the specified bounds.
        /// </summary>
        /// <param name="rand">Random number generator.</param>
        /// <param name="bounds">The number of intrinsic slots to consider.</param>
        /// <returns>The selected intrinsic ID.</returns>
        public abstract string RollIntrinsic(IRandom rand, int bounds);

        /// <summary>
        /// Gets all possible genders for this form.
        /// </summary>
        /// <returns>List of possible genders.</returns>
        public abstract List<Gender> GetPossibleGenders();

        /// <summary>
        /// Gets all possible skins for this form.
        /// </summary>
        /// <returns>List of possible skin IDs.</returns>
        public abstract List<string> GetPossibleSkins();

        /// <summary>
        /// Gets the indices of valid intrinsic slots.
        /// </summary>
        /// <returns>List of valid intrinsic slot indices.</returns>
        public abstract List<int> GetPossibleIntrinsicSlots();

        /// <summary>
        /// Gets all skills learned at or before a specific level.
        /// </summary>
        /// <param name="levelLearned">The level to check.</param>
        /// <param name="relearn">Whether to include skills from earlier levels.</param>
        /// <returns>An enumerable of skill IDs.</returns>
        public IEnumerable<string> GetSkillsAtLevel(int levelLearned, bool relearn)
        {
            for (int ii = 0; ii < LevelSkills.Count; ii++)
            {
                if (LevelSkills[ii].Level == levelLearned || LevelSkills[ii].Level <= levelLearned && relearn)
                {
                    if (DataManager.Instance.DataIndices[DataManager.DataType.Skill].Get(LevelSkills[ii].Skill).Released)
                        yield return LevelSkills[ii].Skill;
                }
            }
        }

        /// <summary>
        /// Generates a list of skills for a monster at the given level, prioritizing recent level-up skills.
        /// </summary>
        /// <param name="level">The monster's current level.</param>
        /// <param name="specifiedSkills">Skills that must be included.</param>
        /// <returns>A list of skill IDs, up to the maximum skill slots.</returns>
        public List<string> RollLatestSkills(int level, List<string> specifiedSkills)
        {
            List<string> skills = new List<string>();
            skills.AddRange(specifiedSkills);

            for (int ii = LevelSkills.Count - 1; ii >= 0 && Dungeon.CharData.MAX_SKILL_SLOTS - skills.Count > 0; ii--)
            {
                if (LevelSkills[ii].Level <= level && !skills.Contains(LevelSkills[ii].Skill))
                {
                    if (DataManager.Instance.DataIndices[DataManager.DataType.Skill].Get(LevelSkills[ii].Skill).Released)
                        skills.Insert(specifiedSkills.Count, LevelSkills[ii].Skill);
                }
            }
            return skills;
        }

    }


    /// <summary>
    /// Summary data for a monster form, used for quick access without loading full form data.
    /// </summary>
    [Serializable]
    public class BaseFormSummary
    {
        /// <summary>
        /// The localized name of the form.
        /// </summary>
        public LocalText Name;

        /// <summary>
        /// Whether this form is released for gameplay.
        /// </summary>
        public bool Released;

        /// <summary>
        /// Whether this is a temporary form that cannot be selected in certain modes.
        /// </summary>
        public bool Temporary;

        /// <summary>
        /// Initializes a new instance of the BaseFormSummary class.
        /// </summary>
        public BaseFormSummary() : base()
        {
            Name = new LocalText();
        }

        /// <summary>
        /// Initializes a new instance of the BaseFormSummary class with the specified values.
        /// </summary>
        /// <param name="name">The localized name of the form.</param>
        /// <param name="released">Whether the form is released.</param>
        /// <param name="temporary">Whether the form is temporary.</param>
        public BaseFormSummary(LocalText name, bool released, bool temporary)
        {
            Name = name;
            Released = released;
            Temporary = temporary;
        }
    }


    /// <summary>
    /// Defines the possible genders for monsters.
    /// </summary>
    public enum Gender
    {
        Unknown = -1,
        Genderless = 0,
        Male = 1,
        Female = 2
    }

    /// <summary>
    /// Defines the different character statistics.
    /// </summary>
    public enum Stat
    {
        None = -1,
        HP = 0,
        Attack = 1,
        Defense = 2,
        MAtk = 3,
        MDef = 4,
        Speed = 5,
        HitRate = 6,
        DodgeRate = 7,
        Range = 8
    };



}