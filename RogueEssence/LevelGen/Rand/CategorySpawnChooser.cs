// <copyright file="CategorySpawnChooser.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// A random picker that selects items from categorized spawn lists.
    /// First selects a category, then picks an item from that category's spawn list.
    /// Used primarily for item categories in maps.
    /// </summary>
    /// <typeparam name="T">The type of item to spawn.</typeparam>
    // TODO: probably make this more generic; this is made specifically for item categories in maps at present.
    [Serializable]
    public class CategorySpawnChooser<T> : IRandPicker<T>
    {
        /// <summary>
        /// Dictionary mapping category names to their spawn lists with associated spawn rates.
        /// </summary>
        public SpawnDict<string, SpawnList<T>> Spawns;

        /// <summary>
        /// Initializes a new instance of the CategorySpawnChooser class with an empty spawn dictionary.
        /// </summary>
        public CategorySpawnChooser()
        {
            Spawns = new SpawnDict<string, SpawnList<T>>();
        }

        /// <summary>
        /// Initializes a new instance of the CategorySpawnChooser class with the specified spawn dictionary.
        /// </summary>
        /// <param name="spawns">The spawn dictionary to use.</param>
        public CategorySpawnChooser(SpawnDict<string, SpawnList<T>> spawns)
        {
            Spawns = spawns;
        }

        /// <summary>
        /// Initializes a new instance of the CategorySpawnChooser class as a deep copy of another.
        /// </summary>
        /// <param name="other">The CategorySpawnChooser to copy.</param>
        public CategorySpawnChooser(CategorySpawnChooser<T> other)
        {
            Spawns = new SpawnDict<string, SpawnList<T>>();
            foreach (string key in other.Spawns.GetKeys())
            {
                SpawnList<T> list = new SpawnList<T>();
                SpawnList<T> otherList = other.Spawns.GetSpawn(key);
                for (int ii = 0; ii < otherList.Count; ii++)
                    list.Add(otherList.GetSpawn(ii), otherList.GetSpawnRate(ii));
                Spawns.Add(key, list, other.Spawns.GetSpawnRate(key));
            }
        }

        /// <summary>
        /// Enumerates all possible outcomes from all categories.
        /// </summary>
        /// <returns>An enumerable of all items from all spawn lists.</returns>
        public IEnumerable<T> EnumerateOutcomes()
        {
            foreach (SpawnList<T> element in Spawns.EnumerateOutcomes())
            {
                foreach (T item in element.EnumerateOutcomes())
                    yield return item;
            }
        }

        /// <summary>
        /// Picks a random item by first selecting a category, then picking from that category.
        /// </summary>
        /// <param name="rand">The random number generator to use.</param>
        /// <returns>A randomly selected item.</returns>
        public T Pick(IRandom rand)
        {
            SpawnDict<string, SpawnList<T>> tempSpawn = new SpawnDict<string, SpawnList<T>>();
            foreach (string key in Spawns.GetKeys())
            {
                SpawnList<T> otherList = Spawns.GetSpawn(key);
                if (!otherList.CanPick)
                    continue;
                tempSpawn.Add(key, otherList, Spawns.GetSpawnRate(key));
            }
            SpawnList<T> choice = tempSpawn.Pick(rand);
            return choice.Pick(rand);
        }

        /// <summary>
        /// Gets a value indicating whether this picker changes state when picking.
        /// Always returns false for this implementation.
        /// </summary>
        public bool ChangesState => false;

        /// <summary>
        /// Gets a value indicating whether this picker can pick an item.
        /// Returns true if at least one category has items that can be picked.
        /// </summary>
        public bool CanPick
        {
            get
            {
                if (!Spawns.CanPick)
                    return false;
                foreach (SpawnList<T> spawn in Spawns.EnumerateOutcomes())
                {
                    if (spawn.CanPick)
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Creates a copy of this picker's state.
        /// </summary>
        /// <returns>A new CategorySpawnChooser with copied state.</returns>
        public IRandPicker<T> CopyState()
        {
            return new CategorySpawnChooser<T>(this);
        }
    }
}
