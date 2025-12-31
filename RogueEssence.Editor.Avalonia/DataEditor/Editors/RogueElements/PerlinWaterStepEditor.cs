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
    /// Editor for IPerlinWaterStep objects. Provides custom string representation showing water percentage and terrain.
    /// </summary>
    public class PerlinWaterStepEditor : Editor<IPerlinWaterStep>
    {
        /// <summary>
        /// Gets a string representation of the perlin water step.
        /// </summary>
        /// <param name="obj">The perlin water step to convert.</param>
        /// <param name="type">The type of the object.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <returns>A formatted string showing water percentage and terrain.</returns>
        public override string GetString(IPerlinWaterStep obj, Type type, object[] attributes)
        {
            PropertyInfo waterInfo = typeof(IPerlinWaterStep).GetProperty(nameof(obj.WaterPercent));
            PropertyInfo terrainInfo = typeof(IWaterStep).GetProperty(nameof(obj.Terrain));
            return string.Format("{0}: {1}% {2}", obj.GetType().GetFormattedTypeName(),
                DataEditor.GetString(obj.WaterPercent, waterInfo.GetMemberInfoType(), waterInfo.GetCustomAttributes(false)),
                DataEditor.GetString(obj.Terrain, terrainInfo.GetMemberInfoType(), terrainInfo.GetCustomAttributes(false)));
        }
    }
}
