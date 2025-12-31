using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using RogueEssence.Dev.ViewModels;
using RogueEssence.Dungeon;
using Avalonia.Input;

namespace RogueEssence.Dev.Views
{
    /// <summary>
    /// User control for displaying and managing a list of map layers.
    /// Supports double-click to edit individual layers.
    /// </summary>
    public class LayerBox : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the LayerBox class.
        /// </summary>
        public LayerBox()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
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
        /// Handles double-click on layer items to open the layer editor.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The pointer released event arguments.</param>
        public async void lbxItems_DoubleClick(object sender, PointerReleasedEventArgs e)
        {
            if (!doubleclick)
                return;
            doubleclick = false;

            ILayerBoxViewModel viewModel = (ILayerBoxViewModel)DataContext;
            if (viewModel == null)
                return;

            await viewModel.EditLayer();
        }
    }
}
