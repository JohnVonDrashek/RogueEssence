using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RogueEssence.Dev.Views
{
    /// <summary>
    /// View for the Tiles tab in the dungeon map editor.
    /// Provides UI for editing individual tile properties and effects.
    /// </summary>
    public class MapTabTiles : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the MapTabTiles class.
        /// </summary>
        public MapTabTiles()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
