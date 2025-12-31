using System;
using System.Collections.Generic;
using RogueElements;
using RogueEssence.Data;

namespace RogueEssence.Dungeon
{
    /// <summary>
    /// Represents a single tile in the dungeon map, combining terrain data with optional effects.
    /// Contains both the terrain type (ground, water, lava) and any effect tiles (traps, stairs).
    /// </summary>
    [Serializable]
    public class Tile : ITile
    {
        /// <summary>
        /// The terrain data for this tile (ground, water, lava, etc.).
        /// </summary>
        [Dev.SubGroup]
        public TerrainTile Data;

        /// <summary>
        /// The effect tile data (traps, wonder tiles, stairs, etc.).
        /// </summary>
        public EffectTile Effect;

        /// <summary>
        /// Gets or sets the terrain ID for this tile.
        /// </summary>
        public string ID { get { return Data.ID; } set { Data.ID = value; } }

        /// <summary>
        /// Initializes a new empty Tile with default terrain and no effects.
        /// </summary>
        public Tile()
        {
            Data = new TerrainTile();
            Effect = new EffectTile();
        }

        /// <summary>
        /// Initializes a new Tile with the specified terrain type.
        /// </summary>
        /// <param name="type">The terrain type ID.</param>
        public Tile(string type)
        {
            Data = new TerrainTile(type);
            Effect = new EffectTile();
        }

        /// <summary>
        /// Initializes a new Tile with terrain type and texture stability setting.
        /// </summary>
        /// <param name="type">The terrain type ID.</param>
        /// <param name="stableTex">Whether to use stable texture coordinates.</param>
        public Tile(string type, bool stableTex)
        {
            Data = new TerrainTile(type, stableTex);
            Effect = new EffectTile();
        }

        /// <summary>
        /// Initializes a new Tile with terrain type and effect at the specified location.
        /// </summary>
        /// <param name="type">The terrain type ID.</param>
        /// <param name="loc">The tile location for effects.</param>
        public Tile(string type, Loc loc)
        {
            Data = new TerrainTile(type);
            Effect = new EffectTile(loc);
        }

        /// <summary>
        /// Creates a copy of another Tile.
        /// </summary>
        /// <param name="other">The Tile to copy.</param>
        protected Tile(Tile other)
        {
            Data = other.Data.Copy();
            Effect = new EffectTile(other.Effect);
        }

        /// <summary>
        /// Creates a copy of this Tile.
        /// </summary>
        /// <returns>A new Tile with the same data.</returns>
        public ITile Copy() { return new Tile(this); }

        /// <summary>
        /// Determines if this tile is equivalent to another based on terrain ID.
        /// </summary>
        /// <param name="other">The tile to compare.</param>
        /// <returns>True if both tiles have the same terrain ID; otherwise, false.</returns>
        public bool TileEquivalent(ITile other)
        {
            Tile tile = other as Tile;
            if (tile == null)
                return false;
            return tile.ID == ID;
        }

        /// <summary>
        /// Returns a string representation of this tile.
        /// </summary>
        /// <returns>A string containing the tile type and any effects.</returns>
        public override string ToString()
        {
            List<string> values = new List<string>();

            if (!String.IsNullOrEmpty(Data.ID))
                values.Add(DataManager.Instance.DataIndices[DataManager.DataType.Terrain].Get(Data.ID).Name.ToLocal());
            if (!String.IsNullOrEmpty(Effect.ID))
                values.Add(DataManager.Instance.DataIndices[DataManager.DataType.Tile].Get(Effect.ID).Name.ToLocal());
            string features = string.Join("/", values);
            return string.Format("{0}: {1}", this.GetType().GetFormattedTypeName(), features);
        }
    }
}
