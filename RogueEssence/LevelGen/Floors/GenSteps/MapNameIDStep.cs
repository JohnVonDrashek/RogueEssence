using System;
using RogueElements;
using RogueEssence.Dev;

namespace RogueEssence.LevelGen
{
    /// <summary>
    /// Sets the Title of the floor, taking in an offset for ID substitutions.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class MapNameIDStep<T> : GenStep<T> where T : BaseMapGenContext
    {
        /// <summary>
        /// The title of the map.
        /// Can include one string format subtituion for floor number.
        /// </summary>
        [SubGroup]
        public LocalText Name;

        /// <summary>
        /// The amount to add to the map ID to get the floor number substituted into the title.
        /// </summary>
        public int IDOffset;


        /// <summary>
        /// Initializes a new instance of the MapNameIDStep class.
        /// </summary>
        public MapNameIDStep()
        {
            Name = new LocalText();

        }

        /// <summary>
        /// Initializes a new instance of the MapNameIDStep class with the specified name and ID offset.
        /// </summary>
        /// <param name="name">The name template for the map.</param>
        /// <param name="idOffset">The offset to add to the map ID for display purposes.</param>
        public MapNameIDStep(LocalText name, int idOffset)
        {
            Name = new LocalText(name);
            IDOffset = idOffset;
        }

        /// <summary>
        /// Initializes a new instance of the MapNameIDStep class with the specified name.
        /// </summary>
        /// <param name="name">The name template for the map.</param>
        public MapNameIDStep(LocalText name)
        {
            Name = new LocalText(name);
        }

        /// <summary>
        /// Applies the map name step, setting the map's title with the floor number substituted.
        /// </summary>
        /// <param name="map">The map generation context to modify.</param>
        public override void Apply(T map)
        {
            map.Map.Name = LocalText.FormatLocalText(Name, (map.Map.ID + IDOffset).ToString());
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", this.GetType().GetFormattedTypeName(), Name.ToLocal());
        }
    }


}
