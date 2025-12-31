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
    /// Editor for IBaseSpawnStep objects. Provides custom string representation showing spawn type and spawner.
    /// </summary>
    public class BaseSpawnStepEditor : Editor<IBaseSpawnStep>
    {
        /// <summary>
        /// Gets a string representation of the base spawn step.
        /// </summary>
        /// <param name="obj">The spawn step to convert.</param>
        /// <param name="type">The type of the object.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <returns>A formatted string showing spawn type and spawner.</returns>
        public override string GetString(IBaseSpawnStep obj, Type type, object[] attributes)
        {
            PropertyInfo memberInfo = typeof(IBaseSpawnStep).GetProperty(nameof(obj.Spawn));
            if (obj.Spawn == null)
                return string.Format("{0}<{1}>: [EMPTY]", obj.GetType().GetFormattedTypeName(), obj.SpawnType.Name);
            return string.Format("{0}<{1}>: {2}", obj.GetType().GetFormattedTypeName(), obj.SpawnType.Name, DataEditor.GetString(obj.Spawn, memberInfo.GetMemberInfoType(), memberInfo.GetCustomAttributes(false)));
        }
    }
    /// <summary>
    /// Editor for IPlaceMobsStep objects. Provides custom string representation showing the spawn configuration.
    /// </summary>
    public class PlaceMobsStepEditor : Editor<IPlaceMobsStep>
    {
        /// <summary>
        /// Gets a string representation of the place mobs step.
        /// </summary>
        /// <param name="obj">The step to convert.</param>
        /// <param name="type">The type of the object.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <returns>A formatted string showing the spawn configuration.</returns>
        public override string GetString(IPlaceMobsStep obj, Type type, object[] attributes)
        {
            PropertyInfo memberInfo = typeof(IPlaceMobsStep).GetProperty(nameof(obj.Spawn));
            if (obj.Spawn == null)
                return string.Format("{0}: [EMPTY]", obj.GetType().GetFormattedTypeName());
            return string.Format("{0}: {1}", obj.GetType().GetFormattedTypeName(), DataEditor.GetString(obj.Spawn, memberInfo.GetMemberInfoType(), memberInfo.GetCustomAttributes(false)));
        }
    }
    /// <summary>
    /// Editor for IMoneySpawnStep objects. Provides custom string representation showing the money range.
    /// </summary>
    public class MoneySpawnStepEditor : Editor<IMoneySpawnStep>
    {
        /// <summary>
        /// Gets a string representation of the money spawn step.
        /// </summary>
        /// <param name="obj">The step to convert.</param>
        /// <param name="type">The type of the object.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <returns>A formatted string showing the money range.</returns>
        public override string GetString(IMoneySpawnStep obj, Type type, object[] attributes)
        {
            PropertyInfo memberInfo = typeof(IMoneySpawnStep).GetProperty(nameof(obj.MoneyRange));
            return string.Format("{0}: {1}", obj.GetType().GetFormattedTypeName(), DataEditor.GetString(obj.MoneyRange, memberInfo.GetMemberInfoType(), memberInfo.GetCustomAttributes(false)));
        }
    }
}
