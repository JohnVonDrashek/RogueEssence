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
    /// Editor for Int32 values. Displays a numeric up-down control for editing integer values.
    /// Supports NumberRangeAttribute for custom min/max constraints and IntRangeAttribute for 1-based indexing.
    /// </summary>
    public class IntEditor : Editor<Int32>
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
        /// Loads a numeric up-down control for editing an integer value.
        /// </summary>
        /// <param name="control">The panel to add controls to.</param>
        /// <param name="parent">The parent object name.</param>
        /// <param name="parentType">The type of the parent object.</param>
        /// <param name="name">The name of the member being edited.</param>
        /// <param name="type">The type of the member.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <param name="member">The integer value to edit.</param>
        /// <param name="subGroupStack">Stack of subgroup types for nested editing.</param>
        public override void LoadWindowControls(StackPanel control, string parent, Type parentType, string name, Type type, object[] attributes, Int32 member, Type[] subGroupStack)
        {
            NumericUpDown nudValue = new NumericUpDown();
            int minimum = Int32.MinValue;
            int maximum = Int32.MaxValue;
            NumberRangeAttribute rangeAtt = ReflectionExt.FindAttribute<NumberRangeAttribute>(attributes);
            if (rangeAtt != null)
            {
                minimum = rangeAtt.Min;
                maximum = rangeAtt.Max;
            }
            IntRangeAttribute intAtt = ReflectionExt.FindAttribute<IntRangeAttribute>(attributes);
            if (intAtt != null)
            {
                if (intAtt.Index1)
                {
                    minimum += 1;
                    if (maximum < Int32.MaxValue)
                        maximum += 1;
                    member += 1;
                }
            }
            nudValue.Minimum = minimum;
            nudValue.Maximum = maximum;
            nudValue.Value = member;

            control.Children.Add(nudValue);
        }

        /// <summary>
        /// Saves the numeric control state and returns the integer value.
        /// Adjusts for 1-based indexing if IntRangeAttribute.Index1 is set.
        /// </summary>
        /// <param name="control">The panel containing the controls.</param>
        /// <param name="name">The name of the member.</param>
        /// <param name="type">The type of the member.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <param name="subGroupStack">Stack of subgroup types for nested editing.</param>
        /// <returns>The integer value from the numeric control.</returns>
        public override Int32 SaveWindowControls(StackPanel control, string name, Type type, object[] attributes, Type[] subGroupStack)
        {
            int controlIndex = 0;

            NumericUpDown nudValue = (NumericUpDown)control.Children[controlIndex];
            int member = (Int32)nudValue.Value;

            IntRangeAttribute intAtt = ReflectionExt.FindAttribute<IntRangeAttribute>(attributes);
            if (intAtt != null && intAtt.Index1)
                member -= 1;
            return member;
        }

    }
}
