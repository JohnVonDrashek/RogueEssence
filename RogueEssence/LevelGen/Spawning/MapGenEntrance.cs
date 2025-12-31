using System;
using RogueElements;
using RogueEssence.Dungeon;

namespace RogueEssence.LevelGen
{
    /// <summary>
    /// Represents an entrance point on a generated map where the player spawns.
    /// </summary>
    [Serializable]
    public class MapGenEntrance : IEntrance
    {
        /// <summary>
        /// The location of the entrance on the map.
        /// </summary>
        [Dev.NonEdited]
        public Loc Loc { get; set; }

        /// <summary>
        /// The direction the player faces when entering.
        /// </summary>
        public Dir8 Dir { get; set; }

        /// <summary>
        /// Initializes a new instance of the MapGenEntrance class.
        /// </summary>
        public MapGenEntrance() { }

        /// <summary>
        /// Initializes a new instance of the MapGenEntrance class with the specified direction.
        /// </summary>
        /// <param name="dir">The facing direction.</param>
        public MapGenEntrance(Dir8 dir)
        {
            Dir = dir;
        }

        /// <summary>
        /// Initializes a new instance of the MapGenEntrance class with the specified location and direction.
        /// </summary>
        /// <param name="loc">The entrance location.</param>
        /// <param name="dir">The facing direction.</param>
        public MapGenEntrance(Loc loc, Dir8 dir)
        {
            Loc = loc;
            Dir = dir;
        }

        /// <summary>
        /// Initializes a new instance of the MapGenEntrance class as a copy of another.
        /// </summary>
        /// <param name="other">The entrance to copy.</param>
        protected MapGenEntrance(MapGenEntrance other)
        {
            Loc = other.Loc;
            Dir = other.Dir;
        }

        /// <summary>
        /// Creates a copy of this entrance.
        /// </summary>
        /// <returns>A new MapGenEntrance with copied data.</returns>
        public ISpawnable Copy() { return new MapGenEntrance(this); }

        /// <summary>
        /// Returns a string representation of this entrance.
        /// </summary>
        /// <returns>A string describing the entrance direction.</returns>
        public override string ToString()
        {
            return String.Format("Entrance: {0}", Dir.ToString());
        }
    }

    /// <summary>
    /// Represents an exit point on a generated map that leads to the next floor or area.
    /// </summary>
    [Serializable]
    public class MapGenExit : IExit
    {
        /// <summary>
        /// The location of the exit on the map.
        /// </summary>
        [Dev.NonEdited]
        public Loc Loc { get; set; }

        /// <summary>
        /// The effect tile that represents this exit (e.g., stairs).
        /// </summary>
        [Dev.SubGroup]
        public EffectTile Tile { get; set; }

        /// <summary>
        /// Initializes a new instance of the MapGenExit class.
        /// </summary>
        public MapGenExit() { Tile = new EffectTile(); }

        /// <summary>
        /// Initializes a new instance of the MapGenExit class with the specified tile.
        /// </summary>
        /// <param name="tile">The effect tile for this exit.</param>
        public MapGenExit(EffectTile tile)
        {
            Tile = tile;
        }

        /// <summary>
        /// Initializes a new instance of the MapGenExit class with the specified location and tile.
        /// </summary>
        /// <param name="loc">The exit location.</param>
        /// <param name="tile">The effect tile for this exit.</param>
        public MapGenExit(Loc loc, EffectTile tile)
        {
            Loc = loc;
            Tile = tile;
        }

        /// <summary>
        /// Initializes a new instance of the MapGenExit class as a copy of another.
        /// </summary>
        /// <param name="other">The exit to copy.</param>
        protected MapGenExit(MapGenExit other)
        {
            Loc = other.Loc;
            Tile = (EffectTile)other.Tile.Copy();
        }

        /// <summary>
        /// Creates a copy of this exit.
        /// </summary>
        /// <returns>A new MapGenExit with copied data.</returns>
        public ISpawnable Copy() { return new MapGenExit(this); }

        /// <summary>
        /// Returns a string representation of this exit.
        /// </summary>
        /// <returns>A string describing the exit tile.</returns>
        public override string ToString()
        {
            return String.Format("Exit: {0}", Tile.ToString());
        }
    }
}
