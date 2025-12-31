using System;

namespace RogueEssence.Dungeon
{
    /// <summary>
    /// Abstract base class for character states that affect gameplay behavior.
    /// Character states are temporary or permanent modifiers applied to characters.
    /// </summary>
    [Serializable]
    public abstract class CharState : GameplayState
    {

    }


    /// <summary>
    /// Abstract base class for character states that include a modifier value.
    /// Used for states that need to track a numerical modification amount.
    /// </summary>
    [Serializable]
    public abstract class ModGenState : CharState
    {
        /// <summary>
        /// The modifier value associated with this state.
        /// </summary>
        public int Mod;

        /// <summary>
        /// Initializes a new ModGenState with default values.
        /// </summary>
        public ModGenState() { }

        /// <summary>
        /// Initializes a new ModGenState with the specified modifier value.
        /// </summary>
        /// <param name="mod">The modifier value.</param>
        public ModGenState(int mod) { Mod = mod; }

        /// <summary>
        /// Creates a copy of another ModGenState.
        /// </summary>
        /// <param name="other">The ModGenState to copy.</param>
        protected ModGenState(ModGenState other) { Mod = other.Mod; }
    }


}
