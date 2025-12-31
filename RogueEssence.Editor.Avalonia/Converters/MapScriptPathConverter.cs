using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Avalonia.Data;
using Avalonia.Data.Converters;
using RogueEssence.Script;

namespace RogueEssence.Dev.Converters
{
    /// <summary>
    /// Converts a map file path to a display string showing the script data directory.
    /// </summary>
    public class MapScriptPathConverter : IValueConverter
    {
        /// <summary>
        /// Converts a map file path to a script data path display string.
        /// </summary>
        /// <param name="value">The map file path.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">The culture info.</param>
        /// <returns>A formatted string showing the script data directory.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string file = (string)value;
            if (file == "")
                return "Script Data [Map not yet saved]";

            string mapscriptdir = LuaEngine.MakeGroundMapScriptPath(Path.GetFileNameWithoutExtension(file), "");
            return String.Format("Script Data [{0}]", mapscriptdir);
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
