using System;

namespace RogueEssence.Dev
{
    /// <summary>
    /// Specifies that a field or property should be populated from a specific game data type.
    /// Enables selection of data entries from the specified DataManager.DataType in the editor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class DataTypeAttribute : PassableAttribute
    {
        public readonly Data.DataManager.DataType DataType;
        public readonly bool IncludeInvalid;

        public DataTypeAttribute(int flags, Data.DataManager.DataType dataType, bool includeInvalid) : base(flags)
        {
            DataType = dataType;
            IncludeInvalid = includeInvalid;
        }
    }

    /// <summary>
    /// Specifies that a field or property should be populated from files in a specific folder.
    /// Enables file selection from the specified folder path in the editor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class DataFolderAttribute : PassableAttribute
    {
        public readonly string FolderPath;

        public DataFolderAttribute(int flags, string folder) : base(flags)
        {
            FolderPath = folder;
        }
    }
}
