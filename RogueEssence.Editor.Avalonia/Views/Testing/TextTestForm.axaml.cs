using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Input;
using Avalonia.Interactivity;
using System;
using RogueEssence;
using RogueEssence.Dev;
using Microsoft.Xna.Framework;
using Avalonia.Threading;
using System.Threading;
using RogueEssence.Dungeon;

namespace RogueEssence.Dev.Views
{
    /// <summary>
    /// Form window for testing text rendering and display.
    /// Used for development and debugging of text-related features.
    /// </summary>
    public class TextTestForm : Window
    {
        /// <summary>
        /// Initializes a new instance of the TextTestForm class.
        /// </summary>
        public TextTestForm()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

    }
}
