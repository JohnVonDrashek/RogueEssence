using Newtonsoft.Json;
using RogueEssence.Dev;
using System;

namespace RogueEssence.Data
{
    /// <summary>
    /// Represents a skill that is learned at a specific level.
    /// Used in monster level-up move lists.
    /// </summary>
    [Serializable]
    public class LevelUpSkill : LearnableSkill
    {
        /// <summary>
        /// The level at which this skill is learned.
        /// </summary>
        public int Level;

        /// <summary>
        /// Initializes a new instance of the LevelUpSkill class.
        /// </summary>
        public LevelUpSkill()
        {
        }

        /// <summary>
        /// Initializes a new instance of the LevelUpSkill class with the specified skill and level.
        /// </summary>
        /// <param name="skill">The skill ID.</param>
        /// <param name="level">The level at which the skill is learned.</param>
        public LevelUpSkill(string skill, int level) : base(skill)
        {
            Level = level;
        }

        /// <summary>
        /// Returns a string representation showing the level and skill name.
        /// </summary>
        /// <returns>A formatted string like "[Lv. X] SkillName".</returns>
        public override string ToString()
        {
            return "[Lv. " + Level + "] " + DataManager.Instance.GetSkill(Skill).Name.ToLocal();
        }

        /// <summary>
        /// Determines whether two LevelUpSkill objects are equal.
        /// </summary>
        /// <param name="obj">The object to compare with.</param>
        /// <returns>True if the skills and levels match.</returns>
        public override bool Equals(object obj)
        {
            LevelUpSkill other = obj as LevelUpSkill;
            if (other == null)
                return false;
            return other.Skill == Skill && other.Level == Level;
        }

        /// <summary>
        /// Gets the hash code for this LevelUpSkill.
        /// </summary>
        /// <returns>A hash code based on level and skill.</returns>
        public override int GetHashCode()
        {
            return Level.GetHashCode() ^ Skill.GetHashCode();
        }
    }

    /// <summary>
    /// Represents a skill that can be learned by a monster.
    /// Base class for different types of learnable skills.
    /// </summary>
    [Serializable]
    public class LearnableSkill
    {
        /// <summary>
        /// The ID of the skill that can be learned.
        /// </summary>
        [JsonConverter(typeof(SkillConverter))]
        [DataType(0, DataManager.DataType.Skill, false)]
        public string Skill;

        /// <summary>
        /// Initializes a new instance of the LearnableSkill class.
        /// </summary>
        public LearnableSkill()
        {
            Skill = "";
        }

        /// <summary>
        /// Initializes a new instance of the LearnableSkill class with the specified skill.
        /// </summary>
        /// <param name="skill">The skill ID.</param>
        public LearnableSkill(string skill)
        {
            Skill = skill;
        }

        /// <summary>
        /// Returns the localized name of the skill.
        /// </summary>
        /// <returns>The skill's display name.</returns>
        public override string ToString()
        {
            return DataManager.Instance.GetSkill(Skill).Name.ToLocal();
        }

        /// <summary>
        /// Determines whether two LearnableSkill objects are equal.
        /// </summary>
        /// <param name="obj">The object to compare with.</param>
        /// <returns>True if the skills match.</returns>
        public override bool Equals(object obj)
        {
            LearnableSkill other = obj as LearnableSkill;
            if (other == null)
                return false;
            return other.Skill == Skill;
        }

        /// <summary>
        /// Gets the hash code for this LearnableSkill.
        /// </summary>
        /// <returns>A hash code based on the skill ID.</returns>
        public override int GetHashCode()
        {
            return Skill.GetHashCode();
        }
    }
}
