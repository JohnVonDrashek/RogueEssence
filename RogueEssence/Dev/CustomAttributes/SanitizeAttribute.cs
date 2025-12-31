using System;

namespace RogueEssence.Dev
{
    /// <summary>
    /// Indicates that string input should be sanitized for safe file system usage.
    /// Removes or replaces characters that are not allowed in file names.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SanitizeAttribute : PassableAttribute
    {
        public SanitizeAttribute(int flags) : base(flags) { }
    }
}
