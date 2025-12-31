using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using RogueEssence.Content;
using RogueEssence.Dungeon;

namespace RogueEssence.Dev.Converters
{
    /// <summary>
    /// Converts a TileFrame to a display string showing the sheet name and texture coordinates.
    /// Shows "[EMPTY]" for empty frames and marks invalid frames with "[X]".
    /// </summary>
    public class FrameConverter : IValueConverter
    {
        /// <summary>
        /// Converts a TileFrame to a descriptive string.
        /// </summary>
        /// <param name="value">The TileFrame to convert.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">The culture info.</param>
        /// <returns>A formatted string describing the frame location.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TileFrame frame)
            {
                if (frame == TileFrame.Empty)
                    return "[EMPTY]";
                long tilePos = GraphicsManager.TileIndex.GetPosition(frame.Sheet, frame.TexLoc);
                if (tilePos > 0)
                    return String.Format("{0}: X{1} Y{2}", frame.Sheet, frame.TexLoc.X, frame.TexLoc.Y);
                else
                    return String.Format("[X] {0}: X{1} Y{2}", frame.Sheet, frame.TexLoc.X, frame.TexLoc.Y);
            }
            return value;
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
