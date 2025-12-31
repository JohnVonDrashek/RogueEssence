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
    /// Converts nullable values to string, showing "[NULL]" for null values.
    /// </summary>
    public class NullableToStringConverter : IValueConverter
    {
        /// <summary>
        /// Converts a nullable value to its string representation.
        /// </summary>
        /// <param name="value">The value to convert (may be null).</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">The culture info.</param>
        /// <returns>"[NULL]" if value is null, otherwise ToString() of the value.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? "[NULL]" : value.ToString();
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
