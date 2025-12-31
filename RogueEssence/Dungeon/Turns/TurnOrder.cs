using System;

namespace RogueEssence.Dungeon
{
    /// <summary>
    /// Represents the current position in the turn order, including tier, faction, and character index.
    /// Used by TurnState to track whose turn it is.
    /// </summary>
    [Serializable]
    public struct TurnOrder
    {
        /// <summary>Turn tier for normal speed characters.</summary>
        public const int TURN_TIER_0 = 0;
        /// <summary>Turn tier at 1/4 through the round (speed +3 only).</summary>
        public const int TURN_TIER_1_4 = 1;
        /// <summary>Turn tier at 1/3 through the round (speed +2 only).</summary>
        public const int TURN_TIER_1_3 = 2;
        /// <summary>Turn tier at 1/2 through the round (speed +1 or +3).</summary>
        public const int TURN_TIER_1_2 = 3;
        /// <summary>Turn tier at 2/3 through the round (speed +2 only).</summary>
        public const int TURN_TIER_2_3 = 4;
        /// <summary>Turn tier at 3/4 through the round (speed +3 only).</summary>
        public const int TURN_TIER_3_4 = 5;

        /// <summary>
        /// The current turn tier within the round.
        /// </summary>
        public int TurnTier;

        /// <summary>
        /// The faction currently taking turns.
        /// </summary>
        public Faction Faction;

        /// <summary>
        /// The index of the current character within the turn list.
        /// </summary>
        public int TurnIndex;

        /// <summary>
        /// Initializes a new TurnOrder with the specified tier, faction, and index.
        /// </summary>
        /// <param name="turnTier">The turn tier.</param>
        /// <param name="faction">The faction taking the turn.</param>
        /// <param name="turnIndex">The character index within the turn list.</param>
        public TurnOrder(int turnTier, Faction faction, int turnIndex)
        {
            TurnTier = turnTier;
            Faction = faction;
            TurnIndex = turnIndex;
        }
    }
}
