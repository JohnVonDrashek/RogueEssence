using System;
using RogueElements;

namespace RogueEssence.Dungeon
{
    /// <summary>
    /// Determines the order in which game events are processed, using multiple priority levels for stable ordering.
    /// Events are sorted by priority, port priority, type, ID, and list index.
    /// </summary>
    public class GameEventPriority : IComparable<GameEventPriority>
    {
        /// <summary>
        /// Priority value for events triggered by the user of an action.
        /// </summary>
        public static readonly Priority USER_PORT_PRIORITY = new Priority(-2);

        /// <summary>
        /// Priority value for events triggered by the target of an action.
        /// </summary>
        public static readonly Priority TARGET_PORT_PRIORITY = new Priority(-1);

        /// <summary>
        /// Categorizes the source of a game event.
        /// </summary>
        public enum EventCause
        {
            /// <summary>No specific cause.</summary>
            None,
            /// <summary>Event from a skill.</summary>
            Skill,
            /// <summary>Event from a map status.</summary>
            MapState,
            /// <summary>Event from a tile effect.</summary>
            Tile,
            /// <summary>Event from terrain.</summary>
            Terrain,
            /// <summary>Event from a character status.</summary>
            Status,
            /// <summary>Event from an equipped item.</summary>
            Equip,
            /// <summary>Event from an intrinsic ability.</summary>
            Intrinsic
        }

        /// <summary>
        /// The developer-specified priority level. Lower values are processed first.
        /// </summary>
        public Priority Priority;

        /// <summary>
        /// Priority based on relationship to the action (user, target, or other).
        /// </summary>
        public Priority PortPriority;

        /// <summary>
        /// The type of event cause, used as a tiebreaker.
        /// </summary>
        public EventCause TypeID;

        /// <summary>
        /// The ID of the owning passive or battle effect, used as a tiebreaker.
        /// </summary>
        public string ID;

        /// <summary>
        /// The position in the effect list, used as a final tiebreaker.
        /// </summary>
        public int ListIndex;

        /// <summary>
        /// Initializes a new GameEventPriority with all ordering properties.
        /// </summary>
        /// <param name="priority">The main priority level.</param>
        /// <param name="portPriority">The port priority (user/target/other).</param>
        /// <param name="typeId">The event cause type.</param>
        /// <param name="id">The effect's ID.</param>
        /// <param name="listIndex">The position in the effect list.</param>
        public GameEventPriority(Priority priority, Priority portPriority, EventCause typeId, string id, int listIndex)
        {
            Priority = priority;
            PortPriority = portPriority;
            TypeID = typeId;
            ID = id;
            ListIndex = listIndex;
        }

        /// <summary>
        /// Compares this priority to another for ordering.
        /// </summary>
        /// <param name="other">The other priority to compare.</param>
        /// <returns>A negative value if this comes first, positive if other comes first, zero if equal.</returns>
        public int CompareTo(GameEventPriority other)
        {
            // If other is not a valid object reference, this instance is greater. 
            if (other == null) return 1;

            int priority = Priority.CompareTo(other.Priority);
            if (priority != 0)
                return priority;

            int portPriority = PortPriority.CompareTo(other.PortPriority);
            if (portPriority != 0)
                return portPriority;

            int typeId = TypeID.CompareTo(other.TypeID);
            if (typeId != 0)
                return typeId;

            int id = ID.CompareTo(other.ID);
            if (id != 0)
                return id;

            int index = ListIndex.CompareTo(other.ListIndex);
            if (index != 0)
                return index;

            return 0;
        }


    }
}
