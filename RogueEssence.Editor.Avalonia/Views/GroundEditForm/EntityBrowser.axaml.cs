using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using RogueElements;
using RogueEssence.Dev.ViewModels;

namespace RogueEssence.Dev.Views
{
    /// <summary>
    /// User control for browsing and selecting entities in the ground editor.
    /// Provides a visual interface for entity placement and selection.
    /// </summary>
    public class EntityBrowser : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the EntityBrowser class.
        /// </summary>
        public EntityBrowser()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

    }
}
