using System;
using System.Collections.Generic;
using RogueEssence.Dungeon;

namespace RogueEssence.Data
{
    /// <summary>
    /// Represents an SOS (Save Our Souls) rescue request mail.
    /// Created when a team is defeated and can be used by other players to attempt a rescue.
    /// </summary>
    [Serializable]
    public class SOSMail : BaseRescueMail
    {
        /// <summary>
        /// Gets the file extension for SOS mail files.
        /// </summary>
        public override string Extension { get { return DataManager.SOS_EXTENSION; } }

        /// <summary>
        /// The ID of the player who rescued this team (if rescued).
        /// </summary>
        public string RescuedBy;

        /// <summary>
        /// Names of the rescuing team members.
        /// </summary>
        public string[] RescuingNames;

        /// <summary>
        /// Monster IDs of the rescuing team.
        /// </summary>
        public MonsterID[] RescuingTeam;

        /// <summary>
        /// Personality discriminators of the rescuing team members.
        /// </summary>
        public int[] RescuingPersonalities;

        /// <summary>
        /// The index of the final statement message shown after rescue.
        /// </summary>
        public int FinalStatement;

        /// <summary>
        /// Initializes a new empty instance of the SOSMail class.
        /// </summary>
        public SOSMail()
        { }

        /// <summary>
        /// Initializes a new SOS mail from the current game progress.
        /// </summary>
        /// <param name="progress">The game progress of the defeated team.</param>
        /// <param name="goal">The location where the team needs rescue.</param>
        /// <param name="goalText">Localized description of the rescue goal.</param>
        /// <param name="dateTime">The date and time of defeat.</param>
        /// <param name="version">The mod versions active during the run.</param>
        public SOSMail(GameProgress progress, ZoneLoc goal, LocalText goalText, string dateTime, List<ModVersion> version)
        {
            TeamName = progress.ActiveTeam.Name;
            TeamID = progress.UUID;
            DateDefeated = dateTime;
            DefeatedVersion = version;

            List<MonsterID> teamProfile = new List<MonsterID>();
            foreach (Character chara in progress.ActiveTeam.Players)
                teamProfile.Add(chara.BaseForm);
            TeamProfile = teamProfile.ToArray();

            Seed = progress.Rand.FirstSeed;
            TurnsTaken = progress.TotalTurns;
            Goal = goal;
            GoalText = goalText;
            OfferedItem = new MapItem();
        }
    }

}
