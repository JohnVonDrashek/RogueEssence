using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;

namespace RogueEssence.Dev.Views
{
    /// <summary>
    /// Dialog window for changing map tile dimensions and retiling the map.
    /// </summary>
    public class MapRetileWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the MapRetileWindow class.
        /// </summary>
        public MapRetileWindow()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }


        /// <summary>
        /// Handles the window loaded event. Applies a workaround for text wrapping bug.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        //TODO: this is a workaround to a bug in text wrapping
        //the window size must be modified in order to invalidate a cached value for width
        //remove this once the bug is fixed
        public void Window_Loaded(object sender, EventArgs e)
        {
            if (Design.IsDesignMode)
                return;
            this.Width = this.Width + 10;
        }

        /// <summary>
        /// Handles the OK button click, closing the dialog with a true result.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        public void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close(true);
        }

        /// <summary>
        /// Handles the Cancel button click, closing the dialog with a false result.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        public void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close(false);
        }
    }
}
