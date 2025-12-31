using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace RogueEssence.Dev.Converters
{
    /// <summary>
    /// Converts an element type string to its corresponding icon image.
    /// </summary>
    public class ElementIconConverter : IValueConverter
    {
        /// <summary>
        /// Converts an element type key to its icon bitmap.
        /// </summary>
        /// <param name="value">The element type key string.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">The culture info.</param>
        /// <returns>The element icon bitmap.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string type = (string)value;
            return DevDataManager.GetElementIcon(type);
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