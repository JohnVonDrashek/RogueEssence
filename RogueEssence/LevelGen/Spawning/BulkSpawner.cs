using RogueElements;
using System;
using System.Collections.Generic;

namespace RogueEssence.LevelGen
{
    /// <summary>
    /// Generates spawnable objects using a spawnlist and a regular list.
    /// The normal list is for choosing specific objects that are ALWAYS spawned.
    /// The spawnlist is for choosing several items randomly.
    /// </summary>
    /// <typeparam name="TGenContext"></typeparam>
    /// <typeparam name="TSpawnable"></typeparam>
    [Serializable]
    public class BulkSpawner<TGenContext, TSpawnable> : IStepSpawner<TGenContext, TSpawnable>
        where TGenContext : IGenContext
        where TSpawnable : ISpawnable
    {
        /// <summary>
        /// Objects that are always spawned.
        /// </summary>
        public List<TSpawnable> SpecificSpawns;

        /// <summary>
        /// An encounter/loot table of random spawnable objects.
        /// </summary>
        public SpawnList<TSpawnable> RandomSpawns;

        /// <summary>
        /// The number of objects to roll from Random Spawns.
        /// </summary>
        public int SpawnAmount;

        /// <summary>
        /// Initializes a new instance of the BulkSpawner class.
        /// </summary>
        public BulkSpawner()
        {
            SpecificSpawns = new List<TSpawnable>();
            RandomSpawns = new SpawnList<TSpawnable>();
        }

        /// <summary>
        /// Initializes a new instance of the BulkSpawner class as a copy of another.
        /// </summary>
        /// <param name="other">The BulkSpawner to copy.</param>
        protected BulkSpawner(BulkSpawner<TGenContext, TSpawnable> other) : this()
        {
            foreach (TSpawnable specificSpawn in other.SpecificSpawns)
                SpecificSpawns.Add((TSpawnable)specificSpawn.Copy());
            SpawnAmount = other.SpawnAmount;
            for (int ii = 0; ii < other.RandomSpawns.Count; ii++)
                RandomSpawns.Add((TSpawnable)other.RandomSpawns.GetSpawn(ii).Copy(), other.RandomSpawns.GetSpawnRate(ii));
        }

        /// <summary>
        /// Creates a copy of this BulkSpawner.
        /// </summary>
        /// <returns>A new BulkSpawner with copied data.</returns>
        public BulkSpawner<TGenContext, TSpawnable> Copy() { return new BulkSpawner<TGenContext, TSpawnable>(this); }

        /// <summary>
        /// Gets all spawns including both specific spawns and randomly rolled spawns.
        /// </summary>
        /// <param name="map">The generation context.</param>
        /// <returns>A list of spawned objects.</returns>
        public List<TSpawnable> GetSpawns(TGenContext map)
        {
            List<TSpawnable> spawns = new List<TSpawnable>();
            foreach (TSpawnable element in SpecificSpawns)
                spawns.Add(element);
            for (int ii = 0; ii < SpawnAmount; ii++)
                spawns.Add(RandomSpawns.Pick(map.Rand));
            
            return spawns;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1} + {2}x{3}", this.GetType().GetFormattedTypeName(), this.SpecificSpawns.ToString(), this.SpawnAmount.ToString(), this.RandomSpawns.ToString());
        }
    }
}
