using System;
using RogueElements;
using AABB;
using RogueEssence.Dungeon;
using System.Collections.Generic;
using RogueEssence.Content;
using System.Runtime.Serialization;

namespace RogueEssence.Ground
{
    /// <summary>
    /// A layer that contains all entity types for a ground map.
    /// Entities include characters, objects, markers, and spawners.
    /// Supports both persistent entities (serialized) and temporary entities (runtime-only).
    /// </summary>
    [Serializable]
    public class EntityLayer : IMapLayer
    {
        /// <summary>
        /// Gets or sets the name of this entity layer.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets whether entities in this layer are visible.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// The list of persistent character entities on this layer.
        /// </summary>
        public List<GroundChar> MapChars;
        /// <summary>
        /// Field for character entities that should not be serialized
        /// </summary>
        [NonSerialized]
        public List<GroundChar> TemporaryChars;

        /// <summary>
        /// The list of persistent object entities on this layer.
        /// </summary>
        public List<GroundObject> GroundObjects;

        /// <summary>
        /// Field for object entities that should not be serialized
        /// </summary>
        [NonSerialized]
        public List<GroundObject> TemporaryObjects;

        /// <summary>
        /// Contains a list of all the NPCs spawners on this map
        /// </summary>
        public List<GroundSpawner> Spawners;

        /// <summary>
        /// A list of ground markers.
        /// </summary>
        public List<GroundMarker> Markers;

        /// <summary>
        /// Creates a new entity layer with the specified name.
        /// </summary>
        /// <param name="name">The name of this layer.</param>
        public EntityLayer(string name)
        {
            Name = name;
            Visible = true;

            GroundObjects = new List<GroundObject>();
            Markers = new List<GroundMarker>();
            MapChars = new List<GroundChar>();
            Spawners = new List<GroundSpawner>();
            TemporaryChars = new List<GroundChar>();
            TemporaryObjects = new List<GroundObject>();
        }

        /// <summary>
        /// Creates a copy of another entity layer.
        /// Note: The entity lists are initialized empty and not copied.
        /// </summary>
        /// <param name="other">The entity layer to copy from.</param>
        protected EntityLayer(EntityLayer other)
        {
            Name = other.Name;
            Visible = other.Visible;

            //just dont copy the contents
            GroundObjects = new List<GroundObject>();
            Markers = new List<GroundMarker>();
            MapChars = new List<GroundChar>();
            Spawners = new List<GroundSpawner>();
            TemporaryChars = new List<GroundChar>();
            TemporaryObjects = new List<GroundObject>();
        }

        /// <summary>
        /// Creates a clone of this entity layer.
        /// </summary>
        /// <returns>A new entity layer with the same name and visibility.</returns>
        public IMapLayer Clone() { return new EntityLayer(this); }

        /// <summary>
        /// Merges another layer into this one. Not implemented.
        /// </summary>
        /// <param name="other">The layer to merge.</param>
        /// <exception cref="NotImplementedException">Always thrown.</exception>
        public void Merge(IMapLayer other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the name of this layer.
        /// </summary>
        /// <returns>The layer name.</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Iterates through all character entities on this layer,
        /// including both persistent and temporary characters.
        /// </summary>
        /// <returns>An enumerable of all characters.</returns>
        public IEnumerable<GroundChar> IterateCharacters()
        {
            foreach (GroundChar player in MapChars)
                yield return player;

            foreach (GroundChar temp in TemporaryChars)
                yield return temp;
        }

        /// <summary>
        /// Iterates through all object entities on this layer,
        /// including both persistent and temporary objects.
        /// </summary>
        /// <returns>An enumerable of all objects.</returns>
        public IEnumerable<GroundObject> IterateObjects()
        {
            foreach (GroundObject v in GroundObjects)
                yield return v;

            foreach (GroundObject v in TemporaryObjects)
                yield return v;

        }


        /// <summary>
        /// Allow iterating through all entities on the map,
        /// characters, objects, markers
        /// </summary>
        /// <returns>An enumerable of all entities on this layer.</returns>
        public IEnumerable<GroundEntity> IterateEntities()
        {
            foreach (GroundEntity v in IterateCharacters())
                yield return v;

            foreach (GroundEntity v in GroundObjects)
                yield return v;

            foreach (GroundEntity v in TemporaryObjects)
                yield return v;

            foreach (GroundEntity v in Markers)
                yield return v;

            foreach (GroundEntity s in Spawners)
                yield return s;
        }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            //Make sure the temp char array is instantiated
            TemporaryChars = new List<GroundChar>();
        }
    }
}

