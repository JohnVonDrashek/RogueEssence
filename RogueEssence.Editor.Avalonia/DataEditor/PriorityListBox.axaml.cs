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
    /// A user control for displaying and editing priority-based lists.
    /// Items are organized by priority values and can be reordered within or across priorities.
    /// </summary>
    public class PriorityListBox : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityListBox"/> class.
        /// Sets up event handlers for add and edit priority buttons.
        /// </summary>
        public PriorityListBox()
        {
            this.InitializeComponent();
            Button addButton = this.FindControl<Button>("PriorityListBoxAddButton");
            addButton.AddHandler(PointerReleasedEvent, PriorityListBoxAddButton_OnPointerReleased, RoutingStrategies.Tunnel);
            
            Button editButton = this.FindControl<Button>("PriorityListBoxEditButton");
            editButton.AddHandler(PointerReleasedEvent, PriorityListBoxEditButton_OnPointerReleased, RoutingStrategies.Tunnel);
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
        /// Handles double-click events on the collection list to edit the selected item.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The pointer released event arguments.</param>
        public void lbxCollection_DoubleClick(object sender, PointerReleasedEventArgs e)
        {
            if (!doubleclick)
                return;
            doubleclick = false;

            ViewModels.PriorityListBoxViewModel viewModel = (ViewModels.PriorityListBoxViewModel)DataContext;
            if (viewModel == null)
                return;
            viewModel.lbxCollection_DoubleClick(sender, e);
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
        private void PriorityListBoxAddButton_OnPointerReleased(object sender, PointerReleasedEventArgs e)
        {
            KeyModifiers modifiers = e.KeyModifiers;
            bool advancedEdit = modifiers.HasFlag(KeyModifiers.Shift);
            PriorityListBoxViewModel vm = (PriorityListBoxViewModel) DataContext;
            vm.btnAdd_Click(advancedEdit);
        }

        /// <summary>
        /// Handles the edit priority button release event. Shift key enables advanced edit mode.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The pointer released event arguments.</param>
        private void PriorityListBoxEditButton_OnPointerReleased(object sender, PointerReleasedEventArgs e)
        {
            KeyModifiers modifiers = e.KeyModifiers;
            bool advancedEdit = modifiers.HasFlag(KeyModifiers.Shift);
            PriorityListBoxViewModel vm = (PriorityListBoxViewModel) DataContext;
            vm.btnEditKey_Click(advancedEdit);
        }
    }
}
