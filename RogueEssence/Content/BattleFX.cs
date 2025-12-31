using System;

namespace RogueEssence.Content
{
    /// <summary>
    /// Represents a battle visual effect including particle emissions, sound, and screen shake.
    /// Used to create coordinated visual and audio feedback during combat.
    /// </summary>
    [Serializable]
    public class BattleFX
    {
        /// <summary>
        /// After playing this VFX, will wait this many milliseconds before moving to the next one.
        /// </summary>
        public int Delay;

        /// <summary>
        /// Do not modify delay due to batle speed
        /// </summary>
        public bool AbsoluteDelay;

        /// <summary>
        /// The sound effect of the VFX
        /// </summary>
        [Dev.Sound(0)]
        public string Sound;

        /// <summary>
        /// The Particle FX
        /// </summary>
        [Dev.SubGroup]
        public FiniteEmitter Emitter;

        /// <summary>
        /// Screen shake and other effects.
        /// </summary>
        [Dev.SubGroup]
        public ScreenMover ScreenMovement;

        /// <summary>
        /// Initializes a new BattleFX with default values.
        /// </summary>
        public BattleFX()
        {
            Emitter = new EmptyFiniteEmitter();
            ScreenMovement = new ScreenMover();
            Sound = "";
        }

        /// <summary>
        /// Initializes a new BattleFX with the specified emitter, sound, and delay.
        /// </summary>
        /// <param name="emitter">The particle emitter to use.</param>
        /// <param name="sound">The sound effect name to play.</param>
        /// <param name="delay">The delay in milliseconds after this effect.</param>
        /// <param name="absolute">If true, the delay is not affected by battle speed settings.</param>
        public BattleFX(FiniteEmitter emitter, string sound, int delay, bool absolute = false)
        {
            Emitter = emitter;
            Sound = sound;
            Delay = delay;
            AbsoluteDelay = absolute;
            ScreenMovement = new ScreenMover();
        }

        /// <summary>
        /// Creates a copy of an existing BattleFX.
        /// </summary>
        /// <param name="other">The BattleFX to copy.</param>
        public BattleFX(BattleFX other)
        {
            Delay = other.Delay;
            AbsoluteDelay = other.AbsoluteDelay;
            Emitter = (FiniteEmitter)other.Emitter.Clone();
            ScreenMovement = new ScreenMover(other.ScreenMovement);
            Sound = other.Sound;
        }

        /// <summary>
        /// Returns a string representation of this battle effect.
        /// </summary>
        /// <returns>A string describing the emitter, sound, and delay settings.</returns>
        public override string ToString()
        {
            string result = Emitter.ToString();
            if (Sound != "")
                result += ", SE:" + Sound;
            if (Delay > 0)
                result += " +" + Delay;
            return result;
        }
    }
}
