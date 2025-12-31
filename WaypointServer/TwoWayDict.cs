using System.Collections.Generic;

namespace WaypointServer
{
    /// <summary>
    /// A bidirectional dictionary that maintains synchronized forward and reverse mappings.
    /// Allows lookup in both directions: from T1 to T2 (forward) and from T2 to T1 (reverse).
    /// </summary>
    /// <typeparam name="T1">The type of keys in the forward direction.</typeparam>
    /// <typeparam name="T2">The type of values in the forward direction (keys in reverse).</typeparam>
    public class TwoWayDict<T1, T2>
    {
        private Dictionary<T1, T2> _forward = new Dictionary<T1, T2>();
        private Dictionary<T2, T1> _reverse = new Dictionary<T2, T1>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TwoWayDict{T1, T2}"/> class with empty forward and reverse dictionaries.
        /// </summary>
        public TwoWayDict()
        {
            this.Forward = new Indexer<T1, T2>(_forward);
            this.Reverse = new Indexer<T2, T1>(_reverse);
        }

        /// <summary>
        /// Provides indexed access to a dictionary with key containment checking.
        /// Used internally to expose forward and reverse lookups.
        /// </summary>
        /// <typeparam name="T3">The type of the keys.</typeparam>
        /// <typeparam name="T4">The type of the values.</typeparam>
        public class Indexer<T3, T4>
        {
            private Dictionary<T3, T4> _dictionary;

            /// <summary>
            /// Initializes a new instance of the <see cref="Indexer{T3, T4}"/> class wrapping the specified dictionary.
            /// </summary>
            /// <param name="dictionary">The dictionary to wrap for indexed access.</param>
            public Indexer(Dictionary<T3, T4> dictionary)
            {
                _dictionary = dictionary;
            }

            /// <summary>
            /// Gets or sets the value associated with the specified key.
            /// </summary>
            /// <param name="index">The key of the value to get or set.</param>
            /// <returns>The value associated with the specified key.</returns>
            public T4 this[T3 index]
            {
                get { return _dictionary[index]; }
                set { _dictionary[index] = value; }
            }

            /// <summary>
            /// Determines whether the dictionary contains the specified key.
            /// </summary>
            /// <param name="key">The key to locate in the dictionary.</param>
            /// <returns><c>true</c> if the dictionary contains an element with the specified key; otherwise, <c>false</c>.</returns>
            public bool Contains(T3 key)
            {
                return _dictionary.ContainsKey(key);
            }
        }

        /// <summary>
        /// Adds a bidirectional mapping between the specified values.
        /// Both the forward (t1 to t2) and reverse (t2 to t1) mappings are created.
        /// </summary>
        /// <param name="t1">The forward key.</param>
        /// <param name="t2">The forward value (and reverse key).</param>
        public void Add(T1 t1, T2 t2)
        {
            _forward.Add(t1, t2);
            _reverse.Add(t2, t1);
        }

        /// <summary>
        /// Removes the bidirectional mapping for the specified forward key.
        /// Both the forward and corresponding reverse entries are removed.
        /// </summary>
        /// <param name="t1">The forward key to remove.</param>
        /// <returns><c>true</c> if the element was successfully found and removed; otherwise, <c>false</c>.</returns>
        public bool RemoveForward(T1 t1)
        {
            if (!_forward.ContainsKey(t1))
                return false;
            T2 reverse_val = _forward[t1];
            _forward.Remove(t1);
            _reverse.Remove(reverse_val);
            return true;
        }

        /// <summary>
        /// Removes the bidirectional mapping for the specified reverse key.
        /// Both the reverse and corresponding forward entries are removed.
        /// </summary>
        /// <param name="t2">The reverse key to remove.</param>
        /// <returns><c>true</c> if the element was successfully found and removed; otherwise, <c>false</c>.</returns>
        public bool RemoveReverse(T2 t2)
        {
            if (!_reverse.ContainsKey(t2))
                return false;
            T1 forward_val = _reverse[t2];
            _reverse.Remove(t2);
            _forward.Remove(forward_val);
            return true;
        }

        /// <summary>
        /// Gets the indexer for forward lookups (T1 to T2).
        /// </summary>
        public Indexer<T1, T2> Forward { get; private set; }

        /// <summary>
        /// Gets the indexer for reverse lookups (T2 to T1).
        /// </summary>
        public Indexer<T2, T1> Reverse { get; private set; }
    }
}
