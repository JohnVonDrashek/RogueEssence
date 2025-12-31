using RogueElements;
using RogueEssence.Dungeon;
using System.Collections.Generic;

namespace RogueEssence.LevelGen
{
    /// <summary>
    /// Interface for spawners that generate multiple teams at once.
    /// </summary>
    /// <typeparam name="T">The type of generation context.</typeparam>
    public interface IMultiTeamSpawner<T> : IMultiTeamStepSpawner where T : IGenContext
    {
        /// <summary>
        /// Gets the spawned teams from this spawner.
        /// </summary>
        /// <param name="map">The generation context.</param>
        /// <returns>A list of spawned teams.</returns>
        List<Team> GetSpawns(T map);
    }

    /// <summary>
    /// Marker interface for multi-team step spawners.
    /// </summary>
    public interface IMultiTeamStepSpawner
    {

    }
}
