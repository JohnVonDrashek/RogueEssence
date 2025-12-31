using System;

namespace RogueEssence.Dev
{
    /// <summary>
    /// Specifies behavior options for collection fields in the editor.
    /// Controls whether deletion of items requires confirmation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class CollectionAttribute : PassableAttribute
    {
        public readonly bool ConfirmDelete;
        public CollectionAttribute(int flags, bool confirmDelete) : base(flags)
        {
            ConfirmDelete = confirmDelete;
        }
    }
}
