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
    /// Editor for IRoomGenDefault objects. Displays a single-tile room representation.
    /// </summary>
    public class RoomGenDefaultEditor : Editor<IRoomGenDefault>
    {
        /// <summary>
        /// Gets a string representation of the default room generator.
        /// </summary>
        /// <param name="obj">The room generator to convert.</param>
        /// <param name="type">The type of the object.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <returns>The string "Single-Tile Room".</returns>
        public override string GetString(IRoomGenDefault obj, Type type, object[] attributes)
        {
            return string.Format("Single-Tile Room");
        }

        /// <summary>
        /// Gets a friendly type string for display purposes.
        /// </summary>
        /// <returns>The string "Single-Tile Room".</returns>
        public override string GetTypeString()
        {
            return string.Format("Single-Tile Room");
        }
    }

    /// <summary>
    /// Editor for ISizedRoomGen objects. Provides custom string representation showing width and height.
    /// </summary>
    public class SizedRoomGenEditor : Editor<ISizedRoomGen>
    {
        /// <summary>
        /// Gets a string representation of the sized room generator.
        /// </summary>
        /// <param name="obj">The room generator to convert.</param>
        /// <param name="type">The type of the object.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <returns>A formatted string showing width and height.</returns>
        public override string GetString(ISizedRoomGen obj, Type type, object[] attributes)
        {
            PropertyInfo widthInfo = typeof(ISizedRoomGen).GetProperty(nameof(obj.Width));
            PropertyInfo heightInfo = typeof(ISizedRoomGen).GetProperty(nameof(obj.Height));
            return string.Format("{0}: {1}x{2}", obj.GetType().GetFormattedTypeName(),
                DataEditor.GetString(obj.Width, widthInfo.GetMemberInfoType(), widthInfo.GetCustomAttributes(false)),
                DataEditor.GetString(obj.Height, heightInfo.GetMemberInfoType(), heightInfo.GetCustomAttributes(false)));
        }
    }

    /// <summary>
    /// Editor for IRoomGenCross objects. Provides custom string representation showing major and minor dimensions.
    /// </summary>
    public class RoomGenCrossEditor : Editor<IRoomGenCross>
    {
        /// <summary>
        /// Gets a string representation of the cross room generator.
        /// </summary>
        /// <param name="obj">The room generator to convert.</param>
        /// <param name="type">The type of the object.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <returns>A formatted string showing major and minor dimensions.</returns>
        public override string GetString(IRoomGenCross obj, Type type, object[] attributes)
        {
            PropertyInfo majorWidthInfo = typeof(IRoomGenCross).GetProperty(nameof(obj.MajorWidth));
            PropertyInfo minorHeightInfo = typeof(IRoomGenCross).GetProperty(nameof(obj.MinorHeight));
            PropertyInfo minorWidthInfo = typeof(IRoomGenCross).GetProperty(nameof(obj.MinorWidth));
            PropertyInfo majorHeightInfo = typeof(IRoomGenCross).GetProperty(nameof(obj.MajorHeight));

            return string.Format("{0}: {1}x{2}+{3}x{4}", obj.GetType().GetFormattedTypeName(),
                DataEditor.GetString(obj.MajorWidth, majorWidthInfo.GetMemberInfoType(), majorWidthInfo.GetCustomAttributes(false)),
                DataEditor.GetString(obj.MinorHeight, minorHeightInfo.GetMemberInfoType(), minorHeightInfo.GetCustomAttributes(false)),
                DataEditor.GetString(obj.MinorWidth, minorWidthInfo.GetMemberInfoType(), minorWidthInfo.GetCustomAttributes(false)),
                DataEditor.GetString(obj.MajorHeight, majorHeightInfo.GetMemberInfoType(), majorHeightInfo.GetCustomAttributes(false)));
        }
    }
}
