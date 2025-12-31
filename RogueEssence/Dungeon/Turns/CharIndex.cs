using System;

namespace RogueEssence.Dungeon
{
    /// <summary>
    /// Represents the allegiance of a team in the dungeon.
    /// </summary>
    public enum Faction
    {
        /// <summary>No faction (invalid).</summary>
        None = -1,
        /// <summary>The player's team.</summary>
        Player = 0,
        /// <summary>Friendly NPCs or allies.</summary>
        Friend = 1,
        /// <summary>Enemy teams.</summary>
        Foe = 2
    }

    /// <summary>
    /// Uniquely identifies a character within the dungeon by faction, team, guest status, and member index.
    /// Used for turn order management and character lookups.
    /// </summary>
    [Serializable]
    public struct CharIndex : IComparable<CharIndex>, IEquatable<CharIndex>
    {
        /// <summary>
        /// The faction this character belongs to.
        /// </summary>
        public Faction Faction;

        /// <summary>
        /// The team index within the faction.
        /// </summary>
        public int Team;

        /// <summary>
        /// Whether this character is a guest member of the team.
        /// </summary>
        public bool Guest;

        /// <summary>
        /// The character's index within the team's member list.
        /// </summary>
        public int Char;

        /// <summary>
        /// Initializes a new CharIndex with all identification properties.
        /// </summary>
        /// <param name="faction">The character's faction.</param>
        /// <param name="teamIndex">The team index.</param>
        /// <param name="guest">Whether the character is a guest.</param>
        /// <param name="memberIndex">The member index within the team.</param>
        public CharIndex(Faction faction, int teamIndex, bool guest, int memberIndex)
        {
            Faction = faction;
            Team = teamIndex;
            Guest = guest;
            Char = memberIndex;
        }
        private static readonly CharIndex invalid = new CharIndex(Faction.None, -1, false, -1);

        /// <summary>
        /// Gets an invalid CharIndex instance.
        /// </summary>
        public static CharIndex Invalid { get { return invalid; } }

        /// <summary>
        /// Determines whether this CharIndex equals another object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>True if the object is a CharIndex with the same values; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return (obj is CharIndex) && Equals((CharIndex)obj);
        }

        /// <summary>
        /// Determines whether this CharIndex equals another CharIndex.
        /// </summary>
        /// <param name="other">The CharIndex to compare.</param>
        /// <returns>True if all properties match; otherwise, false.</returns>
        public bool Equals(CharIndex other)
        {
            return (Faction == other.Faction && Team == other.Team && Guest == other.Guest && Char == other.Char);
        }

        /// <summary>
        /// Gets the hash code for this CharIndex.
        /// </summary>
        /// <returns>A hash code based on team and character index.</returns>
        public override int GetHashCode()
        {
            return Team.GetHashCode() ^ Char.GetHashCode();
        }

        /// <summary>
        /// Tests equality between two CharIndex values.
        /// </summary>
        public static bool operator ==(CharIndex value1, CharIndex value2)
        {
            return value1.Equals(value2);
        }

        /// <summary>
        /// Tests inequality between two CharIndex values.
        /// </summary>
        public static bool operator !=(CharIndex value1, CharIndex value2)
        {
            return !(value1 == value2);
        }

        /// <summary>
        /// Tests if one CharIndex is greater than another.
        /// </summary>
        public static bool operator >(CharIndex value1, CharIndex value2)
        {
            return value1.CompareTo(value2) > 0;
        }

        /// <summary>
        /// Tests if one CharIndex is less than another.
        /// </summary>
        public static bool operator <(CharIndex value1, CharIndex value2)
        {
            return value1.CompareTo(value2) < 0;
        }

        /// <summary>
        /// Tests if one CharIndex is greater than or equal to another.
        /// </summary>
        public static bool operator >=(CharIndex value1, CharIndex value2)
        {
            return value1.CompareTo(value2) >= 0;
        }

        /// <summary>
        /// Tests if one CharIndex is less than or equal to another.
        /// </summary>
        public static bool operator <=(CharIndex value1, CharIndex value2)
        {
            return value1.CompareTo(value2) <= 0;
        }

        /// <summary>
        /// Compares this CharIndex to another for ordering.
        /// </summary>
        /// <param name="other">The CharIndex to compare to.</param>
        /// <returns>A value indicating the relative order.</returns>
        public int CompareTo(CharIndex other)
        {
            // Invalid precedes everything else
            int cmp = this.Faction.CompareTo(other.Faction);
            if (cmp != 0)
                return cmp;

            cmp = this.Team.CompareTo(other.Team);
            if (cmp != 0)
                return cmp;

            cmp = this.Guest.CompareTo(other.Guest);
            if (cmp != 0)
                return cmp;


            cmp = this.Char.CompareTo(other.Char);
            if (cmp != 0)
                return cmp;

            return 0;
        }
    }
}
