using System;

namespace RogueEssence.Dev
{
    /// <summary>
    /// Indicates that a list property should be displayed in a collapsed format in the editor.
    /// Helps reduce visual clutter for long or nested lists.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ListCollapseAttribute : Attribute
    {
        public ListCollapseAttribute() { }
    }
}
