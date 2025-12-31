using System;

namespace RogueEssence.Data
{
    /// <summary>
    /// Represents an elemental type in the game's type system.
    /// Elements define type matchups and resistances for battle calculations.
    /// </summary>
    [Serializable]
    public class ElementData : IEntryData
    {
        /// <summary>
        /// Returns the localized name of this element.
        /// </summary>
        /// <returns>The localized name string.</returns>
        public override string ToString()
        {
            return Name.ToLocal();
        }

        /// <summary>
        /// The localized display name of this element.
        /// </summary>
        public LocalText Name { get; set; }

        /// <summary>
        /// Indicates whether this element is released for gameplay. Always returns true.
        /// </summary>
        public bool Released { get { return true; } }

        /// <summary>
        /// Developer comment describing this element.
        /// </summary>
        [Dev.Multiline(0)]
        public string Comment { get; set; }

        /// <summary>
        /// Generates a summary of this element for indexing purposes.
        /// </summary>
        /// <returns>An EntrySummary containing the element's metadata.</returns>
        public EntrySummary GenerateEntrySummary() { return new EntrySummary(Name, Released, Comment); }

        /// <summary>
        /// The single character symbol representing this element type.
        /// </summary>
        public char Symbol;

        /// <summary>
        /// Initializes a new instance of the ElementData class with default values.
        /// </summary>
        public ElementData()
        {
            Name = new LocalText();
            Comment = "";
        }

        /// <summary>
        /// Initializes a new instance of the ElementData class with the specified name and symbol.
        /// </summary>
        /// <param name="name">The localized name of the element.</param>
        /// <param name="symbol">The character symbol for the element.</param>
        public ElementData(LocalText name, char symbol)
        {
            Name = name;
            Comment = "";
            Symbol = symbol;
        }

        /// <summary>
        /// Gets the display name of the element with white color formatting.
        /// </summary>
        /// <returns>The formatted name string with color tags.</returns>
        public string GetColoredName()
        {
            return String.Format("[color=#FFFFFF]{0}[color]", Name.ToLocal());
        }

        /// <summary>
        /// Gets the display name with the element symbol prepended.
        /// </summary>
        /// <returns>The symbol followed by the colored name.</returns>
        public string GetIconName()
        {
            return String.Format("{0}\u2060{1}", Symbol, GetColoredName());
        }
    }
}
