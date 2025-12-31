using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RogueEssence.Content;
using RogueEssence.Dungeon;
using RogueElements;
using System.IO;
using Avalonia.Controls;
using System.Collections.ObjectModel;
using System.Reactive.Subjects;
using RogueEssence.Dev.Views;
using Microsoft.Xna.Framework;
using Avalonia.Interactivity;
using Avalonia;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using Avalonia.Data.Converters;
using System.Text;
using Newtonsoft.Json;

namespace RogueEssence.Dev
{
    /// <summary>
    /// Central static class for managing data editors and their operations.
    /// Provides functionality for loading and saving controls, finding appropriate editors, and clipboard operations.
    /// </summary>
    public static class DataEditor
    {
        /// <summary>
        /// List of registered editors.
        /// </summary>
        private static List<IEditor> editors;

        /// <summary>
        /// Object stored in the clipboard for copy/paste operations.
        /// </summary>
        public static object clipboardObj;

        /// <summary>
        /// Dictionary mapping types to their friendly display names.
        /// </summary>
        private static Dictionary<Type, string> friendlyTypeNames;

        /// <summary>
        /// Initializes the data editor system.
        /// </summary>
        public static void Init()
        {
            clipboardObj = new object();
            editors = new List<IEditor>();
            friendlyTypeNames = new Dictionary<Type, string>();
        }

        /// <summary>
        /// Adds an editor to the registry, maintaining inheritance order.
        /// </summary>
        /// <param name="editor">The editor to add.</param>
        public static void AddEditor(IEditor editor)
        {
            //maintain inheritance order
            Type convertingType = editor.GetConvertingType();
            for (int ii = 0; ii < editors.Count; ii++)
            {
                if (convertingType.IsSubclassOf(editors[ii].GetConvertingType()))
                {
                    editors.Insert(ii, editor);
                    return;
                }
                else if (convertingType == editors[ii].GetConvertingType() && convertingType != null && editors[ii].GetAttributeType() == null)
                {
                    editors.Insert(ii, editor);
                    return;
                }
            }
            editors.Add(editor);

            string friendlyName = editor.GetTypeString();
            if (friendlyName != null && !friendlyTypeNames.ContainsKey(convertingType))
                friendlyTypeNames[convertingType] = friendlyName;
        }

        /// <summary>
        /// Loads data controls for an object into an editor form.
        /// </summary>
        /// <param name="assetName">The name of the asset being edited.</param>
        /// <param name="obj">The object to edit.</param>
        /// <param name="editor">The editor form to populate.</param>
        public static void LoadDataControls(string assetName, object obj, DataEditForm editor)
        {
            Type editType = obj.GetType();
            LoadClassControls(editor.ControlPanel, assetName, null, obj.ToString(), editType, new object[0], obj, true, new Type[0], false);
            TrackTypeSize(editor, editType);
        }

        /// <summary>
        /// Sets the editor window based on saved values (if there are any), and sets the event to save editor dimensions when changed.
        /// </summary>
        /// <param name="editor"></param>
        /// <param name="editType"></param>
        public static void TrackTypeSize(DataEditForm editor, Type editType)
        {
            Size savedSize;
            if (DevDataManager.GetTypeSize(editType, out savedSize))
            {
                //TODO: avalonia pls, why you increase the width of the window immediately after opening??
                editor.Width = savedSize.Width - 10;
                editor.Height = savedSize.Height;
            }

            void editorWindow_SizeChanged(Size size)
            {
                DevDataManager.SetTypeSize(editType, size);
            }

            editor.GetObservable(TopLevel.ClientSizeProperty).Subscribe(editorWindow_SizeChanged);
        }

        /// <summary>
        /// Finds an appropriate editor for the given type and attributes.
        /// </summary>
        /// <param name="objType">The type of object to edit.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <param name="noSimple">Whether to exclude simple editors.</param>
        /// <returns>The appropriate editor for the type.</returns>
        private static IEditor findEditor(Type objType, object[] attributes, bool noSimple)
        {
            foreach (IEditor editor in editors)
            {
                if (noSimple && editor.SimpleEditor)
                    continue;

                Type editType = editor.GetConvertingType();
                if (editType.IsAssignableFrom(objType))
                {
                    Type attrType = editor.GetAttributeType();
                    if (attrType == null)
                        return editor;
                    else
                    {
                        foreach (object attr in attributes)
                        {
                            if (attr.GetType() == attrType)
                                return editor;
                        }
                    }
                }
            }
            throw new ArgumentException("Unhandled type!");
        }

        /// <summary>
        /// Loads editor controls for a class member into a control panel.
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
        public static void LoadClassControls(StackPanel control, string parent, Type parentType, string name, Type type, object[] attributes, object member, bool isWindow, Type[] subGroupStack, bool advancedEdit)
        {
            IEditor converter = findEditor(type, attributes, advancedEdit);
            converter.LoadClassControls(control, parent, parentType, name, type, attributes, member, isWindow, subGroupStack, advancedEdit);
        }

        /// <summary>
        /// Loads editor controls for window-based editing.
        /// </summary>
        /// <param name="control">The panel to add controls to.</param>
        /// <param name="parent">The parent object name.</param>
        /// <param name="parentType">The type of the parent object.</param>
        /// <param name="name">The name of the member being edited.</param>
        /// <param name="type">The type of the member.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <param name="obj">The object value.</param>
        /// <param name="subGroupStack">Stack of subgroup types for nested editing.</param>
        /// <param name="advancedEdit">Whether advanced edit mode is enabled.</param>
        public static void LoadWindowControls(StackPanel control, string parent, Type parentType, string name, Type type, object[] attributes, object obj, Type[] subGroupStack, bool advancedEdit)
        {
            IEditor converter = findEditor(type, attributes, advancedEdit);
            converter.LoadWindowControls(control, parent, parentType, name, type, attributes, obj, subGroupStack);
        }

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
        public static void LoadMemberControl(string parent, object obj, StackPanel control, string name, Type type, object[] attributes, object member, bool isWindow, Type[] subGroupStack)
        {
            IEditor converter = findEditor(obj.GetType(), attributes, false);
            converter.LoadMemberControl(parent, obj, control, name, type, attributes, member, isWindow, subGroupStack);
        }

        /// <summary>
        /// Saves data from the editor controls back to the object.
        /// </summary>
        /// <param name="obj">The object to save to (passed by reference).</param>
        /// <param name="control">The panel containing the controls.</param>
        /// <param name="subGroupStack">Stack of subgroup types for nested editing.</param>
        public static void SaveDataControls(ref object obj, StackPanel control, Type[] subGroupStack)
        {
            obj = SaveClassControls(control, obj.ToString(), obj.GetType(), new object[0], true, subGroupStack, false);
        }

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
        public static object SaveClassControls(StackPanel control, string name, Type type, object[] attributes, bool isWindow, Type[] subGroupStack, bool advancedEdit)
        {
            IEditor converter = findEditor(type, attributes, advancedEdit);
            return converter.SaveClassControls(control, name, type, attributes, isWindow, subGroupStack, advancedEdit);
        }


        /// <summary>
        /// Saves window controls and returns the resulting object.
        /// </summary>
        /// <param name="control">The panel containing the controls.</param>
        /// <param name="name">The name of the member.</param>
        /// <param name="type">The type of the member.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <param name="subGroupStack">Stack of subgroup types for nested editing.</param>
        /// <param name="advancedEdit">Whether advanced edit mode is enabled.</param>
        /// <returns>The saved object.</returns>
        public static object SaveWindowControls(StackPanel control, string name, Type type, object[] attributes, Type[] subGroupStack, bool advancedEdit)
        {
            IEditor converter = findEditor(type, attributes, advancedEdit);
            return converter.SaveWindowControls(control, name, type, attributes, subGroupStack);
        }


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
        public static object SaveMemberControl(object obj, StackPanel control, string name, Type type, object[] attributes, bool isWindow, Type[] subGroupStack)
        {
            IEditor converter = findEditor(obj.GetType(), attributes, false);
            return converter.SaveMemberControl(obj, control, name, type, attributes, isWindow, subGroupStack);
        }

        /// <summary>
        /// Gets a string representation of an object for display purposes.
        /// </summary>
        /// <param name="obj">The object to convert to string.</param>
        /// <param name="type">The type of the object.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <returns>A string representation of the object.</returns>
        public static string GetString(object obj, Type type, object[] attributes)
        {
            if (obj == null)
                return "NULL";
            IEditor editor = findEditor(obj.GetType(), attributes, false);
            return editor.GetString(obj, type, attributes);
        }

        /// <summary>
        /// Gets the title for an editor window.
        /// </summary>
        /// <param name="parent">The parent object name.</param>
        /// <param name="name">The member name.</param>
        /// <param name="obj">The object being edited.</param>
        /// <param name="type">The type of the object.</param>
        /// <returns>A formatted window title.</returns>
        public static string GetWindowTitle(string parent, string name, object obj, Type type)
        {
            return GetWindowTitle(parent, name, obj, type, new object[0]);
        }

        /// <summary>
        /// Gets the title for an editor window with attributes.
        /// </summary>
        /// <param name="parent">The parent object name.</param>
        /// <param name="name">The member name.</param>
        /// <param name="obj">The object being edited.</param>
        /// <param name="type">The type of the object.</param>
        /// <param name="attributes">The attributes associated with the member.</param>
        /// <returns>A formatted window title.</returns>
        public static string GetWindowTitle(string parent, string name, object obj, Type type, object[] attributes)
        {
            string parentStr = Text.GetMemberTitle(parent);
            string nameStr = Text.GetMemberTitle(name);

            //if (obj == null)
            //    return String.Format("{0}.{1}: New {2}", parentStr, nameStr, type.Name);
            //else
            //    return String.Format("{0}.{1}: {2}", parentStr, nameStr, DataEditor.GetString(obj, type, attributes));
            return String.Format("{0}: {1}", parentStr, nameStr);
        }

        /// <summary>
        /// Sets the clipboard object by serializing a copy of the given object.
        /// </summary>
        /// <param name="obj">The object to copy to clipboard.</param>
        /// <param name="converterType">Optional JSON converter type for serialization.</param>
        public static void SetClipboardObj(object obj, Type converterType)
        {
            try
            {
                if (converterType == null)
                    clipboardObj = ReflectionExt.SerializeCopy(obj);
                else
                {
                    JsonConverter conv = (JsonConverter)Activator.CreateInstance(converterType);
                    clipboardObj = ReflectionExt.SerializeCopy(obj, conv);
                }
            }
            catch (Exception ex)
            {
                DiagManager.Instance.LogError(ex);
            }
        }

        /// <summary>
        /// Extension method that gets a friendly display name for a type.
        /// </summary>
        /// <param name="type">The type to get a friendly name for.</param>
        /// <returns>The friendly type name if registered, otherwise the default display name.</returns>
        public static string GetFriendlyTypeString(this Type type)
        {
            string displayName;
            if (friendlyTypeNames.TryGetValue(type, out displayName))
                return displayName;
            return type.GetDisplayName();
        }
    }
}

