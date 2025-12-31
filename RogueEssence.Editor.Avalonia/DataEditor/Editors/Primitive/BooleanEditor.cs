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
using System.IO;

namespace RogueEssence.Dev
{
    /// <summary>
    /// Editor for Boolean values. Displays a checkbox control for editing true/false values.
    /// </summary>
    public class BooleanEditor : Editor<Boolean>
    {
        /// <summary>
        /// Gets a value indicating whether the editor contents should be shown in a subgroup.
        /// </summary>
        public override bool DefaultSubgroup => true;

        /// <summary>
        /// Gets a value indicating whether the editor contents should have a border decoration.
        /// </summary>
        public override bool DefaultDecoration => false;

        /// <summary>
        /// Gets a value indicating whether the editor should display a label. False because the checkbox contains its own label.
        /// </summary>
        public override bool DefaultLabel => false;

        /// <summary>
        /// Loads a checkbox control for editing a boolean value.
        /// </summary>
        /// <param name="control">The panel to add controls to.</param>
        /// <param name="parent">The parent object name.</param>
        /// <param name="parentType">The type of the parent object.</param>
        /// <param name="name">The name of the member being edited.</param>
        /// <param name="type">The type of the member.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <param name="member">The boolean value to edit.</param>
        /// <param name="subGroupStack">Stack of subgroup types for nested editing.</param>
        public override void LoadWindowControls(StackPanel control, string parent, Type parentType, string name, Type type, object[] attributes, Boolean member, Type[] subGroupStack)
        {
            CheckBox chkValue = new CheckBox();
            chkValue.Margin = new Thickness(0, 4, 0, 0);
            chkValue.Content = Text.GetMemberTitle(name);
            chkValue.IsChecked = member;

            string desc = DevDataManager.GetMemberDoc(parentType, name);
            if (desc != null)
                ToolTip.SetTip(chkValue, desc);

            control.Children.Add(chkValue);
        }

        /// <summary>
        /// Saves the checkbox control state and returns the boolean value.
        /// </summary>
        /// <param name="control">The panel containing the controls.</param>
        /// <param name="name">The name of the member.</param>
        /// <param name="type">The type of the member.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <param name="subGroupStack">Stack of subgroup types for nested editing.</param>
        /// <returns>True if the checkbox is checked, false otherwise.</returns>
        public override Boolean SaveWindowControls(StackPanel control, string name, Type type, object[] attributes, Type[] subGroupStack)
        {
            int controlIndex = 0;
            CheckBox chkValue = (CheckBox)control.Children[controlIndex];
            return chkValue.IsChecked.HasValue && chkValue.IsChecked.Value;
        }

    }
}
