using System;

namespace RogueEssence.Dev
{
    /// <summary>
    /// Specifies that a field or property represents a localization string key.
    /// Enables selection from available string keys in the localization system.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class StringKeyAttribute : PassableAttribute
    {
        public readonly bool IncludeInvalid;
 
        public StringKeyAttribute(int flags, bool includeInvalid) : base(flags)
        {
            IncludeInvalid = includeInvalid;
        }
    }

}
