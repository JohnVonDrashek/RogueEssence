using System;
using RogueEssence.Content;

namespace RogueEssence.Data
{
    /// <summary>
    /// Represents an emote animation that can be displayed above characters.
    /// Emotes are visual feedback for character emotions and reactions.
    /// </summary>
    [Serializable]
    public class EmoteData : IEntryData
    {
        /// <summary>
        /// Returns the localized name of this emote.
        /// </summary>
        /// <returns>The localized name string.</returns>
        public override string ToString()
        {
            return Name.ToLocal();
        }

        /// <summary>
        /// The localized display name of this emote.
        /// </summary>
        public LocalText Name { get; set; }

        /// <summary>
        /// Indicates whether this emote is released for gameplay. Always returns true.
        /// </summary>
        public bool Released { get { return true; } }

        /// <summary>
        /// Developer comment describing this emote.
        /// </summary>
        [Dev.Multiline(0)]
        public string Comment { get; set; }

        /// <summary>
        /// Generates a summary of this emote for indexing purposes.
        /// </summary>
        /// <returns>An EntrySummary containing the emote's metadata.</returns>
        public EntrySummary GenerateEntrySummary() { return new EntrySummary(Name, Released, Comment); }

        /// <summary>
        /// The height offset above the character where the emote is displayed.
        /// </summary>
        public int LocHeight;

        /// <summary>
        /// The animation data for displaying this emote.
        /// </summary>
        public AnimData Anim;

        /// <summary>
        /// Initializes a new instance of the EmoteData class with default values.
        /// </summary>
        public EmoteData()
        {
            Name = new LocalText();
            Comment = "";
            Anim = new AnimData();
        }

        /// <summary>
        /// Initializes a new instance of the EmoteData class with the specified values.
        /// </summary>
        /// <param name="name">The localized name of the emote.</param>
        /// <param name="anim">The animation data for the emote.</param>
        /// <param name="locHeight">The height offset for displaying the emote.</param>
        public EmoteData(LocalText name, AnimData anim, int locHeight)
        {
            Name = name;
            Comment = "";
            Anim = anim;
            LocHeight = locHeight;
        }

        /// <summary>
        /// Gets the display name of the emote with color formatting.
        /// </summary>
        /// <returns>The formatted name string.</returns>
        public string GetColoredName()
        {
            return String.Format("{0}", Name.ToLocal());
        }
    }
}
