using System;

namespace RogueEssence.Dev
{
    /// <summary>
    /// Specifies a custom height for the editor control of a field or property.
    /// Used to provide more vertical space for complex or text-heavy properties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class EditorHeightAttribute : PassableAttribute
    {
        public readonly int Height;
        public EditorHeightAttribute(int flags, int height) : base(flags)
        {
            Height = height;
        }
    }
}
