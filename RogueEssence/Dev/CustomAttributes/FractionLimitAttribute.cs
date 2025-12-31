using System;

namespace RogueEssence.Dev
{
    /// <summary>
    /// Specifies constraints on the sign of numerator and denominator for fraction fields.
    /// Used to limit whether fractions can be negative or require specific sign combinations.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class FractionLimitAttribute : PassableAttribute
    {
        public readonly int NumSign;
        public readonly int DenSign;
        public FractionLimitAttribute(int flags, int numSign, int denSign) : base(flags)
        {
            NumSign = numSign;
            DenSign = denSign;
        }

        public void GetMinVals(out int numMin, out int denMin)
        {
            numMin = NumSign;
            if (numMin < 0)
                numMin = Int32.MinValue;
            denMin = DenSign;
            if (denMin < 0)
                denMin = Int32.MinValue;

        }
    }
}
