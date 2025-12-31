using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;
using System.Collections.ObjectModel;
using Avalonia.Interactivity;
using Avalonia.Controls;
using RogueElements;
using System.Collections;

namespace RogueEssence.Dev.ViewModels
{
    /// <summary>
    /// ViewModel for the ClassBox control that manages display and editing of a single object.
    /// </summary>
    public class ClassBoxViewModel : ViewModelBase
    {
        public object Object { get; private set; }

        private string name;
        public string Name
        {
            get => name;
            set => this.SetIfChanged(ref name, value);
        }

        /// <summary>
        /// Delegate for applying edits to an element.
        /// </summary>
        /// <param name="element">The edited element.</param>
        public delegate void EditElementOp(object element);

        /// <summary>
        /// Delegate for initiating an element edit operation.
        /// </summary>
        /// <param name="element">The element to edit.</param>
        /// <param name="advancedEdit">Whether advanced edit mode is enabled.</param>
        /// <param name="op">The callback operation to perform after editing.</param>
        public delegate void ElementOp(object element, bool advancedEdit, EditElementOp op);

        public StringConv StringConv;

        public event ElementOp OnEditItem;
        public event Action OnMemberChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassBoxViewModel"/> class.
        /// </summary>
        /// <param name="conv">The string converter for display purposes.</param>
        public ClassBoxViewModel(StringConv conv)
        {
            StringConv = conv;
        }

        /// <summary>
        /// Gets the stored object cast to the specified type.
        /// </summary>
        /// <typeparam name="T">The type to cast the object to.</typeparam>
        /// <returns>The object cast to type T.</returns>
        public T GetObject<T>()
        {
            return (T)Object;
        }

        /// <summary>
        /// Loads the object from a source and updates the display name.
        /// </summary>
        /// <param name="source">The source object to load.</param>
        public void LoadFromSource(object source)
        {
            Object = source;
            Name = StringConv.GetString(source);
        }

        /// <summary>
        /// Updates the source object and notifies subscribers of the change.
        /// </summary>
        /// <param name="source">The new source object.</param>
        private void updateSource(object source)
        {
            LoadFromSource(source);
            OnMemberChanged?.Invoke();
        }

        /// <summary>
        /// Handles the edit button click event.
        /// </summary>
        /// <param name="advancedEdit">Whether advanced edit mode is enabled.</param>
        public void btnEdit_Click(bool advancedEdit)
        {
            object element = Object;
            OnEditItem?.Invoke(element, advancedEdit, updateSource);
        }
    }
}
