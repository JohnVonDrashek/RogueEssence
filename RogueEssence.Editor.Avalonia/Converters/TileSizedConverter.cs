using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using RogueElements;
using RogueEssence.Dungeon;

namespace RogueEssence.Dev.Converters
{
    /// <summary>
    /// Multi-value converter that calculates pixel size from tile coordinates and tile size.
    /// Used for calculating widths/heights in the map editor based on tile dimensions.
    /// </summary>
    public class TileSizedConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converts tile coordinates and tile size to pixel dimensions.
        /// </summary>
        /// <param name="values">Array containing [0] Loc tile coordinates and [1] tile size integer.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">String "true" to use Y coordinate, "false" for X coordinate.</param>
        /// <param name="culture">The culture info.</param>
        /// <returns>The calculated pixel dimension.</returns>
        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is Loc tileXY && values[1] is int tileSize)
            {
                bool useY = Boolean.Parse((string)parameter);
                int diff = useY ? tileXY.Y : tileXY.X;
                return diff * tileSize;
            }
            return 0;
        }
    }
}
