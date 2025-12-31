using System;
using System.Diagnostics.CodeAnalysis;

namespace RogueEssence.Dungeon
{
    /// <summary>
    /// Represents a location within a zone segment, combining segment index and floor ID.
    /// A negative segment value with a positive ID refers to ground maps.
    /// </summary>
    [Serializable]
    public struct SegLoc
    {
        /// <summary>
        /// The segment index within the zone. Negative values indicate ground maps.
        /// </summary>
        public int Segment;

        /// <summary>
        /// The floor ID within the segment.
        /// </summary>
        public int ID;

        /// <summary>
        /// Initializes a new SegLoc with the specified segment and floor ID.
        /// </summary>
        /// <param name="segment">The segment index.</param>
        /// <param name="id">The floor ID within the segment.</param>
        public SegLoc(int segment, int id)
        {
            Segment = segment;
            ID = id;
        }

        private static readonly SegLoc invalid = new SegLoc(-1, -1);

        /// <summary>
        /// Gets an invalid SegLoc instance with both segment and ID set to -1.
        /// </summary>
        public static SegLoc Invalid { get { return invalid; } }

        /// <summary>
        /// Determines whether this SegLoc represents a valid location.
        /// </summary>
        /// <returns>True if the ID is greater than -1; otherwise, false.</returns>
        public bool IsValid()
        {
            return (ID > -1);
        }

        /// <summary>
        /// Returns a string representation of this SegLoc.
        /// </summary>
        /// <returns>A string containing the segment and ID values.</returns>
        public override string ToString()
        {
            return String.Format("{0} {1}", Segment, ID);
        }

        /// <summary>
        /// Determines whether this SegLoc equals another object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>True if the object is a SegLoc with the same segment and ID; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is SegLoc))
                return false;

            SegLoc other = (SegLoc)obj;

            return this.Segment == other.Segment && this.ID == other.ID;
        }
    }
}