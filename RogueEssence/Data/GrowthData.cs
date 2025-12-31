using System;

namespace RogueEssence.Data
{
    /// <summary>
    /// Represents an experience growth rate table that determines how much experience
    /// is required to level up for monsters using this growth group.
    /// </summary>
    [Serializable]
    public class GrowthData : IEntryData
    {
        /// <summary>
        /// Returns the localized name of this growth group.
        /// </summary>
        /// <returns>The localized name string.</returns>
        public override string ToString()
        {
            return Name.ToLocal();
        }

        /// <summary>
        /// The localized display name of this growth group.
        /// </summary>
        public LocalText Name { get; set; }

        /// <summary>
        /// Indicates whether this growth group is released for gameplay. Always returns true.
        /// </summary>
        public bool Released { get { return true; } }

        /// <summary>
        /// Developer comment describing this growth group.
        /// </summary>
        [Dev.Multiline(0)]
        public string Comment { get; set; }

        /// <summary>
        /// Generates a summary of this growth group for indexing purposes.
        /// </summary>
        /// <returns>An EntrySummary containing the growth group's metadata.</returns>
        public EntrySummary GenerateEntrySummary() { return new EntrySummary(Name, Released, Comment); }

        /// <summary>
        /// The cumulative experience table. Each index represents the total EXP needed to reach that level.
        /// </summary>
        public int[] EXPTable;

        /// <summary>
        /// Initializes a new instance of the GrowthData class with default values.
        /// </summary>
        public GrowthData()
        {
            Name = new LocalText();
            Comment = "";
        }

        /// <summary>
        /// Initializes a new instance of the GrowthData class with the specified name and experience table.
        /// </summary>
        /// <param name="name">The localized name of the growth group.</param>
        /// <param name="expTable">The cumulative experience table.</param>
        public GrowthData(LocalText name, int[] expTable)
        {
            Name = name;
            Comment = "";
            EXPTable = expTable;
        }

        /// <summary>
        /// Gets the experience required to advance from the specified level to the next level.
        /// </summary>
        /// <param name="level">The current level.</param>
        /// <returns>The experience points needed to reach the next level.</returns>
        public int GetExpToNext(int level)
        {
            return GetExpTo(level, level + 1);
        }

        /// <summary>
        /// Gets the experience required to advance from one level to another.
        /// </summary>
        /// <param name="fromLevel">The starting level.</param>
        /// <param name="toLevel">The target level.</param>
        /// <returns>The experience points needed to reach the target level.</returns>
        public int GetExpTo(int fromLevel, int toLevel)
        {
            return EXPTable[toLevel - 1] - EXPTable[fromLevel - 1];
        }

        /// <summary>
        /// Gets the display name of the growth group with color formatting.
        /// </summary>
        /// <returns>The formatted name string.</returns>
        public string GetColoredName()
        {
            return String.Format("{0}", Name.ToLocal());
        }
    }
}
