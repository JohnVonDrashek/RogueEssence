using System;

namespace RogueEssence.Dungeon
{
    /// <summary>
    /// Abstract base class for all game events that can be triggered during gameplay.
    /// Game events are cloneable effects that modify battle, status, or other game state.
    /// </summary>
    [Serializable]
    public abstract class GameEvent
    {
        /// <summary>
        /// Creates a deep copy of this game event.
        /// </summary>
        /// <returns>A clone of this game event.</returns>
        public abstract GameEvent Clone();
    }
}
