using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using RogueEssence.Dev.ViewModels;

namespace RogueEssence.Dev.Views
{
    /// <summary>
    /// A user control for displaying and editing collections of items in a list format.
    /// Supports adding, editing, and removing items from the collection.
    /// </summary>
    public class CollectionBox : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionBox"/> class.
        /// Sets up the add button event handler.
        /// </summary>
        public CollectionBox()
        {
            this.InitializeComponent();
            Button button = this.FindControl<Button>("CollectionBoxAddButton");
            button.AddHandler(PointerReleasedEvent, CollectionBoxAddButton_OnPointerReleased, RoutingStrategies.Tunnel);
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
        /// Handles double-click events on the collection list to edit the selected item.
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
        /// Sets the context menu for the list box control.
        /// </summary>
        /// <param name="menu">The context menu to set.</param>
        public void SetListContextMenu(ContextMenu menu)
        {
            ListBox lbx = this.FindControl<ListBox>("lbxItems");
            lbx.ContextMenu = menu;
        }

        /// <summary>
        /// Handles the add button release event. Shift key enables advanced edit mode.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The pointer released event arguments.</param>
        private void CollectionBoxAddButton_OnPointerReleased(object sender, PointerReleasedEventArgs e)
        {
            KeyModifiers modifiers = e.KeyModifiers;
            bool advancedEdit = modifiers.HasFlag(KeyModifiers.Shift);
            CollectionBoxViewModel vm = (CollectionBoxViewModel) DataContext;
            vm.btnAdd_Click(advancedEdit);
        }
    }
}
