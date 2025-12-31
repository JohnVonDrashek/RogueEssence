using System;
using RogueEssence.Dungeon;
using RogueEssence.Data;
using RogueElements;

namespace RogueEssence.LevelGen
{
    /// <summary>
    /// Abstract base class for conditions that determine whether a mob can spawn.
    /// Implementations define specific criteria that must be met for spawning.
    /// </summary>
    [Serializable]
    public abstract class MobSpawnCheck
    {
        /// <summary>
        /// Creates a copy of this spawn check.
        /// </summary>
        /// <returns>A new MobSpawnCheck with copied data.</returns>
        public abstract MobSpawnCheck Copy();

        /// <summary>
        /// Determines whether the spawn condition is satisfied.
        /// </summary>
        /// <returns>True if spawning is allowed; otherwise, false.</returns>
        public abstract bool CanSpawn();
    }

}
