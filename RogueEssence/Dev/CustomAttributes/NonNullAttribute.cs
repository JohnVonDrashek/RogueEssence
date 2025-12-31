using System;

namespace RogueEssence.Dev
{
    /// <summary>
    /// Indicates that a field or property cannot be set to null in the editor.
    /// Enforces that a value must always be provided.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class NonNullAttribute : PassableAttribute
    {
        public NonNullAttribute(int flags) : base(flags) { }
    }
}
