using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RogueEssence.Dev.Views
{
    /// <summary>
    /// View for the Properties tab in the ground editor.
    /// Provides UI for editing ground map metadata and settings.
    /// </summary>
    public class GroundTabProperties : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the GroundTabProperties class.
        /// </summary>
        public GroundTabProperties()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
