using System;
using System.Collections.Generic;

namespace RogueEssence
{
    /// <summary>
    /// Provides extension methods and utilities for working with collections.
    /// </summary>
    public static class CollectionExt
    {
        /// <summary>
        /// Delegate for comparing two elements of the same type.
        /// </summary>
        /// <typeparam name="T">The type of elements to compare.</typeparam>
        /// <param name="a">The first element.</param>
        /// <param name="b">The second element.</param>
        /// <returns>Negative if a is less than b, positive if greater, zero if equal.</returns>
        public delegate int CompareFunction<T>(T a, T b);

        /// <summary>
        /// Adds an element to a sorted list while maintaining sort order.
        /// Uses binary search for efficient insertion. The sort is stable.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The sorted list to add to.</param>
        /// <param name="element">The element to add.</param>
        /// <param name="compareFunc">The comparison function for sorting.</param>
        public static void AddToSortedList<T>(List<T> list, T element, CompareFunction<T> compareFunc)
        {
            if (compareFunc == null)
                throw new ArgumentNullException(nameof(compareFunc));

            // stable
            int min = 0;
            int max = list.Count - 1;
            int point = max;
            int compare = -1;

            // binary search
            while (min <= max)
            {
                point = (min + max) / 2;

                compare = compareFunc(list[point], element);

                if (compare > 0)
                {
                    // go down
                    max = point - 1;
                }
                else if (compare < 0)
                {
                    // go up
                    min = point + 1;
                }
                else
                {
                    // go past the last index of equal comparison
                    point++;
                    while (point < list.Count && compareFunc(list[point], element) == 0)
                        point++;
                    list.Insert(point, element);
                    return;
                }
            }

            // no place found
            list.Insert(point + (compare > 0 ? 0 : 1), element);
        }

        /// <summary>
        /// Assigns an element to a list at the specified index, extending the list if necessary.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list to assign to.</param>
        /// <param name="index">The index to assign at.</param>
        /// <param name="element">The element to assign.</param>
        public static void AssignExtendList<T>(List<T> list, int index, T element)
        {
            while (list.Count <= index)
                list.Add(default(T));
            list[index] = element;
        }

        /// <summary>
        /// Gets an element from a list at the specified index, returning default if out of bounds.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list to get from.</param>
        /// <param name="index">The index to get.</param>
        /// <returns>The element at the index, or default(T) if the index is out of bounds.</returns>
        public static T GetExtendList<T>(List<T> list, int index)
        {
            if (index < list.Count)
                return list[index];
            return default(T);
        }
    }
}
