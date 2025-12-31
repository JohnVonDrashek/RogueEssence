using System;

namespace RogueEssence.Dev
{
    /// <summary>
    /// Constrains the types that can be assigned to a polymorphic field.
    /// Limits the type selector in the editor to only show types derived from the specified base class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class TypeConstraintAttribute : PassableAttribute
    {
        public readonly Type BaseClass;
        public TypeConstraintAttribute(int flags, Type type) : base(flags)
        {
            BaseClass = type;
        }
    }

    /// <summary>
    /// Constrains the types that can be assigned to a string-based type reference field.
    /// Used when types are stored as fully qualified type name strings.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class StringTypeConstraintAttribute : PassableAttribute
    {
        public readonly Type BaseClass;
        public StringTypeConstraintAttribute(int flags, Type type) : base(flags)
        {
            BaseClass = type;
        }
    }
}
