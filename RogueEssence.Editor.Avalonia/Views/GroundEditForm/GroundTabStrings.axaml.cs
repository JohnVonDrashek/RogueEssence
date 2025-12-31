using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RogueEssence.Dev.Views
{
    /// <summary>
    /// View for the Strings tab in the ground editor.
    /// Provides UI for managing localized text strings for ground maps.
    /// </summary>
    public class GroundTabStrings : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the GroundTabStrings class.
        /// </summary>
        public GroundTabStrings()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
