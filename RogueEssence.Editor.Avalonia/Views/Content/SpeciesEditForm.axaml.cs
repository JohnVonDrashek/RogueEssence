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

namespace RogueEssence.Dev.Views
{
    /// <summary>
    /// Form window for editing species/character sprite assets.
    /// Provides tools for managing character sprites and animations.
    /// </summary>
    public class SpeciesEditForm : Window
    {
        /// <summary>
        /// Initializes a new instance of the SpeciesEditForm class.
        /// </summary>
        public SpeciesEditForm()
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
