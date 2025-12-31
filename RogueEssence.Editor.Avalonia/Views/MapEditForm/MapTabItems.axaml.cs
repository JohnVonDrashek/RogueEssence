using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RogueEssence.Dev.Views
{
    /// <summary>
    /// View for the Items tab in the dungeon map editor.
    /// Provides UI for placing and configuring item pickups on the map.
    /// </summary>
    public class MapTabItems : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the MapTabItems class.
        /// </summary>
        public MapTabItems()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
