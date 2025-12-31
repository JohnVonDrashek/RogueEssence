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
    /// ViewModel for the map retile dialog.
    /// Allows users to change the tile size of a map (must be a multiple of 8).
    /// </summary>
    public class MapRetileViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MapRetileViewModel class.
        /// </summary>
        /// <param name="origSize">The original tile size.</param>
        /// <param name="msg">The message to display to the user.</param>
        public MapRetileViewModel(int origSize, string msg)
        {
            TileSize = origSize;
            Message = msg;
        }

        private int tileSize;
        public int TileSize
        {
            get { return tileSize; }
            set
            {
                this.SetIfChanged(ref tileSize, value);
                AllowConfirm = (tileSize % 8 == 0);
            }
        }

        private bool allowConfirm;
        public bool AllowConfirm
        {
            get { return allowConfirm; }
            set
            {
                this.SetIfChanged(ref allowConfirm, value);
            }
        }


        private string message;
        public string Message
        {
            get { return message; }
            set
            {
                this.SetIfChanged(ref message, value);
            }
        }
    }
}
