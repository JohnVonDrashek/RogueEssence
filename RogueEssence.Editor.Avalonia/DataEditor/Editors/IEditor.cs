using Avalonia.Controls;
using RogueEssence.Dev.Views;
using System;
using System.Collections.Generic;
using System.Text;

namespace RogueEssence.Dev
{
    /// <summary>
    /// Interface for data editors that can load and save UI controls for editing objects.
    /// </summary>
    public interface IEditor
    {
        /// <summary>
        /// Gets a value indicating whether this is a simple editor.
        /// </summary>
        bool SimpleEditor { get; }

        /// <summary>
        /// Gets the attribute type that this editor handles.
        /// </summary>
        /// <returns>The attribute type, or null if none.</returns>
        Type GetAttributeType();

        /// <summary>
        /// Gets the type this editor can convert/edit.
        /// </summary>
        /// <returns>The type that this editor handles.</returns>
        Type GetConvertingType();

        /// <summary>
        /// Loads editor controls for class-based editing.
        /// </summary>
        /// <param name="control">The panel to add controls to.</param>
        /// <param name="parent">The parent object name.</param>
        /// <param name="parentType">The type of the parent object.</param>
        /// <param name="name">The name of the member being edited.</param>
        /// <param name="type">The type of the member.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <param name="member">The member value.</param>
        /// <param name="isWindow">Whether this is being loaded in a window.</param>
        /// <param name="subGroupStack">Stack of subgroup types for nested editing.</param>
        /// <param name="advancedEdit">Whether advanced edit mode is enabled.</param>
        void LoadClassControls(StackPanel control, string parent, Type parentType, string name, Type type, object[] attributes, object member, bool isWindow, Type[] subGroupStack, bool advancedEdit);

        /// <summary>
        /// Loads editor controls for window-based editing.
        /// </summary>
        /// <param name="control">The panel to add controls to.</param>
        /// <param name="parent">The parent object name.</param>
        /// <param name="parentType">The type of the parent object.</param>
        /// <param name="name">The name of the member being edited.</param>
        /// <param name="type">The type of the member.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <param name="member">The member value.</param>
        /// <param name="subGroupStack">Stack of subgroup types for nested editing.</param>
        void LoadWindowControls(StackPanel control, string parent, Type parentType, string name, Type type, object[] attributes, object member, Type[] subGroupStack);

        /// <summary>
        /// Loads a control for a specific member of an object.
        /// </summary>
        /// <param name="parent">The parent object name.</param>
        /// <param name="obj">The parent object.</param>
        /// <param name="control">The panel to add controls to.</param>
        /// <param name="name">The name of the member.</param>
        /// <param name="type">The type of the member.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <param name="member">The member value.</param>
        /// <param name="isWindow">Whether this is being loaded in a window.</param>
        /// <param name="subGroupStack">Stack of subgroup types for nested editing.</param>
        void LoadMemberControl(string parent, object obj, StackPanel control, string name, Type type, object[] attributes, object member, bool isWindow, Type[] subGroupStack);

        /// <summary>
        /// Saves class controls and returns the resulting object.
        /// </summary>
        /// <param name="control">The panel containing the controls.</param>
        /// <param name="name">The name of the member.</param>
        /// <param name="type">The type of the member.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <param name="isWindow">Whether this is in a window.</param>
        /// <param name="subGroupStack">Stack of subgroup types for nested editing.</param>
        /// <param name="advancedEdit">Whether advanced edit mode is enabled.</param>
        /// <returns>The saved object.</returns>
        object SaveClassControls(StackPanel control, string name, Type type, object[] attributes, bool isWindow, Type[] subGroupStack, bool advancedEdit);

        /// <summary>
        /// Saves window controls and returns the resulting object.
        /// </summary>
        /// <param name="control">The panel containing the controls.</param>
        /// <param name="name">The name of the member.</param>
        /// <param name="type">The type of the member.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <param name="subGroupStack">Stack of subgroup types for nested editing.</param>
        /// <returns>The saved object.</returns>
        object SaveWindowControls(StackPanel control, string name, Type type, object[] attributes, Type[] subGroupStack);

        /// <summary>
        /// Saves a member control and returns the resulting value.
        /// </summary>
        /// <param name="obj">The parent object.</param>
        /// <param name="control">The panel containing the controls.</param>
        /// <param name="name">The name of the member.</param>
        /// <param name="type">The type of the member.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <param name="isWindow">Whether this is in a window.</param>
        /// <param name="subGroupStack">Stack of subgroup types for nested editing.</param>
        /// <returns>The saved member value.</returns>
        object SaveMemberControl(object obj, StackPanel control, string name, Type type, object[] attributes, bool isWindow, Type[] subGroupStack);

        /// <summary>
        /// Gets a string representation of the object for display purposes.
        /// </summary>
        /// <param name="obj">The object to convert to string.</param>
        /// <param name="type">The type of the object.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <returns>A string representation of the object.</returns>
        string GetString(object obj, Type type, object[] attributes);

        /// <summary>
        /// Gets a string representation of this editor's type.
        /// </summary>
        /// <returns>The type string.</returns>
        string GetTypeString();
    }
}
