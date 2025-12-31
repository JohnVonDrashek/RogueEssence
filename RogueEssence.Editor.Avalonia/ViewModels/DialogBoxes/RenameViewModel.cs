using System;
using System.Collections.Generic;
using System.Text;
using RogueEssence;
using RogueEssence.Dungeon;
using RogueEssence.Ground;
using RogueEssence.Data;
using ReactiveUI;

namespace RogueEssence.Dev.ViewModels
{
    /// <summary>
    /// ViewModel for the rename dialog.
    /// Provides a simple text input for renaming items.
    /// </summary>
    public class RenameViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the RenameViewModel class.
        /// </summary>
        public RenameViewModel()
        {
            Name = "";
        }

        private string name;
        public string Name
        {
            get => name;
            set => this.SetIfChanged(ref name, value);
        }

    }
}
