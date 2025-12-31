using System;
using RogueElements;

namespace RogueEssence.LevelGen
{
    /// <summary>
    /// Represents a spawnable money pickup with a specific amount.
    /// </summary>
    [Serializable]
    public struct MoneySpawn : ISpawnable
    {
        /// <summary>
        /// The amount of money in this spawn.
        /// </summary>
        public int Amount;

        /// <summary>
        /// Initializes a new MoneySpawn with the specified amount.
        /// </summary>
        /// <param name="amount">The amount of money.</param>
        public MoneySpawn(int amount)
        {
            Amount = amount;
        }

        /// <summary>
        /// Initializes a new MoneySpawn as a copy of another.
        /// </summary>
        /// <param name="other">The MoneySpawn to copy.</param>
        public MoneySpawn(MoneySpawn other)
        {
            Amount = other.Amount;
        }

        /// <summary>
        /// Creates a copy of this MoneySpawn.
        /// </summary>
        /// <returns>A new MoneySpawn with the same amount.</returns>
        public ISpawnable Copy() { return new MoneySpawn(this); }
    }
}
