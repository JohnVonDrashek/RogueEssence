using System;
using System.Collections.Generic;
using RogueElements;
using RogueEssence;
using RogueEssence.Dungeon;

namespace RogueEssence.LevelGen
{
    /// <summary>
    /// A filter for determining the eligible tiles for an operation.
    /// Tiles must or must not have a panel.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class StairsStencil<TGenContext> : IBlobStencil<TGenContext>
        where TGenContext : StairsMapGenContext
    {
        /// <summary>
        /// Initializes a new instance of the StairsStencil class.
        /// </summary>
        public StairsStencil()
        {
        }

        /// <summary>
        /// Initializes a new instance of the StairsStencil class with the specified inversion setting.
        /// </summary>
        /// <param name="not">If true, test passes for tiles without stairs; if false, passes for tiles with stairs.</param>
        public StairsStencil(bool not)
        {
            this.Not = not;
        }

        /// <summary>
        /// If turned on, test will pass for empty tiles.
        /// </summary>
        public bool Not { get; private set; }

        /// <summary>
        /// Tests whether the blob contains entrances or exits based on the Not setting.
        /// </summary>
        /// <param name="map">The map generation context.</param>
        /// <param name="rect">The bounding rectangle for the blob.</param>
        /// <param name="blobTest">The test function for the blob.</param>
        /// <returns>True if the test passes based on the Not setting and presence of stairs.</returns>
        public bool Test(TGenContext map, Rect rect, Grid.LocTest blobTest)
        {
            foreach (MapGenEntrance ent in map.GenEntrances)
            {
                if (blobTest(ent.Loc))
                    return !this.Not;
            }
            foreach (MapGenExit ent in map.GenExits)
            {
                if (blobTest(ent.Loc))
                    return !this.Not;
            }

            return this.Not;
        }

        public override string ToString()
        {
            if (this.Not)
                return string.Format("No Stairs within Blob");
            return string.Format("Stairs within Blob");
        }
    }
}
