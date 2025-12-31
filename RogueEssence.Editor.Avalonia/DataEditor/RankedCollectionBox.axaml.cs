using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Reactive.Subjects;
using Avalonia.Markup.Xaml.Templates;
using RogueEssence.Dev.ViewModels;

namespace RogueEssence.Dev.Views
{
    /// <summary>
    /// A user control for displaying and editing collections as a data grid with index numbers.
    /// Similar to CollectionBox but displays items in a grid format with visible indices.
    /// </summary>
    public class RankedCollectionBox : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RankedCollectionBox"/> class.
        /// Sets up the add button event handler.
        /// </summary>
        public RankedCollectionBox()
        {
            this.InitializeComponent();
            Button button = this.FindControl<Button>("RankedCollectionBoxAddButton");
            button.AddHandler(PointerReleasedEvent, RankedCollectionBoxAddButton_OnPointerReleased, RoutingStrategies.Tunnel);
        }

        /// <summary>
        /// Loads the XAML component for this control.
        /// </summary>
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        //TODO: there has to be some way to set the ItemTemplate's text binding in code-behind...
        //public void SetConv(IValueConverter conv)
        //{

        //    ListBox lbx = this.FindControl<ListBox>("lbxItems");
        //    //var template = (DataTemplate)lbx.ItemTemplate;
        //    //var content = template.Content;
        //    var subject = lbx.GetBindingSubject(ListBox.ItemTemplateProperty);
        //    //BindingBase bind = (BindingBase)subject.ToBinding();
        //    //bind.Converter = conv;

        //    Console.WriteLine(subject.ToString());
        //}

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
        public void lbxCollection_DoubleClick(object sender, PointerReleasedEventArgs e)
        {
            if (!doubleclick)
                return;
            doubleclick = false;

            ViewModels.CollectionBoxViewModel viewModel = (ViewModels.CollectionBoxViewModel)DataContext;
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
        private void RankedCollectionBoxAddButton_OnPointerReleased(object sender, PointerReleasedEventArgs e)
        {
            KeyModifiers modifiers = e.KeyModifiers;
            bool advancedEdit = modifiers.HasFlag(KeyModifiers.Shift);
            CollectionBoxViewModel vm = (CollectionBoxViewModel) DataContext;
            vm.btnAdd_Click(advancedEdit);
        }
    }
}
