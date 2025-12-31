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
    /// Form window for editing animation assets.
    /// Provides preview and editing capabilities for game animations.
    /// </summary>
    public class AnimEditForm : Window
    {
        /// <summary>
        /// Initializes a new instance of the AnimEditForm class.
        /// </summary>
        public AnimEditForm()
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


        /// <summary>
        /// Handles the window closed event, clearing debug animation state.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        public void Window_Closed(object sender, EventArgs e)
        {
            lock (GameBase.lockObj)
            {
                if (DungeonScene.Instance != null)
                {
                    DungeonScene.Instance.DebugAsset = RogueEssence.Content.GraphicsManager.AssetType.None;
                    DungeonScene.Instance.DebugAnim = "";
                }
            }
        }
    }
}
