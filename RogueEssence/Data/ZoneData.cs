using System;
using System.Collections.Generic;
using RogueEssence.LevelGen;
using RogueEssence.Dungeon;
using RogueEssence.Script;
using System.Runtime.Serialization;
using RogueEssence.Dev;

namespace RogueEssence.Data
{
    /// <summary>
    /// Defines the rogue mode compatibility and transfer permissions for a zone.
    /// </summary>
    public enum RogueStatus
    {
        /// <summary>
        /// Disallowed for Rogue mode.
        /// </summary>
        None,
        /// <summary>
        /// Allowed for rogue mode, cannot transfer anything.
        /// </summary>
        NoTransfer,
        /// <summary>
        /// Allowed for rogue mode, can only transfer items to main save.
        /// </summary>
        ItemTransfer,
        /// <summary>
        /// Allowed for rogue mode, can transfer items and characters to main save.
        /// </summary>
        AllTransfer
    }

    /// <summary>
    /// Interface for zone data with dungeon restrictions and settings.
    /// </summary>
    public interface IZoneData : IEntryData
    {
        /// <summary>
        /// Percent modifier for experience gain.
        /// </summary>
        int ExpPercent { get; set; }

        /// <summary>
        /// Recommended level for the zone.
        /// </summary>
        int Level { get; set; }

        /// <summary>
        /// Whether the player is restricted to one team member.
        /// </summary>
        bool TeamRestrict { get; set; }

        /// <summary>
        /// Maximum team size override.
        /// </summary>
        int TeamSize { get; set; }

        /// <summary>
        /// Whether money must be stored on entry.
        /// </summary>
        bool MoneyRestrict { get; set; }

        /// <summary>
        /// Number of items to keep from bag on entry.
        /// </summary>
        int BagRestrict { get; set; }

        /// <summary>
        /// Maximum bag size in the zone.
        /// </summary>
        int BagSize { get; set; }

        /// <summary>
        /// Number of rescue attempts allowed.
        /// </summary>
        int Rescues { get; set; }

        /// <summary>
        /// Rogue mode compatibility setting.
        /// </summary>
        RogueStatus Rogue { get; set; }

    }

    /// <summary>
    /// Contains all data for a dungeon zone, including floors, restrictions, and settings.
    /// Zones are the main gameplay areas containing dungeon segments.
    /// </summary>
    [Serializable]
    public class ZoneData : IEntryData
    {
        /// <summary>
        /// Returns the localized name of the zone.
        /// </summary>
        /// <returns>The zone name as a string.</returns>
        public override string ToString()
        {
            return Name.ToLocal();
        }

        /// <summary>
        /// The localized name of the zone.
        /// </summary>
        public LocalText Name { get; set; }

        /// <summary>
        /// Whether the zone is released for gameplay.
        /// </summary>
        public bool Released { get; set; }

        /// <summary>
        /// Developer comments for this zone.
        /// </summary>
        [Dev.Multiline(0)]
        public string Comment { get; set; }

        /// <summary>
        /// Turn on to disable EXP gain in the dungeon.
        /// </summary>
        [NonEdited]
        public bool NoEXP { get; set; }

        /// <summary>
        /// Percent to multiply EXP gain for the dungeon.
        /// 0 means no EXP.
        /// </summary>
        public int ExpPercent { get; set; }

        /// <summary>
        /// The recommended level to face the dungeon.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Turn on to cap the team at the recommended level.
        /// </summary>
        public bool LevelCap { get; set; }

        /// <summary>
        /// Turn on to keep the teams moveset during level restrictions.
        /// </summary>
        [SharedRow]
        public bool KeepSkills { get; set; }
        
        /// <summary>
        /// Turn on to force the player to enter with 1 team member.
        /// </summary>
        public bool TeamRestrict { get; set; }

        /// <summary>
        /// Overrides the normal maximum team size.
        /// </summary>
        public int TeamSize { get; set; }

        /// <summary>
        /// Forces all money to be stored on entry.
        /// </summary>
        public bool MoneyRestrict { get; set; }

        /// <summary>
        /// Forces items beyond the Nth slot to be stored upon entry.
        /// </summary>
        public int BagRestrict { get; set; }

        /// <summary>
        /// Exempts Treasure items from BagRestrict limit
        /// </summary>
        public bool KeepTreasure { get; set; }

        /// <summary>
        /// Forces the bag's maximum size.
        /// </summary>
        public int BagSize { get; set; }

        /// <summary>
        /// Turn this on for the zone to remember map layouts and load the old state when returning to the floor.
        /// It's not nice on memory though...
        /// </summary>
        public bool Persistent { get; set; }

        /// <summary>
        /// Rescues allowed for this zone.
        /// </summary>
        public int Rescues { get; set; }

        /// <summary>
        /// Determines if the dungeon can be played in Rogue mode, and what can be transferred.
        /// </summary>
        public RogueStatus Rogue { get; set; }

        /// <summary>
        /// Generates a summary of this zone for indexing and quick access.
        /// </summary>
        /// <returns>A ZoneEntrySummary containing the zone's key metadata.</returns>
        public EntrySummary GenerateEntrySummary()
        {
            int totalFloors = 0;
            foreach (ZoneSegmentBase structure in Segments)
            {
                if (structure.IsRelevant)
                    totalFloors += structure.FloorCount;
            }
            ZoneEntrySummary summary = new ZoneEntrySummary(Name, Released, Comment);
            summary.ExpPercent = ExpPercent;
            summary.Level = Level;
            summary.LevelCap = LevelCap;
            summary.KeepSkills = KeepSkills;
            summary.TeamRestrict = TeamRestrict;
            summary.TeamSize = TeamSize;
            summary.MoneyRestrict = MoneyRestrict;
            summary.BagRestrict = BagRestrict;
            summary.KeepTreasure = KeepTreasure;
            summary.BagSize = BagSize;
            summary.Rescues = Rescues;
            summary.CountedFloors = totalFloors;
            summary.Rogue = Rogue;
            summary.Grounds.AddRange(GroundMaps);
            for (int ii = 0; ii < Segments.Count; ii++)
            {
                if (Segments[ii].FloorCount < 0)
                    summary.Maps.Add(null);
                else
                {
                    HashSet<int> floors = new HashSet<int>();
                    foreach (int id in Segments[ii].GetFloorIDs())
                        floors.Add(id);
                    summary.Maps.Add(floors);
                }
            }
            return summary;
        }

        /// <summary>
        /// Sections of the dungeon.
        /// Ex. Splitting the dungeon into a normal and deeper section.
        /// </summary>
        [Collection(0, true)]
        public List<ZoneSegmentBase> Segments;
        
        /// <summary>
        /// Ground maps associated with this dungeon.
        /// Ex. Cutscene rooms for pre-boss events.
        /// </summary>
        [Dev.DataFolder(1, "Ground/")]
        public List<string> GroundMaps;


        /// <summary>
        /// Initializes a new instance of the ZoneData class with default values.
        /// </summary>
        public ZoneData()
        {
            Name = new LocalText();
            Comment = "";

            ExpPercent = 100;
            Level = -1;
            TeamSize = -1;
            BagRestrict = -1;
            BagSize = -1;

            Segments = new List<ZoneSegmentBase>();
            GroundMaps = new List<string>();
        }

        /// <summary>
        /// Gets the display name with orange color formatting.
        /// </summary>
        /// <returns>The formatted name string with color tags.</returns>
        public string GetColoredName()
        {
            return String.Format("[color=#FFC663]{0}[color]", Name.ToLocal());
        }


        /// <summary>
        /// Creates an active Zone instance from this zone data.
        /// </summary>
        /// <param name="seed">The random seed for the zone.</param>
        /// <param name="zoneIndex">The zone identifier.</param>
        /// <returns>A new Zone instance with this data's settings.</returns>
        public Zone CreateActiveZone(ulong seed, string zoneIndex)
        {
            Zone zone = new Zone(seed, zoneIndex);
            zone.Name = Name;

            zone.ExpPercent = ExpPercent;
            zone.Level = Level;
            zone.LevelCap = LevelCap;
            zone.KeepSkills = KeepSkills;
            zone.TeamRestrict = TeamRestrict;
            zone.TeamSize = TeamSize;
            zone.MoneyRestrict = MoneyRestrict;
            zone.BagRestrict = BagRestrict;
            zone.KeepTreasure = KeepTreasure;
            zone.BagSize = BagSize;
            zone.Persistent = Persistent;

            //NOTE: these are not deep copies!
            zone.Segments = Segments;
            zone.GroundMaps = GroundMaps;
            return zone;
        }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            //TODO: remove on v1.1
            if (Serializer.OldVersion < new Version(0, 7, 22))
            {
                if (!NoEXP)
                    ExpPercent = 100;
            }
        }
    }


    /// <summary>
    /// Summary data for a zone entry, including restrictions and floor counts.
    /// Used for quick access without loading full zone data.
    /// </summary>
    [Serializable]
    public class ZoneEntrySummary : EntrySummary
    {
        /// <summary>
        /// Percent modifier for experience gain.
        /// </summary>
        public int ExpPercent;

        /// <summary>
        /// Recommended level for the zone.
        /// </summary>
        public int Level;

        /// <summary>
        /// Whether level capping is enabled.
        /// </summary>
        public bool LevelCap;

        /// <summary>
        /// Whether to keep skills during level restrictions.
        /// </summary>
        public bool KeepSkills;

        /// <summary>
        /// Whether team is restricted to one member.
        /// </summary>
        public bool TeamRestrict;

        /// <summary>
        /// Maximum team size override.
        /// </summary>
        public int TeamSize;

        /// <summary>
        /// Whether money must be stored on entry.
        /// </summary>
        public bool MoneyRestrict;

        /// <summary>
        /// Number of items to keep from bag on entry.
        /// </summary>
        public int BagRestrict;

        /// <summary>
        /// Whether treasure items are exempt from bag restrictions.
        /// </summary>
        public bool KeepTreasure;

        /// <summary>
        /// Maximum bag size in the zone.
        /// </summary>
        public int BagSize;

        /// <summary>
        /// Number of rescue attempts allowed.
        /// </summary>
        public int Rescues;

        /// <summary>
        /// Total number of floors across all segments.
        /// </summary>
        public int CountedFloors;

        /// <summary>
        /// Rogue mode compatibility setting.
        /// </summary>
        public RogueStatus Rogue;

        /// <summary>
        /// List of ground map names in this zone.
        /// </summary>
        public List<string> Grounds;

        /// <summary>
        /// List of floor ID sets for each segment.
        /// </summary>
        public List<HashSet<int>> Maps;

        /// <summary>
        /// Initializes a new empty instance of the ZoneEntrySummary class.
        /// </summary>
        public ZoneEntrySummary() : base()
        {
            Grounds = new List<string>();
            Maps = new List<HashSet<int>>();
        }

        /// <summary>
        /// Initializes a new instance of the ZoneEntrySummary class with the specified values.
        /// </summary>
        /// <param name="name">The localized name of the zone.</param>
        /// <param name="released">Whether the zone is released for gameplay.</param>
        /// <param name="comment">Developer comment for this zone.</param>
        public ZoneEntrySummary(LocalText name, bool released, string comment)
            : base(name, released, comment)
        {
            Grounds = new List<string>();
            Maps = new List<HashSet<int>>();
        }

        /// <summary>
        /// Gets the floor count for a specific segment.
        /// </summary>
        /// <param name="segidx">The segment index.</param>
        /// <returns>The number of floors in the segment, or -1 if undefined.</returns>
        public int GetFloorCount(int segidx)
        {
            if (Maps[segidx] == null)
                return -1;
            return Maps[segidx].Count;
        }

        /// <summary>
        /// Checks if a segment location is valid within this zone.
        /// </summary>
        /// <param name="segLoc">The segment location to validate.</param>
        /// <returns>True if the location is valid.</returns>
        public bool SegLocValid(SegLoc segLoc)
        {
            if (segLoc.Segment == -1)
                return (0 <= segLoc.ID && segLoc.ID < Grounds.Count);
            else if (0 <= segLoc.Segment && segLoc.Segment < Maps.Count)
            {
                if (Maps[segLoc.Segment] == null)
                    return true;
                return Maps[segLoc.Segment].Contains(segLoc.ID);
            }
            return false;
        }

        /// <summary>
        /// Checks if a ground map name is valid within this zone.
        /// </summary>
        /// <param name="groundName">The ground map name to validate.</param>
        /// <returns>True if the ground map exists in this zone.</returns>
        public bool GroundValid(string groundName)
        {
            return Grounds.Contains(groundName);
        }


        /// <summary>
        /// Gets the display name with orange color formatting.
        /// </summary>
        /// <returns>The formatted name string with color tags.</returns>
        public override string GetColoredName()
        {
            return String.Format("[color=#FFC663]{0}[color]", Name.ToLocal());
        }
    }

}
