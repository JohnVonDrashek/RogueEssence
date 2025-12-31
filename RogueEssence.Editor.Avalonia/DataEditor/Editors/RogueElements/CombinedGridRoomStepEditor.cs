using System;
using System.Collections.Generic;
using System.Text;
using RogueEssence.Content;
using RogueEssence.Dungeon;
using RogueEssence.Data;
using System.Drawing;
using RogueElements;
using Avalonia.Controls;
using RogueEssence.Dev.Views;
using System.Collections;
using Avalonia;
using System.Reactive.Subjects;
using RogueEssence.LevelGen;
using System.Reflection;

namespace RogueEssence.Dev
{
    /// <summary>
    /// Editor for ICombineGridRoomStep objects. Provides custom string representation showing combo count and merge rate.
    /// </summary>
    public class CombinedGridRoomStepEditor : Editor<ICombineGridRoomStep>
    {
        /// <summary>
        /// Gets a string representation of the combined grid room step.
        /// </summary>
        /// <param name="obj">The combined grid room step to convert.</param>
        /// <param name="type">The type of the object.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <returns>A formatted string showing combo count and merge rate.</returns>
        public override string GetString(ICombineGridRoomStep obj, Type type, object[] attributes)
        {
            PropertyInfo mergeRateInfo = typeof(ICombineGridRoomStep).GetProperty(nameof(obj.MergeRate));
            return string.Format("{0}[{1}]: Amount:{2}", obj.GetType().GetFormattedTypeName(),
                obj.Combos.Count,
                DataEditor.GetString(obj.MergeRate, mergeRateInfo.GetMemberInfoType(), mergeRateInfo.GetCustomAttributes(false)));
        }
    }
}