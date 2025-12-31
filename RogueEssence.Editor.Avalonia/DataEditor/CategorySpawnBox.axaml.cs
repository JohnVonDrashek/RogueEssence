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
    /// A user control for displaying and editing category-based spawn lists.
    /// Provides UI for managing hierarchical spawn data with categories and their items.
    /// </summary>
    public class CategorySpawnBox : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CategorySpawnBox"/> class.
        /// Sets up event handlers for add category and add item buttons.
        /// </summary>
        public CategorySpawnBox()
        {
            this.InitializeComponent();
            Button addCategoryButton = this.FindControl<Button>("CategorySpawnBoxAddCategoryButton");
            addCategoryButton.AddHandler(PointerReleasedEvent, CategorySpawnBoxAddCategoryButton_OnPointerReleased, RoutingStrategies.Tunnel);
            Button addItemButtom = this.FindControl<Button>("CategorySpawnBoxAddItemButton");
            addItemButtom.AddHandler(PointerReleasedEvent, CategorySpawnBoxAddItemButton_OnPointerReleased, RoutingStrategies.Tunnel);
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

            ViewModels.CategorySpawnBoxViewModel viewModel = (ViewModels.CategorySpawnBoxViewModel)DataContext;
            if (viewModel == null)
                return;
            viewModel.gridCollection_DoubleClick(sender, e);
        }

        /// <summary>
        /// Handles the add category button release event. Shift key enables advanced edit mode.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The pointer released event arguments.</param>
        private void CategorySpawnBoxAddCategoryButton_OnPointerReleased(object sender, PointerReleasedEventArgs e)
        {
            KeyModifiers modifiers = e.KeyModifiers;
            bool advancedEdit = modifiers.HasFlag(KeyModifiers.Shift);
            CategorySpawnBoxViewModel vm = (CategorySpawnBoxViewModel) DataContext;
            vm.btnAddCategory_Click(advancedEdit);
        }

        /// <summary>
        /// Handles the add item button release event. Shift key enables advanced edit mode.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The pointer released event arguments.</param>
        private void CategorySpawnBoxAddItemButton_OnPointerReleased(object sender, PointerReleasedEventArgs e)
        {
            KeyModifiers modifiers = e.KeyModifiers;
            bool advancedEdit = modifiers.HasFlag(KeyModifiers.Shift);
            CategorySpawnBoxViewModel vm = (CategorySpawnBoxViewModel) DataContext;
            vm.btnAddItem_Click(advancedEdit);
        }
    }
}
