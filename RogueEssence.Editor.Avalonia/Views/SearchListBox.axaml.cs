using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using Avalonia.Input;

namespace RogueEssence.Dev.Views
{
    /// <summary>
    /// A user control that provides a searchable list box with filtering capabilities.
    /// Combines a text search field with a list box for easy item filtering.
    /// </summary>
    public class SearchListBox : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the SearchListBox class.
        /// </summary>
        public SearchListBox()
        {
            this.InitializeComponent();

        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// Handles the data context changed event to set up search text subscriptions.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        public void slb_DataContextChanged(object sender, EventArgs e)
        {
            ViewModels.SearchListBoxViewModel viewModel = (ViewModels.SearchListBoxViewModel)DataContext;
            if (viewModel == null)
                return;
            TextBox textBox = this.FindControl<TextBox>("txtSearch");
            //TODO: memory leak?
            textBox.GetObservable(TextBox.TextProperty).Subscribe(viewModel.txtSearch_TextChanged);
        }

        bool doubleclick;

        /// <summary>
        /// Marks the start of a potential double-click action.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        public void doubleClickStart(object sender, RoutedEventArgs e)
        {
            doubleclick = true;
        }

        /// <summary>
        /// Handles double-click on list items.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The pointer released event arguments.</param>
        public void lbxItems_DoubleClick(object sender, PointerReleasedEventArgs e)
        {
            if (!doubleclick)
                return;
            doubleclick = false;

            ViewModels.SearchListBoxViewModel viewModel = (ViewModels.SearchListBoxViewModel)DataContext;
            if (viewModel == null)
                return;
            viewModel.lbxItems_DoubleClick(sender, e);
        }

        /// <summary>
        /// Sets the context menu for the items list box.
        /// </summary>
        /// <param name="menu">The context menu to assign.</param>
        public void SetListContextMenu(ContextMenu menu)
        {
            ListBox lbx = this.FindControl<ListBox>("lbxItems");
            lbx.ContextMenu = menu;
        }
    }
}
