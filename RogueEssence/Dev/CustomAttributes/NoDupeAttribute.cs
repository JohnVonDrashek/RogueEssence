using System;

namespace RogueEssence.Dev
{
    /// <summary>
    /// Indicates that duplicate values are not allowed in a collection field.
    /// Enforces uniqueness of items within the collection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class NoDupeAttribute : PassableAttribute
    {
        public NoDupeAttribute(int flags) : base(flags) { }
    }
}
