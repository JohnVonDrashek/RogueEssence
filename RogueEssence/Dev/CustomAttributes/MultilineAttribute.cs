using System;

namespace RogueEssence.Dev
{
    /// <summary>
    /// Indicates that a string field should be displayed as a multiline text editor.
    /// Provides a larger text area for editing longer text content.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class MultilineAttribute : PassableAttribute
    {
        public MultilineAttribute(int flags) : base(flags) { }
    }
}
