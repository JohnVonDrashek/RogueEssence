using System;
using RogueEssence.Dungeon;

namespace RogueEssence.Data
{
    /// <summary>
    /// Represents auto-tile data used for automatic tile placement in dungeon generation.
    /// Auto-tiles automatically connect and blend with adjacent tiles of the same type.
    /// </summary>
    [Serializable]
    public class AutoTileData : IEntryData
    {
        /// <summary>
        /// Returns the localized name of this auto-tile.
        /// </summary>
        /// <returns>The localized name string.</returns>
        public override string ToString()
        {
            return Name.ToLocal();
        }

        /// <summary>
        /// The localized display name of this auto-tile.
        /// </summary>
        public LocalText Name { get; set; }

        /// <summary>
        /// Indicates whether this auto-tile is released for gameplay. Always returns true.
        /// </summary>
        public bool Released { get { return true; } }

        /// <summary>
        /// Developer comment describing this auto-tile.
        /// </summary>
        [Dev.Multiline(0)]
        public string Comment { get; set; }

        /// <summary>
        /// Generates a summary of this auto-tile for indexing purposes.
        /// </summary>
        /// <returns>An EntrySummary containing the auto-tile's metadata.</returns>
        public EntrySummary GenerateEntrySummary() { return new EntrySummary(Name, Released, Comment); }

        /// <summary>
        /// The auto-tile configuration that defines how tiles connect and display.
        /// </summary>
        public AutoTileBase Tiles;

        /// <summary>
        /// Initializes a new instance of the AutoTileData class with default values.
        /// </summary>
        public AutoTileData()
        {
            Name = new LocalText();
            Comment = "";
        }

        /// <summary>
        /// Gets the display name of the auto-tile with color formatting.
        /// </summary>
        /// <returns>The formatted name string.</returns>
        public string GetColoredName()
        {
            return String.Format("{0}", Name.ToLocal());
        }
    }
}
