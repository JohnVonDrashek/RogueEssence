using System;
using System.Collections.Generic;
using System.Text;
using RogueEssence;
using RogueEssence.Dungeon;
using RogueEssence.Ground;
using RogueEssence.Data;

namespace RogueEssence.Dev.ViewModels
{
    /// <summary>
    /// Main ViewModel for the Developer Form window.
    /// Contains view models for all development tabs including Game, Player, Data, Travel, Sprites, Script, Mods, and Constants.
    /// </summary>
    public class DevFormViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the DevFormViewModel class with all tab view models.
        /// </summary>
        public DevFormViewModel()
        {
            Game = new DevTabGameViewModel();
            Player = new DevTabPlayerViewModel();
            Data = new DevTabDataViewModel();
            Travel = new DevTabTravelViewModel();
            Sprites = new DevTabSpritesViewModel();
            Script = new DevTabScriptViewModel();
            Mods = new DevTabModsViewModel();
            Constants = new DevTabConstantsViewModel();
        }

        /// <summary>
        /// Gets or sets the Game tab ViewModel for spawning, despawning, and game manipulation.
        /// </summary>
        public DevTabGameViewModel Game { get; set; }

        /// <summary>
        /// Gets or sets the Player tab ViewModel for player character manipulation.
        /// </summary>
        public DevTabPlayerViewModel Player { get; set; }

        /// <summary>
        /// Gets or sets the Data tab ViewModel for editing game data entries.
        /// </summary>
        public DevTabDataViewModel Data { get; set; }

        /// <summary>
        /// Gets or sets the Travel tab ViewModel for zone and dungeon navigation.
        /// </summary>
        public DevTabTravelViewModel Travel { get; set; }

        /// <summary>
        /// Gets or sets the Sprites tab ViewModel for sprite and graphics editing.
        /// </summary>
        public DevTabSpritesViewModel Sprites { get; set; }

        /// <summary>
        /// Gets or sets the Script tab ViewModel for Lua script execution and testing.
        /// </summary>
        public DevTabScriptViewModel Script { get; set; }

        /// <summary>
        /// Gets or sets the Mods tab ViewModel for mod management.
        /// </summary>
        public DevTabModsViewModel Mods { get; set; }

        /// <summary>
        /// Gets or sets the Constants tab ViewModel for editing game constants and effects.
        /// </summary>
        public DevTabConstantsViewModel Constants { get; set; }





    }
}
