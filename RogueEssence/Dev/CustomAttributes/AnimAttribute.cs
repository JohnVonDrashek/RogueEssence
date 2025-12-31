using System;

namespace RogueEssence.Dev
{
    /// <summary>
    /// Specifies that a field or property represents an animation asset.
    /// Provides the folder path where animation assets can be found for selection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class AnimAttribute : PassableAttribute
    {
        public readonly string FolderPath;

        public AnimAttribute(int flags, string folder) : base(flags)
        {
            FolderPath = folder;
        }
    }
}
