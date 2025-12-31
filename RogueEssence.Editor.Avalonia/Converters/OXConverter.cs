using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using RogueEssence.Dungeon;

namespace RogueEssence.Dev.Converters
{
    /// <summary>
    /// Converts a boolean to an O (circle/check) or X icon image.
    /// True displays the O icon, false displays the X icon.
    /// </summary>
    public class OXConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean to an icon.
        /// </summary>
        /// <param name="value">The boolean value.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">The culture info.</param>
        /// <returns>The O icon if true, X icon if false.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool tf = (bool)value;
            return tf ? DevDataManager.IconO : DevDataManager.IconX;
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
