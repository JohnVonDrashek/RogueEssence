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
    /// Editor for IStepSpawner objects. Enables subgroup display for step spawner editing.
    /// </summary>
    public class StepSpawnerEditor : Editor<IStepSpawner>
    {
        /// <summary>
        /// Gets a value indicating whether the editor contents should be shown in a subgroup.
        /// </summary>
        public override bool DefaultSubgroup => true;

    }

    /// <summary>
    /// Editor for IMultiStepSpawner objects. Provides custom string representation showing the picker configuration.
    /// </summary>
    public class MultiStepSpawnerEditor : Editor<IMultiStepSpawner>
    {
        /// <summary>
        /// Gets a value indicating whether the editor contents should be shown in a subgroup.
        /// </summary>
        public override bool DefaultSubgroup => true;

        /// <summary>
        /// Gets a string representation of the multi step spawner.
        /// </summary>
        /// <param name="obj">The spawner to convert.</param>
        /// <param name="type">The type of the object.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <returns>A formatted string showing the picker configuration.</returns>
        public override string GetString(IMultiStepSpawner obj, Type type, object[] attributes)
        {
            PropertyInfo memberInfo = typeof(IMultiStepSpawner).GetProperty(nameof(obj.Picker));
            return string.Format("{0}: {1}", obj.GetType().GetFormattedTypeName(), DataEditor.GetString(obj.Picker, memberInfo.GetMemberInfoType(), memberInfo.GetCustomAttributes(false)));
        }
    }
    /// <summary>
    /// Editor for IPickerSpawner objects. Provides custom string representation showing the picker configuration.
    /// </summary>
    public class PickerSpawnerEditor : Editor<IPickerSpawner>
    {
        /// <summary>
        /// Gets a value indicating whether the editor contents should be shown in a subgroup.
        /// </summary>
        public override bool DefaultSubgroup => true;

        /// <summary>
        /// Gets a string representation of the picker spawner.
        /// </summary>
        /// <param name="obj">The spawner to convert.</param>
        /// <param name="type">The type of the object.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <returns>A formatted string showing the picker configuration.</returns>
        public override string GetString(IPickerSpawner obj, Type type, object[] attributes)
        {
            PropertyInfo memberInfo = typeof(IPickerSpawner).GetProperty(nameof(obj.Picker));
            return string.Format("{0}: {1}", obj.GetType().GetFormattedTypeName(), DataEditor.GetString(obj.Picker, memberInfo.GetMemberInfoType(), memberInfo.GetCustomAttributes(false)));
        }
    }
    /// <summary>
    /// Editor for IDivSpawner objects. Provides custom string representation showing the division amount.
    /// </summary>
    public class MoneyDivSpawnerEditor : Editor<IDivSpawner>
    {
        /// <summary>
        /// Gets a value indicating whether the editor contents should be shown in a subgroup.
        /// </summary>
        public override bool DefaultSubgroup => true;

        /// <summary>
        /// Gets a string representation of the div spawner.
        /// </summary>
        /// <param name="obj">The spawner to convert.</param>
        /// <param name="type">The type of the object.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <returns>A formatted string showing the division amount.</returns>
        public override string GetString(IDivSpawner obj, Type type, object[] attributes)
        {
            PropertyInfo memberInfo = typeof(IDivSpawner).GetProperty(nameof(obj.DivAmount));
            return string.Format("{0}: {1}", obj.GetType().GetFormattedTypeName(), DataEditor.GetString(obj.DivAmount, memberInfo.GetMemberInfoType(), memberInfo.GetCustomAttributes(false)));
        }
    }
    /// <summary>
    /// Editor for IContextSpawner objects. Provides custom string representation showing the spawn amount.
    /// </summary>
    public class ContextSpawnerEditor : Editor<IContextSpawner>
    {
        /// <summary>
        /// Gets a value indicating whether the editor contents should be shown in a subgroup.
        /// </summary>
        public override bool DefaultSubgroup => true;

        /// <summary>
        /// Gets a string representation of the context spawner.
        /// </summary>
        /// <param name="obj">The spawner to convert.</param>
        /// <param name="type">The type of the object.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <returns>A formatted string showing the spawn amount.</returns>
        public override string GetString(IContextSpawner obj, Type type, object[] attributes)
        {
            PropertyInfo memberInfo = typeof(IContextSpawner).GetProperty(nameof(obj.Amount));
            return string.Format("{0}: {1}", obj.GetType().GetFormattedTypeName(), DataEditor.GetString(obj.Amount, memberInfo.GetMemberInfoType(), memberInfo.GetCustomAttributes(false)));
        }
    }
    /// <summary>
    /// Editor for ITeamContextSpawner objects. Provides custom string representation showing the spawn amount.
    /// </summary>
    public class TeamContextSpawnerEditor : Editor<ITeamContextSpawner>
    {
        /// <summary>
        /// Gets a value indicating whether the editor contents should be shown in a subgroup.
        /// </summary>
        public override bool DefaultSubgroup => true;

        /// <summary>
        /// Gets a string representation of the team context spawner.
        /// </summary>
        /// <param name="obj">The spawner to convert.</param>
        /// <param name="type">The type of the object.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <returns>A formatted string showing the spawn amount.</returns>
        public override string GetString(ITeamContextSpawner obj, Type type, object[] attributes)
        {
            PropertyInfo memberInfo = typeof(ITeamContextSpawner).GetProperty(nameof(obj.Amount));
            return string.Format("{0}: {1}", obj.GetType().GetFormattedTypeName(), DataEditor.GetString(obj.Amount, memberInfo.GetMemberInfoType(), memberInfo.GetCustomAttributes(false)));
        }
    }
    /// <summary>
    /// Editor for ILoopedTeamSpawner objects. Provides custom string representation showing the amount spawner.
    /// </summary>
    public class LoopedTeamSpawnerEditor : Editor<ILoopedTeamSpawner>
    {
        /// <summary>
        /// Gets a value indicating whether the editor contents should be shown in a subgroup.
        /// </summary>
        public override bool DefaultSubgroup => true;

        /// <summary>
        /// Gets a string representation of the looped team spawner.
        /// </summary>
        /// <param name="obj">The spawner to convert.</param>
        /// <param name="type">The type of the object.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <returns>A formatted string showing the amount spawner.</returns>
        public override string GetString(ILoopedTeamSpawner obj, Type type, object[] attributes)
        {
            PropertyInfo memberInfo = typeof(ILoopedTeamSpawner).GetProperty(nameof(obj.AmountSpawner));
            return string.Format("{0}: {1}", obj.GetType().GetFormattedTypeName(), DataEditor.GetString(obj.AmountSpawner, memberInfo.GetMemberInfoType(), memberInfo.GetCustomAttributes(false)));
        }
    }
}
