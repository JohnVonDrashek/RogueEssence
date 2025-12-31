using System;

namespace RogueEssence.Dungeon
{
    /// <summary>
    /// Abstract base class for skill states that store mutable skill data.
    /// Skill states modify or enhance skill behavior at runtime.
    /// </summary>
    [Serializable]
    public abstract class SkillState : GameplayState
    {

    }

    /// <summary>
    /// A skill state that stores the base power value for a skill.
    /// </summary>
    [Serializable]
    public class BasePowerState : SkillState
    {
        /// <summary>
        /// The base power value of the skill.
        /// </summary>
        public int Power;

        /// <summary>
        /// Initializes a new BasePowerState with default values.
        /// </summary>
        public BasePowerState() { }

        /// <summary>
        /// Initializes a new BasePowerState with the specified power.
        /// </summary>
        /// <param name="power">The base power value.</param>
        public BasePowerState(int power) { Power = power; }

        /// <summary>
        /// Creates a copy of another BasePowerState.
        /// </summary>
        /// <param name="other">The BasePowerState to copy.</param>
        protected BasePowerState(BasePowerState other) { Power = other.Power; }

        /// <summary>
        /// Creates a clone of this state.
        /// </summary>
        /// <returns>A new BasePowerState with the same values.</returns>
        public override GameplayState Clone() { return new BasePowerState(this); }
    }
}
