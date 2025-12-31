using System.Collections.Generic;

namespace RogueEssence
{
    /// <summary>
    /// A thread-safe Least Recently Used (LRU) cache implementation.
    /// Automatically evicts the oldest items when capacity is exceeded.
    /// </summary>
    /// <typeparam name="K">The type of keys used to identify cached items.</typeparam>
    /// <typeparam name="V">The type of values stored in the cache.</typeparam>
    public class LRUCache<K, V>
    {
        /// <summary>
        /// Internal node structure for the LRU linked list.
        /// </summary>
        private class LRUNode
        {
            public K key;
            public V value;

            public LRUNode(K k, V v)
            {
                key = k;
                value = v;
            }

        }

        Dictionary<K, LinkedListNode<LRUNode>> cacheMap;
        int capacity;
        int total;
        LinkedList<LRUNode> lruList;

        /// <summary>
        /// Delegate for the item removed event.
        /// </summary>
        /// <param name="value">The value being removed from the cache.</param>
        public delegate void ItemRemovedEvent(V value);

        /// <summary>
        /// Event fired when an item is evicted from the cache.
        /// </summary>
        public ItemRemovedEvent OnItemRemoved;

        /// <summary>
        /// Delegate for calculating the size/count of a cached item.
        /// </summary>
        /// <param name="value">The value to measure.</param>
        /// <returns>The size or count of the item.</returns>
        public delegate int ItemCountMethod(V value);

        /// <summary>
        /// Function to calculate the size of cached items for capacity tracking.
        /// </summary>
        public ItemCountMethod ItemCount;

        private object lockObj = new object();

        /// <summary>
        /// Creates a new LRU cache with the specified capacity.
        /// </summary>
        /// <param name="capacity">The maximum capacity of the cache.</param>
        public LRUCache(int capacity)
        {
            this.capacity = capacity;
            ItemCount = defaultCount;
            cacheMap = new Dictionary<K, LinkedListNode<LRUNode>>();
            lruList = new LinkedList<LRUNode>();
        }

        /// <summary>
        /// Adds an item to the cache. Evicts old items if capacity is exceeded.
        /// </summary>
        /// <param name="key">The key to store the item under.</param>
        /// <param name="val">The value to cache.</param>
        public void Add(K key, V val)
        {
            lock (lockObj)
            {
                while (total >= capacity)
                {
                    remove();
                }
                LRUNode cacheItem = new LRUNode(key, val);
                LinkedListNode<LRUNode> node = new LinkedListNode<LRUNode>(cacheItem);
                lruList.AddLast(node);
                cacheMap.Add(key, node);
                total += ItemCount(val);
            }
        }

        /// <summary>
        /// Attempts to retrieve a value from the cache. Marks the item as recently used.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="val">The retrieved value, or default if not found.</param>
        /// <returns>True if the item was found, false otherwise.</returns>
        public bool TryGetValue(K key, out V val)
        {
            lock (lockObj)
            {
                LinkedListNode<LRUNode> node;
                if (cacheMap.TryGetValue(key, out node))
                {
                    val = node.Value.value;

                    lruList.Remove(node);
                    lruList.AddLast(node);
                    return true;
                }
                else
                {
                    val = default(V);
                    return false;
                }
            }
        }

        /// <summary>
        /// Removes all items from the cache, firing OnItemRemoved for each.
        /// </summary>
        public void Clear()
        {
            lock (lockObj)
            {
                while (lruList.Count > 0)
                    remove();
            }
        }

        protected void remove()
        {
            LinkedListNode<LRUNode> node = lruList.First;
            lruList.RemoveFirst();

            cacheMap.Remove(node.Value.key);
            total -= ItemCount(node.Value.value);
            OnItemRemoved?.Invoke(node.Value.value);
        }

        private int defaultCount(V val)
        {
            return 1;
        }
    }
}