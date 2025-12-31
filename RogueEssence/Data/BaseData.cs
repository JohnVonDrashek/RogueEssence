using System;
using RogueEssence.Dungeon;
using RogueElements;

namespace RogueEssence.Data
{
    /// <summary>
    /// Abstract base class for universal game data that can be triggered and reindexed
    /// when content changes. Used for maintaining cross-referenced data structures.
    /// </summary>
    [Serializable]
    public abstract class BaseData
    {
        /// <summary>
        /// Gets the file name used to save and load this data.
        /// </summary>
        public abstract string FileName { get; }

        /// <summary>
        /// Gets the data type that triggers updates to this data when content changes.
        /// </summary>
        public abstract DataManager.DataType TriggerType { get; }

        /// <summary>
        /// Called when content of the trigger type is modified, allowing this data to update accordingly.
        /// </summary>
        /// <param name="idx">The index of the content that changed.</param>
        public abstract void ContentChanged(string idx);

        /// <summary>
        /// Rebuilds all indices and references in this data structure.
        /// </summary>
        public abstract void ReIndex();
    }
}
