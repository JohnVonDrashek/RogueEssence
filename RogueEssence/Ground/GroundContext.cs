using RogueEssence.Dungeon;

namespace RogueEssence.Ground
{
    /// <summary>
    /// Represents the context for item usage in ground mode.
    /// Contains information about the item being used, the character using it, and the target.
    /// </summary>
    public class GroundContext : GameContext
    {
        /// <summary>
        /// Gets or sets the ground character who owns/initiated the item use.
        /// </summary>
        public GroundChar Owner;

        /// <summary>
        /// Gets or sets the inventory item being used.
        /// </summary>
        public InvItem Item;

        /// <summary>
        /// Creates a new ground context for item usage.
        /// </summary>
        /// <param name="item">The item being used.</param>
        /// <param name="owner">The ground character using the item.</param>
        /// <param name="target">The target character of the item use.</param>
        public GroundContext(InvItem item, GroundChar owner, Character target)
        {
            Item = item;
            Owner = owner;
            User = target;
        }

    }
}