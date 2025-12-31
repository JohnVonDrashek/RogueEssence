using System;
using RogueElements;
using RogueEssence.Dungeon;

namespace RogueEssence.Data
{
    /// <summary>
    /// Abstract base class for AI behavior plans. Each plan represents a specific
    /// behavior pattern that can be evaluated to produce game actions.
    /// </summary>
    [Serializable]
    public abstract class BasePlan
    {
        /// <summary>
        /// Initializes a new instance of the BasePlan class.
        /// </summary>
        public BasePlan() { }

        /// <summary>
        /// Creates a new instance of this plan for use by another AI.
        /// </summary>
        /// <returns>A new copy of this plan.</returns>
        public abstract BasePlan CreateNew();

        /// <summary>
        /// Called at the beginning of a floor, or when a character spawns, to initialize the AI plan.
        /// </summary>
        /// <param name="controlledChar">The character being controlled by this AI plan.</param>
        public virtual void Initialize(Character controlledChar) { }

        /// <summary>
        /// Called whenever this plan is switched in from another plan.
        /// </summary>
        /// <param name="currentPlan">The plan that was previously active.</param>
        public virtual void SwitchedIn(BasePlan currentPlan) { }

        /// <summary>
        /// Evaluates the current game state and determines the next action for the controlled character.
        /// </summary>
        /// <param name="controlledChar">The character being controlled by this AI plan.</param>
        /// <param name="preThink">Whether this is a pre-think phase before the actual turn.</param>
        /// <param name="rand">Random number generator for decision making.</param>
        /// <returns>The next game action to execute, or null if this plan cannot produce an action.</returns>
        public abstract GameAction Think(Character controlledChar, bool preThink, IRandom rand);

    }

}
