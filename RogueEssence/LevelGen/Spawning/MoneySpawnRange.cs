using System;
using System.Collections;
using System.Collections.Generic;
using RogueElements;

namespace RogueEssence.LevelGen
{
    /// <summary>
    /// Selects a MoneySpawn with an amount in a predefined range.
    /// Used to randomly determine how much money spawns on a floor.
    /// </summary>
    [Serializable]
    public struct MoneySpawnRange : IRandPicker<MoneySpawn>
    {
        /// <summary>
        /// The minimum amount of money (inclusive).
        /// </summary>
        public int Min;

        /// <summary>
        /// The maximum amount of money (exclusive).
        /// </summary>
        public int Max;

        /// <summary>
        /// Gets a value indicating whether this picker changes state when picking.
        /// Always returns false.
        /// </summary>
        public bool ChangesState { get { return false; } }

        /// <summary>
        /// Gets a value indicating whether this picker can pick a value.
        /// Returns true if Min is less than or equal to Max.
        /// </summary>
        public bool CanPick { get { return Min <= Max; } }

        /// <summary>
        /// Initializes a new MoneySpawnRange with a fixed amount.
        /// </summary>
        /// <param name="num">The fixed money amount.</param>
        public MoneySpawnRange(int num) { Min = num; Max = num; }

        /// <summary>
        /// Initializes a new MoneySpawnRange with the specified range.
        /// </summary>
        /// <param name="min">The minimum amount (inclusive).</param>
        /// <param name="max">The maximum amount (exclusive).</param>
        public MoneySpawnRange(int min, int max) { Min = min; Max = max; }

        /// <summary>
        /// Initializes a new MoneySpawnRange from a RandRange.
        /// </summary>
        /// <param name="other">The RandRange to copy from.</param>
        public MoneySpawnRange(RandRange other)
        {
            Min = other.Min;
            Max = other.Max;
        }

        /// <summary>
        /// Initializes a new MoneySpawnRange as a copy of another.
        /// </summary>
        /// <param name="other">The MoneySpawnRange to copy.</param>
        public MoneySpawnRange(MoneySpawnRange other)
        {
            Min = other.Min;
            Max = other.Max;
        }

        /// <summary>
        /// Creates a copy of this picker's state.
        /// </summary>
        /// <returns>A new MoneySpawnRange with the same values.</returns>
        public IRandPicker<MoneySpawn> CopyState() { return new MoneySpawnRange(this); }

        /// <summary>
        /// Enumerates all possible money spawn outcomes from Min to Max.
        /// </summary>
        /// <returns>An enumerable of all possible MoneySpawn values.</returns>
        public IEnumerable<MoneySpawn> EnumerateOutcomes()
        {
            yield return new MoneySpawn(Min);
            for (int ii = Min + 1; ii < Max; ii++)
                yield return new MoneySpawn(ii);
        }

        /// <summary>
        /// Picks a random MoneySpawn with an amount in the range.
        /// </summary>
        /// <param name="rand">The random number generator to use.</param>
        /// <returns>A MoneySpawn with a random amount in the range.</returns>
        public MoneySpawn Pick(IRandom rand) { return new MoneySpawn(rand.Next(Min, Max)); }

        /// <summary>
        /// Returns a string representation of this range.
        /// </summary>
        /// <returns>A string in the format "[Min,Max)".</returns>
        public override string ToString()
        {
            return string.Format("[{0},{1})", this.Min, this.Max);
        }
    }
}
