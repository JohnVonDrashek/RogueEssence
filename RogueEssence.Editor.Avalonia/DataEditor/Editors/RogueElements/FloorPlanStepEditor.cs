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
    /// Editor for IAddConnectedRoomsStep objects. Provides custom string representation showing amount and hall percentage.
    /// </summary>
    public class AddConnectedRoomsStepEditor : Editor<IAddConnectedRoomsStep>
    {
        /// <summary>
        /// Gets a string representation of the add connected rooms step.
        /// </summary>
        /// <param name="obj">The step to convert.</param>
        /// <param name="type">The type of the object.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <returns>A formatted string showing amount and hall percentage.</returns>
        public override string GetString(IAddConnectedRoomsStep obj, Type type, object[] attributes)
        {
            PropertyInfo amountInfo = typeof(IAddConnectedRoomsStep).GetProperty(nameof(obj.Amount));
            PropertyInfo hallInfo = typeof(IAddConnectedRoomsStep).GetProperty(nameof(obj.HallPercent));
            return string.Format("{0}: Add:{1} Hall:{2}%", obj.GetType().GetFormattedTypeName(),
                DataEditor.GetString(obj.Amount, amountInfo.GetMemberInfoType(), amountInfo.GetCustomAttributes(false)),
                DataEditor.GetString(obj.HallPercent, hallInfo.GetMemberInfoType(), hallInfo.GetCustomAttributes(false)));
        }
    }

    /// <summary>
    /// Editor for IAddDisconnectedRoomsStep objects. Provides custom string representation showing amount.
    /// </summary>
    public class AddDisconnectedRoomsStepEditor : Editor<IAddDisconnectedRoomsStep>
    {
        /// <summary>
        /// Gets a string representation of the add disconnected rooms step.
        /// </summary>
        /// <param name="obj">The step to convert.</param>
        /// <param name="type">The type of the object.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <returns>A formatted string showing amount.</returns>
        public override string GetString(IAddDisconnectedRoomsStep obj, Type type, object[] attributes)
        {
            PropertyInfo amountInfo = typeof(IAddDisconnectedRoomsStep).GetProperty(nameof(obj.Amount));
            return string.Format("{0}: Add:{1}", obj.GetType().GetFormattedTypeName(),
                DataEditor.GetString(obj.Amount, amountInfo.GetMemberInfoType(), amountInfo.GetCustomAttributes(false)));
        }
    }

    /// <summary>
    /// Editor for IConnectRoomStep objects. Provides custom string representation showing connect factor percentage.
    /// </summary>
    public class ConnectRoomStepEditor : Editor<IConnectRoomStep>
    {
        /// <summary>
        /// Gets a string representation of the connect room step.
        /// </summary>
        /// <param name="obj">The step to convert.</param>
        /// <param name="type">The type of the object.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <returns>A formatted string showing connect factor percentage.</returns>
        public override string GetString(IConnectRoomStep obj, Type type, object[] attributes)
        {
            PropertyInfo connectInfo = typeof(IConnectRoomStep).GetProperty(nameof(obj.ConnectFactor));
            return string.Format("{0}: {1}%", obj.GetType().GetFormattedTypeName(),
                DataEditor.GetString(obj.ConnectFactor, connectInfo.GetMemberInfoType(), connectInfo.GetCustomAttributes(false)));
        }
    }

    /// <summary>
    /// Editor for IFloorPathBranch objects. Provides custom string representation showing fill, hall, and branch percentages.
    /// </summary>
    public class FloorPathBranchEditor : Editor<IFloorPathBranch>
    {
        /// <summary>
        /// Gets a string representation of the floor path branch.
        /// </summary>
        /// <param name="obj">The floor path branch to convert.</param>
        /// <param name="type">The type of the object.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <returns>A formatted string showing fill, hall, and branch percentages.</returns>
        public override string GetString(IFloorPathBranch obj, Type type, object[] attributes)
        {
            PropertyInfo fillInfo = typeof(IFloorPathBranch).GetProperty(nameof(obj.FillPercent));
            PropertyInfo hallInfo = typeof(IFloorPathBranch).GetProperty(nameof(obj.HallPercent));
            PropertyInfo branchInfo = typeof(IFloorPathBranch).GetProperty(nameof(obj.BranchRatio));
            return string.Format("{0}: Fill:{1}% Hall:{2}% Branch:{3}%", obj.GetType().GetFormattedTypeName(),
                DataEditor.GetString(obj.FillPercent, fillInfo.GetMemberInfoType(), fillInfo.GetCustomAttributes(false)),
                DataEditor.GetString(obj.HallPercent, hallInfo.GetMemberInfoType(), hallInfo.GetCustomAttributes(false)),
                DataEditor.GetString(obj.BranchRatio, branchInfo.GetMemberInfoType(), branchInfo.GetCustomAttributes(false)));
        }
    }
}
