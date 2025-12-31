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
    /// Editor for strings with DataFolderAttribute that provides data folder selection functionality.
    /// Displays a combo box for selecting data entries from a specific folder.
    /// </summary>
    public class DataFolderEditor : StringEditor
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
        /// Gets the attribute type that this editor handles.
        /// </summary>
        /// <returns>The DataFolderAttribute type.</returns>
        public override Type GetAttributeType() { return typeof(DataFolderAttribute); }

        /// <summary>
        /// Loads window controls for editing data folder selection.
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
            DataFolderAttribute animAtt = ReflectionExt.FindAttribute<DataFolderAttribute>(attributes);
            ComboBox cbValue = new SearchComboBox();
            cbValue.VirtualizationMode = ItemVirtualizationMode.Simple;
            string choice = member;

            List<string> items = new List<string>();
            int chosenIndex = 0;

            string[] dirs = PathMod.GetModFiles(DataManager.DATA_PATH + animAtt.FolderPath);

            for (int ii = 0; ii < dirs.Length; ii++)
            {
                string filename = Path.GetFileNameWithoutExtension(dirs[ii]);
                if (filename == choice)
                    chosenIndex = items.Count;
                items.Add(filename);
            }

            var subject = new Subject<List<string>>();
            cbValue.Bind(ComboBox.ItemsProperty, subject);
            subject.OnNext(items);
            cbValue.SelectedIndex = chosenIndex;
            control.Children.Add(cbValue);
        }


        /// <summary>
        /// Saves window controls and returns the selected folder name.
        /// </summary>
        /// <param name="control">The panel containing the controls.</param>
        /// <param name="name">The name of the member.</param>
        /// <param name="type">The type of the member.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <param name="subGroupStack">Stack of subgroup types for nested editing.</param>
        /// <returns>The selected folder name as a string.</returns>
        public override String SaveWindowControls(StackPanel control, string name, Type type, object[] attributes, Type[] subGroupStack)
        {
            int controlIndex = 0;
            ComboBox cbValue = (ComboBox)control.Children[controlIndex];
            return (string)cbValue.SelectedItem;
        }
    }
}
