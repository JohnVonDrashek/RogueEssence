// <copyright file="ISpawnDict.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Generic interface for a dictionary that maps keys to spawnable items with associated spawn rates.
    /// </summary>
    /// <typeparam name="TK">The type of key.</typeparam>
    /// <typeparam name="TV">The type of spawnable item.</typeparam>
    public interface ISpawnDict<TK, TV>
    {
        /// <summary>
        /// Gets the number of spawn entries in the dictionary.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets the total of all spawn rates.
        /// </summary>
        int SpawnTotal { get; }

        /// <summary>
        /// Adds a new spawn entry with the specified key, spawn item, and rate.
        /// </summary>
        /// <param name="key">The key to identify the entry.</param>
        /// <param name="spawn">The item to spawn.</param>
        /// <param name="rate">The spawn rate weight.</param>
        void Add(TK key, TV spawn, int rate);

        /// <summary>
        /// Removes all entries from the dictionary.
        /// </summary>
        void Clear();

        /// <summary>
        /// Gets an enumerable of all keys in the dictionary.
        /// </summary>
        /// <returns>An enumerable of keys.</returns>
        IEnumerable<TK> GetKeys();

        /// <summary>
        /// Gets the spawn item associated with the specified key.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <returns>The spawn item.</returns>
        TV GetSpawn(TK key);

        /// <summary>
        /// Gets the spawn rate for the specified key.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <returns>The spawn rate.</returns>
        int GetSpawnRate(TK key);

        /// <summary>
        /// Sets the spawn item for the specified key.
        /// </summary>
        /// <param name="key">The key to update.</param>
        /// <param name="spawn">The new spawn item.</param>
        void SetSpawn(TK key, TV spawn);

        /// <summary>
        /// Sets the spawn rate for the specified key.
        /// </summary>
        /// <param name="key">The key to update.</param>
        /// <param name="rate">The new spawn rate.</param>
        void SetSpawnRate(TK key, int rate);

        /// <summary>
        /// Removes the entry with the specified key.
        /// </summary>
        /// <param name="key">The key to remove.</param>
        void Remove(TK key);

        /// <summary>
        /// Determines whether the dictionary contains the specified key.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the key exists; otherwise, false.</returns>
        bool ContainsKey(TK key);
    }

    /// <summary>
    /// Non-generic interface for a dictionary that maps keys to spawnable items with associated spawn rates.
    /// </summary>
    public interface ISpawnDict
    {
        /// <summary>
        /// Gets the number of spawn entries in the dictionary.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets the total of all spawn rates.
        /// </summary>
        int SpawnTotal { get; }

        /// <summary>
        /// Adds a new spawn entry with the specified key, spawn item, and rate.
        /// </summary>
        /// <param name="key">The key to identify the entry.</param>
        /// <param name="spawn">The item to spawn.</param>
        /// <param name="rate">The spawn rate weight.</param>
        void Add(object key, object spawn, int rate);

        /// <summary>
        /// Removes all entries from the dictionary.
        /// </summary>
        void Clear();

        /// <summary>
        /// Gets an enumerable of all keys in the dictionary.
        /// </summary>
        /// <returns>An enumerable of keys.</returns>
        IEnumerable GetKeys();

        /// <summary>
        /// Gets the spawn item associated with the specified key.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <returns>The spawn item.</returns>
        object GetSpawn(object key);

        /// <summary>
        /// Gets the spawn rate for the specified key.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <returns>The spawn rate.</returns>
        int GetSpawnRate(object key);

        /// <summary>
        /// Sets the spawn item for the specified key.
        /// </summary>
        /// <param name="key">The key to update.</param>
        /// <param name="spawn">The new spawn item.</param>
        void SetSpawn(object key, object spawn);

        /// <summary>
        /// Sets the spawn rate for the specified key.
        /// </summary>
        /// <param name="key">The key to update.</param>
        /// <param name="rate">The new spawn rate.</param>
        void SetSpawnRate(object key, int rate);

        /// <summary>
        /// Removes the entry with the specified key.
        /// </summary>
        /// <param name="key">The key to remove.</param>
        void Remove(object key);

        /// <summary>
        /// Determines whether the dictionary contains the specified key.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the key exists; otherwise, false.</returns>
        bool Contains(object key);
    }
}
