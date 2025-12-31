using System;
using RogueEssence.Content;
using Microsoft.Xna.Framework;

namespace RogueEssence.Data
{
    /// <summary>
    /// Represents a skin/appearance variant for monsters.
    /// Skins can have different visual properties and gameplay effects.
    /// </summary>
    [Serializable]
    public class SkinData : IEntryData
    {
        /// <summary>
        /// Returns the localized name of the skin.
        /// </summary>
        /// <returns>The skin name as a string.</returns>
        public override string ToString()
        {
            return Name.ToLocal();
        }

        /// <summary>
        /// The localized name of the skin.
        /// </summary>
        public LocalText Name { get; set; }

        /// <summary>
        /// Whether the skin is released. Always returns true.
        /// </summary>
        public bool Released { get { return true; } }

        /// <summary>
        /// Developer comments for this skin.
        /// </summary>
        [Dev.Multiline(0)]
        public string Comment { get; set; }

        /// <summary>
        /// Index number for sorting skins.
        /// </summary>
        public int IndexNum;
        
        /// <summary>
        /// The symbol displayed next the characters species name.
        /// </summary>
        public char Symbol;
        
        /// <summary>
        /// The color displayed on the minimap.
        /// </summary>
        public Color MinimapColor;
        
        /// <summary>
        /// The VFX effect played when becoming the team leader.
        /// </summary>
        public BattleFX LeaderFX;
        
        /// <summary>
        /// Whether to show the skin type in the member info menu.
        /// </summary>
        public bool Display;
        
        /// <summary>
        /// Whether the character with this skin can be sent home during Roguelocke.
        /// </summary>
        public bool Challenge;

        /// <summary>
        /// Generates a summary of this skin for indexing.
        /// </summary>
        /// <returns>An EntrySummary containing the skin's metadata.</returns>
        public EntrySummary GenerateEntrySummary()
        {
            return new EntrySummary(Name, Released, Comment, IndexNum);
        }

        /// <summary>
        /// Initializes a new instance of the SkinData class with default values.
        /// </summary>
        public SkinData()
        {
            Name = new LocalText();
            Comment = "";
        }

        /// <summary>
        /// Initializes a new instance of the SkinData class with the specified name and symbol.
        /// </summary>
        /// <param name="name">The localized name of the skin.</param>
        /// <param name="symbol">The symbol to display next to character names.</param>
        public SkinData(LocalText name, char symbol)
        {
            Name = name;
            Comment = "";
            Symbol = symbol;
            LeaderFX  = new BattleFX();
        }

        /// <summary>
        /// Gets the display name of the skin.
        /// </summary>
        /// <returns>The skin name as a string.</returns>
        public string GetColoredName()
        {
            return String.Format("{0}", Name.ToLocal());
        }
    }
}
