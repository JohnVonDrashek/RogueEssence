using System;
using RogueElements;
using System.Linq;
using System.Collections.Generic;

namespace RogueEssence.Dungeon
{
    /// <summary>
    /// Interface for map layers that can be cloned and merged.
    /// Map layers are used for decoration, overlay, and other visual elements.
    /// </summary>
    public interface IMapLayer
    {
        /// <summary>
        /// Creates a deep copy of this map layer.
        /// </summary>
        /// <returns>A clone of this map layer.</returns>
        IMapLayer Clone();

        /// <summary>
        /// Merges another map layer's contents into this one.
        /// </summary>
        /// <param name="other">The map layer to merge from.</param>
        void Merge(IMapLayer other);
    }

}

