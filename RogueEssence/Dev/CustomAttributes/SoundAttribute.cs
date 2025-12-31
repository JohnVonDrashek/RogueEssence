using System;

namespace RogueEssence.Dev
{
    /// <summary>
    /// Specifies that a field or property represents a sound effect asset.
    /// Enables sound effect selection with preview capability in the editor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SoundAttribute : PassableAttribute
    {
        public SoundAttribute(int flags) : base(flags) { }
    }

    /// <summary>
    /// Specifies that a field or property represents a music/BGM asset.
    /// Enables music track selection with preview capability in the editor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class MusicAttribute : PassableAttribute
    {
        public MusicAttribute(int flags) : base(flags) { }
    }
}
