using System;
using System.Collections.Generic;
using RogueEssence.Content;
using RogueElements;
using RogueEssence.Data;
using RogueEssence.Dev;
using Newtonsoft.Json;

namespace RogueEssence.Dungeon
{
    /// <summary>
    /// Represents a status effect applied to the entire map, such as weather or traps.
    /// Map statuses affect all characters and tiles on the current floor.
    /// </summary>
    [Serializable]
    public class MapStatus : PassiveActive
    {
        /// <summary>
        /// Gets the event cause type for map statuses.
        /// </summary>
        /// <returns>The MapState event cause.</returns>
        public override GameEventPriority.EventCause GetEventCause()
        {
            return GameEventPriority.EventCause.MapState;
        }

        /// <summary>
        /// Gets the passive data for this map status.
        /// </summary>
        /// <returns>The map status data from the data manager.</returns>
        public override PassiveData GetData() { return DataManager.Instance.GetMapStatus(ID); }

        /// <summary>
        /// Gets the localized display name for this map status.
        /// </summary>
        /// <returns>The map status name.</returns>
        public override string GetDisplayName() { return DataManager.Instance.GetMapStatus(ID).Name.ToLocal(); }

        /// <summary>
        /// Gets the unique identifier for this map status.
        /// </summary>
        /// <returns>The map status ID.</returns>
        public override string GetID() { return ID; }

        /// <summary>
        /// The unique identifier for this map status.
        /// </summary>
        [JsonConverter(typeof(MapStatusConverter))]
        [DataType(0, DataManager.DataType.MapStatus, false)]
        public string ID { get; set; }

        /// <summary>
        /// The collection of states attached to this map status.
        /// </summary>
        public StateCollection<MapStatusState> StatusStates;

        /// <summary>
        /// The visual emitter for this map status.
        /// </summary>
        public SwitchOffEmitter Emitter;

        /// <summary>
        /// Whether this map status is hidden from the player.
        /// </summary>
        public bool Hidden;

        /// <summary>
        /// Initializes a new empty MapStatus.
        /// </summary>
        public MapStatus() : base()
        {
            ID = "";
            StatusStates = new StateCollection<MapStatusState>();
            Emitter = new Content.EmptySwitchOffEmitter();
        }

        /// <summary>
        /// Initializes a new MapStatus with the specified ID.
        /// </summary>
        /// <param name="index">The map status ID.</param>
        public MapStatus(string index) : this()
        {
            ID = index;
        }

        /// <summary>
        /// Loads default states and emitter from the data definition.
        /// </summary>
        public void LoadFromData()
        {
            MapStatusData entry = DataManager.Instance.GetMapStatus(ID);

            foreach (MapStatusState state in entry.StatusStates)
            {
                if (!StatusStates.Contains(state.GetType()))
                    StatusStates.Set(state.Clone<MapStatusState>());
            }

            Emitter = (SwitchOffEmitter)entry.Emitter.Clone();//Clone Use Case; convert to Instantiate?
            Hidden = entry.DefaultHidden;
        }

        /// <summary>
        /// Starts the visual emitter for this map status.
        /// </summary>
        /// <param name="effects">The effect layers to add the emitter to.</param>
        public void StartEmitter(List<IFinishableSprite>[] effects)
        {
            Emitter.SetupEmit(Loc.Zero, Loc.Zero, Dir8.Down);
            effects[(int)DrawLayer.NoDraw].Add(Emitter);
        }

        /// <summary>
        /// Stops the visual emitter for this map status.
        /// </summary>
        public void EndEmitter()
        {
            Emitter.SwitchOff();
        }
    }
}

