using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace RogueEssence.Dev.Views
{
    /// <summary>
    /// Dialog window for resizing maps with configurable dimensions.
    /// </summary>
    public class MapResizeWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the MapResizeWindow class.
        /// </summary>
        public MapResizeWindow()
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
