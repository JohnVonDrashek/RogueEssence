using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace RogueEssence.Dev.Converters
{
    /// <summary>
    /// Value converter that compares a value to a parameter and returns a boolean result.
    /// Useful for radio button bindings where the selected value should match a specific parameter.
    /// </summary>
    public class ComparisonConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value by comparing it to the parameter.
        /// </summary>
        /// <param name="value">The value to compare.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">The parameter to compare against.</param>
        /// <param name="culture">The culture info.</param>
        /// <returns>True if value equals parameter, otherwise false.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.Equals(parameter);
        }

        /// <summary>
        /// Converts back from boolean to the parameter value if true.
        /// </summary>
        /// <param name="value">The boolean value.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">The parameter to return if value is true.</param>
        /// <param name="culture">The culture info.</param>
        /// <returns>The parameter if value is true, otherwise DoNothing.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.Equals(true) == true ? parameter : BindingOperations.DoNothing;
        }
    }
}
