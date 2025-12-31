using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RogueEssence.Dev.Views
{
    /// <summary>
    /// View for the Data tab in the developer form.
    /// Provides UI for editing various game data types like monsters, items, and skills.
    /// </summary>
    public class DevTabData : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the DevTabData class.
        /// </summary>
        public DevTabData()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
