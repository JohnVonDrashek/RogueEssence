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
    /// Editor for Type values. Displays a combo box for selecting from assignable types.
    /// Uses TypeConstraintAttribute to determine the base type for available selections.
    /// </summary>
    public class TypeEditor : Editor<Type>
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
        /// Gets a value indicating whether the editor should display a label. False because the control includes its own label.
        /// </summary>
        public override bool DefaultLabel => false;

        /// <summary>
        /// Gets a value indicating whether the editor should display type information.
        /// </summary>
        public override bool DefaultType => true;

        /// <summary>
        /// Loads a combo box control for selecting a type from available assignable types.
        /// </summary>
        /// <param name="control">The panel to add controls to.</param>
        /// <param name="parent">The parent object name.</param>
        /// <param name="parentType">The type of the parent object.</param>
        /// <param name="name">The name of the member being edited.</param>
        /// <param name="type">The type of the member.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <param name="member">The Type value to edit.</param>
        /// <param name="subGroupStack">Stack of subgroup types for nested editing.</param>
        public override void LoadWindowControls(StackPanel control, string parent, Type parentType, string name, Type type, object[] attributes, Type member, Type[] subGroupStack)
        {
            TypeConstraintAttribute dataAtt = ReflectionExt.FindAttribute<TypeConstraintAttribute>(attributes);
            Type baseType = dataAtt.BaseClass;

            Type[] children = baseType.GetAssignableTypes();
            control.DataContext = children;

            Avalonia.Controls.Grid sharedRowPanel = getSharedRowPanel(2);

            TextBlock lblType = new TextBlock();
            lblType.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
            lblType.Text = name + ":";
            sharedRowPanel.Children.Add(lblType);
            sharedRowPanel.ColumnDefinitions[0].Width = new GridLength(30);
            lblType.SetValue(Avalonia.Controls.Grid.ColumnProperty, 0);

            ComboBox cbValue = new SearchComboBox();
            cbValue.Margin = new Thickness(4, 0, 0, 0);
            sharedRowPanel.Children.Add(cbValue);
            cbValue.SetValue(Avalonia.Controls.Grid.ColumnProperty, 1);

            List<string> items = new List<string>();
            int selection = 0;
            for (int ii = 0; ii < children.Length; ii++)
            {
                Type childType = children[ii];
                items.Add(childType.GetFriendlyTypeString());

                if (childType == (Type)member)
                    selection = ii;
            }

            var subject = new Subject<List<string>>();
            cbValue.Bind(ComboBox.ItemsProperty, subject);
            subject.OnNext(items);
            cbValue.SelectedIndex = selection;

            control.Children.Add(sharedRowPanel);

        }

        /// <summary>
        /// Saves the combo box selection and returns the selected Type.
        /// </summary>
        /// <param name="control">The panel containing the controls.</param>
        /// <param name="name">The name of the member.</param>
        /// <param name="type">The type of the member.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <param name="subGroupStack">Stack of subgroup types for nested editing.</param>
        /// <returns>The selected Type value.</returns>
        public override Type SaveWindowControls(StackPanel control, string name, Type type, object[] attributes, Type[] subGroupStack)
        {
            int controlIndex = 0;
            TypeConstraintAttribute dataAtt = ReflectionExt.FindAttribute<TypeConstraintAttribute>(attributes);
            Type baseType = dataAtt.BaseClass;

            Type[] children = (Type[])control.DataContext;

            Avalonia.Controls.Grid subGrid = (Avalonia.Controls.Grid)control.Children[controlIndex];
            ComboBox cbValue = (ComboBox)subGrid.Children[1];
            return children[cbValue.SelectedIndex];
        }
    }
}
