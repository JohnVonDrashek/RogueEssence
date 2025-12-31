using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using RogueEssence.Content;
using RogueEssence.Data;
using RogueEssence.Dungeon;

namespace RogueEssence.Dev.Converters
{
    /// <summary>
    /// Converts a data entry index string to its localized display name.
    /// The parameter specifies the data type (Monster, Skill, Item, etc.).
    /// </summary>
    public class DataEntryConverter : IValueConverter
    {
        /// <summary>
        /// Converts a data entry key to its localized name.
        /// </summary>
        /// <param name="value">The data entry key string.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">The data type as a string-encoded integer.</param>
        /// <param name="culture">The culture info.</param>
        /// <returns>The localized entry name, or "**EMPTY**" if not found.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string idx)
            {
                DataManager.DataType dataType = (DataManager.DataType)Int32.Parse((string)parameter);
                EntryDataIndex nameIndex = DataManager.Instance.DataIndices[dataType];
                if (nameIndex.ContainsKey(idx))
                    return nameIndex.Get(idx).Name.ToLocal();
                return "**EMPTY**";
            }
            return value;
        }

        /// <summary>
        /// Convert back is not supported for this converter.
        /// </summary>
        /// <exception cref="NotSupportedException">Always thrown as conversion back is not supported.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
