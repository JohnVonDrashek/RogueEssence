using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Input;
using RogueEssence.Dev.ViewModels;

namespace RogueEssence.Dev.Views
{
    /// <summary>
    /// View for the Mods tab in the developer form.
    /// Provides UI for managing game modifications and mod configuration.
    /// </summary>
    public class DevTabMods : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the DevTabMods class.
        /// </summary>
        public DevTabMods()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

    }
}
