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
    /// Converts a TileFrame to its bitmap image representation.
    /// </summary>
    public class TileConverter : IValueConverter
    {
        /// <summary>
        /// Converts a TileFrame to a bitmap image.
        /// </summary>
        /// <param name="value">The TileFrame to convert.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">The culture info.</param>
        /// <returns>The tile bitmap, or null if the sheet is null.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TileFrame tileFrame = (TileFrame)value;
            if (tileFrame.Sheet == null)
                return null;
            return DevDataManager.GetTile(tileFrame);
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
