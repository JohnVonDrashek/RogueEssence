using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Drawing;
using System.Text;
using RogueElements;

namespace RogueEssence.Dungeon
{
    /// <summary>
    /// Abstract base class for all gameplay states that can be cloned.
    /// Gameplay states are mutable data attached to characters, items, skills, or maps.
    /// </summary>
    [Serializable]
    public abstract class GameplayState
    {
        /// <summary>
        /// Creates a typed clone of this state.
        /// </summary>
        /// <typeparam name="T">The state type to cast to.</typeparam>
        /// <returns>A clone cast to the specified type.</returns>
        public T Clone<T>() where T : GameplayState { return (T)Clone(); }

        /// <summary>
        /// Creates a deep copy of this gameplay state.
        /// </summary>
        /// <returns>A clone of this state.</returns>
        public abstract GameplayState Clone();
    }

    /// <summary>
    /// A type-indexed collection of gameplay states, allowing only one state of each type.
    /// Used to store character states, item states, and other gameplay modifiers.
    /// </summary>
    /// <typeparam name="T">The base type of states in this collection.</typeparam>
    [Serializable]
    public class StateCollection<T> : TypeDict<T> where T : GameplayState
    {
        /// <summary>
        /// Initializes a new empty StateCollection.
        /// </summary>
        public StateCollection() { }

        /// <summary>
        /// Creates a deep copy of another StateCollection.
        /// </summary>
        /// <param name="other">The StateCollection to copy.</param>
        protected StateCollection(StateCollection<T> other) : this()
        {
            foreach (T obj in other)
                Set((T)obj.Clone());
        }

        /// <summary>
        /// Creates a clone of this StateCollection.
        /// </summary>
        /// <returns>A new StateCollection with cloned states.</returns>
        public StateCollection<T> Clone() { return new StateCollection<T>(this); }

        /// <summary>
        /// Gets a state by type, returning default if not found.
        /// </summary>
        /// <typeparam name="K">The state type to retrieve.</typeparam>
        /// <returns>The state if found, otherwise default.</returns>
        public K GetWithDefault<K>() where K : T
        {
            K state;
            if (TryGet(out state))
                return state;
            return default;
        }

        /// <summary>
        /// Gets a state by runtime type, returning default if not found.
        /// </summary>
        /// <param name="type">The type of state to retrieve.</param>
        /// <returns>The state if found, otherwise default.</returns>
        public T GetWithDefault(Type type)
        {
            T state;
            if (TryGet(type, out state))
                return state;
            return default;
        }

        /// <summary>
        /// Returns a string representation of this collection.
        /// </summary>
        /// <returns>A comma-separated list of states, or empty message.</returns>
        public override string ToString()
        {
            if (Count == 0)
                return "[Empty " + typeof(T).ToString() + "s]";
            StringBuilder builder = new StringBuilder();
            int count = 0;
            int total = Count;
            foreach (T value in this)
            {
                if (count == 3)
                {
                    builder.Append("...");
                    break;
                }
                builder.Append(value.ToString());
                count++;
                if (count < total)
                    builder.Append(", ");
            }
            return builder.ToString();
        }
    }

    /// <summary>
    /// Interface for non-generic state collection operations.
    /// </summary>
    public interface IStateCollection : IEnumerable
    {
        /// <summary>
        /// Sets a state value by its type.
        /// </summary>
        /// <param name="value">The state to set.</param>
        void Set(object value);

        /// <summary>
        /// Gets a state by type.
        /// </summary>
        /// <param name="type">The type of state to retrieve.</param>
        /// <returns>The state if found.</returns>
        object Get(Type type);

        /// <summary>
        /// Removes all states from the collection.
        /// </summary>
        void Clear();

        /// <summary>
        /// Checks if a state of the specified type exists.
        /// </summary>
        /// <param name="type">The type to check for.</param>
        /// <returns>True if a state of that type exists.</returns>
        bool Contains(Type type);

        /// <summary>
        /// Removes a state by type.
        /// </summary>
        /// <param name="type">The type of state to remove.</param>
        void Remove(Type type);
    }

}
