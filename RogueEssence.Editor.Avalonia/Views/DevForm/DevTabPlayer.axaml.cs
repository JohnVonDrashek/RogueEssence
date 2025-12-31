using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RogueEssence.Dev.Views
{
    /// <summary>
    /// View for the Player tab in the developer form.
    /// Provides UI for player character manipulation including species, forms, and animations.
    /// </summary>
    public class DevTabPlayer : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the DevTabPlayer class.
        /// </summary>
        public DevTabPlayer()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }


    }
}
