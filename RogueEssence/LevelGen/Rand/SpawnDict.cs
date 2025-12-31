// <copyright file="SpawnDict.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Selects an item randomly from a weighted dictionary.
    /// Maps keys to spawnable items with associated spawn rate weights.
    /// </summary>
    /// <typeparam name="TK">The type of key used to identify spawn entries.</typeparam>
    /// <typeparam name="TV">The type of spawnable item.</typeparam>
    [Serializable]
    public class SpawnDict<TK, TV> : IRandPicker<TV>, ISpawnDict<TK, TV>, ISpawnDict
    {
        private readonly Dictionary<TK, SpawnRate> spawns;
        private int spawnTotal;

        /// <summary>
        /// Initializes a new instance of the SpawnDict class.
        /// </summary>
        public SpawnDict()
        {
            this.spawns = new Dictionary<TK, SpawnRate>();
        }

        /// <summary>
        /// Initializes a new instance of the SpawnDict class as a copy of another.
        /// </summary>
        /// <param name="other">The SpawnDict to copy.</param>
        protected SpawnDict(SpawnDict<TK, TV> other)
        {
            this.spawns = new Dictionary<TK, SpawnRate>();

            foreach (TK key in other.spawns.Keys)
                this.spawns.Add(key, new SpawnRate(other.spawns[key].Spawn, other.spawns[key].Rate));
        }

        /// <summary>
        /// Gets the number of spawn entries in the dictionary.
        /// </summary>
        public int Count => this.spawns.Count;

        /// <summary>
        /// Gets the total of all spawn rates.
        /// </summary>
        public int SpawnTotal => this.spawnTotal;

        /// <summary>
        /// Gets a value indicating whether this picker can pick an item.
        /// Returns true if the spawn total is greater than 0.
        /// </summary>
        public bool CanPick => this.spawnTotal > 0;

        /// <summary>
        /// Gets a value indicating whether this picker changes state when picking.
        /// Always returns false.
        /// </summary>
        public bool ChangesState => false;

        /// <summary>
        /// Creates a copy of this picker's state.
        /// </summary>
        /// <returns>A new SpawnDict with copied entries.</returns>
        public IRandPicker<TV> CopyState() => new SpawnDict<TK, TV>(this);

        /// <summary>
        /// Adds a new spawn entry with the specified key, spawn item, and rate.
        /// </summary>
        /// <param name="key">The key to identify the entry.</param>
        /// <param name="spawn">The item to spawn.</param>
        /// <param name="rate">The spawn rate weight (must be 0 or higher).</param>
        /// <exception cref="ArgumentException">Thrown when rate is negative.</exception>
        public void Add(TK key, TV spawn, int rate)
        {
            if (rate < 0)
                throw new ArgumentException("Spawn rate must be 0 or higher.");
            this.spawns.Add(key, new SpawnRate(spawn, rate));
            this.spawnTotal += rate;
        }

        /// <summary>
        /// Removes all entries from the dictionary.
        /// </summary>
        public void Clear()
        {
            this.spawns.Clear();
            this.spawnTotal = 0;
        }

        /// <summary>
        /// Enumerates all possible spawn outcomes.
        /// </summary>
        /// <returns>An enumerable of all spawnable items.</returns>
        public IEnumerable<TV> EnumerateOutcomes()
        {
            foreach (SpawnRate element in this.spawns.Values)
                yield return element.Spawn;
        }

        /// <summary>
        /// Gets an enumerable of all keys in the dictionary.
        /// </summary>
        /// <returns>An enumerable of keys.</returns>
        public IEnumerable<TK> GetKeys()
        {
            foreach (TK key in this.spawns.Keys)
                yield return key;
        }

        IEnumerable<TK> ISpawnDict<TK, TV>.GetKeys()
        {
            return GetKeys();
        }

        IEnumerable ISpawnDict.GetKeys()
        {
            return GetKeys();
        }

        /// <summary>
        /// Picks a random spawn item based on weighted rates.
        /// </summary>
        /// <param name="random">The random number generator to use.</param>
        /// <returns>A randomly selected spawn item.</returns>
        /// <exception cref="InvalidOperationException">Thrown when spawn total is 0.</exception>
        public TV Pick(IRandom random)
        {
            TK key = this.PickKey(random);
            return this.spawns[key].Spawn;
        }

        /// <summary>
        /// Picks a random key based on weighted rates.
        /// </summary>
        /// <param name="random">The random number generator to use.</param>
        /// <returns>A randomly selected key.</returns>
        /// <exception cref="InvalidOperationException">Thrown when spawn total is 0.</exception>
        public TK PickKey(IRandom random)
        {
            if (this.spawnTotal > 0)
            {
                int rand = random.Next(this.spawnTotal);
                int total = 0;
                foreach (TK key in this.spawns.Keys)
                {
                    total += this.spawns[key].Rate;
                    if (rand < total)
                        return key;
                }
            }

            throw new InvalidOperationException("Cannot spawn from a SpawnDict of total rate 0!");
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
        /// Sets the spawn item for the specified key.
        /// </summary>
        /// <param name="key">The key to update.</param>
        /// <param name="spawn">The new spawn item.</param>
        public void SetSpawn(TK key, TV spawn)
        {
            this.spawns[key] = new SpawnRate(spawn, this.spawns[key].Rate);
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
            this.spawnTotal = this.spawnTotal - this.spawns[key].Rate + rate;
            this.spawns[key] = new SpawnRate(this.spawns[key].Spawn, rate);
        }

        /// <summary>
        /// Removes the entry with the specified key.
        /// </summary>
        /// <param name="key">The key to remove.</param>
        public void Remove(TK key)
        {
            this.spawnTotal -= this.spawns[key].Rate;
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

        public override bool Equals(object obj)
        {
            if (!(obj is SpawnDict<TK, TV> other))
                return false;
            if (this.spawns.Count != other.spawns.Count)
                return false;
            foreach (TK key in this.spawns.Keys)
            {
                if (!other.spawns.ContainsKey(key))
                    return false;
                if (!this.spawns[key].Spawn.Equals(other.spawns[key].Spawn))
                    return false;
                if (this.spawns[key].Rate != other.spawns[key].Rate)
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            int code = 0;
            foreach (TK key in this.spawns.Keys)
                code ^= this.spawns[key].Spawn.GetHashCode() ^ key.GetHashCode() ^ this.spawns[key].Rate;
            return code;
        }

        void ISpawnDict.Add(object key, object spawn, int rate)
        {
            this.Add((TK)key, (TV)spawn, rate);
        }

        object ISpawnDict.GetSpawn(object key)
        {
            return this.GetSpawn((TK)key);
        }

        void ISpawnDict.SetSpawn(object key, object spawn)
        {
            this.SetSpawn((TK)key, (TV)spawn);
        }

        int ISpawnDict.GetSpawnRate(object key)
        {
            return this.GetSpawnRate((TK)key);
        }

        void ISpawnDict.SetSpawnRate(object key, int rate)
        {
            this.SetSpawnRate((TK)key, rate);
        }

        public void Remove(object key)
        {
            this.Remove((TK)key);
        }

        bool ISpawnDict.Contains(object key)
        {
            return this.ContainsKey((TK)key);
        }

        [Serializable]
        private struct SpawnRate
        {
            public TV Spawn;
            public int Rate;

            public SpawnRate(TV item, int rate)
            {
                this.Spawn = item;
                this.Rate = rate;
            }
        }
    }
}
