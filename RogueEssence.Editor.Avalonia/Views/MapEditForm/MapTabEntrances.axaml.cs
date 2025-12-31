using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RogueEssence.Dev.Views
{
    /// <summary>
    /// View for the Entrances tab in the dungeon map editor.
    /// Provides UI for placing and configuring map entry and exit points.
    /// </summary>
    public class MapTabEntrances : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the MapTabEntrances class.
        /// </summary>
        public MapTabEntrances()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
