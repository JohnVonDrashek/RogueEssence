using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RogueEssence.Dev.Views
{
    /// <summary>
    /// View for the Decorations tab in the dungeon map editor.
    /// Provides UI for placing and editing decorative tile elements.
    /// </summary>
    public class MapTabDecorations : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the MapTabDecorations class.
        /// </summary>
        public MapTabDecorations()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
