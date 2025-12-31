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
using RogueEssence.Dev.ViewModels;
using Avalonia.Interactivity;

namespace RogueEssence.Dev
{
    /// <summary>
    /// Editor for IDictionary values. Displays a dictionary box for editing key-value pairs.
    /// Supports editing both keys and values with separate edit dialogs.
    /// </summary>
    public class DictionaryEditor : Editor<IDictionary>
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
        /// Loads dictionary box controls for editing key-value pairs.
        /// </summary>
        /// <param name="control">The panel to add controls to.</param>
        /// <param name="parent">The parent object name.</param>
        /// <param name="parentType">The type of the parent object.</param>
        /// <param name="name">The name of the member being edited.</param>
        /// <param name="type">The type of the member.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <param name="member">The dictionary to edit.</param>
        /// <param name="subGroupStack">Stack of subgroup types for nested editing.</param>
        public override void LoadWindowControls(StackPanel control, string parent, Type parentType, string name, Type type, object[] attributes, IDictionary member, Type[] subGroupStack)
        {
            Type keyType = ReflectionExt.GetBaseTypeArg(typeof(IDictionary<,>), type, 0);
            Type elementType = ReflectionExt.GetBaseTypeArg(typeof(IDictionary<,>), type, 1);

            DictionaryBox lbxValue = new DictionaryBox();
            
            EditorHeightAttribute heightAtt = ReflectionExt.FindAttribute<EditorHeightAttribute>(attributes);
            if (heightAtt != null)
                lbxValue.MaxHeight = heightAtt.Height;
            else
                lbxValue.MaxHeight = 200;

            DictionaryBoxViewModel vm = new DictionaryBoxViewModel(control.GetOwningForm(), new StringConv(elementType, ReflectionExt.GetPassableAttributes(2, attributes)));

            CollectionAttribute confirmAtt = ReflectionExt.FindAttribute<CollectionAttribute>(attributes);
            if (confirmAtt != null)
                vm.ConfirmDelete = confirmAtt.ConfirmDelete;

            lbxValue.DataContext = vm;
            lbxValue.MinHeight = lbxValue.MaxHeight;//TODO: Uptake Avalonia fix for improperly updating Grid control dimensions

            //add lambda expression for editing a single element
            vm.OnEditItem += (object key, object element, bool advancedEdit, DictionaryBoxViewModel.EditElementOp op) =>
            {
                string elementName = name + "[" + key.ToString() + "]";
                DataEditForm frmData = new DataEditForm();
                frmData.Title = DataEditor.GetWindowTitle(parent, elementName, element, elementType, ReflectionExt.GetPassableAttributes(2, attributes));

                DataEditor.LoadClassControls(frmData.ControlPanel, parent, null, elementName, elementType, ReflectionExt.GetPassableAttributes(2, attributes), element, true, new Type[0], advancedEdit);
                DataEditor.TrackTypeSize(frmData, elementType);

                frmData.SelectedOKEvent += async () =>
                {
                    element = DataEditor.SaveClassControls(frmData.ControlPanel, elementName, elementType, ReflectionExt.GetPassableAttributes(2, attributes), true, new Type[0], advancedEdit);
                    op(key, key, element);
                    return true;
                };

                control.GetOwningForm().RegisterChild(frmData);
                frmData.Show();
            };

            vm.OnEditKey += (object key, object element, bool advancedEdit, DictionaryBoxViewModel.EditElementOp op) =>
            {
                string elementName = name + "<Key>";
                DataEditForm frmData = new DataEditForm();
                frmData.Title = DataEditor.GetWindowTitle(parent, elementName, key, keyType, ReflectionExt.GetPassableAttributes(1, attributes));

                DataEditor.LoadClassControls(frmData.ControlPanel, parent, null, elementName, keyType, ReflectionExt.GetPassableAttributes(1, attributes), key, true, new Type[0], advancedEdit);
                DataEditor.TrackTypeSize(frmData, keyType);

                frmData.SelectedOKEvent += async () =>
                {
                    object newKey = DataEditor.SaveClassControls(frmData.ControlPanel, elementName, keyType, ReflectionExt.GetPassableAttributes(1, attributes), true, new Type[0], advancedEdit);
                    op(key, newKey, element);
                    return true;
                };

                control.GetOwningForm().RegisterChild(frmData);
                frmData.Show();
            };

            vm.LoadFromDict(member);
            lbxValue.SetListContextMenu(CreateContextMenu(control, type, vm));
            control.Children.Add(lbxValue);
        }

        /// <summary>
        /// Creates a context menu for dictionary operations including key renaming.
        /// </summary>
        /// <param name="control">The parent stack panel control.</param>
        /// <param name="type">The dictionary type.</param>
        /// <param name="vm">The dictionary box view model.</param>
        /// <returns>A context menu with dictionary-specific operations.</returns>
        public static ContextMenu CreateContextMenu(StackPanel control, Type type, DictionaryBoxViewModel vm)
        {
            Type keyType = ReflectionExt.GetBaseTypeArg(typeof(IDictionary<,>), type, 0);
            Type elementType = ReflectionExt.GetBaseTypeArg(typeof(IDictionary<,>), type, 1);

            ContextMenu copyPasteStrip = new ContextMenu();

            MenuItem renameToolStripMenuItem = new MenuItem();

            Avalonia.Collections.AvaloniaList<object> list = (Avalonia.Collections.AvaloniaList<object>)copyPasteStrip.Items;
            list.AddRange(new MenuItem[] {
                            renameToolStripMenuItem});

            renameToolStripMenuItem.Header = "Rename " + elementType.Name;

            renameToolStripMenuItem.Click += async (object copySender, RoutedEventArgs copyE) =>
            {
                if (vm.SelectedIndex > -1)
                    vm.EditKey(vm.SelectedIndex);
                else
                    await MessageBox.Show(control.GetOwningForm(), String.Format("No index selected!"), "Invalid Operation", MessageBox.MessageBoxButtons.Ok);
            };
            return copyPasteStrip;
        }

        /// <summary>
        /// Saves the dictionary box controls and returns the resulting dictionary.
        /// </summary>
        /// <param name="control">The panel containing the controls.</param>
        /// <param name="name">The name of the member.</param>
        /// <param name="type">The type of the member.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <param name="subGroupStack">Stack of subgroup types for nested editing.</param>
        /// <returns>The edited dictionary.</returns>
        public override IDictionary SaveWindowControls(StackPanel control, string name, Type type, object[] attributes, Type[] subGroupStack)
        {
            int controlIndex = 0;

            DictionaryBox lbxValue = (DictionaryBox)control.Children[controlIndex];
            DictionaryBoxViewModel mv = (DictionaryBoxViewModel)lbxValue.DataContext;
            return mv.GetDict(type);
        }
    }
}
