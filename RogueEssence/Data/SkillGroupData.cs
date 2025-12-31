using System;

namespace RogueEssence.Data
{
    /// <summary>
    /// Represents a skill group for categorizing learnable skills.
    /// Monsters can belong to skill groups to learn shared TM/tutor moves.
    /// </summary>
    [Serializable]
    public class SkillGroupData : IEntryData
    {
        /// <summary>
        /// Returns the localized name of the skill group.
        /// </summary>
        /// <returns>The skill group name as a string.</returns>
        public override string ToString()
        {
            return Name.ToLocal();
        }

        /// <summary>
        /// The localized name of the skill group.
        /// </summary>
        public LocalText Name { get; set; }

        /// <summary>
        /// Whether the skill group is released. Always returns true.
        /// </summary>
        public bool Released { get { return true; } }

        /// <summary>
        /// Developer comments for this skill group.
        /// </summary>
        [Dev.Multiline(0)]
        public string Comment { get; set; }

        /// <summary>
        /// Generates a summary of this skill group for indexing.
        /// </summary>
        /// <returns>An EntrySummary containing the skill group's metadata.</returns>
        public EntrySummary GenerateEntrySummary() { return new EntrySummary(Name, Released, Comment); }

        /// <summary>
        /// Initializes a new instance of the SkillGroupData class with default values.
        /// </summary>
        public SkillGroupData()
        {
            Name = new LocalText();
            Comment = "";
        }

        /// <summary>
        /// Initializes a new instance of the SkillGroupData class with the specified name.
        /// </summary>
        /// <param name="name">The localized name of the skill group.</param>
        public SkillGroupData(LocalText name)
        {
            Name = name;
            Comment = "";
        }

        /// <summary>
        /// Gets the display name with green color formatting.
        /// </summary>
        /// <returns>The formatted name string with color tags.</returns>
        public string GetColoredName()
        {
            return String.Format("[color=#00FF00]{0}[color]", Name.ToLocal());
        }
    }
}
