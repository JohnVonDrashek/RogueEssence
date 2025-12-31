using System;
using RogueElements;

namespace RogueEssence.LevelGen
{
    /// <summary>
    /// Abstract base class for zone-level generation steps that apply across multiple floors.
    /// Zone steps can modify the generation process for entire dungeon segments.
    /// </summary>
    [Serializable]
    public abstract class ZoneStep
    {
        /// <summary>
        /// Creates a shallow copy of this zone step and initializes any runtime variables.
        /// </summary>
        /// <param name="seed">The random seed for initialization.</param>
        /// <returns>A new instance of the zone step ready for use.</returns>
        public abstract ZoneStep Instantiate(ulong seed);

        /// <summary>
        /// Applies this zone step to the generation process by adding generation steps to the queue.
        /// </summary>
        /// <param name="zoneContext">The zone generation context.</param>
        /// <param name="context">The map generation context.</param>
        /// <param name="queue">The priority queue of generation steps to modify.</param>
        public abstract void Apply(ZoneGenContext zoneContext, IGenContext context, StablePriorityQueue<Priority, IGenStep> queue);
    }
}
