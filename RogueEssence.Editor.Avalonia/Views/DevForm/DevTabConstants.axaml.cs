using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RogueEssence.Dev.Views
{
    /// <summary>
    /// View for the Constants tab in the developer form.
    /// Provides UI for editing game constants and effects.
    /// </summary>
    public class DevTabConstants : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the DevTabConstants class.
        /// </summary>
        public DevTabConstants()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
