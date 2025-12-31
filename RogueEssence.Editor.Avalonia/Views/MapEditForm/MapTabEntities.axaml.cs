using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RogueEssence.Dev.Views
{
    /// <summary>
    /// View for the Entities tab in the dungeon map editor.
    /// Provides UI for placing and configuring entities like enemies and NPCs.
    /// </summary>
    public class MapTabEntities : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the MapTabEntities class.
        /// </summary>
        public MapTabEntities()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
