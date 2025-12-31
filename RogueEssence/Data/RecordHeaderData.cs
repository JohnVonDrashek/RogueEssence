using System;
using System.Collections.Generic;

namespace RogueEssence.Data
{
    /// <summary>
    /// Contains metadata for a game replay record, including player info, score, and run details.
    /// Used to display replay entries in lists and manage high score tables.
    /// </summary>
    public class RecordHeaderData
    {
        /// <summary>
        /// Maximum number of high scores to track per dungeon.
        /// </summary>
        public const int MAX_HIGH_SCORES = 12;

        /// <summary>
        /// The game version when this record was created.
        /// </summary>
        public Version Version;

        /// <summary>
        /// The player's team name.
        /// </summary>
        public string Name;

        /// <summary>
        /// The date and time when the run ended.
        /// </summary>
        public string DateTimeString;

        /// <summary>
        /// The location where the run ended.
        /// </summary>
        public string LocationString;

        /// <summary>
        /// The zone/dungeon ID where the run took place.
        /// </summary>
        public string Zone;

        /// <summary>
        /// The final score achieved in the run.
        /// </summary>
        public int Score;

        /// <summary>
        /// The file path to the replay data.
        /// </summary>
        public string Path;

        /// <summary>
        /// Whether this was a rogue mode run.
        /// </summary>
        public bool IsRogue;

        /// <summary>
        /// Whether this run used a specific seed.
        /// </summary>
        public bool IsSeeded;

        /// <summary>
        /// The random seed used for the run.
        /// </summary>
        public ulong Seed;

        /// <summary>
        /// Whether the player marked this replay as a favorite.
        /// </summary>
        public bool IsFavorite;

        /// <summary>
        /// The outcome of the run (cleared, failed, etc.).
        /// </summary>
        public GameProgress.ResultType Result;

        /// <summary>
        /// Initializes a new instance of the RecordHeaderData class.
        /// </summary>
        /// <param name="path">The file path to the replay data.</param>
        public RecordHeaderData(string path)
        {
            Path = path;
            Name = "";
            DateTimeString = "";
            LocationString = "";
        }

        /// <summary>
        /// Generate all high score tables in real time, organized by dungeon, by going through all replays
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, List<RecordHeaderData>> LoadHighScores()
        {
            Dictionary<string, List<RecordHeaderData>> highScores = new Dictionary<string, List<RecordHeaderData>>();

            List<RecordHeaderData> records = DataManager.Instance.GetRecordHeaders(PathMod.ModSavePath(DataManager.REPLAY_PATH), DataManager.REPLAY_EXTENSION);
            
            foreach (RecordHeaderData record in records)
            {
                if (record.IsRogue && !record.IsSeeded)
                {
                    if (!highScores.ContainsKey(record.Zone))
                        highScores[record.Zone] = new List<RecordHeaderData>();

                    List<RecordHeaderData> dungeonHighScore = highScores[record.Zone];

                    int placing = 0;
                    for (int ii = 0; ii < dungeonHighScore.Count; ii++)
                    {
                        if (dungeonHighScore[ii].Score <= record.Score)
                            break;
                        placing++;
                    }
                    dungeonHighScore.Insert(placing, record);
                    if (dungeonHighScore.Count > RecordHeaderData.MAX_HIGH_SCORES)
                        dungeonHighScore.RemoveRange(RecordHeaderData.MAX_HIGH_SCORES, dungeonHighScore.Count - RecordHeaderData.MAX_HIGH_SCORES);
                }
            }

            return highScores;
        }
    }
}
