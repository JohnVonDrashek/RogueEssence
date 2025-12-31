namespace RogueEssence.Dungeon
{
    /// <summary>
    /// Represents a fractional multiplier used for damage, stat modifications, and other calculations.
    /// Automatically reduces fractions and handles special cases like neutralization.
    /// </summary>
    public struct Multiplier
    {
        /// <summary>
        /// The numerator of the fraction.
        /// </summary>
        public int Numerator;

        /// <summary>
        /// The denominator of the fraction.
        /// </summary>
        public int Denominator;

        /// <summary>
        /// Initializes a new Multiplier with the specified numerator and denominator.
        /// </summary>
        /// <param name="num">The numerator.</param>
        /// <param name="den">The denominator.</param>
        public Multiplier(int num, int den)
        {
            Numerator = num;
            Denominator = den;
        }

        /// <summary>
        /// Adds (multiplies) another fractional multiplier to this one.
        /// Automatically reduces the result and handles special values.
        /// </summary>
        /// <param name="num">The numerator to multiply by.</param>
        /// <param name="den">The denominator to multiply by.</param>
        public void AddMultiplier(int num, int den)
        {
            if (num == 0)
            {
                Numerator = 0;
                Denominator = 1;
            }
            else if (num < 0)
            {
                Numerator = -1;
                Denominator = 1;
            }
            else if (Numerator > 0)
            {
                Numerator *= num;
                Denominator *= den;
                for (int ii = 5; ii > 1; ii--)
                {
                    if (Numerator % ii == 0 && Denominator % ii == 0)
                    {
                        Numerator /= ii;
                        Denominator /= ii;
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the multiplier has been neutralized (reduced to zero or negative).
        /// </summary>
        /// <returns>True if the multiplier is neutralized; otherwise, false.</returns>
        public bool IsNeutralized()
        {
            return Numerator <= 0;
        }

        /// <summary>
        /// Applies this multiplier to a base amount.
        /// </summary>
        /// <param name="baseAmt">The base amount to multiply.</param>
        /// <returns>The multiplied result, or -1 if the multiplier is special.</returns>
        public int Multiply(int baseAmt)
        {
            if (Numerator == -1)
                return -1;
            else
                return baseAmt * Numerator / Denominator;
        }

        /// <summary>
        /// Returns a string representation of this multiplier as a fraction.
        /// </summary>
        /// <returns>A string in the format "numerator/denominator".</returns>
        public override string ToString()
        {
            return string.Format("{0}/{1}", Numerator, Denominator);
        }
    }

}
