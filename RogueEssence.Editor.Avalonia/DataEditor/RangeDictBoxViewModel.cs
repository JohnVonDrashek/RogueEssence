using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;
using System.Collections.ObjectModel;
using Avalonia.Interactivity;
using Avalonia.Controls;
using RogueElements;
using System.Collections;
using RogueEssence.Dev.Views;
using RogueEssence.LevelGen;
using System.Linq;

namespace RogueEssence.Dev.ViewModels
{
    /// <summary>
    /// Represents a single element in a range dictionary with start and end bounds.
    /// </summary>
    public class RangeDictElement : ViewModelBase
    {
        private int start;
        public int Start
        {
            get { return start; }
            set
            {
                start = value;
                DisplayStart = DisplayStart;
            }
        }
        private int end;
        public int End
        {
            get { return end; }
            set
            {
                end = value;
                DisplayEnd = DisplayEnd;
            }
        }

        //TODO: the separation of display vs. internal value can be offloaded
        //to the already existing converter system
        public int DisplayStart
        {
            get { return start + addMin; }
            set { this.RaisePropertyChanged(); }
        }
        public int DisplayEnd
        {
            get { return end + addMax; }
            set { this.RaisePropertyChanged(); }
        }

        private object value;
        public object Value
        {
            get { return value; }
        }

        private int addMin;
        private int addMax;

        public string DisplayValue
        {
            get { return conv.GetString(value); }
        }

        private StringConv conv;

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeDictElement"/> class.
        /// </summary>
        /// <param name="conv">The string converter for display purposes.</param>
        /// <param name="addMin">The offset to add to the start value for display.</param>
        /// <param name="addMax">The offset to add to the end value for display.</param>
        /// <param name="start">The start of the range.</param>
        /// <param name="end">The end of the range.</param>
        /// <param name="value">The value associated with this range.</param>
        public RangeDictElement(StringConv conv, int addMin, int addMax, int start, int end, object value)
        {
            this.conv = conv;
            this.addMin = addMin;
            this.addMax = addMax;
            this.start = start;
            this.end = end;
            this.value = value;
        }
    }

    /// <summary>
    /// ViewModel for the RangeDictBox control that manages range-keyed collections.
    /// </summary>
    public class RangeDictBoxViewModel : ViewModelBase
    {
        public ObservableCollection<RangeDictElement> Collection { get; }

        private int currentElement;
        public int CurrentElement
        {
            get { return currentElement; }
            set
            {
                this.SetIfChanged(ref currentElement, value);
                if (currentElement > -1)
                {
                    settingRange = true;
                    CurrentStart = Collection[currentElement].DisplayStart;
                    CurrentEnd = Collection[currentElement].DisplayEnd;
                    settingRange = false;
                }
                else
                {
                    settingRange = true;
                    CurrentStart = 0 + AddMin;
                    CurrentEnd = 1 + AddMax;
                    settingRange = false;
                }
            }
        }

        private int currentStart;
        public int CurrentStart
        {
            get { return currentStart; }
            set
            {
                this.SetIfChanged(ref currentStart, value);
                if (currentElement > -1)
                {
                    Collection[currentElement].Start = currentStart - AddMin;
                    EraseRange(new IntRange(Collection[currentElement].Start, Collection[currentElement].End), currentElement);
                }
            }
        }

        private int currentEnd;
        public int CurrentEnd
        {
            get { return currentEnd; }
            set
            {
                this.SetIfChanged(ref currentEnd, value);
                if (currentElement > -1)
                {
                    Collection[currentElement].End = currentEnd - AddMax;
                    EraseRange(new IntRange(Collection[currentElement].Start, Collection[currentElement].End), currentElement);
                }
            }
        }


        public bool Index1;
        public bool Inclusive;

        public int AddMin
        {
            get { return Index1 ? 1 : 0; }
        }
        public int AddMax
        {
            get
            {
                int result = Index1 ? 1 : 0;
                if (Inclusive)
                    result -= 1;
                return result;
            }
        }

        /// <summary>
        /// Delegate for applying edits to a range dictionary element.
        /// </summary>
        /// <param name="key">The range key.</param>
        /// <param name="element">The edited element.</param>
        public delegate void EditElementOp(IntRange key, object element);

        /// <summary>
        /// Delegate for initiating an element edit operation.
        /// </summary>
        /// <param name="key">The range key.</param>
        /// <param name="element">The element to edit.</param>
        /// <param name="advancedEdit">Whether advanced edit mode is enabled.</param>
        /// <param name="op">The callback operation to perform after editing.</param>
        public delegate void ElementOp(IntRange key, object element, bool advancedEdit, EditElementOp op);

        public event ElementOp OnEditKey;
        public event ElementOp OnEditItem;
        public event Action OnMemberChanged;

        public StringConv StringConv;

        private Window parent;

        public bool ConfirmDelete;

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeDictBoxViewModel"/> class.
        /// </summary>
        /// <param name="parent">The parent window for dialog display.</param>
        /// <param name="conv">The string converter for display purposes.</param>
        public RangeDictBoxViewModel(Window parent, StringConv conv)
        {
            StringConv = conv;
            this.parent = parent;
            Collection = new ObservableCollection<RangeDictElement>();
        }

        /// <summary>
        /// Gets the collection as a typed range dictionary.
        /// </summary>
        /// <typeparam name="T">The range dictionary type to create.</typeparam>
        /// <returns>A range dictionary of the specified type.</returns>
        public T GetDict<T>() where T : IRangeDict
        {
            return (T)GetDict(typeof(T));
        }

        /// <summary>
        /// Gets the collection as a range dictionary of the specified type.
        /// </summary>
        /// <param name="type">The type of range dictionary to create.</param>
        /// <returns>A range dictionary instance containing all collection entries.</returns>
        public IRangeDict GetDict(Type type)
        {
            IRangeDict result = (IRangeDict)Activator.CreateInstance(type);
            foreach (RangeDictElement item in Collection)
                result.SetRange(item.Value, new IntRange(item.Start, item.End));
            return result;
        }

        /// <summary>
        /// Loads the collection from a range dictionary source.
        /// </summary>
        /// <param name="source">The range dictionary to load from.</param>
        public void LoadFromDict(IRangeDict source)
        {
            Collection.Clear();
            foreach (IntRange obj in source.EnumerateRanges())
            {
                for (int ii = 0; ii <= Collection.Count; ii++)
                {
                    if (ii == Collection.Count || obj.Min < Collection[ii].Start)
                    {
                        Collection.Insert(ii, new RangeDictElement(StringConv, AddMin, AddMax, obj.Min, obj.Max, source.GetItem(obj.Min)));
                        break;
                    }
                }
            }
        }



        private void editItem(IntRange key, object element)
        {
            int index = getIndexFromKey(key);
            Collection[index] = new RangeDictElement(StringConv, AddMin, AddMax, Collection[index].Start, Collection[index].End, element);
            CurrentElement = index;
            OnMemberChanged?.Invoke();
        }

        private void insertKey(IntRange key, object element)
        {
            bool advancedEdit = false;
            OnEditItem(key, element, advancedEdit, insertItem);
        }

        private void insertItem(IntRange key, object element)
        {
            EraseRange(key, -1);
            for (int ii = 0; ii <= Collection.Count; ii++)
            {
                if (ii == Collection.Count || key.Min < Collection[ii].Start)
                {
                    Collection.Insert(ii, new RangeDictElement(StringConv, AddMin, AddMax, key.Min, key.Max, element));
                    CurrentElement = ii;
                    break;
                }
            }
            OnMemberChanged?.Invoke();
        }

        /// <summary>
        /// Inserts an element at the specified index with a range starting from the previous element's end.
        /// </summary>
        /// <param name="index">The index at which to insert.</param>
        /// <param name="element">The element to insert.</param>
        public void InsertOnKey(int index, object element)
        {
            IntRange key = new IntRange(0);
            if (0 <= index && index < Collection.Count)
            {
                key = new IntRange(Collection[index].End);
            }

            EraseRange(key, -1);
            for (int ii = 0; ii <= Collection.Count; ii++)
            {
                if (ii == Collection.Count || key.Min < Collection[ii].Start)
                {
                    Collection.Insert(ii, new RangeDictElement(StringConv, AddMin, AddMax, key.Min, key.Max, element));
                    CurrentElement = ii;
                    break;
                }
            }
            OnMemberChanged?.Invoke();
        }

        private void EraseRange(IntRange range, int exceptionIdx)
        {
            for (int ii = Collection.Count - 1; ii >= 0; ii--)
            {
                if (exceptionIdx == ii)
                    continue;
                if (range.Min <= Collection[ii].Start && Collection[ii].End <= range.Max)
                    Collection.RemoveAt(ii);
                else if (Collection[ii].Start < range.Min && range.Max < Collection[ii].End)
                {
                    Collection[ii] = new RangeDictElement(StringConv, AddMin, AddMax, Collection[ii].Start, range.Min, Collection[ii].Value);
                    Collection.Insert(ii+1, new RangeDictElement(StringConv, AddMin, AddMax, range.Max, Collection[ii].End, Collection[ii].Value));
                }
                else if (range.Min <= Collection[ii].Start && Collection[ii].Start < range.Max)
                    Collection[ii] = new RangeDictElement(StringConv, AddMin, AddMax, range.Max, Collection[ii].End, Collection[ii].Value);
                else if (range.Min < Collection[ii].End && Collection[ii].End <= range.Max)
                    Collection[ii] = new RangeDictElement(StringConv, AddMin, AddMax, Collection[ii].Start, range.Min, Collection[ii].Value);
            }
        }

        private int getIndexFromKey(IntRange key)
        {
            int curIndex = 0;
            foreach (RangeDictElement item in Collection)
            {
                if (item.Start == key.Min && item.End == key.Max)
                    return curIndex;
                curIndex++;
            }
            return -1;
        }


        /// <summary>
        /// Handles double-click events on the collection list to edit the selected item.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The routed event arguments.</param>
        public void lbxCollection_DoubleClick(object sender, RoutedEventArgs e)
        {
            //int index = lbxDictionary.IndexFromPoint(e.X, e.Y);
            int index = CurrentElement;
            if (index > -1)
            {
                RangeDictElement item = Collection[index];
                bool advancedEdit = false;
                OnEditItem?.Invoke(new IntRange(item.Start, item.End), item.Value, advancedEdit, editItem);
            }
        }

        /// <summary>
        /// Handles the add button click event.
        /// </summary>
        public void btnAdd_Click()
        {
            IntRange newKey = new IntRange(0);
            object element = null;
            bool advancedEdit = false;
            OnEditKey?.Invoke(newKey, element, advancedEdit, insertKey);
        }

        /// <summary>
        /// Handles the delete button click event.
        /// </summary>
        public async void btnDelete_Click()
        {
            if (CurrentElement > -1 && CurrentElement < Collection.Count)
            {
                if (ConfirmDelete)
                {
                    MessageBox.MessageBoxResult result = await MessageBox.Show(parent, "Are you sure you want to delete this item:\n" + Collection[currentElement].DisplayValue, "Confirm Delete",
                    MessageBox.MessageBoxButtons.YesNo);
                    if (result == MessageBox.MessageBoxResult.No)
                        return;
                }

                Collection.RemoveAt(CurrentElement);
                OnMemberChanged?.Invoke();
            }
        }

        /// <summary>
        /// Flag to prevent recursive limit adjustments.
        /// </summary>
        bool settingRange;

        /// <summary>
        /// Adjusts the start or end limit to maintain valid range constraints.
        /// </summary>
        /// <param name="newVal">The new value being set.</param>
        /// <param name="changeEnd">Whether the end value is being changed (true) or start value (false).</param>
        public void AdjustOtherLimit(int newVal, bool changeEnd)
        {
            int newStart = CurrentStart;
            int newEnd = CurrentEnd;

            if (changeEnd)
                newEnd = newVal;
            else
                newStart = newVal;

            if (!settingRange && newEnd < newStart)
            {
                if (changeEnd)
                    CurrentStart = newEnd;
                else
                    CurrentEnd = newStart;
            }
        }
    }
}
