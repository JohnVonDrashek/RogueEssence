using System;
using RogueEssence.Dungeon;
using RogueEssence.Content;
using RogueElements;
using Microsoft.Xna.Framework;
using RogueEssence.Dev;

namespace RogueEssence.Data
{
    /// <summary>
    /// Contains data for interactive tile objects like traps, switches, and passages.
    /// Defines appearance, interaction type, and triggered effects.
    /// </summary>
    [Serializable]
    public class TileData : PassiveData, IDescribedData
    {
        /// <summary>
        /// Returns the localized name of the tile.
        /// </summary>
        /// <returns>The tile name as a string.</returns>
        public override string ToString()
        {
            return Name.ToLocal();
        }

        /// <summary>
        /// Defines how the tile can be triggered or interacted with.
        /// </summary>
        public enum TriggerType
        {
            None,
            Site,
            Passage,
            Trap,
            Switch,
            Blocker,
            Unlockable
        }

        /// <summary>
        /// The name of the data
        /// </summary>
        public LocalText Name { get; set; }

        /// <summary>
        /// The description of the data
        /// </summary>
        [Dev.Multiline(0)]
        public LocalText Desc { get; set; }

        /// <summary>
        /// Is it released and allowed to show up in the game?
        /// </summary>
        public bool Released { get; set; }

        /// <summary>
        /// Comments visible to only developers
        /// </summary>
        [Dev.Multiline(0)]
        public string Comment { get; set; }

        /// <summary>
        /// Generates a summary of this tile for indexing.
        /// </summary>
        /// <returns>An EntrySummary containing the tile's metadata.</returns>
        public EntrySummary GenerateEntrySummary() { return new EntrySummary(Name, Released, Comment); }

        /// <summary>
        /// The object animation used for this tile
        /// </summary>
        public ObjAnimData Anim;

        /// <summary>
        /// The offset for which to draw the object animation
        /// </summary>
        public Loc Offset;


        /// <summary>
        /// The layer to draw the tile on. Only supports Bottom, Back, and Front for now.
        /// </summary>
        public DrawLayer Layer;

        //public bool BlockLight;

        /// <summary>
        /// Prevents items from landing on it.
        /// </summary>
        public bool BlockItem;

        /// <summary>
        /// Determines how the tile can be interacted with
        /// </summary>
        public TriggerType StepType;

        /// <summary>
        /// Texture for the minimap icon
        /// </summary>
        public Loc MinimapIcon;

        /// <summary>
        /// Color for the minimap icon
        /// </summary>
        public Color MinimapColor;

        /// <summary>
        /// What happens when a character walks on the tile.
        /// Also triggers if forced on or winds up on it in any other way.
        /// </summary>
        [ListCollapse]
        public PriorityList<SingleCharEvent> LandedOnTiles;

        /// <summary>
        /// What happens when the character voluntarily triggers the tile.
        /// </summary>
        [ListCollapse]
        public PriorityList<SingleCharEvent> InteractWithTiles;

        /// <summary>
        /// Initializes a new instance of the TileData class with default values.
        /// </summary>
        public TileData()
        {
            Name = new LocalText();
            Desc = new LocalText();
            Comment = "";
            Anim = new ObjAnimData();
            LandedOnTiles = new PriorityList<SingleCharEvent>();
            InteractWithTiles = new PriorityList<SingleCharEvent>();
        }

        /// <summary>
        /// Gets the colored name for the tile
        /// </summary>
        /// <returns></returns>
        public string GetColoredName()
        {
            return String.Format("[color=#00FF00]{0}[color]", Name.ToLocal());
        }
    }
}
