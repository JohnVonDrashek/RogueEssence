using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Reactive.Subjects;
using Avalonia.VisualTree;
using Avalonia.Input;

namespace RogueEssence.Dev.Views
{
    /// <summary>
    /// A user control for displaying and editing range-based dictionaries.
    /// Items are keyed by integer ranges (start-end) with adjustable bounds.
    /// </summary>
    public class RangeDictBox : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RangeDictBox"/> class.
        /// </summary>
        public RangeDictBox()
        {
            this.InitializeComponent();
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

            ViewModels.RangeDictBoxViewModel viewModel = (ViewModels.RangeDictBoxViewModel)DataContext;
            if (viewModel == null)
                return;
            viewModel.lbxCollection_DoubleClick(sender, e);
        }

        /// <summary>
        /// Handles changes to the start value numeric control.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The value changed event arguments.</param>
        public void nudStart_ValueChanged(object sender, NumericUpDownValueChangedEventArgs e)
        {
            ViewModels.RangeDictBoxViewModel viewModel = (ViewModels.RangeDictBoxViewModel)DataContext;
            viewModel.AdjustOtherLimit((int)e.NewValue, false);
        }

        /// <summary>
        /// Handles changes to the end value numeric control.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The value changed event arguments.</param>
        public void nudEnd_ValueChanged(object sender, NumericUpDownValueChangedEventArgs e)
        {
            ViewModels.RangeDictBoxViewModel viewModel = (ViewModels.RangeDictBoxViewModel)DataContext;
            viewModel.AdjustOtherLimit((int)e.NewValue, true);
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
    }
}
