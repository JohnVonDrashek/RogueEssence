using System;
using RogueElements;
using System.Collections.Generic;

namespace RogueEssence
{
    /// <summary>
    /// Finds all fully unreachable tiles that aren't impassable and turns them impassable.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class FillImpassableStep<T> : GenStep<T>
        where T : class, ITiledGenContext
    {
        /// <summary>
        /// Initializes a new instance of the FillImpassableStep class.
        /// </summary>
        public FillImpassableStep()
        {
        }

        /// <summary>
        /// Applies the step to fill unreachable tiles with impassable terrain.
        /// Note: This step is not yet implemented.
        /// </summary>
        /// <param name="map">The map generation context to modify.</param>
        public override void Apply(T map)
        {
            //TODO: find all fully unreachable tiles and fill in with impassable

        }
    }
}
