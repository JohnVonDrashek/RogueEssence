using System;
using System.Collections.Generic;
using RogueElements;

namespace RogueEssence.LevelGen
{
    /// <summary>
    /// Spawns objects on tiles that are not connected to the main path.
    /// Mostly obsolete; use regular spawning and pick rooms marked as disconnected
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class DisconnectedSpawnStep<T, E, F> : BaseSpawnStep<T, E>
        where T : class, ITiledGenContext, IPlaceableGenContext<E>, IViewPlaceableGenContext<F>
        where E : ISpawnable
        where F : ISpawnable
    {
        /// <summary>
        /// Initializes a new instance of the DisconnectedSpawnStep class.
        /// </summary>
        public DisconnectedSpawnStep() { }

        /// <summary>
        /// Initializes a new instance of the DisconnectedSpawnStep class with the specified spawner.
        /// </summary>
        /// <param name="spawn">The spawner to use.</param>
        public DisconnectedSpawnStep(IStepSpawner<T, E> spawn) : base(spawn) { }

        /// <summary>
        /// Distributes spawns to tiles that are not connected to the main path.
        /// Uses flood fill from the first viewable location to identify disconnected areas.
        /// </summary>
        /// <param name="map">The generation context.</param>
        /// <param name="spawns">The list of spawns to distribute.</param>
        public override void DistributeSpawns(T map, List<E> spawns)
        {
            bool[][] connectionGrid = new bool[map.Width][];
            for (int xx = 0; xx < map.Width; xx++)
            {
                connectionGrid[xx] = new bool[map.Height];
                for (int yy = 0; yy < map.Height; yy++)
                    connectionGrid[xx][yy] = false;
            }

            //first mark all tiles in the main path
            Grid.FloodFill(new Rect(0, 0, map.Width, map.Height),
            (Loc testLoc) =>
            {
                if (connectionGrid[testLoc.X][testLoc.Y])
                    return true;

                return map.TileBlocked(testLoc);
            },
            (Loc testLoc) =>
            {
                return true;
            },
            (Loc fillLoc) =>
            {
                connectionGrid[fillLoc.X][fillLoc.Y] = true;
            },
            map.GetLoc(0));


            //obtain all tiles not in the main path
            List<Loc> freeTiles = new List<Loc>();

            for (int xx = 0; xx < map.Width; xx++)
            {
                for (int yy = 0; yy < map.Height; yy++)
                {
                    if (((IPlaceableGenContext<E>)map).CanPlaceItem(new Loc(xx,yy)) && !connectionGrid[xx][yy])
                        freeTiles.Add(new Loc(xx, yy));
                }
            }

            //spawn there
            for (int ii = 0; ii < spawns.Count && freeTiles.Count > 0; ii++)
            {
                E item = spawns[ii];

                int randIndex = map.Rand.Next(freeTiles.Count);
                map.PlaceItem(freeTiles[randIndex], item);
                freeTiles.RemoveAt(randIndex);
            }
        }
    }
}
