using Newtonsoft.Json;
using NLua;
using RogueElements;
using RogueEssence.Data;
using RogueEssence.Dev;
using System;
using System.Collections.Generic;

namespace RogueEssence.Dungeon
{
    /// <summary>
    /// Abstract base class for map status states that store mutable map-wide effect data.
    /// </summary>
    [Serializable]
    public abstract class MapStatusState : GameplayState
    {

    }

    /// <summary>
    /// A map status state that tracks a countdown timer for duration-based map effects.
    /// </summary>
    [Serializable]
    public class MapCountDownState : MapStatusState
    {
        /// <summary>
        /// The countdown counter value.
        /// </summary>
        public int Counter;

        /// <summary>
        /// Initializes a new MapCountDownState with default values.
        /// </summary>
        public MapCountDownState() { }

        /// <summary>
        /// Initializes a new MapCountDownState with the specified counter.
        /// </summary>
        /// <param name="counter">The initial counter value.</param>
        public MapCountDownState(int counter) { Counter = counter; }

        /// <summary>
        /// Creates a copy of another MapCountDownState.
        /// </summary>
        /// <param name="other">The MapCountDownState to copy.</param>
        protected MapCountDownState(MapCountDownState other) { Counter = other.Counter; }

        /// <summary>
        /// Creates a clone of this state.
        /// </summary>
        /// <returns>A new MapCountDownState with the same values.</returns>
        public override GameplayState Clone() { return new MapCountDownState(this); }
    }

    /// <summary>
    /// A map status state that stores a target location on the map.
    /// </summary>
    [Serializable]
    public class MapLocState : MapStatusState
    {
        /// <summary>
        /// The target tile location.
        /// </summary>
        public Loc Target;

        /// <summary>
        /// Initializes a new MapLocState with default values.
        /// </summary>
        public MapLocState() { }

        /// <summary>
        /// Initializes a new MapLocState with the specified target location.
        /// </summary>
        /// <param name="target">The target location.</param>
        public MapLocState(Loc target) { Target = target; }

        /// <summary>
        /// Creates a copy of another MapLocState.
        /// </summary>
        /// <param name="other">The MapLocState to copy.</param>
        protected MapLocState(MapLocState other) { Target = other.Target; }

        /// <summary>
        /// Creates a clone of this state.
        /// </summary>
        /// <returns>A new MapLocState with the same values.</returns>
        public override GameplayState Clone() { return new MapLocState(this); }
    }

    /// <summary>
    /// A map status state marking a weather effect.
    /// </summary>
    [Serializable]
    public class MapWeatherState : MapStatusState
    {
        /// <summary>
        /// Initializes a new MapWeatherState.
        /// </summary>
        public MapWeatherState() { }

        /// <summary>
        /// Creates a clone of this state.
        /// </summary>
        /// <returns>A new MapWeatherState.</returns>
        public override GameplayState Clone() { return new MapWeatherState(); }
    }

    /// <summary>
    /// A map status state that stores an index value.
    /// </summary>
    [Serializable]
    public class MapIndexState : MapStatusState
    {
        /// <summary>
        /// The index value.
        /// </summary>
        public int Index;

        /// <summary>
        /// Initializes a new MapIndexState with default values.
        /// </summary>
        public MapIndexState() { }

        /// <summary>
        /// Initializes a new MapIndexState with the specified index.
        /// </summary>
        /// <param name="index">The index value.</param>
        public MapIndexState(int index) { Index = index; }

        /// <summary>
        /// Creates a copy of another MapIndexState.
        /// </summary>
        /// <param name="other">The MapIndexState to copy.</param>
        protected MapIndexState(MapIndexState other) { Index = other.Index; }

        /// <summary>
        /// Creates a clone of this state.
        /// </summary>
        /// <returns>A new MapIndexState with the same values.</returns>
        public override GameplayState Clone() { return new MapIndexState(this); }
    }

    /// <summary>
    /// A map status state that stores a reference to another map status by ID.
    /// </summary>
    [Serializable]
    public class MapIDState : MapStatusState
    {
        /// <summary>
        /// The referenced map status ID.
        /// </summary>
        [DataType(0, DataManager.DataType.MapStatus, false)]
        public string ID;

        /// <summary>
        /// Initializes a new MapIDState with an empty ID.
        /// </summary>
        public MapIDState() { ID = ""; }

        /// <summary>
        /// Initializes a new MapIDState with the specified ID.
        /// </summary>
        /// <param name="index">The map status ID.</param>
        public MapIDState(string index) { ID = index; }

        /// <summary>
        /// Creates a copy of another MapIDState.
        /// </summary>
        /// <param name="other">The MapIDState to copy.</param>
        protected MapIDState(MapIDState other) { ID = other.ID; }

        /// <summary>
        /// Creates a clone of this state.
        /// </summary>
        /// <returns>A new MapIDState with the same values.</returns>
        public override GameplayState Clone() { return new MapIDState(this); }
    }

    /// <summary>
    /// A map status state that stores check events for conditional behavior.
    /// </summary>
    [Serializable]
    public class MapCheckState : MapStatusState
    {
        /// <summary>
        /// The list of check events to evaluate.
        /// </summary>
        public List<SingleCharEvent> CheckEvents;

        /// <summary>
        /// Initializes a new MapCheckState with an empty event list.
        /// </summary>
        public MapCheckState() { CheckEvents = new List<SingleCharEvent>(); }

        /// <summary>
        /// Creates a copy of another MapCheckState.
        /// </summary>
        /// <param name="other">The MapCheckState to copy.</param>
        protected MapCheckState(MapCheckState other)
        {
            CheckEvents = new List<SingleCharEvent>();
            foreach (SingleCharEvent effect in other.CheckEvents)
                CheckEvents.Add((SingleCharEvent)effect.Clone());
        }

        /// <summary>
        /// Creates a clone of this state.
        /// </summary>
        /// <returns>A new MapCheckState with the same values.</returns>
        public override GameplayState Clone() { return new MapCheckState(this); }
    }
}
