using System;
using RogueElements;
using System.Collections.Generic;
using RogueEssence.LevelGen;
using RogueEssence.Dev;
using RogueEssence.Data;
using RogueEssence.Dungeon;
using Newtonsoft.Json;

namespace RogueEssence.LevelGen
{
    /// <summary>
    /// Orients all already-placed compass tiles to point to points of interest.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class DetectItemStep<T> : GenStep<T>
        where T : StairsMapGenContext
    {
        /// <summary>
        /// Tile used as compass.
        /// </summary>
        [JsonConverter(typeof(TileConverter))]
        [DataType(0, DataManager.DataType.Item, false)]
        public string FindItem;

        /// <summary>
        /// Initializes a new instance of the DetectItemStep class.
        /// </summary>
        public DetectItemStep()
        {
        }

        /// <summary>
        /// Initializes a new instance of the DetectItemStep class with the specified item to find.
        /// </summary>
        /// <param name="item">The item ID to search for.</param>
        public DetectItemStep(string item)
        {
            FindItem = item;
        }

        /// <summary>
        /// Applies the detection step, throwing an exception if the specified item is not found on the map.
        /// </summary>
        /// <param name="map">The map generation context to search.</param>
        /// <exception cref="Exception">Thrown when the specified item is not found.</exception>
        public override void Apply(T map)
        {
            foreach(MapItem item in map.Items)
            {
                if (!item.IsMoney && item.Value == FindItem)
                    return;
            }

            throw new Exception("Did not find tile " + FindItem + "!");
        }
    }
}
