using Newtonsoft.Json;
using RogueEssence.Dev;
using System;

namespace RogueEssence.Dungeon
{
    /// <summary>
    /// Represents a skill in a character's base skill slot, including the skill ID, charges, and forget permission.
    /// This is the persistent skill data that gets converted to Skill for active use.
    /// </summary>
    [Serializable]
    public class SlotSkill
    {
        /// <summary>
        /// The unique identifier of the skill.
        /// </summary>
        [JsonConverter(typeof(SkillConverter))]
        public string SkillNum;

        /// <summary>
        /// The remaining charges/PP for this skill.
        /// </summary>
        public int Charges;

        /// <summary>
        /// Whether this skill can be forgotten or replaced.
        /// </summary>
        public bool CanForget;

        /// <summary>
        /// Initializes a new empty SlotSkill.
        /// </summary>
        public SlotSkill() : this("") { }

        /// <summary>
        /// Initializes a new SlotSkill with the specified skill ID.
        /// </summary>
        /// <param name="skillNum">The skill identifier.</param>
        public SlotSkill(string skillNum)
        {
            SkillNum = skillNum;
            CanForget = true;
        }

        /// <summary>
        /// Creates a copy of another SlotSkill.
        /// </summary>
        /// <param name="other">The SlotSkill to copy.</param>
        public SlotSkill(SlotSkill other)
        {
            SkillNum = other.SkillNum;
            CanForget = other.CanForget;
        }
    }
}
