using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;
using System.Collections.ObjectModel;
using Avalonia.Interactivity;
using Avalonia.Controls;
using RogueElements;
using System.Collections;
using Avalonia.Input;
using RogueEssence.Dev.Views;

namespace RogueEssence.Dev.ViewModels
{
    /// <summary>
    /// Represents a single key-value element in a dictionary collection.
    /// </summary>
    public class DictionaryElement
    {
        private object key;
        public object Key
        {
            get { return key; }
        }
        private object value;
        public object Value
        {
            get { return value; }
        }
        public string DisplayValue
        {
            get { return conv.GetString(value); }
        }

        private StringConv conv;

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryElement"/> class.
        /// </summary>
        /// <param name="conv">The string converter for display purposes.</param>
        /// <param name="key">The dictionary key.</param>
        /// <param name="value">The dictionary value.</param>
        public DictionaryElement(StringConv conv, object key, object value)
        {
            this.conv = conv;
            this.key = key;
            this.value = value;
        }
    }

    /// <summary>
    /// ViewModel for the DictionaryBox control that manages key-value pair collections.
    /// </summary>
    public class DictionaryBoxViewModel : ViewModelBase
    {
        public ObservableCollection<DictionaryElement> Collection { get; }

        private int selectedIndex;
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { this.SetIfChanged(ref selectedIndex, value); }
        }

        /// <summary>
        /// Delegate for applying edits to a dictionary element.
        /// </summary>
        /// <param name="oldKey">The original key.</param>
        /// <param name="newKey">The new key (may be the same as oldKey).</param>
        /// <param name="element">The element value.</param>
        public delegate void EditElementOp(object oldKey, object newKey, object element);

        /// <summary>
        /// Delegate for initiating an element edit operation.
        /// </summary>
        /// <param name="key">The key of the element.</param>
        /// <param name="element">The element to edit.</param>
        /// <param name="advancedEdit">Whether advanced edit mode is enabled.</param>
        /// <param name="op">The callback operation to perform after editing.</param>
        public delegate void ElementOp(object key, object element, bool advancedEdit, EditElementOp op);

        public event ElementOp OnEditKey;
        public event ElementOp OnEditItem;
        public event Action OnMemberChanged;

        public StringConv StringConv;

        private Window parent;

        public bool ConfirmDelete;

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryBoxViewModel"/> class.
        /// </summary>
        /// <param name="parent">The parent window for dialog display.</param>
        /// <param name="conv">The string converter for display purposes.</param>
        public DictionaryBoxViewModel(Window parent, StringConv conv)
        {
            StringConv = conv;
            this.parent = parent;
            Collection = new ObservableCollection<DictionaryElement>();
        }

        /// <summary>
        /// Gets the collection as a typed dictionary.
        /// </summary>
        /// <typeparam name="T">The dictionary type to create.</typeparam>
        /// <returns>A dictionary of the specified type.</returns>
        public T GetDict<T>() where T : IDictionary
        {
            return (T)GetDict(typeof(T));
        }

        /// <summary>
        /// Gets the collection as a dictionary of the specified type.
        /// </summary>
        /// <param name="type">The type of dictionary to create.</param>
        /// <returns>A dictionary instance containing all collection entries.</returns>
        public IDictionary GetDict(Type type)
        {
            IDictionary result = (IDictionary)Activator.CreateInstance(type);
            foreach (DictionaryElement item in Collection)
                result.Add(item.Key, item.Value);
            return result;
        }

        /// <summary>
        /// Loads the collection from a source dictionary.
        /// </summary>
        /// <param name="source">The source dictionary to load from.</param>
        public void LoadFromDict(IDictionary source)
        {
            Collection.Clear();
            foreach (object obj in source.Keys)
                Collection.Add(new DictionaryElement(StringConv, obj, source[obj]));
        }



        private void editItem(object oldKey, object key, object element)
        {
            int index = getIndexFromKey(key);
            Collection[index] = new DictionaryElement(StringConv, Collection[index].Key, element);
            SelectedIndex = index;
            OnMemberChanged?.Invoke();
        }
        private async void editKey(object oldKey, object key, object element)
        {
            int existingIndex = getIndexFromKey(key);
            if (existingIndex > -1)
            {
                await MessageBox.Show(parent, "Dictionary already contains this key!", "Error", MessageBox.MessageBoxButtons.Ok);
                return;
            }

            int index = getIndexFromKey(oldKey);
            Collection[index] = new DictionaryElement(StringConv, key, element);
            SelectedIndex = index;
            OnMemberChanged?.Invoke();
        }

        private async void insertKey(object oldKey, object key, object element)
        {
            int existingIndex = getIndexFromKey(key);
            if (existingIndex > -1)
            {
                await MessageBox.Show(parent, "Dictionary already contains this key!", "Error", MessageBox.MessageBoxButtons.Ok);
                return;
            }
            bool advancedEdit = false;
            OnEditItem(key, element, advancedEdit, insertItem);
        }

        private void insertItem(object oldKey, object key, object element)
        {
            Collection.Add(new DictionaryElement(StringConv, key, element));
            SelectedIndex = Collection.Count-1;
            OnMemberChanged?.Invoke();
        }

        private int getIndexFromKey(object key)
        {
            int curIndex = 0;
            foreach (DictionaryElement item in Collection)
            {
                if (item.Key.Equals(key))
                    return curIndex;
                curIndex++;
            }
            return -1;
        }

        /// <summary>
        /// Initiates editing of a dictionary key at the specified index.
        /// </summary>
        /// <param name="index">The index of the element whose key to edit.</param>
        public void EditKey(int index)
        {
            bool advancedEdit = false;
            if (index > -1)
            {
                DictionaryElement item = Collection[index];
                OnEditKey?.Invoke(item.Key, item.Value, advancedEdit, editKey);
            }
        }

        /// <summary>
        /// Handles double-click events on the collection list to edit the selected item value.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The pointer released event arguments.</param>
        public void lbxCollection_DoubleClick(object sender, PointerReleasedEventArgs e)
        {
            //int index = lbxDictionary.IndexFromPoint(e.X, e.Y);
            int index = SelectedIndex;
            KeyModifiers modifiers = e.KeyModifiers;
            bool advancedEdit = modifiers.HasFlag(KeyModifiers.Shift);
            if (index > -1)
            {
                DictionaryElement item = Collection[index];
                OnEditItem?.Invoke(item.Key, item.Value, advancedEdit, editItem);
            }
        }

        /// <summary>
        /// Handles the add button click event to add a new dictionary entry.
        /// </summary>
        /// <param name="advancedEdit">Whether advanced edit mode is enabled.</param>
        public void btnAdd_Click(bool advancedEdit)
        {
            object newKey = null;
            object element = null;
            OnEditKey?.Invoke(newKey, element, advancedEdit, insertKey);
        }

        /// <summary>
        /// Handles the delete button click event to remove the selected entry.
        /// </summary>
        public async void btnDelete_Click()
        {
            if (SelectedIndex > -1 && SelectedIndex < Collection.Count)
            {
                if (ConfirmDelete)
                {
                    MessageBox.MessageBoxResult result = await MessageBox.Show(parent, "Are you sure you want to delete this item:\n" + Collection[SelectedIndex].DisplayValue, "Confirm Delete",
                    MessageBox.MessageBoxButtons.YesNo);
                    if (result == MessageBox.MessageBoxResult.No)
                        return;
                }

                Collection.RemoveAt(SelectedIndex);
                OnMemberChanged?.Invoke();
            }
        }
    }
}
