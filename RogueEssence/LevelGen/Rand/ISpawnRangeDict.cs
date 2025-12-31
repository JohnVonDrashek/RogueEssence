// <copyright file="ISpawnRangeDict.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Generic interface for a dictionary that maps keys to spawnable items with spawn rates and floor ranges.
    /// Used for items that should only appear on certain floor ranges.
    /// </summary>
    /// <typeparam name="TK">The type of key.</typeparam>
    /// <typeparam name="TV">The type of spawnable item.</typeparam>
    public interface ISpawnRangeDict<TK, TV> : IEnumerable<TV>, IEnumerable
    {
        /// <summary>
        /// Gets the number of spawn entries in the dictionary.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Adds a new spawn entry with the specified key, spawn item, floor range, and rate.
        /// </summary>
        /// <param name="key">The key to identify the entry.</param>
        /// <param name="spawn">The item to spawn.</param>
        /// <param name="range">The floor range where this item can spawn.</param>
        /// <param name="rate">The spawn rate weight.</param>
        void Add(TK key, TV spawn, IntRange range, int rate);

        /// <summary>
        /// Removes all entries from the dictionary.
        /// </summary>
        void Clear();

        /// <summary>
        /// Gets the spawn item associated with the specified key.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <returns>The spawn item.</returns>
        TV GetSpawn(TK key);

        /// <summary>
        /// Gets the floor range for the specified key.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <returns>The floor range.</returns>
        IntRange GetSpawnRange(TK key);

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
        /// Sets the floor range for the specified key.
        /// </summary>
        /// <param name="key">The key to update.</param>
        /// <param name="range">The new floor range.</param>
        void SetSpawnRange(TK key, IntRange range);

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
    /// Non-generic interface for a dictionary that maps keys to spawnable items with spawn rates and floor ranges.
    /// </summary>
    public interface ISpawnRangeDict : IEnumerable
    {
        /// <summary>
        /// Gets the number of spawn entries in the dictionary.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Adds a new spawn entry with the specified key, spawn item, floor range, and rate.
        /// </summary>
        /// <param name="key">The key to identify the entry.</param>
        /// <param name="spawn">The item to spawn.</param>
        /// <param name="range">The floor range where this item can spawn.</param>
        /// <param name="rate">The spawn rate weight.</param>
        void Add(object key, object spawn, IntRange range, int rate);

        /// <summary>
        /// Removes all entries from the dictionary.
        /// </summary>
        void Clear();

        /// <summary>
        /// Gets the spawn item associated with the specified key.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <returns>The spawn item.</returns>
        object GetSpawn(object key);

        /// <summary>
        /// Gets the floor range for the specified key.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <returns>The floor range.</returns>
        IntRange GetSpawnRange(object key);

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
        /// Sets the floor range for the specified key.
        /// </summary>
        /// <param name="key">The key to update.</param>
        /// <param name="range">The new floor range.</param>
        void SetSpawnRange(object key, IntRange range);

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
