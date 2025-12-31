using System;
using RogueEssence.Dungeon;
using RogueElements;
using System.Collections.Generic;

namespace RogueEssence.LevelGen
{
    /// <summary>
    /// Interface for generators that spawn teams from a map context.
    /// </summary>
    /// <typeparam name="T">The type of map context.</typeparam>
    public interface ITeamSpawnGenerator<T>
    {
        /// <summary>
        /// Spawns a team on the specified map.
        /// </summary>
        /// <param name="map">The map context for spawning.</param>
        /// <returns>The spawned team.</returns>
        Team Spawn(T map);
    }

    /// <summary>
    /// Abstract base class for spawners that create teams of mobs.
    /// Handles the logic for choosing mob spawns and creating the team.
    /// </summary>
    [Serializable]
    public abstract class TeamSpawner : ITeamSpawnGenerator<IMobSpawnMap>
    {
        /// <summary>
        /// Gets or sets whether the spawned team is an explorer team (ally) or monster team (enemy).
        /// </summary>
        public bool Explorer { get; set; }

        /// <summary>
        /// Gets all possible mob spawns that could be chosen.
        /// </summary>
        /// <returns>A spawn list of possible mob spawns.</returns>
        public abstract SpawnList<MobSpawn> GetPossibleSpawns();

        /// <summary>
        /// Chooses which mobs to spawn based on the random generator.
        /// </summary>
        /// <param name="rand">The random number generator.</param>
        /// <returns>A list of chosen mob spawns.</returns>
        public abstract List<MobSpawn> ChooseSpawns(IRandom rand);

        /// <summary>
        /// Spawns a team with the chosen mobs.
        /// </summary>
        /// <param name="map">The map context for spawning.</param>
        /// <returns>The spawned team, or null if no mobs were chosen.</returns>
        public Team Spawn(IMobSpawnMap map)
        {
            List<MobSpawn> chosenSpawns = ChooseSpawns(map.Rand);

            if (chosenSpawns.Count > 0)
            {
                Team team;
                if (Explorer)
                    team = new ExplorerTeam();
                else
                    team = new MonsterTeam();
                foreach (MobSpawn chosenSpawn in chosenSpawns)
                    chosenSpawn.Spawn(team, map);
                return team;
            }
            else
                return null;
        }

        /// <summary>
        /// Creates a clone of this team spawner.
        /// </summary>
        /// <returns>A new TeamSpawner with the same configuration.</returns>
        public abstract TeamSpawner Clone();
    }
}
