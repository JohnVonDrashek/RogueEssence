using System;

namespace RogueEssence.Dev
{
    /// <summary>
    /// Specifies an alternative display name for a field or property in the editor.
    /// Allows renaming properties in the UI without changing the underlying code.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class AliasAttribute : PassableAttribute
    {
        public readonly string Name;
 
        public AliasAttribute(int flags, string name) : base(flags)
        {
            Name = name;
        }
    }
}
