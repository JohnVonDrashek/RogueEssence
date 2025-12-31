using System;

namespace RogueEssence.Dev
{
    /// <summary>
    /// Abstract base class for attributes that can be passed to editor controls.
    /// Contains a flags field that determines how the attribute is propagated through nested types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public abstract class PassableAttribute : Attribute
    {
        public readonly int PassableArgFlag;

        public PassableAttribute(int flags)
        {
            PassableArgFlag = flags;
        }
    }
}
