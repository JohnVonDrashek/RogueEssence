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
    /// Converts a tileset name string to its bitmap image representation.
    /// </summary>
    public class TilesetConverter : IValueConverter
    {
        /// <summary>
        /// Converts a tileset name to its bitmap image.
        /// </summary>
        /// <param name="value">The tileset name string.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">The culture info.</param>
        /// <returns>The tileset bitmap, or null if the name is empty.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string tileset = (string)value;
            if (String.IsNullOrEmpty(tileset))
                return null;
            return DevDataManager.GetTileset(tileset);
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
