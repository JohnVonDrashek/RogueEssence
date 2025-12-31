using RogueEssence.Dev;
using RogueEssence.Dungeon;
using System;
using System.Collections.Generic;

namespace RogueEssence.Data
{
    /// <summary>
    /// Parameters for starting a new game, including starting characters and location.
    /// </summary>
    [Serializable]
    public class StartParams
    {
        /// <summary>
        /// The list of starting characters for the player.
        /// </summary>
        public List<StartChar> Chars;

        /// <summary>
        /// The personality value for the starting character.
        /// </summary>
        public int Personality;

        /// <summary>
        /// The starting map location.
        /// </summary>
        public ZoneLoc Map;

        /// <summary>
        /// The starting level for characters.
        /// </summary>
        public int Level;

        /// <summary>
        /// The maximum level characters can reach.
        /// </summary>
        public int MaxLevel;

        /// <summary>
        /// List of team names/identifiers.
        /// </summary>
        public List<string> Teams;
    }


    /// <summary>
    /// Represents a starting character configuration.
    /// </summary>
    [Serializable]
    public class StartChar
    {
        /// <summary>
        /// The monster ID including species, form, skin, and gender.
        /// </summary>
        [MonsterID(0, false, false, true, true)]
        public MonsterID ID;

        /// <summary>
        /// The custom name for this starting character.
        /// </summary>
        public string Name;

        /// <summary>
        /// Initializes a new instance of the StartChar class with default values.
        /// </summary>
        public StartChar()
        {
            Name = "";
        }

        /// <summary>
        /// Initializes a new instance of the StartChar class with the specified ID and name.
        /// </summary>
        /// <param name="id">The monster ID for this character.</param>
        /// <param name="name">The custom name for this character.</param>
        public StartChar(MonsterID id, string name)
        {
            ID = id;
            Name = name;
        }

        /// <summary>
        /// Returns the string representation of the monster ID.
        /// </summary>
        /// <returns>The monster ID as a string.</returns>
        public override string ToString()
        {
            return ID.ToString();
        }
    }
}
