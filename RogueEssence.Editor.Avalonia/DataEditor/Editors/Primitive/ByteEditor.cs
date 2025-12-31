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
    /// Editor for Byte values. Displays a numeric up-down control for editing byte values (0-255).
    /// Supports NumberRangeAttribute for custom min/max constraints.
    /// </summary>
    public class ByteEditor : Editor<Byte>
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
        /// Loads a numeric up-down control for editing a byte value.
        /// </summary>
        /// <param name="control">The panel to add controls to.</param>
        /// <param name="parent">The parent object name.</param>
        /// <param name="parentType">The type of the parent object.</param>
        /// <param name="name">The name of the member being edited.</param>
        /// <param name="type">The type of the member.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <param name="member">The byte value to edit.</param>
        /// <param name="subGroupStack">Stack of subgroup types for nested editing.</param>
        public override void LoadWindowControls(StackPanel control, string parent, Type parentType, string name, Type type, object[] attributes, Byte member, Type[] subGroupStack)
        {
            NumericUpDown nudValue = new NumericUpDown();
            nudValue.Minimum = byte.MinValue;
            nudValue.Maximum = byte.MaxValue;
            NumberRangeAttribute rangeAtt = ReflectionExt.FindAttribute<NumberRangeAttribute>(attributes);
            if (rangeAtt != null)
            {
                nudValue.Minimum = rangeAtt.Min;
                nudValue.Maximum = rangeAtt.Max;
            }
            nudValue.Value = (byte)member;

            control.Children.Add(nudValue);
        }

        /// <summary>
        /// Saves the numeric control state and returns the byte value.
        /// </summary>
        /// <param name="control">The panel containing the controls.</param>
        /// <param name="name">The name of the member.</param>
        /// <param name="type">The type of the member.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <param name="subGroupStack">Stack of subgroup types for nested editing.</param>
        /// <returns>The byte value from the numeric control.</returns>
        public override Byte SaveWindowControls(StackPanel control, string name, Type type, object[] attributes, Type[] subGroupStack)
        {
            int controlIndex = 0;
            NumericUpDown nudValue = (NumericUpDown)control.Children[controlIndex];
            return (byte)nudValue.Value;
        }

    }
}
