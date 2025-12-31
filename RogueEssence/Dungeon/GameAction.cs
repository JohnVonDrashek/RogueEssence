using System;
using System.Collections.Generic;
using RogueElements;

namespace RogueEssence.Dungeon
{
    /// <summary>
    /// Represents a player or AI action in the dungeon, including movement, attacks, item usage, and other commands.
    /// </summary>
    [Serializable]
    public class GameAction
    {
        /// <summary>
        /// Defines all possible action types that can be performed in the dungeon.
        /// </summary>
        public enum ActionType
        {
            /// <summary>No action.</summary>
            None = -1,
            /// <summary>Change facing direction only.</summary>
            Dir = 0,
            /// <summary>Move in a direction.</summary>
            Move,
            /// <summary>Perform a basic attack or interact with an object.</summary>
            Attack,
            /// <summary>Pick up an item from the ground.</summary>
            Pickup,
            /// <summary>Interact with a tile effect.</summary>
            Tile,
            /// <summary>Use an item from inventory or held.</summary>
            UseItem,
            /// <summary>Give an item to a team member.</summary>
            Give,
            /// <summary>Take an item from a team member.</summary>
            Take,
            /// <summary>Drop an item on the ground.</summary>
            Drop,
            /// <summary>Throw an item.</summary>
            Throw,
            /// <summary>Use a skill.</summary>
            UseSkill,
            /// <summary>Wait and skip the turn.</summary>
            Wait,
            /// <summary>Toggle team mode.</summary>
            TeamMode,
            /// <summary>Shift team member order.</summary>
            ShiftTeam,
            /// <summary>Set a new team leader.</summary>
            SetLeader,
            /// <summary>Send a team member home.</summary>
            SendHome,
            /// <summary>Set team tactics.</summary>
            Tactics,
            /// <summary>Shift skill order.</summary>
            ShiftSkill,
            /// <summary>Enable or disable a skill.</summary>
            SetSkill,
            /// <summary>Sort inventory items.</summary>
            SortItems,
            /// <summary>Give up the dungeon run.</summary>
            GiveUp,
            /// <summary>Perform a rescue action.</summary>
            Rescue,
            /// <summary>Change a game option.</summary>
            Option,
        };

        /// <summary>
        /// The type of action to perform.
        /// </summary>
        public ActionType Type;

        /// <summary>
        /// The direction associated with this action.
        /// </summary>
        public Dir8 Dir;

        private List<int> args;

        /// <summary>
        /// Gets an argument at the specified index.
        /// </summary>
        /// <param name="index">The index of the argument to retrieve.</param>
        /// <returns>The argument value at the specified index.</returns>
        public int this[int index]
        {
            get
            {
                return args[index];
            }
        }

        /// <summary>
        /// Gets the number of arguments for this action.
        /// </summary>
        public int ArgCount { get { return args.Count; } }

        /// <summary>
        /// Initializes a new empty GameAction.
        /// </summary>
        public GameAction()
        {
            this.args = new List<int>();
        }

        /// <summary>
        /// Initializes a new GameAction with the specified type, direction, and arguments.
        /// </summary>
        /// <param name="type">The type of action.</param>
        /// <param name="dir">The direction for the action.</param>
        /// <param name="args">Additional arguments for the action.</param>
        public GameAction(ActionType type, Dir8 dir, params int[] args)
        {
            Type = type;
            Dir = dir;
            this.args = new List<int>();
            for (int ii = 0; ii < args.Length; ii++)
            {
                this.args.Add(args[ii]);
            }
        }

        /// <summary>
        /// Creates a copy of another GameAction.
        /// </summary>
        /// <param name="other">The GameAction to copy.</param>
        public GameAction(GameAction other)
        {
            Type = other.Type;
            Dir = other.Dir;
            this.args = new List<int>();
            this.args.AddRange(other.args);
        }

        /// <summary>
        /// Adds an argument to this action.
        /// </summary>
        /// <param name="arg">The argument value to add.</param>
        public void AddArg(int arg)
        {
            args.Add(arg);
        }

        /// <summary>
        /// Returns a string representation of this action.
        /// </summary>
        /// <returns>A string describing the action type, direction, and arguments.</returns>
        public override string ToString()
        {
            return String.Format("{0} {1} [{2}]", Type.ToString(), Dir.ToString(), String.Join(", ", args));
        }
    }
}
