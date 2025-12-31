using RogueElements;
using System;
using System.Collections;
using System.Collections.Generic;

namespace RogueEssence.LevelGen
{
    /// <summary>
    /// Selects an integer in a predefined range.  Starts with minimum and continually rolls until a failure.  Chance of higher numbers decays exponentially.
    /// </summary>
    [Serializable]
    public struct RandDecay : IRandPicker<int>, IEquatable<RandDecay>
    {
        /// <summary>
        /// The minimum value that can be picked.
        /// </summary>
        public int Min;

        /// <summary>
        /// The maximum value that can be picked.
        /// </summary>
        public int Max;

        /// <summary>
        /// The percentage chance (0-100) to roll again for a higher number.
        /// </summary>
        public int Rate;

        /// <summary>
        /// Initializes a new RandDecay that always returns the specified number.
        /// </summary>
        /// <param name="num">The fixed number to return.</param>
        public RandDecay(int num)
        {
            this.Min = num;
            this.Max = num;
            this.Rate = 0;
        }

        /// <summary>
        /// Initializes a new RandDecay with the specified range and decay rate.
        /// </summary>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <param name="rate">The percentage chance to roll for a higher number.</param>
        public RandDecay(int min, int max, int rate)
        {
            this.Min = min;
            this.Max = max;
            this.Rate = rate;
        }

        /// <summary>
        /// Initializes a new RandDecay as a copy of another.
        /// </summary>
        /// <param name="other">The RandDecay to copy.</param>
        public RandDecay(RandDecay other)
        {
            this.Min = other.Min;
            this.Max = other.Max;
            this.Rate = other.Rate;
        }

        /// <summary>
        /// Gets an empty RandDecay that always returns 0.
        /// </summary>
        public static RandDecay Empty => new RandDecay(0);

        /// <summary>
        /// Gets a value indicating whether this picker changes state when picking.
        /// Always returns false.
        /// </summary>
        public bool ChangesState => false;

        /// <summary>
        /// Gets a value indicating whether this picker can pick a value.
        /// Returns true if Min is less than or equal to Max.
        /// </summary>
        public bool CanPick => this.Min <= this.Max;

        /// <summary>
        /// Determines whether two RandDecay instances are equal.
        /// </summary>
        public static bool operator ==(RandDecay lhs, RandDecay rhs) => lhs.Equals(rhs);

        /// <summary>
        /// Determines whether two RandDecay instances are not equal.
        /// </summary>
        public static bool operator !=(RandDecay lhs, RandDecay rhs) => !lhs.Equals(rhs);

        /// <summary>
        /// Creates a copy of this picker's state.
        /// </summary>
        /// <returns>A new RandDecay with the same values.</returns>
        public IRandPicker<int> CopyState() => new RandDecay(this);

        /// <summary>
        /// Enumerates all possible outcomes from Min to Max.
        /// </summary>
        /// <returns>An enumerable of all possible integers in the range.</returns>
        public IEnumerable<int> EnumerateOutcomes()
        {
            yield return this.Min;
            for (int ii = this.Min + 1; ii <= this.Max; ii++)
                yield return ii;
        }

        /// <summary>
        /// Picks a random integer using exponential decay probability.
        /// Starts at Min and repeatedly rolls for a chance to increment.
        /// </summary>
        /// <param name="rand">The random number generator to use.</param>
        /// <returns>A randomly selected integer in the range.</returns>
        public int Pick(IRandom rand)
        {
            int cur = this.Min;
            while (cur < this.Max)
            {
                if (rand.Next(100) < this.Rate)
                    cur++;
                else
                    break;
            }
            return cur;
        }

        /// <summary>
        /// Determines whether this RandDecay equals another.
        /// </summary>
        /// <param name="other">The RandDecay to compare.</param>
        /// <returns>True if equal; otherwise, false.</returns>
        public bool Equals(RandDecay other) => this.Min == other.Min && this.Max == other.Max && this.Rate == other.Rate;

        /// <summary>
        /// Determines whether this RandDecay equals another object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>True if equal; otherwise, false.</returns>
        public override bool Equals(object obj) => (obj is RandDecay) && this.Equals((RandDecay)obj);

        /// <summary>
        /// Gets the hash code for this RandDecay.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode() => unchecked(191 + (this.Min.GetHashCode() * 313) ^ (this.Max.GetHashCode() * 739));

        /// <summary>
        /// Returns a string representation of this RandDecay.
        /// </summary>
        /// <returns>A string in the format "Min+Rate%^(Max-Min)".</returns>
        public override string ToString()
        {
            return string.Format("{0}+{1}%^{2}", this.Min, this.Rate, this.Max - this.Min);
        }
    }
}
