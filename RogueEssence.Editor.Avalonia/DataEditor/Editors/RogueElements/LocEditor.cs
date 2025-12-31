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
    /// Editor for Loc (location/coordinate) values. Displays X and Y numeric controls for editing 2D coordinates.
    /// </summary>
    public class LocEditor : Editor<Loc>
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
        /// Loads X and Y numeric controls for editing a location.
        /// </summary>
        /// <param name="control">The panel to add controls to.</param>
        /// <param name="parent">The parent object name.</param>
        /// <param name="parentType">The type of the parent object.</param>
        /// <param name="name">The name of the member being edited.</param>
        /// <param name="type">The type of the member.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <param name="member">The location value to edit.</param>
        /// <param name="subGroupStack">Stack of subgroup types for nested editing.</param>
        public override void LoadWindowControls(StackPanel control, string parent, Type parentType, string name, Type type, object[] attributes, Loc member, Type[] subGroupStack)
        {
            Avalonia.Controls.Grid innerPanel = getSharedRowPanel(4);

            TextBlock lblX = new TextBlock();
            lblX.Text = "X:";
            lblX.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
            lblX.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right;
            innerPanel.Children.Add(lblX);
            innerPanel.ColumnDefinitions[0].Width = new GridLength(30);
            lblX.SetValue(Avalonia.Controls.Grid.ColumnProperty, 0);

            NumericUpDown nudValueX = new NumericUpDown();
            nudValueX.Margin = new Thickness(4, 0, 0, 0);
            nudValueX.Minimum = Int32.MinValue;
            nudValueX.Maximum = Int32.MaxValue;
            nudValueX.Value = member.X;
            innerPanel.Children.Add(nudValueX);
            nudValueX.SetValue(Avalonia.Controls.Grid.ColumnProperty, 1);

            TextBlock lblY = new TextBlock();
            lblY.Margin = new Thickness(8, 0, 0, 0);
            lblY.Text = "Y:";
            lblY.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
            lblY.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right;
            innerPanel.Children.Add(lblY);
            innerPanel.ColumnDefinitions[2].Width = new GridLength(30);
            lblY.SetValue(Avalonia.Controls.Grid.ColumnProperty, 2);


            NumericUpDown nudValueY = new NumericUpDown();
            nudValueY.Margin = new Thickness(4, 0, 0, 0);
            nudValueY.Minimum = Int32.MinValue;
            nudValueY.Maximum = Int32.MaxValue;
            nudValueY.Value = member.Y;
            innerPanel.Children.Add(nudValueY);
            nudValueY.SetValue(Avalonia.Controls.Grid.ColumnProperty, 3);

            control.Children.Add(innerPanel);
        }

        /// <summary>
        /// Saves the numeric controls and returns the location value.
        /// </summary>
        /// <param name="control">The panel containing the controls.</param>
        /// <param name="name">The name of the member.</param>
        /// <param name="type">The type of the member.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <param name="subGroupStack">Stack of subgroup types for nested editing.</param>
        /// <returns>The edited Loc value.</returns>
        public override Loc SaveWindowControls(StackPanel control, string name, Type type, object[] attributes, Type[] subGroupStack)
        {
            int controlIndex = 0;

            Avalonia.Controls.Grid innerControl = (Avalonia.Controls.Grid)control.Children[controlIndex];
            int innerControlIndex = 0;

            innerControlIndex++;
            NumericUpDown nudValueX = (NumericUpDown)innerControl.Children[innerControlIndex];
            innerControlIndex++;
            innerControlIndex++;
            NumericUpDown nudValueY = (NumericUpDown)innerControl.Children[innerControlIndex];
            return new Loc((int)nudValueX.Value, (int)nudValueY.Value);
        }
    }
}
