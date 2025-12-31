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

namespace RogueEssence.Dev
{
    /// <summary>
    /// Editor for Priority values. Displays a text box for editing dot-separated priority numbers.
    /// </summary>
    public class PriorityEditor : Editor<Priority>
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
        /// Loads a text box control for editing a priority value in dot notation.
        /// </summary>
        /// <param name="control">The panel to add controls to.</param>
        /// <param name="parent">The parent object name.</param>
        /// <param name="parentType">The type of the parent object.</param>
        /// <param name="name">The name of the member being edited.</param>
        /// <param name="type">The type of the member.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <param name="member">The priority value to edit.</param>
        /// <param name="subGroupStack">Stack of subgroup types for nested editing.</param>
        public override void LoadWindowControls(StackPanel control, string parent, Type parentType, string name, Type type, object[] attributes, Priority member, Type[] subGroupStack)
        {
            //for strings, use an edit textbox
            TextBox txtValue = new TextBox();
            txtValue.Text = member.ToString();
            control.Children.Add(txtValue);
        }

        /// <summary>
        /// Saves the text box control and returns the parsed priority value.
        /// </summary>
        /// <param name="control">The panel containing the controls.</param>
        /// <param name="name">The name of the member.</param>
        /// <param name="type">The type of the member.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <param name="subGroupStack">Stack of subgroup types for nested editing.</param>
        /// <returns>The parsed Priority value from dot-separated text.</returns>
        public override Priority SaveWindowControls(StackPanel control, string name, Type type, object[] attributes, Type[] subGroupStack)
        {
            int controlIndex = 0;

            //attempt to parse
            //TODO: enforce validation
            TextBox txtValue = (TextBox)control.Children[controlIndex];
            string[] divText = txtValue.Text.Split('.');
            int[] divNums = new int[divText.Length];
            for (int ii = 0; ii < divText.Length; ii++)
            {
                int res;
                if (int.TryParse(divText[ii], out res))
                    divNums[ii] = res;
                else
                {
                    divNums = null;
                    break;
                }
            }
            return new Priority(divNums);
        }
    }
}
