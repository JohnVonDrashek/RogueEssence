using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RogueEssence.Dev.Views
{
    /// <summary>
    /// View for the Effects tab in the dungeon map editor.
    /// Provides UI for configuring map-wide effects and tile effects.
    /// </summary>
    public class MapTabEffects : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the MapTabEffects class.
        /// </summary>
        public MapTabEffects()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
