using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace RogueEssence.Dev.Converters
{
    /// <summary>
    /// Converts a string to a boolean indicating whether it is not null or empty.
    /// </summary>
    public class StringNotEmptyConverter : IValueConverter
    {
        /// <summary>
        /// Checks if the string is not null or empty.
        /// </summary>
        /// <param name="value">The string to check.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">The culture info.</param>
        /// <returns>True if the string has content, otherwise false.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string file = (string)value;
            return !String.IsNullOrEmpty(file);
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
