using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RogueEssence.Dev.Views
{
    /// <summary>
    /// View for the Decorations tab in the ground editor.
    /// Provides UI for placing and editing decorative elements on ground maps.
    /// </summary>
    public class GroundTabDecorations : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the GroundTabDecorations class.
        /// </summary>
        public GroundTabDecorations()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
