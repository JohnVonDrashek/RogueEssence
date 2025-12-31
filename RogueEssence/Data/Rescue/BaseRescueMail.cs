using System;
using System.Collections.Generic;
using RogueEssence.Dungeon;

namespace RogueEssence.Data
{
    /// <summary>
    /// Abstract base class for rescue mail messages (SOS and AOK).
    /// Contains common data about the team that needs rescue and the rescue location.
    /// </summary>
    [Serializable]
    public abstract class BaseRescueMail
    {
        /// <summary>
        /// Gets the file extension for this type of mail.
        /// </summary>
        public abstract string Extension { get; }

        /// <summary>
        /// The name of the team that needs rescue.
        /// </summary>
        public string TeamName { get; set; }

        /// <summary>
        /// The unique identifier of the team that needs rescue.
        /// </summary>
        public string TeamID { get; set; }

        /// <summary>
        /// The random seed used for the dungeon run.
        /// </summary>
        public ulong Seed { get; set; }

        /// <summary>
        /// Additional seed for rescue-specific randomization.
        /// </summary>
        public byte RescueSeed { get; set; }

        /// <summary>
        /// Total turns taken before defeat.
        /// </summary>
        public int TurnsTaken { get; set; }

        /// <summary>
        /// The date when the team was defeated.
        /// </summary>
        public string DateDefeated { get; set; }

        /// <summary>
        /// The zone location where the team needs to be rescued.
        /// </summary>
        public ZoneLoc Goal { get; set; }

        /// <summary>
        /// The mod versions active when the team was defeated.
        /// </summary>
        public List<ModVersion> DefeatedVersion { get; set; }

        /// <summary>
        /// The item offered as a reward for rescue.
        /// </summary>
        public MapItem OfferedItem { get; set; }

        /// <summary>
        /// Localized text describing the rescue goal.
        /// </summary>
        public LocalText GoalText { get; set; }

        /// <summary>
        /// Monster IDs of the team members that need rescue.
        /// </summary>
        public MonsterID[] TeamProfile { get; set; }
    }
}
