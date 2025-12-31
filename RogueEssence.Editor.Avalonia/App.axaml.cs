using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using RogueEssence.Dev.ViewModels;
using RogueEssence.Dev.Views;

namespace RogueEssence.Dev
{
    /// <summary>
    /// Main application class for the RogueEssence Editor.
    /// Handles application initialization and lifetime management.
    /// </summary>
    public class App : Application
    {
        /// <summary>
        /// Initializes the application by loading the XAML resources.
        /// </summary>
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// Called when framework initialization is completed. Creates the main DevForm window.
        /// </summary>
        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new DevForm
                {
                    DataContext = new DevFormViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
