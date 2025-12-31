using System;

namespace RogueEssence.Dev
{
    /// <summary>
    /// Indicates that a list should display items with rank numbers in the editor.
    /// Controls whether the rank numbering starts at 0 or 1.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class RankedListAttribute : PassableAttribute
    {
        public readonly bool Index1;
        public RankedListAttribute(int flags, bool index1) : base(flags)
        {
            Index1 = index1;
        }
    }
}
