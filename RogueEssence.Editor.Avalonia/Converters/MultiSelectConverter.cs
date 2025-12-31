using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using RogueElements;
using RogueEssence.Dungeon;

namespace RogueEssence.Dev.Converters
{
    /// <summary>
    /// Converts a Loc (location/size) to a display string showing the multi-select dimensions.
    /// </summary>
    public class MultiSelectConverter : IValueConverter
    {
        /// <summary>
        /// Converts a Loc to a multi-select dimension string.
        /// </summary>
        /// <param name="value">The Loc representing the selection size.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">The culture info.</param>
        /// <returns>A formatted string showing the selection dimensions.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Loc size = (Loc)value;
            //if (size != Loc.One)
            return String.Format("Multi-Select: {0}x{1}", size.X, size.Y);
            //return "";
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
