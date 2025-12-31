using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using RogueEssence.Content;
using RogueEssence.Dungeon;
using RogueEssence.Data;
using System.Drawing;
using RogueElements;
using Avalonia.Controls;
using RogueEssence.Dev.Views;
using RogueEssence.Dev.ViewModels;

namespace RogueEssence.Dev
{
    /// <summary>
    /// Editor for HashSet values. Displays a collection box that prevents duplicate entries.
    /// Note: Due to lack of parameterless interface for HashSet, concrete type implementations are needed.
    /// </summary>
    /// <typeparam name="T">The type of elements in the hash set.</typeparam>
    public class HashSetEditor<T> : Editor<HashSet<T>>
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
        /// Loads a collection box control for editing hash set elements with duplicate prevention.
        /// </summary>
        /// <param name="control">The panel to add controls to.</param>
        /// <param name="parent">The parent object name.</param>
        /// <param name="parentType">The type of the parent object.</param>
        /// <param name="name">The name of the member being edited.</param>
        /// <param name="type">The type of the member.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <param name="member">The hash set to edit.</param>
        /// <param name="subGroupStack">Stack of subgroup types for nested editing.</param>
        public override void LoadWindowControls(StackPanel control, string parent, Type parentType, string name, Type type, object[] attributes, HashSet<T> member, Type[] subGroupStack)
        {
            Type elementType = typeof(T);

            CollectionBox lbxValue = new CollectionBox();

            EditorHeightAttribute heightAtt = ReflectionExt.FindAttribute<EditorHeightAttribute>(attributes);
            if (heightAtt != null)
                lbxValue.MaxHeight = heightAtt.Height;
            else
                lbxValue.MaxHeight = 200;

            CollectionBoxViewModel vm = new CollectionBoxViewModel(control.GetOwningForm(), new StringConv(elementType, ReflectionExt.GetPassableAttributes(1, attributes)));

            CollectionAttribute confirmAtt = ReflectionExt.FindAttribute<CollectionAttribute>(attributes);
            if (confirmAtt != null)
                vm.ConfirmDelete = confirmAtt.ConfirmDelete;

            lbxValue.DataContext = vm;
            lbxValue.MinHeight = lbxValue.MaxHeight;//TODO: Uptake Avalonia fix for improperly updating Grid control dimensions

            //add lambda expression for editing a single element
            vm.OnEditItem += (int index, object element, bool advancedEdit, CollectionBoxViewModel.EditElementOp op) =>
            {
                string elementName = name + "[" + index + "]";
                DataEditForm frmData = new DataEditForm();
                frmData.Title = DataEditor.GetWindowTitle(parent, elementName, element, elementType, ReflectionExt.GetPassableAttributes(1, attributes));

                //TODO: make this a member and reference it that way
                DataEditor.LoadClassControls(frmData.ControlPanel, parent, parentType, elementName, elementType, ReflectionExt.GetPassableAttributes(1, attributes), element, true, new Type[0], advancedEdit);
                DataEditor.TrackTypeSize(frmData, elementType);

                frmData.SelectedOKEvent += async () =>
                {
                    object newElement = DataEditor.SaveClassControls(frmData.ControlPanel, elementName, elementType, ReflectionExt.GetPassableAttributes(1, attributes), true, new Type[0], advancedEdit);

                    bool itemExists = false;

                    List<object> states = (List<object>)vm.GetList(typeof(List<object>));
                    for (int ii = 0; ii < states.Count; ii++)
                    {
                        //ignore the current index being edited
                        //if the element is null, then we are editing a new object, so skip
                        if (ii != index || element == null)
                        {
                            if (states[ii].Equals(newElement))
                                itemExists = true;
                        }
                    }

                    if (itemExists)
                    {
                        await MessageBox.Show(control.GetOwningForm(), "Cannot add duplicate items.", "Entry already exists.", MessageBox.MessageBoxButtons.Ok);
                        return false;
                    }
                    else
                    {
                        op(index, newElement);
                        return true;
                    }
                };

                control.GetOwningForm().RegisterChild(frmData);
                frmData.Show();
            };

            List<object> states = new List<object>();
            foreach (object state in member)
                states.Add(state);
            vm.LoadFromList(states);
            control.Children.Add(lbxValue);
        }

        /// <summary>
        /// Saves the collection box controls and returns the resulting hash set.
        /// </summary>
        /// <param name="control">The panel containing the controls.</param>
        /// <param name="name">The name of the member.</param>
        /// <param name="type">The type of the member.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <param name="subGroupStack">Stack of subgroup types for nested editing.</param>
        /// <returns>A new hash set containing the edited elements.</returns>
        public override HashSet<T> SaveWindowControls(StackPanel control, string name, Type type, object[] attributes, Type[] subGroupStack)
        {
            int controlIndex = 0;

            IControl lbxValue = control.Children[controlIndex];
            HashSet<T> member = (HashSet<T>)Activator.CreateInstance(type);
            CollectionBoxViewModel mv = (CollectionBoxViewModel)lbxValue.DataContext;
            List<object> states = (List<object>)mv.GetList(typeof(List<object>));
            for (int ii = 0; ii < states.Count; ii++)
                member.Add((T)states[ii]);
            return member;


        }
    }
}
