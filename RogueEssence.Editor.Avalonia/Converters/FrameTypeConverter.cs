using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using RogueEssence.Content;
using RogueEssence.Data;
using RogueEssence.Dungeon;

namespace RogueEssence.Dev.Converters
{
    /// <summary>
    /// Converts a frame type index to the corresponding action name string.
    /// </summary>
    public class FrameTypeConverter : IValueConverter
    {
        /// <summary>
        /// Converts a frame type index to an action name.
        /// </summary>
        /// <param name="value">The frame type index.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">The culture info.</param>
        /// <returns>The action name, or "**EMPTY**" if index is out of range.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int idx)
            {
                if (idx >= 0 && idx < GraphicsManager.Actions.Count)
                    return GraphicsManager.Actions[idx].Name;
                return "**EMPTY**";
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
