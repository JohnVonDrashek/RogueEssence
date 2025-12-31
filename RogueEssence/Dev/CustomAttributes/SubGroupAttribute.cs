using System;

namespace RogueEssence.Dev
{
    /// <summary>
    /// Indicates that a property should be displayed as a collapsible sub-group in the editor.
    /// Helps organize complex objects with many properties into logical sections.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SubGroupAttribute : Attribute
    {
        public SubGroupAttribute() { }
    }

    /// <summary>
    /// Indicates that a property should be visually separated from adjacent properties in the editor.
    /// Adds visual spacing or dividers between property groups.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SepGroupAttribute : Attribute
    {
        public SepGroupAttribute() { }
    }
}
