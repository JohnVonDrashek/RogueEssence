namespace RogueEssence.Dungeon
{
    /// <summary>
    /// Represents the result of a game action, indicating whether it failed, succeeded, or consumed a turn.
    /// </summary>
    public class ActionResult
    {
        /// <summary>
        /// Defines the possible outcomes of a game action.
        /// </summary>
        public enum ResultType
        {
            /// <summary>
            /// The action failed and no turn was consumed.
            /// </summary>
            Fail,
            /// <summary>
            /// The action succeeded without consuming a turn.
            /// </summary>
            Success,
            /// <summary>
            /// The action succeeded and consumed the character's turn.
            /// </summary>
            TurnTaken
        }

        /// <summary>
        /// The result type of this action.
        /// </summary>
        public ResultType Success;
    }
}
