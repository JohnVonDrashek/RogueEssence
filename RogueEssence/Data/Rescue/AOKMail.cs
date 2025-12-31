using System;
using System.Collections.Generic;
using RogueEssence.Dungeon;

namespace RogueEssence.Data
{
    /// <summary>
    /// Represents an A-OK (Acknowledgment) mail confirming a successful rescue.
    /// Contains data about both the rescued team and the rescuing team.
    /// </summary>
    [Serializable]
    public class AOKMail : BaseRescueMail
    {
        /// <summary>
        /// Gets the file extension for AOK mail files.
        /// </summary>
        public override string Extension { get { return DataManager.AOK_EXTENSION; } }

        /// <summary>
        /// The name of the team that performed the rescue.
        /// </summary>
        public string RescuingTeam;

        /// <summary>
        /// The unique identifier of the rescuing player.
        /// </summary>
        public string RescuingID;

        /// <summary>
        /// The date when the rescue was completed.
        /// </summary>
        public string DateRescued;

        /// <summary>
        /// Names of the rescuing team members.
        /// </summary>
        public string[] RescuingNames;

        /// <summary>
        /// Monster IDs of the rescuing team members.
        /// </summary>
        public MonsterID[] RescuingProfile;

        /// <summary>
        /// Personality discriminators of the rescuing team members.
        /// </summary>
        public int[] RescuingPersonalities;

        /// <summary>
        /// The index of the final statement message.
        /// </summary>
        public int FinalStatement;

        /// <summary>
        /// The replay data of the rescue mission.
        /// </summary>
        public ReplayData RescueReplay;

        /// <summary>
        /// Initializes a new empty instance of the AOKMail class.
        /// </summary>
        public AOKMail()
        { }

        /// <summary>
        /// Initializes a new AOK mail from an SOS mail after successful rescue.
        /// </summary>
        /// <param name="sos">The original SOS mail that was responded to.</param>
        /// <param name="progress">The game progress of the rescuing team.</param>
        /// <param name="dateTime">The date and time of the rescue.</param>
        /// <param name="replay">The replay data of the rescue mission.</param>
        public AOKMail(SOSMail sos, GameProgress progress, string dateTime, ReplayData replay)
        {
            TeamName = sos.TeamName;
            TeamID = sos.TeamID;
            Seed = sos.Seed;
            TurnsTaken = sos.TurnsTaken;
            RescueSeed = sos.RescueSeed;
            DateDefeated = sos.DateDefeated;
            Goal = sos.Goal;
            DefeatedVersion = sos.DefeatedVersion;
            OfferedItem = sos.OfferedItem;
            GoalText = sos.GoalText;
            TeamProfile = sos.TeamProfile;

            RescuingTeam = progress.ActiveTeam.Name;
            RescuingID = progress.UUID;
            DateRescued = dateTime;
            List<string> teamNames = new List<string>();
            List<MonsterID> teamProfile = new List<MonsterID>();
            List<int> teamPersonalities = new List<int>();
            foreach (Character chara in progress.ActiveTeam.Players)
            {
                teamNames.Add(chara.BaseName);
                teamProfile.Add(chara.BaseForm);
                teamPersonalities.Add(chara.Discriminator);
            }
            RescuingNames = teamNames.ToArray();
            RescuingProfile = teamProfile.ToArray();
            RescuingPersonalities = teamPersonalities.ToArray();

            RescueReplay = replay;
        }
    }
}
