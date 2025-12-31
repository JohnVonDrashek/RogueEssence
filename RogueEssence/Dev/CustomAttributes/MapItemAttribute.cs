using System;

namespace RogueEssence.Dev
{
    /// <summary>
    /// Specifies that a field or property represents a map item with optional price display.
    /// Controls whether the item editor includes a price field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class MapItemAttribute : PassableAttribute
    {
        public readonly bool IncludePrice;

        public MapItemAttribute(int flags, bool price) : base(flags)
        {
            IncludePrice = price;
        }
    }
}
