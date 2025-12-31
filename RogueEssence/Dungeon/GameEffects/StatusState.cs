using System;

namespace RogueEssence.Dungeon
{
    /// <summary>
    /// Abstract base class for status effect states attached to characters.
    /// Status states store mutable data for status effects.
    /// </summary>
    [Serializable]
    public abstract class StatusState : GameplayState
    {

    }

    /// <summary>
    /// A status state that tracks a count value.
    /// </summary>
    [Serializable]
    public class CountState : StatusState
    {
        /// <summary>
        /// The count value for this status.
        /// </summary>
        public int Count;

        /// <summary>
        /// Initializes a new CountState with default values.
        /// </summary>
        public CountState() { }

        /// <summary>
        /// Initializes a new CountState with the specified count.
        /// </summary>
        /// <param name="count">The initial count value.</param>
        public CountState(int count) { Count = count; }

        /// <summary>
        /// Creates a copy of another CountState.
        /// </summary>
        /// <param name="other">The CountState to copy.</param>
        protected CountState(CountState other) { Count = other.Count; }

        /// <summary>
        /// Creates a clone of this state.
        /// </summary>
        /// <returns>A new CountState with the same values.</returns>
        public override GameplayState Clone() { return new CountState(this); }
    }

    /// <summary>
    /// A status state that tracks a stack value for stackable effects.
    /// </summary>
    [Serializable]
    public class StackState : StatusState
    {
        /// <summary>
        /// The number of stacks for this status.
        /// </summary>
        public int Stack;

        /// <summary>
        /// Initializes a new StackState with default values.
        /// </summary>
        public StackState() { }

        /// <summary>
        /// Initializes a new StackState with the specified stack count.
        /// </summary>
        /// <param name="stack">The initial stack count.</param>
        public StackState(int stack) { Stack = stack; }

        /// <summary>
        /// Creates a copy of another StackState.
        /// </summary>
        /// <param name="other">The StackState to copy.</param>
        protected StackState(StackState other) { Stack = other.Stack; }

        /// <summary>
        /// Creates a clone of this state.
        /// </summary>
        /// <returns>A new StackState with the same values.</returns>
        public override GameplayState Clone() { return new StackState(this); }
    }

    /// <summary>
    /// A status state that tracks a countdown timer for duration-based effects.
    /// </summary>
    [Serializable]
    public class CountDownState : StatusState
    {
        /// <summary>
        /// The countdown counter value.
        /// </summary>
        public int Counter;

        /// <summary>
        /// Initializes a new CountDownState with default values.
        /// </summary>
        public CountDownState() { }

        /// <summary>
        /// Initializes a new CountDownState with the specified counter.
        /// </summary>
        /// <param name="counter">The initial counter value.</param>
        public CountDownState(int counter) { Counter = counter; }

        /// <summary>
        /// Creates a copy of another CountDownState.
        /// </summary>
        /// <param name="other">The CountDownState to copy.</param>
        protected CountDownState(CountDownState other) { Counter = other.Counter; }

        /// <summary>
        /// Creates a clone of this state.
        /// </summary>
        /// <returns>A new CountDownState with the same values.</returns>
        public override GameplayState Clone() { return new CountDownState(this); }
    }

    /// <summary>
    /// A status state that stores script call information for scripted effects.
    /// </summary>
    [Serializable]
    public class ScriptCallState : StatusState
    {
        /// <summary>
        /// The script function to call.
        /// </summary>
        [RogueEssence.Dev.Sanitize(0)]
        public string Script;

        /// <summary>
        /// The argument table to pass to the script.
        /// </summary>
        [RogueEssence.Dev.Multiline(0)]
        public string ArgTable;

        /// <summary>
        /// Initializes a new ScriptCallState with default values.
        /// </summary>
        public ScriptCallState() { Script = ""; ArgTable = "{}"; }

        /// <summary>
        /// Initializes a new ScriptCallState with the specified script and arguments.
        /// </summary>
        /// <param name="script">The script function name.</param>
        /// <param name="argTable">The argument table as a Lua string.</param>
        public ScriptCallState(string script, string argTable) { Script = script; ArgTable = argTable; }

        /// <summary>
        /// Creates a copy of another ScriptCallState.
        /// </summary>
        /// <param name="other">The ScriptCallState to copy.</param>
        protected ScriptCallState(ScriptCallState other) { Script = other.Script; ArgTable = other.ArgTable; }

        /// <summary>
        /// Creates a clone of this state.
        /// </summary>
        /// <returns>A new ScriptCallState with the same values.</returns>
        public override GameplayState Clone() { return new ScriptCallState(this); }
    }
}
