using System;

namespace RogueEssence.Dungeon
{
    /// <summary>
    /// Interface for characters that participate in the turn-based system.
    /// Provides properties needed for turn order calculation and management.
    /// </summary>
    public interface ITurnChar
    {
        /// <summary>
        /// Gets or sets whether the character is dead.
        /// </summary>
        bool Dead { get; set; }

        /// <summary>
        /// Gets or sets whether the character has used their turn this round.
        /// </summary>
        bool TurnUsed { get; set; }

        /// <summary>
        /// Gets or sets the turn wait counter for turn order calculation.
        /// </summary>
        int TurnWait { get; set; }

        /// <summary>
        /// Gets or sets the character's movement speed, affecting turn frequency.
        /// </summary>
        int MovementSpeed { get; set; }
    }
}
