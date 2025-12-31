using System;
using RogueElements;
using RogueEssence.Dev;

namespace RogueEssence.LevelGen
{
    /// <summary>
    /// Wrapper class that associates a generation step with a priority for ordering in the generation queue.
    /// </summary>
    /// <typeparam name="T">The type of generation step.</typeparam>
    [Serializable]
    public class GenPriority<T> : IGenPriority where T : IGenStep
    {
        /// <summary>
        /// The priority value that determines when this step executes relative to others.
        /// </summary>
        public Priority Priority { get; set; }

        /// <summary>
        /// The generation step to execute.
        /// </summary>
        [SubGroup]
        public T Item;

        /// <summary>
        /// Initializes a new instance of the GenPriority class.
        /// </summary>
        public GenPriority() { }

        /// <summary>
        /// Initializes a new instance of the GenPriority class with the specified effect.
        /// </summary>
        /// <param name="effect">The generation step.</param>
        public GenPriority(T effect)
        {
            Item = effect;
        }

        /// <summary>
        /// Initializes a new instance of the GenPriority class with the specified priority and effect.
        /// </summary>
        /// <param name="priority">The priority value.</param>
        /// <param name="effect">The generation step.</param>
        public GenPriority(Priority priority, T effect)
        {
            Priority = priority;
            Item = effect;
        }

        /// <summary>
        /// Gets the generation step item.
        /// </summary>
        /// <returns>The generation step.</returns>
        public IGenStep GetItem() { return Item; }


        public override string ToString()
        {
            if (Item != null)
                return string.Format("{0}: {1}", Priority.ToString(), Item.ToString());
            else
                return string.Format("{0}: [EMPTY]", Priority.ToString());
        }
    }

    /// <summary>
    /// Interface for prioritized generation steps.
    /// </summary>
    public interface IGenPriority
    {
        /// <summary>
        /// Gets or sets the priority value for ordering.
        /// </summary>
        Priority Priority { get; set; }

        /// <summary>
        /// Gets the generation step item.
        /// </summary>
        /// <returns>The generation step.</returns>
        IGenStep GetItem();
    }
}
