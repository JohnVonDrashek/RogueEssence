using System;
using System.Globalization;
using Avalonia.Data.Converters;
using RogueEssence.Data;

namespace RogueEssence.Dev.Converters
{
    /// <summary>
    /// Converts a skill category enum to its corresponding icon image.
    /// </summary>
    public class SkillCategoryIconConverter : IValueConverter
    {
        /// <summary>
        /// Converts a skill category to its icon.
        /// </summary>
        /// <param name="value">The SkillCategory enum value.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">The culture info.</param>
        /// <returns>The skill category icon bitmap.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BattleData.SkillCategory category = (BattleData.SkillCategory)value;
            return DevDataManager.GetSkillCategoryIcon(category);
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