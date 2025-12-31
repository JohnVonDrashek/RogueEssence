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
    /// Form window for editing localized string resources.
    /// Provides interface for managing game text and translations.
    /// </summary>
    public class StringsEditForm : Window
    {
        /// <summary>
        /// Initializes a new instance of the StringsEditForm class.
        /// </summary>
        public StringsEditForm()
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
