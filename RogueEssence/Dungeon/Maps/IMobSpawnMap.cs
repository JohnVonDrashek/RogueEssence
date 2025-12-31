using System;
using System.Collections;
using System.Collections.Generic;
using RogueElements;
using Microsoft.Xna.Framework.Graphics;
using RogueEssence.Content;
using RogueEssence.Data;
using RogueEssence.LevelGen;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;
using RogueEssence.Script;

namespace RogueEssence.Dungeon
{
    /// <summary>
    /// Interface for maps that support mob spawning functionality.
    /// Provides access to random number generation and map state for spawning.
    /// </summary>
    public interface IMobSpawnMap
    {
        /// <summary>
        /// Gets the random number generator for this map.
        /// </summary>
        IRandom Rand { get; }

        /// <summary>
        /// Gets whether the map has begun gameplay.
        /// </summary>
        bool Begun { get; }

        /// <summary>
        /// Gets the unique identifier for this map.
        /// </summary>
        int ID { get; }
    }

}
