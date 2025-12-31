using Microsoft.Xna.Framework;
using RogueElements;

namespace RogueEssence
{
    /// <summary>
    /// Provides extension methods for converting between XNA/MonoGame types and RogueElements types.
    /// </summary>
    public static class XNAExt
    {
        /// <summary>
        /// Converts a Loc to a Vector2.
        /// </summary>
        /// <param name="loc">The Loc to convert.</param>
        /// <returns>A Vector2 with the same X and Y values.</returns>
        public static Vector2 ToVector2(this Loc loc)
        {
            return new Vector2(loc.X, loc.Y);
        }

        /// <summary>
        /// Converts a Vector2 to a Loc.
        /// </summary>
        /// <param name="loc">The Vector2 to convert.</param>
        /// <returns>A Loc with the X and Y values cast to integers.</returns>
        public static Loc ToLoc(this Vector2 loc)
        {
            return new Loc((int)loc.X, (int)loc.Y);
        }
    }
}
