using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;

namespace RogueEssence.Dev
{
    /// <summary>
    /// Entry point for the RogueEssence Editor application.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main entry point for the application. Don't use any Avalonia, third-party APIs or any
        /// SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        /// yet and stuff might break.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(string[] args)
        {
            AppBuilder builder = BuildAvaloniaApp();
            builder.StartWithClassicDesktopLifetime(args);
        }

        /// <summary>
        /// Avalonia configuration, don't remove; also used by visual designer.
        /// </summary>
        /// <returns>The configured AppBuilder instance.</returns>
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();
    }
}
