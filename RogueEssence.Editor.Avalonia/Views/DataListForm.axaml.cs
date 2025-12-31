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
    /// Form window for displaying and managing lists of data entries.
    /// Provides a searchable list interface for game data editing.
    /// </summary>
    public class DataListForm : Window
    {
        /// <summary>
        /// Initializes a new instance of the DataListForm class.
        /// </summary>
        public DataListForm()
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
        /// Sets the context menu for the items list box.
        /// </summary>
        /// <param name="menu">The context menu to assign.</param>
        public void SetListContextMenu(ContextMenu menu)
        {
            SearchListBox lbx = this.FindControl<SearchListBox>("lbxItems");
            lbx.ContextMenu = menu;
        }
    }
}
