using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using RogueElements;
using RogueEssence.Dungeon;

namespace RogueEssence.Data
{

    /// <summary>
    /// Represents an AI tactic that defines how a character behaves autonomously in the game.
    /// Contains a list of plans that are evaluated in order to determine the next action.
    /// </summary>
    [Serializable]
    public class AITactic : IEntryData
    {
        public override string ToString()
        {
            return Name.ToLocal();
        }

        public LocalText Name { get; set; }
        public bool Released { get; set; }

        [Dev.Multiline(0)]
        public string Comment { get; set; }

        /// <summary>
        /// Generates a summary of this AI tactic for indexing purposes.
        /// </summary>
        /// <returns>An AIEntrySummary containing the tactic's metadata.</returns>
        public EntrySummary GenerateEntrySummary() { return new AIEntrySummary(Name, Released, Comment, Assignable); }

        /// <summary>
        /// Unique identifier for this AI tactic.
        /// </summary>
        [JsonConverter(typeof(Dev.AIConverter))]
        public string ID;

        /// <summary>
        /// Can be assigned via tactics menu
        /// </summary>
        public bool Assignable;

        /// <summary>
        /// The ordered list of AI plans to evaluate when determining the next action.
        /// Plans are evaluated in sequence until one returns a valid action.
        /// </summary>
        public List<BasePlan> Plans;

        [NonSerialized]
        protected BasePlan currentPlan;

        /// <summary>
        /// Evaluates all plans in order and returns the first valid action found.
        /// </summary>
        /// <param name="controlledChar">The character being controlled by this AI.</param>
        /// <param name="preThink">Whether this is a pre-think phase before the actual turn.</param>
        /// <param name="rand">Random number generator for decision making.</param>
        /// <returns>The next game action to execute, or a Wait action if no plan succeeds.</returns>
        public GameAction GetNextMove(Character controlledChar, bool preThink, IRandom rand)
        {
            foreach (BasePlan plan in Plans)
            {
                GameAction result = AttemptPlan(controlledChar, plan, preThink, rand);
                if (result != null)
                    return result;
            }

            currentPlan = null;
            return new GameAction(GameAction.ActionType.Wait, Dir8.None);
        }

        /// <summary>
        /// Attempts to execute a single plan and handles plan switching logic.
        /// </summary>
        /// <param name="controlledChar">The character being controlled by this AI.</param>
        /// <param name="plan">The plan to attempt.</param>
        /// <param name="preThink">Whether this is a pre-think phase before the actual turn.</param>
        /// <param name="rand">Random number generator for decision making.</param>
        /// <returns>The action from the plan if successful, or null if the plan fails.</returns>
        protected GameAction AttemptPlan(Character controlledChar, BasePlan plan, bool preThink, IRandom rand)
        {
            if ((currentPlan != null) && (currentPlan.GetType() == plan.GetType()))
                return currentPlan.Think(controlledChar, preThink, rand);
            else
            {
                plan.SwitchedIn(currentPlan);
                GameAction result = plan.Think(controlledChar, preThink, rand);
                if (result != null)
                    currentPlan = plan;
                return result;
            }
        }

        /// <summary>
        /// Initializes a new instance of the AITactic class with default values.
        /// </summary>
        public AITactic()
        {
            ID = "";
            Name = new LocalText();
            Comment = "";
            Plans = new List<BasePlan>();
        }

        /// <summary>
        /// Creates a copy of an existing AITactic.
        /// </summary>
        /// <param name="other">The AITactic to copy from.</param>
        public AITactic(AITactic other) : this()
        {
            ID = other.ID;
            Assignable = other.Assignable;
            Name = new LocalText(other.Name);
            Comment = other.Comment;
            foreach (BasePlan plan in other.Plans)
                Plans.Add(plan.CreateNew());
        }

        /// <summary>
        /// Initializes all plans for the controlled character at the start of a floor or when spawned.
        /// </summary>
        /// <param name="controlledChar">The character being controlled by this AI.</param>
        public void Initialize(Character controlledChar)
        {
            foreach (BasePlan plan in Plans)
                plan.Initialize(controlledChar);
        }

        /// <summary>
        /// Gets the next action for the controlled character, handling any errors gracefully.
        /// </summary>
        /// <param name="controlledChar">The character being controlled by this AI.</param>
        /// <param name="rand">Random number generator for decision making.</param>
        /// <param name="preThink">Whether this is a pre-think phase before the actual turn.</param>
        /// <returns>The next game action to execute.</returns>
        public GameAction GetAction(Character controlledChar, IRandom rand, bool preThink)
        {
            try
            {
                return GetNextMove(controlledChar, preThink, rand);
            }
            catch (Exception ex)
            {
                DiagManager.Instance.LogError(new Exception("AI Error\n", ex));
            }
            return new GameAction(GameAction.ActionType.Wait, Dir8.None);
        }

        /// <summary>
        /// Gets the display name of the AI tactic with color formatting.
        /// </summary>
        /// <returns>The formatted name string.</returns>
        public string GetColoredName()
        {
            return String.Format("{0}", Name.ToLocal());
        }
    }


    /// <summary>
    /// Summary data for an AI tactic entry, used for indexing and display purposes.
    /// </summary>
    [Serializable]
    public class AIEntrySummary : EntrySummary
    {
        /// <summary>
        /// Indicates whether this tactic can be assigned via the tactics menu.
        /// </summary>
        public bool Assignable;

        /// <summary>
        /// Initializes a new instance of the AIEntrySummary class.
        /// </summary>
        public AIEntrySummary() : base()
        {

        }

        /// <summary>
        /// Initializes a new instance of the AIEntrySummary class with the specified values.
        /// </summary>
        /// <param name="name">The localized name of the tactic.</param>
        /// <param name="released">Whether the tactic is released for gameplay.</param>
        /// <param name="comment">Developer comment for this tactic.</param>
        /// <param name="assignable">Whether the tactic can be assigned via menu.</param>
        public AIEntrySummary(LocalText name, bool released, string comment, bool assignable)
            : base(name, released, comment)
        {
            Assignable = assignable;
        }

        /// <summary>
        /// Gets the display name of the AI tactic with color formatting.
        /// </summary>
        /// <returns>The formatted name string.</returns>
        public override string GetColoredName()
        {
            return String.Format("{0}", Name.ToLocal());
        }
    }
}

