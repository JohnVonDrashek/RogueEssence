using RogueElements;

namespace RogueEssence.Dungeon
{
    /// <summary>
    /// Provides static arrays for remapping direction indices to Dir8 values in different orderings.
    /// Used for input processing and movement calculations.
    /// </summary>
    public static class DirRemap
    {

        /// <summary>
        /// Direction mapping in wrapped order (cardinal directions first, then diagonals).
        /// Layout: 526 / 1X3 / 407 where X is center.
        /// </summary>
        public static readonly Dir8[] WRAPPED_DIR8 = { Dir8.Down, Dir8.Left, Dir8.Up, Dir8.Right, Dir8.DownLeft, Dir8.UpLeft, Dir8.UpRight, Dir8.DownRight };

        /// <summary>
        /// Direction mapping in focused order (prioritizes directions facing the character).
        /// Layout: 576 / 3X4 / 102 where X is center.
        /// </summary>
        public static readonly Dir8[] FOCUSED_DIR8 = { Dir8.Down, Dir8.DownLeft, Dir8.DownRight, Dir8.Left, Dir8.Right, Dir8.UpLeft, Dir8.UpRight, Dir8.Up };

    }
}
