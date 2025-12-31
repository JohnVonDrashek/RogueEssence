using System;
using System.Globalization;
using Avalonia.Data.Converters;
using RogueEssence.Data;

namespace RogueEssence.Dev.Converters
{
    /// <summary>
    /// Checks if a data entry string is null, empty, or the default value for the specified data type.
    /// Returns true if the value represents "none" or is empty.
    /// </summary>
    public class IsNoneOrEmptyConverter : IValueConverter
    {
        /// <summary>
        /// Checks if the value is null, empty, or the default for the data type.
        /// </summary>
        /// <param name="value">The data entry string to check.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">The data type as a string-encoded integer.</param>
        /// <param name="culture">The culture info.</param>
        /// <returns>True if value is none or empty, otherwise false.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DataManager.DataType dataType = (DataManager.DataType)Int32.Parse((string)parameter);
            string s = (string) value;
            s = s.ToLower();

            bool res = (String.IsNullOrEmpty(s) || s == DataManager.Instance.GetDefaultData(dataType));
            return res;
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

    /// <summary>
    /// Inverse of IsNoneOrEmptyConverter - returns true if value has a valid, non-default entry.
    /// </summary>
    public class IsNotNoneOrEmptyConverter : IValueConverter
    {
        /// <summary>
        /// Checks if the value is NOT null, empty, or the default for the data type.
        /// </summary>
        /// <param name="value">The data entry string to check.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">The data type as a string-encoded integer.</param>
        /// <param name="culture">The culture info.</param>
        /// <returns>True if value is a valid entry, otherwise false.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DataManager.DataType dataType = (DataManager.DataType)Int32.Parse((string)parameter);
            string s = (string)value;
            s = s.ToLower();
            bool res = !(String.IsNullOrEmpty(s) || s == DataManager.Instance.GetDefaultData(dataType));
            return res;
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