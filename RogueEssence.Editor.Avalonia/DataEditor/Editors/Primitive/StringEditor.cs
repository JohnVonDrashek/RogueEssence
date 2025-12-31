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
    /// Editor for String values. Displays a text box for editing string content.
    /// Supports MultilineAttribute for multi-line text input and SanitizeAttribute for text sanitization on save.
    /// </summary>
    public class StringEditor : Editor<String>
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
        /// Loads a text box control for editing a string value.
        /// </summary>
        /// <param name="control">The panel to add controls to.</param>
        /// <param name="parent">The parent object name.</param>
        /// <param name="parentType">The type of the parent object.</param>
        /// <param name="name">The name of the member being edited.</param>
        /// <param name="type">The type of the member.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <param name="member">The string value to edit.</param>
        /// <param name="subGroupStack">Stack of subgroup types for nested editing.</param>
        public override void LoadWindowControls(StackPanel control, string parent, Type parentType, string name, Type type, object[] attributes, String member, Type[] subGroupStack)
        {
            //for strings, use an edit textbox
            TextBox txtValue = new TextBox();
            //txtValue.Dock = DockStyle.Fill;
            MultilineAttribute attribute = ReflectionExt.FindAttribute<MultilineAttribute>(attributes);
            if (attribute != null)
            {
                txtValue.AcceptsReturn = true;
                txtValue.Height = 80;
                //txtValue.Size = new Size(0, 80);
            }
            //else
            //    txtValue.Size = new Size(0, 20);
            txtValue.Text = (member == null) ? "" : member;
            control.Children.Add(txtValue);
        }

        /// <summary>
        /// Saves the text box control state and returns the string value.
        /// Applies text sanitization if SanitizeAttribute is present.
        /// </summary>
        /// <param name="control">The panel containing the controls.</param>
        /// <param name="name">The name of the member.</param>
        /// <param name="type">The type of the member.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <param name="subGroupStack">Stack of subgroup types for nested editing.</param>
        /// <returns>The string value from the text box, possibly sanitized.</returns>
        public override String SaveWindowControls(StackPanel control, string name, Type type, object[] attributes, Type[] subGroupStack)
        {
            int controlIndex = 0;

            TextBox txtValue = (TextBox)control.Children[controlIndex];

            SanitizeAttribute attribute = ReflectionExt.FindAttribute<SanitizeAttribute>(attributes);
            if (attribute != null)
                return Text.Sanitize(txtValue.Text);

            return txtValue.Text;
        }
    }
}
