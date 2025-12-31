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
using Avalonia;

namespace RogueEssence.Dev
{
    /// <summary>
    /// Editor for IPriorityList values. Displays a priority list box for editing elements with priority ordering.
    /// Supports editing both element values and their priorities with copy/paste functionality.
    /// </summary>
    public class PriorityListEditor : Editor<IPriorityList>
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
        /// Gets a value indicating whether the editor should display a label. False because it handles its own labeling.
        /// </summary>
        public override bool DefaultLabel => false;

        /// <summary>
        /// Loads a priority list box control for editing a priority-ordered collection.
        /// </summary>
        /// <param name="control">The panel to add controls to.</param>
        /// <param name="parent">The parent object name.</param>
        /// <param name="parentType">The type of the parent object.</param>
        /// <param name="name">The name of the member being edited.</param>
        /// <param name="type">The type of the member.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <param name="member">The priority list to edit.</param>
        /// <param name="subGroupStack">Stack of subgroup types for nested editing.</param>
        public override void LoadWindowControls(StackPanel control, string parent, Type parentType, string name, Type type, object[] attributes, IPriorityList member, Type[] subGroupStack)
        {
            Type elementType = ReflectionExt.GetBaseTypeArg(typeof(IPriorityList<>), type, 0);

            PriorityListBox lbxValue = new PriorityListBox();

            EditorHeightAttribute heightAtt = ReflectionExt.FindAttribute<EditorHeightAttribute>(attributes);
            if (heightAtt != null)
                lbxValue.MaxHeight = heightAtt.Height;
            else
                lbxValue.MaxHeight = 220;

            PriorityListBoxViewModel vm = new PriorityListBoxViewModel(control.GetOwningForm(), new StringConv(elementType, ReflectionExt.GetPassableAttributes(2, attributes)));
            lbxValue.DataContext = vm;
            CollectionAttribute confirmAtt = ReflectionExt.FindAttribute<CollectionAttribute>(attributes);
            if (confirmAtt != null)
                vm.ConfirmDelete = confirmAtt.ConfirmDelete;
            else
                vm.ConfirmDelete = true;

            lbxValue.SetListContextMenu(createContextMenu(control, type, vm));
            lbxValue.MinHeight = lbxValue.MaxHeight;//TODO: Uptake Avalonia fix for improperly updating Grid control dimensions

            //add lambda expression for editing a single element
            vm.OnEditItem = (Priority priority, int index, object element, bool advancedEdit, PriorityListBoxViewModel.EditElementOp op) =>
            {
                string elementName = name + "[" + priority.ToString() + "]";
                DataEditForm frmData = new DataEditForm();
                frmData.Title = DataEditor.GetWindowTitle(parent, elementName, element, elementType, ReflectionExt.GetPassableAttributes(2, attributes));

                DataEditor.LoadClassControls(frmData.ControlPanel, parent, null, elementName, elementType, ReflectionExt.GetPassableAttributes(2, attributes), element, true, new Type[0], advancedEdit);
                DataEditor.TrackTypeSize(frmData, elementType);

                frmData.SelectedOKEvent += async () =>
                {
                    element = DataEditor.SaveClassControls(frmData.ControlPanel, elementName, elementType, ReflectionExt.GetPassableAttributes(2, attributes), true, new Type[0], advancedEdit);
                    op(priority, index, element);
                    return true;
                };

                control.GetOwningForm().RegisterChild(frmData);
                frmData.Show();
            };
            vm.OnEditPriority = (Priority priority, int index, bool advancedEdit, PriorityListBoxViewModel.EditPriorityOp op) =>
            {
                string elementName = name + "<Priority>";
                DataEditForm frmData = new DataEditForm();
                frmData.Title = DataEditor.GetWindowTitle(parent, elementName, priority, typeof(Priority), ReflectionExt.GetPassableAttributes(1, attributes));

                DataEditor.LoadClassControls(frmData.ControlPanel, parent, null, elementName, typeof(Priority), ReflectionExt.GetPassableAttributes(1, attributes), priority, true, new Type[0], advancedEdit);
                DataEditor.TrackTypeSize(frmData, typeof(Priority));

                frmData.SelectedOKEvent += async () =>
                {
                    object priorityObj = DataEditor.SaveClassControls(frmData.ControlPanel, elementName, typeof(Priority), ReflectionExt.GetPassableAttributes(1, attributes), true, new Type[0], advancedEdit);
                    op(priority, index, (Priority)priorityObj);
                    return true;
                };

                control.GetOwningForm().RegisterChild(frmData);
                frmData.Show();
            };

            vm.LoadFromList(member);


            ListCollapseAttribute collapseAtt = ReflectionExt.FindAttribute<ListCollapseAttribute>(attributes);

            string desc = DevDataManager.GetMemberDoc(parentType, name);
            if (collapseAtt != null)
            {

                Expander expander = new Expander();
                expander.Header = Text.GetMemberTitle(name) + ":";
                if (desc != null)
                    ToolTip.SetTip(expander, desc);
                expander.IsExpanded = member.Count > 0;
                expander.Content = lbxValue;
                control.Children.Add(expander);
            }
            else
            {
                LoadLabelControl(control, name, desc);
                control.Children.Add(lbxValue);
            }
        }


        private static ContextMenu createContextMenu(StackPanel control, Type type, PriorityListBoxViewModel vm)
        {
            Type elementType = ReflectionExt.GetBaseTypeArg(typeof(IPriorityList<>), type, 0);

            ContextMenu copyPasteStrip = new ContextMenu();

            MenuItem copyToolStripMenuItem = new MenuItem();
            MenuItem pasteToolStripMenuItem = new MenuItem();

            Avalonia.Collections.AvaloniaList<object> list = (Avalonia.Collections.AvaloniaList<object>)copyPasteStrip.Items;
            list.AddRange(new MenuItem[] {
                            copyToolStripMenuItem,
                            pasteToolStripMenuItem});

            copyToolStripMenuItem.Header = "Copy List Element: " + elementType.Name;
            pasteToolStripMenuItem.Header = "Insert List Element: " + elementType.Name;

            copyToolStripMenuItem.Click += async (object copySender, RoutedEventArgs copyE) =>
            {
                if (vm.SelectedIndex > -1)
                {
                    object obj = vm.Collection[vm.SelectedIndex].Value;
                    DataEditor.SetClipboardObj(obj, null);
                }
                else
                    await MessageBox.Show(control.GetOwningForm(), String.Format("No index selected!"), "Invalid Operation", MessageBox.MessageBoxButtons.Ok);
            };
            pasteToolStripMenuItem.Click += async (object copySender, RoutedEventArgs copyE) =>
            {
                Type type1 = DataEditor.clipboardObj.GetType();
                Type type2 = elementType;
                if (type2.IsAssignableFrom(type1))
                {
                    int idx = vm.SelectedIndex;
                    if (idx < 0)
                        idx = vm.Collection.Count;
                    vm.InsertOnKey(idx, DataEditor.clipboardObj);
                }
                else
                    await MessageBox.Show(control.GetOwningForm(), String.Format("Incompatible types:\n{0}\n{1}", type1.AssemblyQualifiedName, type2.AssemblyQualifiedName), "Invalid Operation", MessageBox.MessageBoxButtons.Ok);
            };
            return copyPasteStrip;
        }


        public override IPriorityList SaveWindowControls(StackPanel control, string name, Type type, object[] attributes, Type[] subGroupStack)
        {
            int controlIndex = 0;

            ListCollapseAttribute collapseAtt = ReflectionExt.FindAttribute<ListCollapseAttribute>(attributes);

            PriorityListBox lbxValue;
            if (collapseAtt != null)
                lbxValue = (PriorityListBox)((Expander)control.Children[controlIndex]).Content;
            else
            {
                controlIndex++;
                lbxValue = (PriorityListBox)control.Children[controlIndex];
            }

            PriorityListBoxViewModel mv = (PriorityListBoxViewModel)lbxValue.DataContext;
            return mv.GetList(type);
        }
    }
}
