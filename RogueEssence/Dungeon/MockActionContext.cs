using System.Collections.Generic;
using RogueElements;
using RogueEssence.Data;

namespace RogueEssence.Dungeon
{
    /// <summary>
    /// A simplified action context used for simulating or previewing actions without full battle processing.
    /// Implements IActionContext with minimal functionality for testing hitboxes and explosions.
    /// </summary>
    public class MockActionContext : IActionContext
    {
        /// <summary>
        /// Gets or sets the character performing the action.
        /// </summary>
        public Character User { get; set; }

        /// <summary>
        /// Gets or sets the target character of the action.
        /// </summary>
        public Character Target { get; set; }

        /// <summary>
        /// Gets or sets the final tile location where the strike ends.
        /// </summary>
        public Loc StrikeEndTile { get; set; }

        /// <summary>
        /// Gets or sets the list of tile locations affected by the strike.
        /// </summary>
        public List<Loc> StrikeLandTiles { get; set; }

        /// <summary>
        /// Gets or sets the type of battle action being performed.
        /// </summary>
        public BattleActionType ActionType { get; set; }

        /// <summary>
        /// Gets or sets the combat action defining the hitbox behavior.
        /// </summary>
        public CombatAction HitboxAction { get; set; }

        /// <summary>
        /// Gets or sets the explosion data for area-of-effect actions.
        /// </summary>
        public ExplosionData Explosion { get; set; }

        /// <summary>
        /// Gets or sets the battle data containing effects and properties.
        /// </summary>
        public BattleData Data { get; set; }

        /// <summary>
        /// Initializes a new MockActionContext with a user and hitbox action.
        /// </summary>
        /// <param name="user">The character performing the action.</param>
        /// <param name="hitboxAction">The combat action defining the hitbox.</param>
        public MockActionContext(Character user, CombatAction hitboxAction)
        {
            User = user;
            StrikeLandTiles = new List<Loc>();
            HitboxAction = hitboxAction;
        }

        /// <summary>
        /// Initializes a new MockActionContext with full action data.
        /// </summary>
        /// <param name="user">The character performing the action.</param>
        /// <param name="hitboxAction">The combat action defining the hitbox.</param>
        /// <param name="explosion">The explosion data for area effects.</param>
        /// <param name="data">The battle data for the action.</param>
        public MockActionContext(Character user, CombatAction hitboxAction, ExplosionData explosion, BattleData data)
        {
            User = user;
            StrikeLandTiles = new List<Loc>();
            HitboxAction = hitboxAction;
            Explosion = explosion;
            Data = data;
        }

        /// <summary>
        /// Targets a tile location with an explosion effect.
        /// </summary>
        /// <param name="target">The target tile location.</param>
        /// <returns>A coroutine that releases the explosion at the target location.</returns>
        public IEnumerator<YieldInstruction> TargetTileWithExplosion(Loc target)
        {
            //release explosion
            yield return CoroutineManager.Instance.StartCoroutine(Explosion.ReleaseExplosion(target, User, ProcessHitLoc, ProcessHitTile));
        }

        /// <summary>
        /// Processes a hit at the specified tile location, targeting any character present.
        /// </summary>
        /// <param name="loc">The location to process the hit.</param>
        /// <returns>A coroutine that processes the hit animation if a valid target is found.</returns>
        public IEnumerator<YieldInstruction> ProcessHitLoc(Loc loc)
        {
            Character charTarget = ZoneManager.Instance.CurrentMap.GetCharAtLoc(loc);
            if (charTarget != null && DungeonScene.Instance.IsTargeted(User, charTarget, Explosion.TargetAlignments))
            {
                Target = charTarget;
                yield return CoroutineManager.Instance.StartCoroutine(DungeonScene.Instance.ProcessEndAnim(User, Target, Data));//hit the character
            }
        }

        /// <summary>
        /// Processes a hit on a tile. This mock implementation does nothing.
        /// </summary>
        /// <param name="loc">The location of the tile hit.</param>
        /// <returns>An empty coroutine.</returns>
        public IEnumerator<YieldInstruction> ProcessHitTile(Loc loc)
        {
            yield break;
        }
    }
}
