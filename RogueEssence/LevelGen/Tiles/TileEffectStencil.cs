using System;
using System.Collections.Generic;
using RogueElements;
using RogueEssence;
using RogueEssence.Data;
using RogueEssence.Dev;
using RogueEssence.Dungeon;

namespace RogueEssence.LevelGen
{
    /// <summary>
    /// A filter for determining the eligible tiles for an operation.
    /// Tiles must or must not have a panel.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class TileEffectStencil<TGenContext> : ITerrainStencil<TGenContext>
        where TGenContext : BaseMapGenContext
    {
        /// <summary>
        /// Initializes a new instance of the TileEffectStencil class.
        /// </summary>
        public TileEffectStencil()
        {
        }

        /// <summary>
        /// Initializes a new instance of the TileEffectStencil class with the specified inversion setting.
        /// </summary>
        /// <param name="not">If true, test passes for tiles without effects; if false, passes for tiles with effects.</param>
        public TileEffectStencil(bool not)
        {
            this.Not = not;
        }

        /// <summary>
        /// If turned on, test will pass for empty tiles.
        /// </summary>
        public bool Not { get; private set; }

        /// <summary>
        /// Tests whether the tile has an effect based on the Not setting.
        /// </summary>
        /// <param name="map">The map generation context.</param>
        /// <param name="loc">The location to test.</param>
        /// <returns>True if the test passes based on the Not setting and presence of tile effects.</returns>
        public bool Test(TGenContext map, Loc loc)
        {
            Tile checkTile = (Tile)map.GetTile(loc);
            return (String.IsNullOrEmpty(checkTile.Effect.ID) == this.Not);
        }

        public override string ToString()
        {
            if (this.Not)
                return string.Format("Tiles without Effects");
            return string.Format("Tiles with Effects");
        }
    }

    /// <summary>
    /// A filter for determining the eligible tiles for an operation.
    /// Tiles must have a specific panel.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class MatchTileEffectStencil<TGenContext> : ITerrainStencil<TGenContext>
        where TGenContext : BaseMapGenContext
    {
        /// <summary>
        /// Initializes a new instance of the MatchTileEffectStencil class.
        /// </summary>
        public MatchTileEffectStencil()
        {
        }

        /// <summary>
        /// Initializes a new instance of the MatchTileEffectStencil class with the specified effect to match.
        /// </summary>
        /// <param name="effect">The effect ID to match against.</param>
        public MatchTileEffectStencil(string effect)
        {
            this.Effect = effect;
        }

        /// <summary>
        /// The tile effect ID that tiles must have to pass the test.
        /// </summary>
        [DataType(0, DataManager.DataType.Tile, false)]
        public string Effect;

        /// <summary>
        /// Tests whether the tile has the specified effect.
        /// </summary>
        /// <param name="map">The map generation context.</param>
        /// <param name="loc">The location to test.</param>
        /// <returns>True if the tile's effect matches the specified Effect ID.</returns>
        public bool Test(TGenContext map, Loc loc)
        {
            Tile checkTile = (Tile)map.GetTile(loc);
            return (checkTile.Effect.ID == this.Effect);
        }

        public override string ToString()
        {
            if (this.Effect == null)
                return string.Format("Match [EMPTY]");
            return string.Format("Match {0}", this.Effect.ToString());
        }
    }
}
