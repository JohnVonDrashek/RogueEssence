using System;

namespace RogueEssence.Dev
{
    /// <summary>
    /// Indicates that a field or property should not be displayed or editable in the editor.
    /// Used to hide internal or computed properties from the user interface.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class NonEditedAttribute : Attribute
    {
        public NonEditedAttribute() { }
    }
}
