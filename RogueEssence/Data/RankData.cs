using RogueEssence.Dev;
using System;

namespace RogueEssence.Data
{
    /// <summary>
    /// Represents a player rank/tier that determines bag size and progression.
    /// Ranks are earned through fame points and unlock larger inventory capacity.
    /// </summary>
    [Serializable]
    public class RankData : IEntryData
    {
        public override string ToString()
        {
            return Name.ToLocal();
        }

        public LocalText Name { get; set; }
        public bool Released { get { return true; } }
        [Dev.Multiline(0)]
        public string Comment { get; set; }

        public int BagSize;
        public int FameToNext;

        [DataType(0, DataManager.DataType.Rank, false)]
        public string Next;

        public EntrySummary GenerateEntrySummary() { return new EntrySummary(Name, Released, Comment); }

        public RankData()
        {
            Name = new LocalText();
            Comment = "";
        }

        public RankData(LocalText name, int bagSize, int fameToNext, string next)
        {
            Name = name;
            Comment = "";
            Next = next;
            BagSize = bagSize;
            FameToNext = fameToNext;
        }

        public string GetColoredName()
        {
            return String.Format("[color=#FFA5FF]{0}[color]", Name.ToLocal());
        }
    }
}
