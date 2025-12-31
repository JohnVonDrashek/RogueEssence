using System.Collections.Generic;
using RogueElements;

namespace RogueEssence.LevelGen
{
    /// <summary>
    /// Interface for generation contexts that support post-processing tiles.
    /// Post-processing tiles track tile status flags for terrain, panels, and items.
    /// </summary>
    public interface IPostProcGenContext : ITiledGenContext
    {
        /// <summary>
        /// Gets the post-processing tile data at the specified location.
        /// </summary>
        /// <param name="loc">The location to retrieve post-processing data for.</param>
        /// <returns>The post-processing tile at the specified location.</returns>
        PostProcTile GetPostProc(Loc loc);

        /// <summary>
        /// Gets the 2D grid of post-processing tiles for the entire map.
        /// </summary>
        PostProcTile[][] PostProcGrid { get; }
    }

    /// <summary>
    /// Interface for generation contexts that support unbreakable terrain.
    /// Unbreakable terrain cannot be modified by generation steps.
    /// </summary>
    public interface IUnbreakableGenContext : ITiledGenContext
    {
        /// <summary>
        /// Gets the tile type used for unbreakable terrain.
        /// </summary>
        ITile UnbreakableTerrain { get; }
    }

    /// <summary>
    /// Interface for generation contexts that support placing groups of spawnable entities.
    /// </summary>
    /// <typeparam name="E">The type of group spawnable entity to place.</typeparam>
    public interface IGroupPlaceableGenContext<E> : ITiledGenContext
        where E : IGroupSpawnable
    {
        /// <summary>
        /// Gets a list of free tile locations within the specified rectangle.
        /// </summary>
        /// <param name="rect">The rectangular area to search for free tiles.</param>
        /// <returns>A list of locations where items can be placed.</returns>
        List<Loc> GetFreeTiles(Rect rect);

        /// <summary>
        /// Determines whether an item can be placed at the specified location.
        /// </summary>
        /// <param name="loc">The location to check.</param>
        /// <returns>True if an item can be placed at the location; otherwise, false.</returns>
        bool CanPlaceItem(Loc loc);

        /// <summary>
        /// Places a batch of spawnable items at the specified locations.
        /// </summary>
        /// <param name="itemBatch">The batch of items to place.</param>
        /// <param name="locs">The array of locations to place items at.</param>
        void PlaceItems(E itemBatch, Loc[] locs);
    }

    /// <summary>
    /// Marker interface for entities that can be spawned as a group.
    /// Implementations represent batches of related entities spawned together.
    /// </summary>
    public interface IGroupSpawnable
    {

    }
}
