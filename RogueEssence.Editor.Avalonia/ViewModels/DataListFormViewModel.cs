using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using System.Threading.Tasks;

namespace RogueEssence.Dev.ViewModels
{
    /// <summary>
    /// Container for data operations in the editor, supporting hierarchical menus of actions.
    /// Used to define operations like Re-Index, Resave as File, etc.
    /// </summary>
    public class DataOpContainer
    {
        /// <summary>
        /// Delegate type for async task actions.
        /// </summary>
        public delegate Task TaskAction();

        /// <summary>
        /// The action to execute when this operation is invoked.
        /// </summary>
        public TaskAction CommandAction;

        /// <summary>
        /// Gets the display name of this operation.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the collection of child operations for hierarchical menus.
        /// </summary>
        public ObservableCollection<DataOpContainer> Items { get; }

        /// <summary>
        /// Initializes a new instance of the DataOpContainer class.
        /// </summary>
        /// <param name="name">The display name of the operation.</param>
        /// <param name="command">The async action to execute.</param>
        /// <param name="items">Optional child operations.</param>
        public DataOpContainer(string name, TaskAction command, params DataOpContainer[] items)
        {
            Name = name;
            CommandAction = command;
            Items = new ObservableCollection<DataOpContainer>();
            foreach (DataOpContainer op in items)
                Items.Add(op);

        }

        /// <summary>
        /// Executes the command action asynchronously.
        /// </summary>
        /// <returns>A task representing the async operation.</returns>
        public async Task Command()
        {
            await CommandAction();
        }
    }

    /// <summary>
    /// ViewModel for the data list form, providing searchable data entry selection
    /// with add, delete, and save operations for game data types.
    /// </summary>
    public class DataListFormViewModel : ViewModelBase
    {
        public event Action SelectedOKEvent;
        public event Action SelectedAddEvent;
        public event Action SelectedDeleteEvent;
        public event Action SelectedSaveFileEvent;
        public event Action SelectedSaveDiffEvent;

        public ObservableCollection<DataOpContainer> OpList { get; }

        private List<string> keys;

        /// <summary>
        /// Initializes a new instance of the DataListFormViewModel class.
        /// </summary>
        public DataListFormViewModel()
        {
            SearchList = new SearchListBoxViewModel();
            OpList = new ObservableCollection<DataOpContainer>();

            SearchList.SetName("Select Item");
            SearchList.ListBoxMouseDoubleClick += slbEntries_MouseDoubleClick;

            keys = new List<string>();
        }

        public SearchListBoxViewModel SearchList { get; set; }

        public string ChosenAsset { get { return SearchList.InternalIndex > -1 ? keys[SearchList.InternalIndex] : null; } }

        private string name;
        public string Name
        {
            get { return name; }
            set { this.SetIfChanged(ref name, value); }
        }

        /// <summary>
        /// Sets the entries to display in the list from a dictionary of key-value pairs.
        /// </summary>
        /// <param name="entries">Dictionary mapping asset keys to display names.</param>
        public void SetEntries(Dictionary<string, string> entries)
        {
            SearchList.Clear();
            keys.Clear();
            List<string> items = new List<string>();
            foreach (string key in entries.Keys)
            {
                keys.Add(key);
                items.Add(key + ": " + entries[key]);
            }
            SearchList.SetItems(items);
        }

        /// <summary>
        /// Sets the available operations for the Edit menu.
        /// </summary>
        /// <param name="ops">The operations to add to the menu.</param>
        public void SetOps(params DataOpContainer[] ops)
        {
            DataOpContainer edit = new DataOpContainer("_Edit", null, ops);
            OpList.Add(edit);
        }

        /// <summary>
        /// Modifies an existing entry's display text.
        /// </summary>
        /// <param name="index">The key of the entry to modify.</param>
        /// <param name="entry">The new display name.</param>
        public void ModifyEntry(string index, string entry)
        {
            int intIndex = keys.IndexOf(index);
            SearchList.SetInternalEntry(intIndex, index + ": " + entry);
        }

        /// <summary>
        /// Adds a new entry to the list.
        /// </summary>
        /// <param name="key">The asset key for the new entry.</param>
        /// <param name="entry">The display name for the new entry.</param>
        public void AddEntry(string key, string entry)
        {
            keys.Add(key);
            SearchList.AddItem(key + ": " + entry);
        }

        /// <summary>
        /// Deletes an entry from the list by key.
        /// </summary>
        /// <param name="key">The key of the entry to delete.</param>
        public void DeleteEntry(string key)
        {
            int idx = keys.IndexOf(key);
            keys.RemoveAt(idx);
            SearchList.RemoveInternalAt(idx);
        }

        /// <summary>
        /// Handles the Add button click event.
        /// </summary>
        public void btnAdd_Click()
        {
            SelectedAddEvent?.Invoke();
        }

        /// <summary>
        /// Handles the Delete button click event.
        /// </summary>
        public void btnDelete_Click()
        {
            SelectedDeleteEvent?.Invoke();
        }

        /// <summary>
        /// Handles double-click on a list entry to open for editing.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The routed event arguments.</param>
        public void slbEntries_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            //int index = slbEntries.IndexFromPoint(e.Location);
            //if (index != ListBox.NoMatches)
            //{
            //    ChosenEntry = slbEntries.GetInternalIndex(index);
            //    SelectedOKEvent?.Invoke();
            //}

            if (SearchList.InternalIndex > -1)
                SelectedOKEvent?.Invoke();
        }

        /// <summary>
        /// Handles the Save as File menu click event.
        /// </summary>
        public void mnuDataFile_Click()
        {
            if (SearchList.InternalIndex > -1)
                SelectedSaveFileEvent?.Invoke();
        }

        /// <summary>
        /// Handles the Save as Diff/Patch menu click event.
        /// </summary>
        public void mnuDataDiff_Click()
        {
            if (SearchList.InternalIndex > -1)
                SelectedSaveDiffEvent?.Invoke();
        }
    }
}
