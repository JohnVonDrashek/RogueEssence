using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RogueEssence.Dev.Views
{
    /// <summary>
    /// View for the Travel tab in the developer form.
    /// Provides UI for navigating between zones and dungeons during development.
    /// </summary>
    public class DevTabTravel : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the DevTabTravel class.
        /// </summary>
        public DevTabTravel()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }


    }
}
