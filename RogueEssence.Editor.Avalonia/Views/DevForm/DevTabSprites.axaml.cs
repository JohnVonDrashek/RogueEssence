using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RogueEssence.Dev.Views
{
    /// <summary>
    /// View for the Sprites tab in the developer form.
    /// Provides UI for managing and editing sprite graphics and animations.
    /// </summary>
    public class DevTabSprites : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the DevTabSprites class.
        /// </summary>
        public DevTabSprites()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
