using System;

namespace RogueEssence.Dungeon
{
    /// <summary>
    /// A generic wrapper that holds an element along with an optional back reference index.
    /// Used for skills and other elements that may need to reference backup copies.
    /// </summary>
    /// <typeparam name="T">The type of the element being referenced.</typeparam>
    [Serializable]
    public class BackReference<T>
    {
        /// <summary>
        /// The actual element being wrapped.
        /// </summary>
        public T Element;

        /// <summary>
        /// The index referencing a backup copy, or -1 if no backup exists.
        /// </summary>
        public int BackRef;

        /// <summary>
        /// Initializes a new empty BackReference with no backup.
        /// </summary>
        public BackReference() { BackRef = -1; }

        /// <summary>
        /// Initializes a new BackReference with the specified element and no backup.
        /// </summary>
        /// <param name="element">The element to wrap.</param>
        public BackReference(T element)
        {
            Element = element;
            BackRef = -1;
        }

        /// <summary>
        /// Initializes a new BackReference with the specified element and backup reference.
        /// </summary>
        /// <param name="element">The element to wrap.</param>
        /// <param name="backRef">The index of the backup copy.</param>
        public BackReference(T element, int backRef)
        {
            Element = element;
            BackRef = backRef;
        }
    }

}
