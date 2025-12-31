using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RogueEssence.Dev.Views
{
    /// <summary>
    /// View for the Terrain tab in the dungeon map editor.
    /// Provides UI for editing terrain types like walls, water, and lava.
    /// </summary>
    public class MapTabTerrain : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the MapTabTerrain class.
        /// </summary>
        public MapTabTerrain()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
