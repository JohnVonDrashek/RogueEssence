using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RogueEssence.Dev.Views
{
    /// <summary>
    /// View for the Game tab in the developer form.
    /// Provides UI for game runtime controls like spawning entities and toggling visibility.
    /// </summary>
    public class DevTabGame : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the DevTabGame class.
        /// </summary>
        public DevTabGame()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
