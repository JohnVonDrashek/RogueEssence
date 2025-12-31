using System;
using RogueElements;

namespace RogueEssence.Dungeon
{
    /// <summary>
    /// Represents a single frame reference for a tile's visual appearance.
    /// Contains the sprite sheet name and texture coordinates.
    /// </summary>
    [Serializable]
    public struct TileFrame
    {
        /// <summary>
        /// An empty TileFrame with no sheet reference.
        /// </summary>
        public static readonly TileFrame Empty = new TileFrame(new Loc(), "");

        /// <summary>
        /// The name of the sprite sheet containing this tile.
        /// </summary>
        public string Sheet;

        /// <summary>
        /// The texture coordinates within the sprite sheet.
        /// </summary>
        public Loc TexLoc;

        /// <summary>
        /// Initializes a new TileFrame with the specified texture location and sheet.
        /// </summary>
        /// <param name="texture">The texture coordinates.</param>
        /// <param name="sheet">The sprite sheet name.</param>
        public TileFrame(Loc texture, string sheet)
        {
            TexLoc = texture;
            Sheet = sheet;
        }

        /// <summary>
        /// Returns a string representation of this TileFrame.
        /// </summary>
        /// <returns>A string describing the tile location, or "[EMPTY]" if no sheet.</returns>
        public override string ToString()
        {
            if (Sheet == "")
                return "[EMPTY]";
            return String.Format("Tile {0}: {1}", Sheet, TexLoc.ToString());
        }

        /// <summary>
        /// Determines whether this TileFrame equals another object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>True if the object is an equal TileFrame; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return (obj is TileFrame) && Equals((TileFrame)obj);
        }

        /// <summary>
        /// Determines whether this TileFrame equals another TileFrame.
        /// </summary>
        /// <param name="other">The TileFrame to compare.</param>
        /// <returns>True if both texture location and sheet match; otherwise, false.</returns>
        public bool Equals(TileFrame other)
        {
            return (TexLoc == other.TexLoc && Sheet == other.Sheet);
        }

        /// <summary>
        /// Gets the hash code for this TileFrame.
        /// </summary>
        /// <returns>A hash code based on texture location and sheet.</returns>
        public override int GetHashCode()
        {
            return TexLoc.GetHashCode() ^ Sheet.GetHashCode();
        }

        /// <summary>
        /// Tests equality between two TileFrame values.
        /// </summary>
        public static bool operator ==(TileFrame value1, TileFrame value2)
        {
            return value1.Equals(value2);
        }

        /// <summary>
        /// Tests inequality between two TileFrame values.
        /// </summary>
        public static bool operator !=(TileFrame value1, TileFrame value2)
        {
            return !(value1 == value2);
        }

    }
}
