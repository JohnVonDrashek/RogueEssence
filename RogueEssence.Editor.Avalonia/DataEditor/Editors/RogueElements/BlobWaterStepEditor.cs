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
    /// Editor for IBlobWaterStep objects. Provides custom string representation showing blob amount and area scale.
    /// </summary>
    public class BlobWaterStepEditor : Editor<IBlobWaterStep>
    {
        /// <summary>
        /// Gets a string representation of the blob water step.
        /// </summary>
        /// <param name="obj">The blob water step to convert.</param>
        /// <param name="type">The type of the object.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <returns>A formatted string showing blob amount and area scale.</returns>
        public override string GetString(IBlobWaterStep obj, Type type, object[] attributes)
        {
            PropertyInfo blobInfo = typeof(IBlobWaterStep).GetProperty(nameof(obj.Blobs));
            PropertyInfo areaInfo = typeof(IBlobWaterStep).GetProperty(nameof(obj.AreaScale));
            return string.Format("{0}: Amt: {1} Size: {2}", obj.GetType().GetFormattedTypeName(),
                DataEditor.GetString(obj.Blobs, blobInfo.GetMemberInfoType(), blobInfo.GetCustomAttributes(false)),
                DataEditor.GetString(obj.AreaScale, areaInfo.GetMemberInfoType(), areaInfo.GetCustomAttributes(false)));
        }
    }
}
