using RogueElements;
using System;
using System.Collections.Generic;
using RogueEssence.Dungeon;
using Newtonsoft.Json;
using RogueEssence.Dev;

namespace RogueEssence.LevelGen
{
    /// <summary>
    /// Spawns a box item containing a random item determined by a base spawner.
    /// The box wraps the contained item using the specified box item ID.
    /// </summary>
    /// <typeparam name="TGenContext">The type of generation context.</typeparam>
    [Serializable]
    public class BoxSpawner<TGenContext> : IStepSpawner<TGenContext, MapItem>
        where TGenContext : IGenContext
    {
        /// <summary>
        /// Initializes a new instance of the BoxSpawner class.
        /// </summary>
        public BoxSpawner()
        {
        }

        /// <summary>
        /// Initializes a new instance of the BoxSpawner class with the specified box ID and base spawner.
        /// </summary>
        /// <param name="id">The item ID of the box that contains the items.</param>
        /// <param name="spawner">The spawner that determines what items go in the boxes.</param>
        public BoxSpawner(string id, IStepSpawner<TGenContext, MapItem> spawner)
        {
            this.BaseSpawner = spawner;
            this.BoxID = id;
        }

        /// <summary>
        /// The item ID of the box containing the item.
        /// </summary>
        [JsonConverter(typeof(ItemConverter))]
        [DataType(0, Data.DataManager.DataType.Item, false)]
        public string BoxID { get; set; }

        /// <summary>
        /// The spawner that decides what item the box holds.
        /// </summary>
        public IStepSpawner<TGenContext, MapItem> BaseSpawner { get; set; }

        /// <summary>
        /// Gets the spawned items wrapped in boxes.
        /// </summary>
        /// <param name="map">The generation context.</param>
        /// <returns>A list of box items, each containing an item from the base spawner.</returns>
        public List<MapItem> GetSpawns(TGenContext map)
        {
            if (this.BaseSpawner is null)
                return new List<MapItem>();

            List<MapItem> baseItems = this.BaseSpawner.GetSpawns(map);
            List<MapItem> copyResults = new List<MapItem>();

            foreach (MapItem item in baseItems)
                copyResults.Add(MapItem.CreateBox(BoxID, item.Value));

            return copyResults;
        }

        public override string ToString()
        {
            string baseSpawnerString = "NULL";
            if (this.BaseSpawner != null)
                baseSpawnerString = this.BaseSpawner.ToString();
            return string.Format("{0}: {1}, {2}", this.GetType().GetFormattedTypeName(), this.BoxID.ToString(), baseSpawnerString);
        }
    }
}
