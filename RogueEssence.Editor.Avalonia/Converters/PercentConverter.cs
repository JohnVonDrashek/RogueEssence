using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace RogueEssence.Dev.Converters
{
    /// <summary>
    /// Converts a decimal value (0-1 range) to a percentage string and back.
    /// For example, 0.5 becomes "50.00%".
    /// </summary>
    public class PercentConverter : IValueConverter
    {
        /// <summary>
        /// Converts a decimal to a percentage string.
        /// </summary>
        /// <param name="value">The decimal value (0-1 range).</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">The culture info.</param>
        /// <returns>A formatted percentage string.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return String.Format("{0:0.00}%", (double)value * 100);
        }

        /// <summary>
        /// Converts a percentage string back to a decimal.
        /// </summary>
        /// <param name="value">The percentage string (e.g., "50.00%").</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">The culture info.</param>
        /// <returns>The decimal value (0-1 range).</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strVal = ((string)value);
            return Double.Parse(strVal.Substring(0, strVal.Length-1)) / 100;
        }
    }
}
