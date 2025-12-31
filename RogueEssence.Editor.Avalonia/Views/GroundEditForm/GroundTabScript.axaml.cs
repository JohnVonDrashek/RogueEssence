using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RogueEssence.Dev.Views
{
    /// <summary>
    /// View for the Script tab in the ground editor.
    /// Provides UI for managing and editing scripts associated with ground maps.
    /// </summary>
    public class GroundTabScript : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the GroundTabScript class.
        /// </summary>
        public GroundTabScript()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
