using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RogueEssence.Dev.Views
{
    /// <summary>
    /// View for the Walls tab in the ground editor.
    /// Provides UI for placing and editing wall collision data on ground maps.
    /// </summary>
    public class GroundTabWalls : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the GroundTabWalls class.
        /// </summary>
        public GroundTabWalls()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
