using System;

namespace RogueEssence.Dev
{
    /// <summary>
    /// Specifies that a field or property represents an animation frame type selection.
    /// Controls whether only dash-style frames or all frame types are available.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class FrameTypeAttribute : PassableAttribute
    {
        public readonly bool DashOnly;
 
        public FrameTypeAttribute(int flags, bool dashOnly) : base(flags)
        {
            DashOnly = dashOnly;
        }
    }
}
