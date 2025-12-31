using System;
using System.Collections;
using System.Collections.Generic;

namespace RogueEssence
{
    /// <summary>
    /// A generic list implementation that raises events when items are added, removed, or changed.
    /// Implements all standard list interfaces for full compatibility.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    [Serializable]
    public class EventedList<T> : ICollection<T>, IEnumerable<T>, IEnumerable, IList<T>, IReadOnlyCollection<T>, IReadOnlyList<T>, ICollection, IList
    {
        private List<T> list;

        /// <summary>
        /// Delegate for list modification events.
        /// </summary>
        /// <param name="index">The index of the affected item.</param>
        /// <param name="item">The item being added, removed, or set.</param>
        public delegate void EventedListAction(int index, T item);

        /// <summary>
        /// Raised when an item is about to be changed at a specific index.
        /// </summary>
        public event EventedListAction ItemChanging;

        /// <summary>
        /// Raised when an item is about to be added to the list.
        /// </summary>
        public event EventedListAction ItemAdding;

        /// <summary>
        /// Raised when an item is about to be removed from the list.
        /// </summary>
        public event EventedListAction ItemRemoving;

        /// <summary>
        /// Raised when all items are about to be cleared from the list.
        /// </summary>
        public event Action ItemsClearing;

        /// <summary>
        /// Gets or sets the item at the specified index.
        /// </summary>
        /// <param name="index">The index of the item.</param>
        /// <returns>The item at the specified index.</returns>
        public T this[int index]
        {
            get => list[index];
            set
            {
                ItemChanging?.Invoke(index, value);
                list[index] = value;
            }
        }
        object IList.this[int index]
        {
            get => list[index];
            set
            {
                ItemChanging?.Invoke(index, (T)value);
                list[index] = (T)value;
            }
        }

        /// <summary>
        /// Gets the number of items in the list.
        /// </summary>
        public int Count => list.Count;

        /// <summary>
        /// Gets a value indicating whether the list is read-only.
        /// </summary>
        public bool IsReadOnly => ((IList)list).IsReadOnly;

        /// <summary>
        /// Gets a value indicating whether the list has a fixed size.
        /// </summary>
        public bool IsFixedSize => ((IList)list).IsFixedSize;

        /// <summary>
        /// Gets a value indicating whether access to the list is synchronized.
        /// </summary>
        public bool IsSynchronized => ((IList)list).IsSynchronized;

        /// <summary>
        /// Gets an object that can be used to synchronize access to the list.
        /// </summary>
        public object SyncRoot => ((IList)list).SyncRoot;

        /// <summary>
        /// Initializes a new instance of the EventedList class.
        /// </summary>
        public EventedList()
        {
            list = new List<T>();
        }

        /// <summary>
        /// Adds an item to the list.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void Add(T item)
        {
            ItemAdding?.Invoke(list.Count, item);
            list.Add(item);
        }

        int IList.Add(object value)
        {
            ItemAdding?.Invoke(list.Count, (T)value);
            return ((IList)list).Add(value);
        }

        /// <summary>
        /// Removes all items from the list.
        /// </summary>
        public void Clear()
        {
            ItemsClearing?.Invoke();
            list.Clear();
        }

        /// <summary>
        /// Determines whether the list contains a specific item.
        /// </summary>
        /// <param name="item">The item to locate.</param>
        /// <returns>True if the item is found; otherwise, false.</returns>
        public bool Contains(T item)
        {
            return list.Contains(item);
        }

        bool IList.Contains(object value)
        {
            return Contains((T)value);
        }

        /// <summary>
        /// Copies the elements of the list to an array.
        /// </summary>
        /// <param name="array">The destination array.</param>
        /// <param name="arrayIndex">The starting index in the array.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((IList)list).CopyTo(array, index);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the list.
        /// </summary>
        /// <returns>An enumerator for the list.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns the index of an item in the list.
        /// </summary>
        /// <param name="item">The item to locate.</param>
        /// <returns>The index of the item, or -1 if not found.</returns>
        public int IndexOf(T item)
        {
            return list.IndexOf(item);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((T)value);
        }

        /// <summary>
        /// Inserts an item at the specified index.
        /// </summary>
        /// <param name="index">The index at which to insert.</param>
        /// <param name="item">The item to insert.</param>
        public void Insert(int index, T item)
        {
            ItemAdding?.Invoke(index, item);
            list.Insert(index, item);
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (T)value);
        }

        /// <summary>
        /// Removes the first occurrence of an item from the list.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>True if the item was removed; otherwise, false.</returns>
        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index > -1)
            {
                RemoveAt(index);
                return true;
            }
            return false;
        }

        void IList.Remove(object value)
        {
            Remove((T)value);
        }

        /// <summary>
        /// Removes the item at the specified index.
        /// </summary>
        /// <param name="index">The index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            ItemRemoving?.Invoke(index, list[index]);
            list.RemoveAt(index);
        }
    }
}
