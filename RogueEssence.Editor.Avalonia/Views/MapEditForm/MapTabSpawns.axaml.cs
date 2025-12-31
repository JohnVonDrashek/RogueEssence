using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RogueEssence.Dev.Views
{
    /// <summary>
    /// View for the Spawns tab in the dungeon map editor.
    /// Provides UI for configuring spawn points and spawn rules.
    /// </summary>
    public class MapTabSpawns : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the MapTabSpawns class.
        /// </summary>
        public MapTabSpawns()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
