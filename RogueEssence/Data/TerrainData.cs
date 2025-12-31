using System;
using RogueEssence.Dungeon;
using RogueElements;
using Microsoft.Xna.Framework;
using RogueEssence.Dev;

namespace RogueEssence.Data
{
    /// <summary>
    /// Contains data for terrain types on dungeon tiles.
    /// Defines passability, elemental properties, and visual effects for terrain.
    /// </summary>
    [Serializable]
    public class TerrainData : PassiveData, IEntryData
    {
        /// <summary>
        /// Returns the localized name of the terrain.
        /// </summary>
        /// <returns>The terrain name as a string.</returns>
        public override string ToString()
        {
            return Name.ToLocal();
        }

        /// <summary>
        /// Flags indicating which movement types can pass through the terrain.
        /// </summary>
        [Flags]
        public enum Mobility
        {
            Impassable = -1,
            Passable = 0,
            Water = 1,
            Lava = 2,
            Abyss = 4,
            Block = 8,
            All = 15
        }

        /// <summary>
        /// Defines what happens to items landing on this terrain.
        /// </summary>
        public enum TileItemLand
        {
            Normal,
            Fall,
            Destroy
        }

        /// <summary>
        /// Defines how items appear visually on this terrain.
        /// </summary>
        public enum TileItemDraw
        {
            Normal,
            Transparent,
            Hide
        }

        /// <summary>
        /// Defines whether items can exist on this terrain.
        /// </summary>
        public enum TileItemAllowance
        {
            Allow,
            Force,
            Forbid
        }

        /// <summary>
        /// The localized name of the terrain.
        /// </summary>
        public LocalText Name { get; set; }

        /// <summary>
        /// Whether the terrain is released. Always returns true.
        /// </summary>
        public bool Released { get { return true; } }

        /// <summary>
        /// Developer comments for this terrain.
        /// </summary>
        [Dev.Multiline(0)]
        public string Comment { get; set; }

        /// <summary>
        /// Generates a summary of this terrain for indexing.
        /// </summary>
        /// <returns>An EntrySummary containing the terrain's metadata.</returns>
        public EntrySummary GenerateEntrySummary() { return new EntrySummary(Name, Released, Comment); }

        /// <summary>
        /// The elemental type associated with this terrain.
        /// </summary>
        [Dev.DataType(0, DataManager.DataType.Element, false)]
        public int Element;

        /// <summary>
        /// The mobility flags defining what can pass through this terrain.
        /// </summary>
        public Mobility BlockType;

        /// <summary>
        /// The color shown on the minimap for this terrain.
        /// </summary>
        public Color MinimapColor;

        /// <summary>
        /// Whether diagonal movement through this terrain is blocked.
        /// </summary>
        public bool BlockDiagonal;

        /// <summary>
        /// Whether this terrain blocks light/visibility.
        /// </summary>
        public bool BlockLight;

        /// <summary>
        /// The type of shadow cast by this terrain.
        /// </summary>
        public int ShadowType;

        /// <summary>
        /// What happens when an item lands on this tile.
        /// </summary>
        public TileItemLand ItemLand;

        /// <summary>
        /// How the item's visibility is affected when it's on this tile.
        /// </summary>
        public TileItemDraw ItemDraw;

        /// <summary>
        /// Determines whether items are allowed on this tile, if they have to be forced on, or if they flat out forbid them.
        /// </summary>
        public TileItemAllowance ItemAllow;

        /// <summary>
        /// Special variables that this terrain contains.
        /// They are potentially checked against in a select number of battle events.
        /// </summary>
        [ListCollapse]
        public StateCollection<TerrainState> TerrainStates;

        /// <summary>
        /// Events triggered when a character lands on this terrain.
        /// </summary>
        [ListCollapse]
        public PriorityList<SingleCharEvent> LandedOnTiles;

        /// <summary>
        /// Initializes a new instance of the TerrainData class with default values.
        /// </summary>
        public TerrainData()
        {
            Name = new LocalText();
            Comment = "";
            TerrainStates = new StateCollection<TerrainState>();
            LandedOnTiles = new PriorityList<SingleCharEvent>();
        }

        /// <summary>
        /// Gets the display name of the terrain.
        /// </summary>
        /// <returns>The terrain name as a string.</returns>
        public string GetColoredName()
        {
            return String.Format("{0}", Name.ToLocal());
        }
    }
}
