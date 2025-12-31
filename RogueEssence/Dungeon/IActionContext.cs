using System.Collections.Generic;
using RogueElements;
using RogueEssence.Data;

namespace RogueEssence.Dungeon
{
    /// <summary>
    /// Interface defining the context for a battle action, including the user, target, and action data.
    /// </summary>
    public interface IActionContext
    {
        /// <summary>
        /// Gets or sets the character performing the action.
        /// </summary>
        Character User { get; set; }

        /// <summary>
        /// Gets or sets the target character of the action.
        /// </summary>
        Character Target { get; set; }

        /// <summary>
        /// Gets or sets the final tile location where the strike ends.
        /// </summary>
        Loc StrikeEndTile { get; set; }

        /// <summary>
        /// Gets or sets the list of tile locations affected by the strike.
        /// </summary>
        List<Loc> StrikeLandTiles { get; set; }

        /// <summary>
        /// Gets or sets the type of battle action being performed.
        /// </summary>
        BattleActionType ActionType { get; set; }

        /// <summary>
        /// Gets or sets the combat action defining the hitbox behavior.
        /// </summary>
        CombatAction HitboxAction { get; set; }

        /// <summary>
        /// Gets or sets the explosion data for area-of-effect actions.
        /// </summary>
        ExplosionData Explosion { get; set; }

        /// <summary>
        /// Gets or sets the battle data containing effects and properties.
        /// </summary>
        BattleData Data { get; set; }

        /// <summary>
        /// Targets a tile location with an explosion effect.
        /// </summary>
        /// <param name="target">The target tile location.</param>
        /// <returns>A coroutine for the explosion effect.</returns>
        IEnumerator<YieldInstruction> TargetTileWithExplosion(Loc target);

        /// <summary>
        /// Processes a hit at the specified tile location.
        /// </summary>
        /// <param name="loc">The location to process the hit.</param>
        /// <returns>A coroutine for processing the hit.</returns>
        IEnumerator<YieldInstruction> ProcessHitLoc(Loc loc);
    }
}
