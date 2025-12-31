using System;

namespace RogueEssence.LevelGen
{
    /// <summary>
    /// Flags representing post-processing status types for tiles during map generation.
    /// Used to track what has been placed or modified on a tile.
    /// </summary>
    [Flags]
    public enum PostProcType
    {
        /// <summary>
        /// No post-processing status.
        /// </summary>
        None = 0,

        /// <summary>
        /// Terrain has been placed or modified on this tile.
        /// </summary>
        Terrain = 1,

        /// <summary>
        /// A panel (floor effect) has been placed on this tile.
        /// </summary>
        Panel = 2,

        /// <summary>
        /// An item has been placed on this tile.
        /// </summary>
        Item = 4,
    }

    /// <summary>
    /// Represents post-processing information for a single tile during map generation.
    /// Tracks what elements have been placed or modified on the tile.
    /// </summary>
    public class PostProcTile
    {
        /// <summary>
        /// The current post-processing status flags for this tile.
        /// </summary>
        public PostProcType Status;

        /// <summary>
        /// Initializes a new instance of the PostProcTile class with no status.
        /// </summary>
        public PostProcTile()
        { }

        /// <summary>
        /// Initializes a new instance of the PostProcTile class with the specified status.
        /// </summary>
        /// <param name="status">The initial post-processing status.</param>
        public PostProcTile(PostProcType status)
        {
            Status = status;
        }

        /// <summary>
        /// Initializes a new instance of the PostProcTile class as a copy of another tile.
        /// </summary>
        /// <param name="other">The tile to copy.</param>
        public PostProcTile(PostProcTile other) : this()
        {
            Status = other.Status;
        }

        /// <summary>
        /// Adds the status flags from another tile to this tile using bitwise OR.
        /// </summary>
        /// <param name="other">The tile whose status flags to add.</param>
        public void AddMask(PostProcTile other)
        {
            Status |= other.Status;
        }
    }
}
