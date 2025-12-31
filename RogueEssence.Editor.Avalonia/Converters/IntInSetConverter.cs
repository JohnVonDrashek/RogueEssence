using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Avalonia.Data;
using Avalonia.Data.Converters;
using System.Linq;

namespace RogueEssence.Dev.Converters
{
    /// <summary>
    /// Checks if an integer value is contained in a set of values specified by the parameter.
    /// Parameter should be a pipe-delimited string of values (e.g., "1|2|3").
    /// </summary>
    public class IntInSetConverter : IValueConverter
    {
        /// <summary>
        /// Checks if the value is in the set specified by the parameter.
        /// </summary>
        /// <param name="value">The integer value to check.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">A pipe-delimited string of valid values.</param>
        /// <param name="culture">The culture info.</param>
        /// <returns>True if value is in the set, otherwise false.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string[] parameters = ((string)parameter).Split('|');
            return parameters.Contains(value.ToString());
        }

        /// <summary>
        /// Convert back is not implemented for this converter.
        /// </summary>
        /// <exception cref="NotImplementedException">Always thrown as conversion back is not implemented.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
