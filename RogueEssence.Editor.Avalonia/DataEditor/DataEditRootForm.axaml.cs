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
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace RogueEssence.Dev.Views
{
    /// <summary>
    /// A root data editor form that extends DataEditForm with Apply functionality.
    /// Used as the top-level editor window for editing game data.
    /// </summary>
    public class DataEditRootForm : DataEditForm
    {
        /// <summary>
        /// Loads the XAML component for this form.
        /// </summary>
        protected override void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// Handles the Apply button click event. Saves children and invokes the OK event handler without closing.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The routed event arguments.</param>
        public async void btnApply_Click(object sender, RoutedEventArgs e)
        {
            await SaveChildren();
            if (SelectedOKEvent != null)
                await SelectedOKEvent.Invoke();
        }

        /// <summary>
        /// Handles the window closing event. Saves editor settings before closing.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The cancel event arguments.</param>
        public override async void Window_Closing(object sender, CancelEventArgs e)
        {
            base.Window_Closing(sender, e);

            DevDataManager.SaveEditorSettings();
        }
    }
}
