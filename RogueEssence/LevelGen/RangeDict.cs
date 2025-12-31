using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using RogueElements;

namespace RogueEssence.LevelGen
{
    /// <summary>
    /// A dictionary-like collection that maps integer ranges to values.
    /// Allows storing values that apply to ranges of floor numbers or indices.
    /// </summary>
    /// <typeparam name="T">The type of value to store for each range.</typeparam>
    [Serializable]
    public class RangeDict<T> : IRangeDict<T>, IRangeDict
    {
        private readonly List<RangeNode> nodes;

        /// <summary>
        /// Gets the number of ranges currently stored in the dictionary.
        /// </summary>
        public int RangeCount => nodes.Count;

        /// <summary>
        /// Initializes a new instance of the RangeDict class.
        /// </summary>
        public RangeDict()
        {
            nodes = new List<RangeNode>();
        }

        /// <summary>
        /// Removes all ranges from the dictionary.
        /// </summary>
        public void Clear()
        {
            nodes.Clear();
        }

        /// <summary>
        /// Gets the total count of all indices covered by all ranges.
        /// </summary>
        /// <returns>The sum of all range lengths.</returns>
        public int GetTotalCount()
        {
            int length = 0;

            foreach (RangeNode node in nodes)
            {
                length += node.Range.Max - node.Range.Min;
            }

            return length;
        }

        /// <summary>
        /// Sets an item for a specific range, erasing any existing overlapping ranges.
        /// </summary>
        /// <param name="item">The item to associate with the range.</param>
        /// <param name="range">The integer range to set.</param>
        public void SetRange(T item, IntRange range)
        {
            //TODO: make this use binary search for O(logn) access time
            EraseRange(range);
            nodes.Add(new RangeNode(item, range));
        }

        void IRangeDict.SetRange(object item, IntRange range)
        {
            SetRange((T)item, range);
        }

        /// <summary>
        /// Erases all items that fall within the specified range.
        /// Splits ranges that partially overlap with the erased range.
        /// </summary>
        /// <param name="range">The range to erase.</param>
        public void EraseRange(IntRange range)
        {
            //TODO: make this use binary search for O(logn) access time
            for (int ii = nodes.Count - 1; ii >= 0; ii--)
            {
                if (range.Min <= nodes[ii].Range.Min && nodes[ii].Range.Max <= range.Max)
                    nodes.RemoveAt(ii);
                else if (nodes[ii].Range.Min < range.Min && range.Max < nodes[ii].Range.Max)
                {
                    nodes[ii] = new RangeNode(nodes[ii].Item, new IntRange(nodes[ii].Range.Min, range.Min));
                    nodes.Insert(ii+1, new RangeNode(nodes[ii].Item, new IntRange(range.Max, nodes[ii].Range.Max)));
                }
                else if (range.Min <= nodes[ii].Range.Min && nodes[ii].Range.Min < range.Max)
                    nodes[ii] = new RangeNode(nodes[ii].Item, new IntRange(range.Max, nodes[ii].Range.Max));
                else if (range.Min < nodes[ii].Range.Max && nodes[ii].Range.Max <= range.Max)
                    nodes[ii] = new RangeNode(nodes[ii].Item, new IntRange(nodes[ii].Range.Min, range.Min));
            }
        }

        /// <summary>
        /// Gets the item at the specified index.
        /// </summary>
        /// <param name="index">The index to look up.</param>
        /// <returns>The item associated with the range containing the index.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when no range contains the index.</exception>
        public T GetItem(int index)
        {
            //TODO: make this use binary search for O(logn) access time
            foreach (RangeNode node in nodes)
            {
                if (node.Range.Min <= index && index < node.Range.Max)
                    return node.Item;
            }
            throw new KeyNotFoundException();
        }

        object IRangeDict.GetItem(int index)
        {
            return GetItem(index);
        }

        /// <summary>
        /// Attempts to get the item at the specified index.
        /// </summary>
        /// <param name="index">The index to look up.</param>
        /// <param name="item">When this method returns, contains the item if found; otherwise, the default value.</param>
        /// <returns>True if an item was found at the index; otherwise, false.</returns>
        public bool TryGetItem(int index, out T item)
        {
            //TODO: make this use binary search for O(logn) access time
            foreach (RangeNode node in nodes)
            {
                if (node.Range.Min <= index && index < node.Range.Max)
                {
                    item = node.Item;
                    return true;
                }
            }
            item = default(T);
            return false;
        }

        /// <summary>
        /// Enumerates all ranges stored in the dictionary.
        /// </summary>
        /// <returns>An enumerable of all ranges.</returns>
        public IEnumerable<IntRange> EnumerateRanges()
        {
            foreach (RangeNode node in nodes)
                yield return node.Range;
        }

        /// <summary>
        /// Gets the item at the specified index.
        /// </summary>
        /// <param name="index">The index to look up.</param>
        /// <returns>The item associated with the range containing the index.</returns>
        public T this[int index]
        {
            get { return GetItem(index); }
        }

        /// <summary>
        /// Determines whether any range contains the specified index.
        /// </summary>
        /// <param name="index">The index to check.</param>
        /// <returns>True if a range contains the index; otherwise, false.</returns>
        public bool ContainsItem(int index)
        {
            //TODO: make this use binary search for O(logn) access time
            foreach (RangeNode node in nodes)
            {
                if (node.Range.Min <= index && index < node.Range.Max)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Internal structure representing a range-item pair.
        /// </summary>
        [Serializable]
        private struct RangeNode
        {
            /// <summary>
            /// The item associated with this range.
            /// </summary>
            public T Item;

            /// <summary>
            /// The integer range.
            /// </summary>
            public IntRange Range;

            /// <summary>
            /// Initializes a new RangeNode.
            /// </summary>
            /// <param name="item">The item to store.</param>
            /// <param name="range">The range to associate with the item.</param>
            public RangeNode(T item, IntRange range)
            {
                this.Item = item;
                this.Range = range;
            }
        }
    }


    /// <summary>
    /// Generic interface for a dictionary that maps integer ranges to values.
    /// </summary>
    /// <typeparam name="T">The type of value to store.</typeparam>
    public interface IRangeDict<T>
    {
        /// <summary>
        /// Removes all ranges from the dictionary.
        /// </summary>
        void Clear();

        /// <summary>
        /// Sets an item for a specific range.
        /// </summary>
        /// <param name="item">The item to associate with the range.</param>
        /// <param name="range">The integer range.</param>
        void SetRange(T item, IntRange range);

        /// <summary>
        /// Erases all items within the specified range.
        /// </summary>
        /// <param name="range">The range to erase.</param>
        void EraseRange(IntRange range);

        /// <summary>
        /// Gets the item at the specified index.
        /// </summary>
        /// <param name="index">The index to look up.</param>
        /// <returns>The item at the index.</returns>
        T GetItem(int index);

        /// <summary>
        /// Determines whether any range contains the specified index.
        /// </summary>
        /// <param name="index">The index to check.</param>
        /// <returns>True if found; otherwise, false.</returns>
        bool ContainsItem(int index);

        /// <summary>
        /// Enumerates all ranges in the dictionary.
        /// </summary>
        /// <returns>An enumerable of ranges.</returns>
        IEnumerable<IntRange> EnumerateRanges();
    }

    /// <summary>
    /// Non-generic interface for a dictionary that maps integer ranges to values.
    /// </summary>
    public interface IRangeDict
    {
        /// <summary>
        /// Removes all ranges from the dictionary.
        /// </summary>
        void Clear();

        /// <summary>
        /// Sets an item for a specific range.
        /// </summary>
        /// <param name="item">The item to associate with the range.</param>
        /// <param name="range">The integer range.</param>
        void SetRange(object item, IntRange range);

        /// <summary>
        /// Erases all items within the specified range.
        /// </summary>
        /// <param name="range">The range to erase.</param>
        void EraseRange(IntRange range);

        /// <summary>
        /// Gets the item at the specified index.
        /// </summary>
        /// <param name="index">The index to look up.</param>
        /// <returns>The item at the index.</returns>
        object GetItem(int index);

        /// <summary>
        /// Determines whether any range contains the specified index.
        /// </summary>
        /// <param name="index">The index to check.</param>
        /// <returns>True if found; otherwise, false.</returns>
        bool ContainsItem(int index);

        /// <summary>
        /// Enumerates all ranges in the dictionary.
        /// </summary>
        /// <returns>An enumerable of ranges.</returns>
        IEnumerable<IntRange> EnumerateRanges();
    }
}
