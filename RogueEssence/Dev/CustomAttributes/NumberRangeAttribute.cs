using System;

namespace RogueEssence.Dev
{
    /// <summary>
    /// Specifies minimum and maximum allowed values for a numeric field.
    /// Constrains the input to a specific range in the editor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class NumberRangeAttribute : PassableAttribute
    {
        public readonly int Min;
        public readonly int Max;

        public NumberRangeAttribute(int flags, int min, int max) : base(flags)
        {
            Min = min;
            Max = max;
        }
    }

    /// <summary>
    /// Specifies display options for IntRange fields in the editor.
    /// Controls whether the range should be displayed as 1-indexed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class IntRangeAttribute : PassableAttribute
    {
        public readonly bool Index1;

        public IntRangeAttribute(int flags, bool index1) : base(flags)
        {
            Index1 = index1;
        }
    }
}
