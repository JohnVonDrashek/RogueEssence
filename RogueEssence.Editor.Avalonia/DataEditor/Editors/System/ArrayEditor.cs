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
using RogueEssence.Dev.ViewModels;

namespace RogueEssence.Dev
{
    /// <summary>
    /// Editor for Array values. Displays a collection box for editing array elements.
    /// Supports RankedListAttribute for indexed display and EditorHeightAttribute for custom height.
    /// </summary>
    public class ArrayEditor : Editor<Array>
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
        /// Loads collection box controls for editing an array.
        /// </summary>
        /// <param name="control">The panel to add controls to.</param>
        /// <param name="parent">The parent object name.</param>
        /// <param name="parentType">The type of the parent object.</param>
        /// <param name="name">The name of the member being edited.</param>
        /// <param name="type">The type of the member.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <param name="member">The array to edit.</param>
        /// <param name="subGroupStack">Stack of subgroup types for nested editing.</param>
        public override void LoadWindowControls(StackPanel control, string parent, Type parentType, string name, Type type, object[] attributes, Array member, Type[] subGroupStack)
        {
            RankedListAttribute rangeAtt = ReflectionExt.FindAttribute<RankedListAttribute>(attributes);

            if (rangeAtt != null)
            {
                RankedCollectionBox lbxValue = new RankedCollectionBox();

                EditorHeightAttribute heightAtt = ReflectionExt.FindAttribute<EditorHeightAttribute>(attributes);
                if (heightAtt != null)
                    lbxValue.MaxHeight = heightAtt.Height;
                else
                    lbxValue.MaxHeight = 180;

                CollectionBoxViewModel vm = createViewModel(control, parent, name, type, attributes, member, rangeAtt.Index1);
                lbxValue.DataContext = vm;
                lbxValue.SetListContextMenu(ListEditor.CreateContextMenu(control, type, vm));
                lbxValue.MinHeight = lbxValue.MaxHeight;//TODO: Uptake Avalonia fix for improperly updating Grid control dimensions
                control.Children.Add(lbxValue);
            }
            else
            {
                CollectionBox lbxValue = new CollectionBox();

                EditorHeightAttribute heightAtt = ReflectionExt.FindAttribute<EditorHeightAttribute>(attributes);
                if (heightAtt != null)
                    lbxValue.MaxHeight = heightAtt.Height;
                else
                    lbxValue.MaxHeight = 180;

                CollectionBoxViewModel vm = createViewModel(control, parent, name, type, attributes, member, false);
                lbxValue.DataContext = vm;
                lbxValue.SetListContextMenu(ListEditor.CreateContextMenu(control, type, vm));
                lbxValue.MinHeight = lbxValue.MaxHeight;//TODO: Uptake Avalonia fix for improperly updating Grid control dimensions
                control.Children.Add(lbxValue);
            }

        }

        /// <summary>
        /// Creates a view model for the collection box with element editing capabilities.
        /// </summary>
        /// <param name="control">The parent stack panel control.</param>
        /// <param name="parent">The parent object name.</param>
        /// <param name="name">The name of the array member.</param>
        /// <param name="type">The type of the array.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <param name="member">The array to create a view model for.</param>
        /// <param name="index1">Whether to use 1-based indexing.</param>
        /// <returns>A configured CollectionBoxViewModel.</returns>
        private CollectionBoxViewModel createViewModel(StackPanel control, string parent, string name, Type type, object[] attributes, Array member, bool index1)
        {
            Type elementType = type.GetElementType();

            CollectionBoxViewModel vm = new CollectionBoxViewModel(control.GetOwningForm(), new StringConv(elementType, ReflectionExt.GetPassableAttributes(1, attributes)));
            vm.Index1 = index1;
            CollectionAttribute confirmAtt = ReflectionExt.FindAttribute<CollectionAttribute>(attributes);
            if (confirmAtt != null)
                vm.ConfirmDelete = confirmAtt.ConfirmDelete;

            //add lambda expression for editing a single element
            vm.OnEditItem += (int index, object element, bool advancedEdit, CollectionBoxViewModel.EditElementOp op) =>
            {
                string elementName = name + "[" + index + "]";
                DataEditForm frmData = new DataEditForm();
                frmData.Title = DataEditor.GetWindowTitle(parent, elementName, element, elementType, ReflectionExt.GetPassableAttributes(0, attributes));

                DataEditor.LoadClassControls(frmData.ControlPanel, parent, null, elementName, elementType, ReflectionExt.GetPassableAttributes(0, attributes), element, true, new Type[0], advancedEdit);
                DataEditor.TrackTypeSize(frmData, elementType);

                frmData.SelectedOKEvent += async () =>
                {
                    element = DataEditor.SaveClassControls(frmData.ControlPanel, elementName, elementType, ReflectionExt.GetPassableAttributes(0, attributes), true, new Type[0], advancedEdit);
                    op(index, element);
                    return true;
                };

                control.GetOwningForm().RegisterChild(frmData);
                frmData.Show();
            };


            List<object> objList = new List<object>();
            for (int ii = 0; ii < member.Length; ii++)
                objList.Add(member.GetValue(ii));

            vm.LoadFromList(objList);
            return vm;
        }

        /// <summary>
        /// Saves the collection box controls and returns the resulting array.
        /// </summary>
        /// <param name="control">The panel containing the controls.</param>
        /// <param name="name">The name of the member.</param>
        /// <param name="type">The type of the member.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <param name="subGroupStack">Stack of subgroup types for nested editing.</param>
        /// <returns>A new array containing the edited elements.</returns>
        public override Array SaveWindowControls(StackPanel control, string name, Type type, object[] attributes, Type[] subGroupStack)
        {
            int controlIndex = 0;
            //TODO: 2D array grid support
            //if (type.GetElementType().IsArray)

            IControl lbxValue = control.Children[controlIndex];
            CollectionBoxViewModel mv = (CollectionBoxViewModel)lbxValue.DataContext;
            List<object> objList = (List<object>)mv.GetList(typeof(List<object>));

            Array array = Array.CreateInstance(type.GetElementType(), objList.Count);
            for (int ii = 0; ii < objList.Count; ii++)
                array.SetValue(objList[ii], ii);

            return array;
        }
    }
}
