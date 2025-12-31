using System;

namespace RogueEssence.Content
{
    /// <summary>
    /// Represents an emote effect that displays an animation with optional sound.
    /// Used for showing emotions or reactions above characters.
    /// </summary>
    [Serializable]
    public class EmoteFX
    {
        /// <summary>
        /// The delay in milliseconds before playing the next effect.
        /// </summary>
        public int Delay;

        /// <summary>
        /// The sound effect to play with this emote.
        /// </summary>
        [Dev.Sound(0)]
        public string Sound;

        /// <summary>
        /// The height offset above the character to display the emote.
        /// </summary>
        public int LocHeight;

        /// <summary>
        /// The animation data for the emote visual.
        /// </summary>
        public AnimData Anim;

        /// <summary>
        /// Creates a new EmoteFX with default values.
        /// </summary>
        public EmoteFX()
        {
            Sound = "";
        }
        /// <summary>
        /// Creates a new EmoteFX with the specified parameters.
        /// </summary>
        /// <param name="anim">The animation data for the emote.</param>
        /// <param name="locHeight">The height offset above the character.</param>
        /// <param name="sound">The sound effect name to play.</param>
        /// <param name="delay">The delay in milliseconds after playing.</param>
        public EmoteFX(AnimData anim, int locHeight, string sound, int delay)
        {
            Anim = anim;
            LocHeight = locHeight;
            Sound = sound;
            Delay = delay;
        }

        /// <summary>
        /// Creates a copy of an existing EmoteFX.
        /// </summary>
        /// <param name="other">The EmoteFX to copy.</param>
        public EmoteFX(EmoteFX other)
        {
            Anim = other.Anim;
            LocHeight = other.LocHeight;
            Delay = other.Delay;
            Sound = other.Sound;
        }


        /// <summary>
        /// Returns a string representation of this emote effect.
        /// </summary>
        /// <returns>A string describing the animation, sound, and delay settings.</returns>
        public override string ToString()
        {
            string result = Anim.ToString();
            if (Sound != "")
                result += ", SE:" + Sound;
            if (Delay > 0)
                result += " +" + Delay;
            return result;
        }
    }
}
