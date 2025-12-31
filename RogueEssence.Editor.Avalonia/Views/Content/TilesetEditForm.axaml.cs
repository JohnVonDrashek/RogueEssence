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
    /// Form window for editing tileset assets.
    /// Provides tools for managing and previewing tile graphics.
    /// </summary>
    public class TilesetEditForm : Window
    {
        /// <summary>
        /// Initializes a new instance of the TilesetEditForm class.
        /// </summary>
        public TilesetEditForm()
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
