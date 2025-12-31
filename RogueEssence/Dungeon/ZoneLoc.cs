using Newtonsoft.Json;
using RogueEssence.Data;
using RogueEssence.Dev;
using System;

namespace RogueEssence.Dungeon
{
    /// <summary>
    /// Represents a complete location within a zone, including the zone ID, segment location, and entry point.
    /// Used for dungeon navigation and map transitions.
    /// </summary>
    [Serializable]
    public struct ZoneLoc
    {
        /// <summary>
        /// The unique identifier of the zone.
        /// </summary>
        [JsonConverter(typeof(DungeonConverter))]
        [DataType(0, DataManager.DataType.Zone, false)]
        public string ID;

        /// <summary>
        /// The segment and floor location within the zone.
        /// </summary>
        public SegLoc StructID;

        /// <summary>
        /// The entry point index for spawning at the location.
        /// </summary>
        public int EntryPoint;

        /// <summary>
        /// Initializes a new ZoneLoc with separate segment and floor ID values.
        /// </summary>
        /// <param name="id">The zone identifier.</param>
        /// <param name="structure">The segment index.</param>
        /// <param name="structId">The floor ID within the segment.</param>
        /// <param name="entryPoint">The entry point index.</param>
        public ZoneLoc(string id, int structure, int structId, int entryPoint)
        {
            ID = id;
            StructID = new SegLoc(structure, structId);
            EntryPoint = entryPoint;
        }

        /// <summary>
        /// Initializes a new ZoneLoc with a SegLoc and default entry point of 0.
        /// </summary>
        /// <param name="id">The zone identifier.</param>
        /// <param name="structId">The segment location.</param>
        public ZoneLoc(string id, SegLoc structId)
        {
            ID = id;
            StructID = structId;
            EntryPoint = 0;
        }

        /// <summary>
        /// Initializes a new ZoneLoc with full specification.
        /// </summary>
        /// <param name="id">The zone identifier.</param>
        /// <param name="structId">The segment location.</param>
        /// <param name="entryPoint">The entry point index.</param>
        public ZoneLoc(string id, SegLoc structId, int entryPoint)
        {
            ID = id;
            StructID = structId;
            EntryPoint = entryPoint;
        }


        private static readonly ZoneLoc invalid = new ZoneLoc("", new SegLoc(-1, -1), -1);

        /// <summary>
        /// Gets an invalid ZoneLoc instance.
        /// </summary>
        public static ZoneLoc Invalid { get { return invalid; } }

        /// <summary>
        /// Determines whether this ZoneLoc represents a valid location.
        /// </summary>
        /// <returns>True if the ID is not empty and the StructID is valid; otherwise, false.</returns>
        public bool IsValid()
        {
            return (!String.IsNullOrEmpty(ID)) && StructID.IsValid();
        }

        /// <summary>
        /// Returns a string representation of this ZoneLoc.
        /// </summary>
        /// <returns>A string containing the zone ID, segment location, and entry point.</returns>
        public override string ToString()
        {
            return String.Format("{0} {1} {2}", ID, StructID.ToString(), EntryPoint);
        }
    }
}