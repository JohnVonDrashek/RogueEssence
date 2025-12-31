using System;
using RogueEssence.Dungeon;
using System.Collections.Generic;
using RogueElements;
using RogueEssence.Data;

namespace RogueEssence.LevelGen
{
    /// <summary>
    /// Sets the map's own events.  These events work similarly to the Universal Event, which works game-wide.
    /// These events work map-wide.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class MapEffectStep<T> : GenStep<T> where T : BaseMapGenContext
    {
        /// <summary>
        /// The object containing the events.
        /// </summary>
        [Dev.SubGroup]
        public ActiveEffect Effect;

        /// <summary>
        /// Initializes a new instance of the MapEffectStep class with an empty effect.
        /// </summary>
        public MapEffectStep()
        {
            Effect = new ActiveEffect();
        }

        /// <summary>
        /// Initializes a new instance of the MapEffectStep class with the specified effect.
        /// </summary>
        /// <param name="effect">The active effect to apply to the map.</param>
        public MapEffectStep(ActiveEffect effect)
        {
            Effect = effect;
        }

        /// <summary>
        /// Applies the map effect step, adding the effect to the map's event system.
        /// </summary>
        /// <param name="map">The map generation context to modify.</param>
        public override void Apply(T map)
        {
            map.Map.MapEffect.AddOther(Effect);
        }

        public override string ToString()
        {
            return string.Format("{0}[{1}]", this.GetType().GetFormattedTypeName(), this.Effect.GetTotalCount());
        }
    }
}
