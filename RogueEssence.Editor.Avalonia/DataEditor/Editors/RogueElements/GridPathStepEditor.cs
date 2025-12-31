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
    /// Editor for IGridPathCircle objects. Provides custom string representation showing circle room ratio and paths percentage.
    /// </summary>
    public class GridPathCircleEditor : Editor<IGridPathCircle>
    {
        /// <summary>
        /// Gets a string representation of the grid path circle.
        /// </summary>
        /// <param name="obj">The grid path circle to convert.</param>
        /// <param name="type">The type of the object.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <returns>A formatted string showing fill and paths percentages.</returns>
        public override string GetString(IGridPathCircle obj, Type type, object[] attributes)
        {
            PropertyInfo fillInfo = typeof(IGridPathCircle).GetProperty(nameof(obj.CircleRoomRatio));
            PropertyInfo pathsInfo = typeof(IGridPathCircle).GetProperty(nameof(obj.Paths));
            return string.Format("{0}: Fill:{1}% Paths:{2}%", obj.GetType().GetFormattedTypeName(),
                DataEditor.GetString(obj.CircleRoomRatio, fillInfo.GetMemberInfoType(), fillInfo.GetCustomAttributes(false)),
                DataEditor.GetString(obj.Paths, pathsInfo.GetMemberInfoType(), pathsInfo.GetCustomAttributes(false)));
        }
    }

    /// <summary>
    /// Editor for IGridPathBranch objects. Provides custom string representation showing room and branch ratios.
    /// </summary>
    public class GridPathBranchEditor : Editor<IGridPathBranch>
    {
        /// <summary>
        /// Gets a string representation of the grid path branch.
        /// </summary>
        /// <param name="obj">The grid path branch to convert.</param>
        /// <param name="type">The type of the object.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <returns>A formatted string showing fill and branch percentages.</returns>
        public override string GetString(IGridPathBranch obj, Type type, object[] attributes)
        {
            PropertyInfo fillInfo = typeof(IGridPathBranch).GetProperty(nameof(obj.RoomRatio));
            PropertyInfo branchInfo = typeof(IGridPathBranch).GetProperty(nameof(obj.BranchRatio));
            return string.Format("{0}: Fill:{1}% Branch:{2}%", obj.GetType().GetFormattedTypeName(),
                DataEditor.GetString(obj.RoomRatio, fillInfo.GetMemberInfoType(), fillInfo.GetCustomAttributes(false)),
                DataEditor.GetString(obj.BranchRatio, branchInfo.GetMemberInfoType(), branchInfo.GetCustomAttributes(false)));
        }
    }

    /// <summary>
    /// Editor for IGridPathGrid objects. Provides custom string representation showing room and hall ratios.
    /// </summary>
    public class GridPathGridEditor : Editor<IGridPathGrid>
    {
        /// <summary>
        /// Gets a string representation of the grid path grid.
        /// </summary>
        /// <param name="obj">The grid path grid to convert.</param>
        /// <param name="type">The type of the object.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <returns>A formatted string showing fill and branch percentages.</returns>
        public override string GetString(IGridPathGrid obj, Type type, object[] attributes)
        {
            PropertyInfo fillInfo = typeof(IGridPathGrid).GetProperty(nameof(obj.RoomRatio));
            PropertyInfo branchInfo = typeof(IGridPathGrid).GetProperty(nameof(obj.HallRatio));
            return string.Format("{0}: Fill:{1}% Branch:{2}%", obj.GetType().GetFormattedTypeName(),
                DataEditor.GetString(obj.RoomRatio, fillInfo.GetMemberInfoType(), fillInfo.GetCustomAttributes(false)),
                DataEditor.GetString(obj.HallRatio, branchInfo.GetMemberInfoType(), branchInfo.GetCustomAttributes(false)));
        }
        
        /// <summary>
        /// Gets a friendly type string for display purposes.
        /// </summary>
        /// <returns>The string "Grid Path Crossroads".</returns>
        public override string GetTypeString()
        {
            return "Grid Path Crossroads";
        }
    }
}
