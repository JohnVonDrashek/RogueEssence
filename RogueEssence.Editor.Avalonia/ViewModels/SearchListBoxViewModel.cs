using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;
using System.Collections.ObjectModel;
using Avalonia.Interactivity;
using Avalonia.Controls;

namespace RogueEssence.Dev.ViewModels
{
    /// <summary>
    /// ViewModel for a searchable list box control that supports filtering items by text.
    /// Maintains an internal list of entries and a filtered view based on search text,
    /// with mapping between displayed indices and internal indices.
    /// </summary>
    public class SearchListBoxViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the SearchListBoxViewModel class.
        /// </summary>
        public SearchListBoxViewModel()
        {
            entries = new List<string>();
            entryMap = new List<int>();
            SearchItems = new ObservableCollection<string>();
            DataName = "";
            SearchText = "";
        }

        //total items in the box
        private List<string> entries;
        //maps index from the search list to index in the entries list
        private List<int> entryMap;

        private string dataName;
        public string DataName
        {
            get { return dataName; }
            set { this.SetIfChanged(ref dataName, value); }
        }

        private string searchText;
        public string SearchText
        {
            get { return searchText; }
            set { this.SetIfChanged(ref searchText, value); }
        }

        public ObservableCollection<string> SearchItems { get; }
        private void setVisualItem(int idx, string val)
        {
            int tmp = SelectedSearchIndex;
            SearchItems[idx] = val;
            SelectedSearchIndex = tmp;
        }



        public int Count => entries.Count;


        private int selectedSearchIndex;
        public int SelectedSearchIndex
        {
            get { return selectedSearchIndex; }
            set
            {
                InternalIndex = (value > -1 && value < entryMap.Count) ? entryMap[value] : -1;
                this.RaiseAndSet(ref selectedSearchIndex, value);
                SelectedIndexChanged?.Invoke();
            }
        }

        public int InternalIndex { get; private set; }


        public event EventHandler<RoutedEventArgs> ListBoxMouseDoubleClick;
        public event Action SelectedIndexChanged;

        /// <summary>
        /// Sets the display name label for the list box.
        /// </summary>
        /// <param name="name">The name to display (colon will be appended).</param>
        public void SetName(string name)
        {
            DataName = name + ":";
        }

        /// <summary>
        /// Sets the items in the list box, applying the current search filter.
        /// </summary>
        /// <param name="items">The list of string items to display.</param>
        public void SetItems(List<string> items)
        {
            entries.Clear();

            foreach (string item in items)
            {
                entries.Add(item);
                if (SearchText == "" || entries[entries.Count - 1].IndexOf(SearchText, StringComparison.CurrentCultureIgnoreCase) > -1)
                {
                    SearchItems.Add(entries[entries.Count - 1]);
                    entryMap.Add(entries.Count - 1);
                }
            }
        }

        /// <summary>
        /// Adds a new item to the list and selects it if it matches the current search filter.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void AddItem(string item)
        {
            entries.Add(item);

            if (SearchText == "" || entries[entries.Count - 1].IndexOf(SearchText, StringComparison.CurrentCultureIgnoreCase) > -1)
            {
                SearchItems.Add(entries[entries.Count - 1]);
                entryMap.Add(entries.Count - 1);
                SelectedSearchIndex = SearchItems.Count - 1;
            }
        }

        /// <summary>
        /// Clears all items from the list and resets the search text.
        /// </summary>
        public void Clear()
        {
            entries.Clear();

            SearchItems.Clear();
            entryMap.Clear();
            SearchText = "";
        }

        /// <summary>
        /// Removes the item at the specified search list index.
        /// </summary>
        /// <param name="index">The index in the filtered search list.</param>
        public void RemoveAt(int index)
        {
            RemoveInternalAt(entryMap[index]);
        }

        /// <summary>
        /// Gets the item at the specified search list index.
        /// </summary>
        /// <param name="index">The index in the filtered search list.</param>
        /// <returns>The item string at the specified index.</returns>
        public string GetItem(int index)
        {
            return entries[entryMap[index]];
        }

        /// <summary>
        /// Sets the item at the specified search list index.
        /// </summary>
        /// <param name="index">The index in the filtered search list.</param>
        /// <param name="entry">The new entry value.</param>
        public void SetItem(int index, string entry)
        {
            entries[entryMap[index]] = entry;

            if (SearchText == "" || entries[entryMap[index]].IndexOf(SearchText, StringComparison.CurrentCultureIgnoreCase) > -1)
            {
                setVisualItem(index, entry);
            }
            else
            {
                SearchItems.RemoveAt(index);
                entryMap.RemoveAt(index);
            }
        }

        /// <summary>
        /// Gets the internal index for a given search list index.
        /// </summary>
        /// <param name="index">The index in the filtered search list.</param>
        /// <returns>The corresponding index in the internal entries list.</returns>
        public int GetInternalIndex(int index)
        {
            return entryMap[index];
        }

        /// <summary>
        /// Gets the entry at the specified internal index.
        /// </summary>
        /// <param name="internalIndex">The index in the internal entries list.</param>
        /// <returns>The entry string at the specified internal index.</returns>
        public string GetInternalEntry(int internalIndex)
        {
            return entries[internalIndex];
        }

        /// <summary>
        /// Sets the entry at the specified internal index, updating the filtered view as needed.
        /// </summary>
        /// <param name="internalIndex">The index in the internal entries list.</param>
        /// <param name="entry">The new entry value.</param>
        public void SetInternalEntry(int internalIndex, string entry)
        {
            bool oldAppears = (SearchText == "" || entries[internalIndex].IndexOf(SearchText, StringComparison.CurrentCultureIgnoreCase) > -1);
            bool newAppears = (SearchText == "" || entry.IndexOf(SearchText, StringComparison.CurrentCultureIgnoreCase) > -1);
            entries[internalIndex] = entry;

            int shownIndex = entryMap.IndexOf(internalIndex);

            if (oldAppears && newAppears)
            {
                //change
                setVisualItem(shownIndex, entry);
            }
            else if (oldAppears)
            {
                //remove
                SearchItems.RemoveAt(shownIndex);
                entryMap.RemoveAt(shownIndex);
            }
            else if (newAppears)
            {
                //add
                for (int ii = 0; ii < entryMap.Count; ii++)
                {
                    if (entryMap[ii] < internalIndex)
                    {
                        SearchItems.Insert(ii, entry);
                        entryMap.Insert(ii, internalIndex);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Removes the entry at the specified internal index and updates the index mappings.
        /// </summary>
        /// <param name="internalIndex">The index in the internal entries list.</param>
        public void RemoveInternalAt(int internalIndex)
        {
            entries.RemoveAt(internalIndex);

            int shownIndex = entryMap.IndexOf(internalIndex);
            if (shownIndex > -1)
            {
                //remove
                SearchItems.RemoveAt(shownIndex);
                entryMap.RemoveAt(shownIndex);
            }

            for (int ii = 0; ii < entryMap.Count; ii++)
            {
                if (entryMap[ii] > internalIndex)
                    entryMap[ii] = entryMap[ii] - 1;
            }
        }

        private void RefreshFilter()
        {
            int internalIndex = -1;
            if (SelectedSearchIndex > -1)
                internalIndex = InternalIndex;
            SearchItems.Clear();
            entryMap.Clear();

            int index = -1;
            for (int ii = 0; ii < entries.Count; ii++)
            {
                if (SearchText == "" || entries[ii].IndexOf(SearchText, StringComparison.CurrentCultureIgnoreCase) > -1)
                {
                    entryMap.Add(ii);
                    SearchItems.Add(entries[ii]);
                    if (ii == internalIndex)
                        index = entryMap.Count - 1;
                }
            }
            if (index > -1)
                SelectedSearchIndex = index;
        }

        //public int IndexFromPoint(Point p)
        //{
        //    return lbxItems.IndexFromPoint(p);
        //}

        /// <summary>
        /// Handles text changed events for the search text box.
        /// </summary>
        /// <param name="text">The new search text.</param>
        public void txtSearch_TextChanged(string text)
        {
            RefreshFilter();
        }

        /// <summary>
        /// Handles double-click events on list box items.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The routed event arguments.</param>
        public void lbxItems_DoubleClick(object sender, RoutedEventArgs e)
        {
            ListBoxMouseDoubleClick?.Invoke(sender, e);
        }

    }
}
