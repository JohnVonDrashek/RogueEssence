using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RogueEssence.Dev.Views
{
    /// <summary>
    /// View for the Properties tab in the dungeon map editor.
    /// Provides UI for editing map metadata like name, music, and settings.
    /// </summary>
    public class MapTabProperties : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the MapTabProperties class.
        /// </summary>
        public MapTabProperties()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
