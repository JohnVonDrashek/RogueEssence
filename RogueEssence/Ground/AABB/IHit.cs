namespace AABB
{
    using System;
	
    using RogueElements;


	/// <summary>
	/// Represents a fraction used for precise collision time calculation.
	/// Stores numerator and denominator separately to avoid floating-point precision issues.
	/// </summary>
    public struct Multiplier : IEquatable<Multiplier>
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
		/// Gets the minimum representable value.
		/// </summary>
        public static Multiplier MinValue { get { return new Multiplier(0, 0); } }

		/// <summary>
		/// Gets the maximum representable value.
		/// </summary>
        public static Multiplier MaxValue { get { return new Multiplier(1, 0); } }

		/// <summary>
		/// Determines whether this instance equals the specified object.
		/// </summary>
		/// <param name="obj">The object to compare.</param>
		/// <returns>True if equal; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Multiplier)
            {
                return Equals((Multiplier)obj);
            }

            return false;
        }
		/// <summary>
		/// Determines whether this instance equals another Multiplier.
		/// </summary>
		/// <param name="other">The Multiplier to compare.</param>
		/// <returns>True if equal; otherwise, false.</returns>
        public bool Equals(Multiplier other)
        {
            return (this == other);
        }

		/// <summary>
		/// Determines if two Multipliers are equal.
		/// </summary>
        public static bool operator ==(Multiplier a, Multiplier b)
        {
            return (a.Numerator == b.Numerator) && (a.Denominator == b.Denominator);
        }

		/// <summary>
		/// Determines if two Multipliers are not equal.
		/// </summary>
        public static bool operator !=(Multiplier a, Multiplier b)
        {
            return !(a == b);
        }

		/// <summary>
		/// Determines if one Multiplier is less than another.
		/// </summary>
        public static bool operator <(Multiplier a, Multiplier b)
        {
            if (a == MinValue || b == MaxValue)
                return a != b;
            else if (a == MaxValue || b == MinValue)
                return false;

            if ((b.Denominator < 0) != (a.Denominator < 0))
                return (a.Numerator * b.Denominator > b.Numerator * a.Denominator);
            else
                return (a.Numerator * b.Denominator < b.Numerator * a.Denominator);
        }

		/// <summary>
		/// Determines if one Multiplier is greater than another.
		/// </summary>
        public static bool operator >(Multiplier a, Multiplier b)
        {
            if (a == MinValue || b == MaxValue)
                return false;
            else if (a == MaxValue || b == MinValue)
                return a != b;

            if ((b.Denominator < 0) != (a.Denominator < 0))
                return (a.Numerator * b.Denominator < b.Numerator * a.Denominator);
            else
                return (a.Numerator * b.Denominator > b.Numerator * a.Denominator);
        }

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>A hash code.</returns>
        public override int GetHashCode()
        {
            return Numerator.GetHashCode() ^ Numerator.GetHashCode();
        }

		/// <summary>
		/// Returns the smaller of two Multipliers.
		/// </summary>
		/// <param name="value1">The first value.</param>
		/// <param name="value2">The second value.</param>
		/// <returns>The smaller Multiplier.</returns>
        public static Multiplier Min(Multiplier value1, Multiplier value2)
        {
            if (value1 < value2)
                return value1;
            else
                return value2;
        }

		/// <summary>
		/// Returns the larger of two Multipliers.
		/// </summary>
		/// <param name="value1">The first value.</param>
		/// <param name="value2">The second value.</param>
		/// <returns>The larger Multiplier.</returns>
        public static Multiplier Max(Multiplier value1, Multiplier value2)
        {
            if (value1 > value2)
                return value1;
            else
                return value2;
        }
    }
	/// <summary>
	/// Represents a hit point out of a collision.
	/// </summary>
	public interface IHit
	{
		/// <summary>
		/// Gets the collided box.
		/// </summary>
		/// <value>The box.</value>
		IObstacle Box { get; }

		/// <summary>
		/// Gets the normal vector of the collided box side.
		/// </summary>
		/// <value>The normal.</value>
		Dir4 Normal { get;  }

		/// <summary>
		/// Gets the amount of movement needed from origin to get the impact position.
		/// </summary>
		/// <value>The amount.</value>
        Multiplier Amount { get; }

		/// <summary>
		/// Gets the impact position.
		/// </summary>
		/// <value>The position.</value>
		Loc Position { get;  }

		/// <summary>
		/// Indicates whether the hit point is nearer than an other from a given point. Warning: this should only be used
		/// for multiple calculation of the same box movement (amount is compared first, then distance).
		/// </summary>
		/// <returns><c>true</c>, if nearest was ised, <c>false</c> otherwise.</returns>
		/// <param name="than">Than.</param>
		/// <param name="from">From.</param>
		bool IsNearest(IHit than, Loc from);
	}
}

