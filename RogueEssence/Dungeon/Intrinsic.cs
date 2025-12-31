using System;
using Newtonsoft.Json;
using RogueEssence.Data;
using RogueEssence.Dev;

namespace RogueEssence.Dungeon
{
    /// <summary>
    /// Represents an intrinsic ability that a character possesses, providing passive effects during gameplay.
    /// </summary>
    [Serializable]
    public class Intrinsic : PassiveActive
    {
        /// <summary>
        /// Gets the event cause type for intrinsic abilities.
        /// </summary>
        /// <returns>The Intrinsic event cause.</returns>
        public override GameEventPriority.EventCause GetEventCause()
        {
            return GameEventPriority.EventCause.Intrinsic;
        }

        /// <summary>
        /// Gets the passive data associated with this intrinsic.
        /// </summary>
        /// <returns>The intrinsic's passive data from the data manager.</returns>
        public override PassiveData GetData() { return DataManager.Instance.GetIntrinsic(ID); }

        /// <summary>
        /// Gets the colored display name of this intrinsic.
        /// </summary>
        /// <returns>The intrinsic's display name with color formatting.</returns>
        public override string GetDisplayName() { return DataManager.Instance.GetIntrinsic(ID).GetColoredName(); }

        /// <summary>
        /// Gets the unique identifier of this intrinsic.
        /// </summary>
        /// <returns>The intrinsic's ID as a string.</returns>
        public override string GetID() { return ID.ToString(); }

        /// <summary>
        /// The unique identifier for this intrinsic ability.
        /// </summary>
        [JsonConverter(typeof(IntrinsicConverter))]
        [DataType(0, DataManager.DataType.Intrinsic, false)]
        public string ID { get; set; }

        /// <summary>
        /// Initializes a new empty Intrinsic.
        /// </summary>
        public Intrinsic() : base()
        {
            ID = "";
        }

        /// <summary>
        /// Initializes a new Intrinsic with the specified ID.
        /// </summary>
        /// <param name="index">The intrinsic's unique identifier.</param>
        public Intrinsic(string index)
        {
            ID = index;
        }

    }
}
