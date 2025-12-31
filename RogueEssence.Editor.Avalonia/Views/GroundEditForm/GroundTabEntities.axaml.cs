using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RogueEssence.Dev.Views
{
    /// <summary>
    /// View for the Entities tab in the ground editor.
    /// Provides UI for placing and editing entities like NPCs and objects on ground maps.
    /// </summary>
    public class GroundTabEntities : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the GroundTabEntities class.
        /// </summary>
        public GroundTabEntities()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
