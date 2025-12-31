using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace RogueEssence.Dev.Converters
{
    /// <summary>
    /// Value converter that compares a value to a parameter and returns "X" if equal, empty string otherwise.
    /// Used for displaying checkmarks or indicators in UI elements.
    /// </summary>
    public class ComparisonXConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value by comparing it to the parameter.
        /// </summary>
        /// <param name="value">The value to compare.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">The parameter to compare against.</param>
        /// <param name="culture">The culture info.</param>
        /// <returns>"X" if value equals parameter, otherwise empty string.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? res = value?.Equals(parameter);
            if (res.HasValue && res.Value)
                return "X";
            return "";
        }

        /// <summary>
        /// Converts back from "X" string to the parameter value.
        /// </summary>
        /// <param name="value">The string value.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">The parameter to return if value is "X".</param>
        /// <param name="culture">The culture info.</param>
        /// <returns>The parameter if value is "X", otherwise DoNothing.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.Equals("X") == true ? parameter : BindingOperations.DoNothing;
        }
    }
}
