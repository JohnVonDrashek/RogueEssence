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
    public class DetectTileStep<T> : GenStep<T>
        where T : StairsMapGenContext
    {
        /// <summary>
        /// Tile used as compass.
        /// </summary>
        [JsonConverter(typeof(TileConverter))]
        [DataType(0, DataManager.DataType.Tile, false)]
        public string FindTile;

        /// <summary>
        /// Initializes a new instance of the DetectTileStep class.
        /// </summary>
        public DetectTileStep()
        {
        }

        /// <summary>
        /// Initializes a new instance of the DetectTileStep class with the specified tile to find.
        /// </summary>
        /// <param name="tile">The tile ID to search for.</param>
        public DetectTileStep(string tile)
        {
            FindTile = tile;
        }

        /// <summary>
        /// Applies the detection step, throwing an exception if the specified tile is not found on the map.
        /// </summary>
        /// <param name="map">The map generation context to search.</param>
        /// <exception cref="Exception">Thrown when the specified tile is not found.</exception>
        public override void Apply(T map)
        {
            for (int xx = 0; xx < map.Width; xx++)
            {
                for (int yy = 0; yy < map.Height; yy++)
                {
                    Loc tileLoc = new Loc(xx, yy);
                    Tile tile = map.Map.GetTile(tileLoc);
                    if (tile.Effect.ID == FindTile)
                        return;
                }
            }

            throw new Exception("Did not find tile " + FindTile + "!");
        }
    }
}
