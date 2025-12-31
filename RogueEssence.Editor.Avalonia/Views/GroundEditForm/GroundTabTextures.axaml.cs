using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RogueEssence.Dev.Views
{
    /// <summary>
    /// View for the Textures tab in the ground editor.
    /// Provides UI for selecting and applying tile textures to ground maps.
    /// </summary>
    public class GroundTabTextures : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the GroundTabTextures class.
        /// </summary>
        public GroundTabTextures()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
