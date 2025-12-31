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
using System.Reflection;

namespace RogueEssence.Dev
{
    /// <summary>
    /// Editor for ILoopedRand objects. Provides custom string representation showing the amount spawner.
    /// </summary>
    public class LoopedRandEditor : Editor<ILoopedRand>
    {
        /// <summary>
        /// Gets a string representation of the looped rand.
        /// </summary>
        /// <param name="obj">The looped rand to convert.</param>
        /// <param name="type">The type of the object.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <returns>A formatted string showing the amount spawner.</returns>
        public override string GetString(ILoopedRand obj, Type type, object[] attributes)
        {
            if (obj.AmountSpawner == null)
                return string.Format("{0}[EMPTY]", type.GetFormattedTypeName());

            PropertyInfo memberInfo = typeof(ILoopedRand).GetProperty(nameof(obj.AmountSpawner));
            return string.Format("{0}[{1}]", type.GetFormattedTypeName(), DataEditor.GetString(obj.AmountSpawner, memberInfo.GetMemberInfoType(), memberInfo.GetCustomAttributes(false)));
        }
    }

    /// <summary>
    /// Editor for IPresetMultiRand objects. Provides custom string representation showing preset spawn count.
    /// </summary>
    public class PresetMultiRandEditor : Editor<IPresetMultiRand>
    {
        /// <summary>
        /// Gets a string representation of the preset multi rand.
        /// </summary>
        /// <param name="obj">The preset multi rand to convert.</param>
        /// <param name="type">The type of the object.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <returns>A formatted string showing the preset spawns.</returns>
        public override string GetString(IPresetMultiRand obj, Type type, object[] attributes)
        {
            if (obj.Count == 1)
            {
                object spawn = obj.ToSpawn[0];
                return string.Format("{{{0}}}", spawn.ToString());
            }
            return string.Format("{0}[{1}]", this.GetType().GetFormattedTypeName(), obj.Count);
        }
    }
}
