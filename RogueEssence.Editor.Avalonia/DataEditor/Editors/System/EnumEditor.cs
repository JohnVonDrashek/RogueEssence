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
    /// Editor for Enum values. Displays either a combo box for single selection or checkboxes for flags enums.
    /// Automatically detects FlagsAttribute to determine the appropriate UI.
    /// </summary>
    public class EnumEditor : Editor<Enum>
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
        /// Gets a value indicating whether the editor should display type information.
        /// </summary>
        public override bool DefaultType => true;

        /// <summary>
        /// Loads controls for editing an enum value. Uses checkboxes for flags enums, combo box for regular enums.
        /// </summary>
        /// <param name="control">The panel to add controls to.</param>
        /// <param name="parent">The parent object name.</param>
        /// <param name="parentType">The type of the parent object.</param>
        /// <param name="name">The name of the member being edited.</param>
        /// <param name="type">The type of the enum.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <param name="member">The enum value to edit.</param>
        /// <param name="subGroupStack">Stack of subgroup types for nested editing.</param>
        public override void LoadWindowControls(StackPanel control, string parent, Type parentType, string name, Type type, object[] attributes, Enum member, Type[] subGroupStack)
        {
            Array enums = type.GetEnumValues();
            if (type.GetCustomAttributes(typeof(FlagsAttribute), false).Length > 0)
            {
                List<CheckBox> checkboxes = new List<CheckBox>();
                for (int ii = 0; ii < enums.Length; ii++)
                {
                    int numeric = (int)enums.GetValue(ii);
                    int num1s = 0;
                    for (int jj = 0; jj < 32; jj++)
                    {
                        if ((numeric & 0x1) == 1)
                            num1s++;
                        numeric = numeric >> 1;
                    }
                    if (num1s == 1)
                    {
                        CheckBox chkValue = new CheckBox();
                        if (checkboxes.Count > 0)
                            chkValue.Margin = new Thickness(4, 0, 0, 0);
                        chkValue.Content = enums.GetValue(ii).ToString();
                        chkValue.IsChecked = member.HasFlag((Enum)enums.GetValue(ii));
                        checkboxes.Add(chkValue);
                    }
                }

                Avalonia.Controls.Grid innerPanel = getSharedRowPanel(checkboxes.Count);
                for (int ii = 0; ii < checkboxes.Count; ii++)
                {
                    innerPanel.Children.Add(checkboxes[ii]);
                    checkboxes[ii].SetValue(Avalonia.Controls.Grid.ColumnProperty, ii);
                }

                control.Children.Add(innerPanel);
            }
            else
            {
                //for enums, use a combobox
                ComboBox cbValue = new SearchComboBox();
                cbValue.VirtualizationMode = ItemVirtualizationMode.Simple;

                List<string> items = new List<string>();
                int selection = 0;
                for (int ii = 0; ii < enums.Length; ii++)
                {
                    items.Add(enums.GetValue(ii).ToString());
                    if (Enum.Equals(enums.GetValue(ii), member))
                        selection = ii;
                }

                var subject = new Subject<List<string>>();
                cbValue.Bind(ComboBox.ItemsProperty, subject);
                subject.OnNext(items);
                cbValue.SelectedIndex = selection;
                {
                    string typeDesc = DevDataManager.GetMemberDoc(type, enums.GetValue(cbValue.SelectedIndex).ToString());
                    ToolTip.SetTip(cbValue, typeDesc);
                }
                cbValue.SelectionChanged += (object sender, SelectionChangedEventArgs e) =>
                {
                    string typeDesc = DevDataManager.GetMemberDoc(type, enums.GetValue(cbValue.SelectedIndex).ToString());
                    ToolTip.SetTip(cbValue, typeDesc);
                };
                control.Children.Add(cbValue);
            }
        }

        /// <summary>
        /// Saves the enum controls and returns the selected enum value.
        /// Handles both flags enums (combining checkbox values) and regular enums (combo box selection).
        /// </summary>
        /// <param name="control">The panel containing the controls.</param>
        /// <param name="name">The name of the member.</param>
        /// <param name="type">The type of the enum.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <param name="subGroupStack">Stack of subgroup types for nested editing.</param>
        /// <returns>The selected enum value.</returns>
        public override Enum SaveWindowControls(StackPanel control, string name, Type type, object[] attributes, Type[] subGroupStack)
        {
            int controlIndex = 0;

            Array enums = type.GetEnumValues();
            if (type.GetCustomAttributes(typeof(FlagsAttribute), false).Length > 0)
            {
                Avalonia.Controls.Grid innerControl = (Avalonia.Controls.Grid)control.Children[controlIndex];
                int innerControlIndex = 0;

                int pending = 0;
                for (int ii = 0; ii < enums.Length; ii++)
                {
                    int numeric = (int)enums.GetValue(ii);
                    int num1s = 0;
                    for (int jj = 0; jj < 32; jj++)
                    {
                        if ((numeric & 0x1) == 1)
                            num1s++;
                        numeric = numeric >> 1;
                    }
                    if (num1s == 1)
                    {
                        CheckBox chkValue = (CheckBox)innerControl.Children[innerControlIndex];
                        pending |= ((chkValue.IsChecked.HasValue && chkValue.IsChecked.Value) ? 1 : 0) * (int)enums.GetValue(ii);
                        innerControlIndex++;
                    }
                }
                return (Enum)Enum.ToObject(type, pending);
            }
            else
            {
                ComboBox cbValue = (ComboBox)control.Children[controlIndex];
                return (Enum)enums.GetValue(cbValue.SelectedIndex);
            }
        }
    }
}
