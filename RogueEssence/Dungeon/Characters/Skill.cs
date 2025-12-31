using System;
using Newtonsoft.Json;
using RogueEssence.Data;
using RogueEssence.Dev;

namespace RogueEssence.Dungeon
{
    /// <summary>
    /// Represents an active skill on a character, including charges, enabled state, and sealed state.
    /// This is the runtime skill data used during dungeon exploration.
    /// </summary>
    [Serializable]
    public class Skill
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
        /// Whether this skill is currently enabled for use.
        /// </summary>
        public bool Enabled;

        /// <summary>
        /// Whether this skill has been sealed by an effect and cannot be used.
        /// </summary>
        public bool Sealed;

        /// <summary>
        /// Initializes a new empty Skill with no charges.
        /// </summary>
        public Skill()
            : this("", 0)
        { }

        /// <summary>
        /// Initializes a new Skill with the specified ID and charges, enabled by default.
        /// </summary>
        /// <param name="skillNum">The skill identifier.</param>
        /// <param name="charges">The number of charges.</param>
        public Skill(string skillNum, int charges)
            : this(skillNum, charges, true)
        { }

        /// <summary>
        /// Initializes a new Skill with full specification.
        /// </summary>
        /// <param name="skillNum">The skill identifier.</param>
        /// <param name="charges">The number of charges.</param>
        /// <param name="enabled">Whether the skill is enabled.</param>
        public Skill(string skillNum, int charges, bool enabled)
        {
            SkillNum = skillNum;
            Charges = charges;
            Enabled = enabled;
        }
    }
}
