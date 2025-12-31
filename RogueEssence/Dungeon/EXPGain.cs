using System;

namespace RogueEssence.Dungeon
{
    /// <summary>
    /// Represents experience points gained from defeating a monster in the dungeon.
    /// </summary>
    [Serializable]
    public struct EXPGain
    {
        /// <summary>
        /// The identifier of the monster that was defeated.
        /// </summary>
        public MonsterID SlainMonster;

        /// <summary>
        /// The level of the defeated monster, used for experience calculation.
        /// </summary>
        public int Level;

        /// <summary>
        /// Initializes a new instance of the EXPGain struct.
        /// </summary>
        /// <param name="slain">The identifier of the slain monster.</param>
        /// <param name="level">The level of the slain monster.</param>
        public EXPGain(MonsterID slain, int level)
        {
            SlainMonster = slain;
            Level = level;
        }
    }
}
