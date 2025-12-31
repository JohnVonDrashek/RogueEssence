// <copyright file="SpawnRangeDict.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// A data structure representing spawn rates of items spread across a range of floors.
    /// Maps keys to spawnable items with associated spawn rates and floor ranges.
    /// </summary>
    /// <typeparam name="TK">The type of key used to identify spawn entries.</typeparam>
    /// <typeparam name="TV">The type of spawnable item.</typeparam>
    // TODO: Binary Space Partition Tree
    [Serializable]
    public class SpawnRangeDict<TK, TV> : ISpawnRangeDict<TK, TV>, ISpawnRangeDict
    {
        private readonly Dictionary<TK, SpawnRange> spawns;

        /// <summary>
        /// Initializes a new instance of the SpawnRangeDict class.
        /// </summary>
        public SpawnRangeDict()
        {
            this.spawns = new Dictionary<TK, SpawnRange>();
        }

        /// <summary>
        /// Gets the number of spawn entries in the dictionary.
        /// </summary>
        public int Count => this.spawns.Count;

        /// <summary>
        /// Adds a new spawn entry with the specified key, spawn item, floor range, and rate.
        /// </summary>
        /// <param name="key">The key to identify the entry.</param>
        /// <param name="spawn">The item to spawn.</param>
        /// <param name="range">The floor range where this item can spawn.</param>
        /// <param name="rate">The spawn rate weight.</param>
        /// <exception cref="ArgumentException">Thrown when rate is negative or range length is 0 or less.</exception>
        public void Add(TK key, TV spawn, IntRange range, int rate)
        {
            if (rate < 0)
                throw new ArgumentException("Spawn rate must be 0 or higher.");
            if (range.Length <= 0)
                throw new ArgumentException("Spawn range must be 1 or higher.");
            this.spawns.Add(key, new SpawnRange(spawn, rate, range));
        }

        /// <summary>
        /// Removes all entries from the dictionary.
        /// </summary>
        public void Clear()
        {
            this.spawns.Clear();
        }

        /// <summary>
        /// Gets an enumerator for all spawnable items.
        /// </summary>
        /// <returns>An enumerator of spawnable items.</returns>
        public IEnumerator<TV> GetEnumerator()
        {
            foreach (SpawnRange spawn in this.spawns.Values)
                yield return spawn.Spawn;
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <summary>
        /// Gets an enumerable of all keys in the dictionary.
        /// </summary>
        /// <returns>An enumerable of keys.</returns>
        public IEnumerable<TK> GetKeys()
        {
            foreach (TK key in this.spawns.Keys)
                yield return key;
        }

        /// <summary>
        /// Gets a SpawnDict containing only items available at the specified level.
        /// </summary>
        /// <param name="level">The floor level to filter by.</param>
        /// <returns>A SpawnDict with items that can spawn at the level.</returns>
        public SpawnDict<TK, TV> GetSpawnList(int level)
        {
            SpawnDict<TK, TV> newList = new SpawnDict<TK, TV>();
            foreach (TK key in this.spawns.Keys)
            {
                SpawnRange spawn = this.spawns[key];
                if (spawn.Range.Min <= level && level < spawn.Range.Max)
                    newList.Add(key, spawn.Spawn, spawn.Rate);
            }

            return newList;
        }

        /// <summary>
        /// Determines whether any item can be picked at the specified level.
        /// </summary>
        /// <param name="level">The floor level to check.</param>
        /// <returns>True if at least one item is available at the level; otherwise, false.</returns>
        public bool CanPick(int level)
        {
            foreach (TK key in this.spawns.Keys)
            {
                SpawnRange spawn = this.spawns[key];
                if (spawn.Range.Min <= level && level < spawn.Range.Max && spawn.Rate > 0)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Picks a random spawn item based on weighted rates for the specified level.
        /// </summary>
        /// <param name="random">The random number generator to use.</param>
        /// <param name="level">The floor level to pick from.</param>
        /// <returns>A randomly selected spawn item available at the level.</returns>
        /// <exception cref="InvalidOperationException">Thrown when no items are available at the level.</exception>
        public TV Pick(IRandom random, int level)
        {
            int spawnTotal = 0;
            List<SpawnRange> spawns = new List<SpawnRange>();
            foreach (SpawnRange spawn in this.GetLevelSpawns(level))
            {
                spawns.Add(spawn);
                spawnTotal += spawn.Rate;
            }

            if (spawnTotal > 0)
            {
                int rand = random.Next(spawnTotal);
                int total = 0;
                for (int ii = 0; ii < spawns.Count; ii++)
                {
                    total += spawns[ii].Rate;
                    if (rand < total)
                        return spawns[ii].Spawn;
                }
            }

            throw new InvalidOperationException("Cannot spawn from a spawnlist of total rate 0!");
        }

        /// <summary>
        /// Gets the spawn item associated with the specified key.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <returns>The spawn item.</returns>
        public TV GetSpawn(TK key)
        {
            return this.spawns[key].Spawn;
        }

        /// <summary>
        /// Gets the spawn rate for the specified key.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <returns>The spawn rate.</returns>
        public int GetSpawnRate(TK key)
        {
            return this.spawns[key].Rate;
        }

        /// <summary>
        /// Gets the floor range for the specified key.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <returns>The floor range.</returns>
        public IntRange GetSpawnRange(TK key)
        {
            return this.spawns[key].Range;
        }

        /// <summary>
        /// Sets the spawn item for the specified key.
        /// </summary>
        /// <param name="key">The key to update.</param>
        /// <param name="spawn">The new spawn item.</param>
        public void SetSpawn(TK key, TV spawn)
        {
            this.spawns[key] = new SpawnRange(spawn, this.spawns[key].Rate, this.spawns[key].Range);
        }

        /// <summary>
        /// Sets the spawn rate for the specified key.
        /// </summary>
        /// <param name="key">The key to update.</param>
        /// <param name="rate">The new spawn rate.</param>
        /// <exception cref="ArgumentException">Thrown when rate is negative.</exception>
        public void SetSpawnRate(TK key, int rate)
        {
            if (rate < 0)
                throw new ArgumentException("Spawn rate must be 0 or higher.");
            this.spawns[key] = new SpawnRange(this.spawns[key].Spawn, rate, this.spawns[key].Range);
        }

        /// <summary>
        /// Sets the floor range for the specified key.
        /// </summary>
        /// <param name="key">The key to update.</param>
        /// <param name="range">The new floor range.</param>
        public void SetSpawnRange(TK key, IntRange range)
        {
            this.spawns[key] = new SpawnRange(this.spawns[key].Spawn, this.spawns[key].Rate, range);
        }

        /// <summary>
        /// Removes the entry with the specified key.
        /// </summary>
        /// <param name="key">The key to remove.</param>
        public void Remove(TK key)
        {
            this.spawns.Remove(key);
        }

        /// <summary>
        /// Determines whether the dictionary contains the specified key.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the key exists; otherwise, false.</returns>
        public bool ContainsKey(TK key)
        {
            return this.spawns.ContainsKey(key);
        }

        void ISpawnRangeDict.Add(object key, object spawn, IntRange range, int rate)
        {
            this.Add((TK)key, (TV)spawn, range, rate);
        }

        object ISpawnRangeDict.GetSpawn(object key)
        {
            return this.GetSpawn((TK)key);
        }

        int ISpawnRangeDict.GetSpawnRate(object key)
        {
            return this.GetSpawnRate((TK)key);
        }

        IntRange ISpawnRangeDict.GetSpawnRange(object key)
        {
            return this.GetSpawnRange((TK)key);
        }

        void ISpawnRangeDict.SetSpawn(object key, object spawn)
        {
            this.SetSpawn((TK)key, (TV)spawn);
        }

        void ISpawnRangeDict.SetSpawnRate(object key, int rate)
        {
            this.SetSpawnRate((TK)key, rate);
        }

        void ISpawnRangeDict.SetSpawnRange(object key, IntRange range)
        {
            this.SetSpawnRange((TK)key, range);
        }

        void ISpawnRangeDict.Remove(object key)
        {
            this.Remove((TK)key);
        }

        bool ISpawnRangeDict.Contains(object key)
        {
            return this.spawns.ContainsKey((TK)key);
        }

        private IEnumerable<SpawnRange> GetLevelSpawns(int level)
        {
            foreach (TK key in this.spawns.Keys)
            {
                SpawnRange spawn = this.spawns[key];
                if (spawn.Range.Min <= level && level < spawn.Range.Max)
                    yield return spawn;
            }
        }

        [Serializable]
        private struct SpawnRange
        {
            public TV Spawn;
            public int Rate;
            public IntRange Range;

            public SpawnRange(TV item, int rate, IntRange range)
            {
                this.Spawn = item;
                this.Rate = rate;
                this.Range = range;
            }
        }
    }
}
