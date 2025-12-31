using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RogueEssence.Dev.Views
{
    /// <summary>
    /// View for the Textures tab in the dungeon map editor.
    /// Provides UI for selecting and applying tile textures to the map.
    /// </summary>
    public class MapTabTextures : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the MapTabTextures class.
        /// </summary>
        public MapTabTextures()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
