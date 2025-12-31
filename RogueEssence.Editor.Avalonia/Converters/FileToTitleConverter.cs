using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace RogueEssence.Dev.Converters
{
    /// <summary>
    /// Converts a file path to a window title, extracting just the filename without extension.
    /// Returns "New File" for empty paths.
    /// </summary>
    public class FileToTitleConverter : IValueConverter
    {
        /// <summary>
        /// Converts a file path to a display title.
        /// </summary>
        /// <param name="value">The file path string.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">The culture info.</param>
        /// <returns>The filename without extension, or "New File" if empty.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string file = (string)value;
            if (file == "")
                return "New File";
            else
                return Path.GetFileNameWithoutExtension(file);
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
