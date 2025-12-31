using System;

namespace RogueEssence.Dev
{
    /// <summary>
    /// Indicates that this property should share a horizontal row with the previous property in the editor.
    /// Allows placing multiple fields on the same line to save vertical space.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SharedRowAttribute : Attribute
    {
        public SharedRowAttribute() { }
    }
}
