using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using RogueElements;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Reactive.Subjects;
using Avalonia.Input;
using RogueEssence.Dev.ViewModels;

namespace RogueEssence.Dev.Views
{
    /// <summary>
    /// A user control for displaying and editing weighted spawn lists.
    /// Items have spawn weights that determine their probability of being selected.
    /// </summary>
    public class SpawnListBox : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpawnListBox"/> class.
        /// Sets up the add button event handler.
        /// </summary>
        public SpawnListBox()
        {
            this.InitializeComponent();
            Button button = this.FindControl<Button>("SpawnListBoxAddButton");
            button.AddHandler(PointerReleasedEvent, SpawnListBoxAddButton_OnPointerReleased, RoutingStrategies.Tunnel);
        }

        /// <summary>
        /// Loads the XAML component for this control.
        /// </summary>
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// Tracks whether a double-click sequence has started.
        /// </summary>
        bool doubleclick;

        /// <summary>
        /// Marks the beginning of a potential double-click event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The routed event arguments.</param>
        public void doubleClickStart(object sender, RoutedEventArgs e)
        {
            doubleclick = true;
        }

        /// <summary>
        /// Handles double-click events on the collection grid to edit the selected item.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The pointer released event arguments.</param>
        public void gridCollection_DoubleClick(object sender, PointerReleasedEventArgs e)
        {
            if (!doubleclick)
                return;
            doubleclick = false;

            ViewModels.SpawnListBoxViewModel viewModel = (ViewModels.SpawnListBoxViewModel)DataContext;
            if (viewModel == null)
                return;
            viewModel.gridCollection_DoubleClick(sender, e);
        }

        /// <summary>
        /// Sets the context menu for the data grid control.
        /// </summary>
        /// <param name="menu">The context menu to set.</param>
        public void SetListContextMenu(ContextMenu menu)
        {
            DataGrid lbx = this.FindControl<DataGrid>("gridItems");
            lbx.ContextMenu = menu;
        }

        /// <summary>
        /// Handles the add button release event. Shift key enables advanced edit mode.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The pointer released event arguments.</param>
        private void SpawnListBoxAddButton_OnPointerReleased(object sender, PointerReleasedEventArgs e)
        {
            KeyModifiers modifiers = e.KeyModifiers;
            bool advancedEdit = modifiers.HasFlag(KeyModifiers.Shift);
            SpawnListBoxViewModel vm = (SpawnListBoxViewModel) DataContext;
            vm.btnAdd_Click(advancedEdit);
        }
    }
}
