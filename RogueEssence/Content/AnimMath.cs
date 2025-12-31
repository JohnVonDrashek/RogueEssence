using System;

namespace RogueEssence.Content
{
    /// <summary>
    /// Provides mathematical utility functions for animation calculations,
    /// including arc trajectories and linear interpolation.
    /// </summary>
    public class AnimMath
    {
        /// <summary>
        /// Calculates the height of a parabolic arc at a given horizontal position.
        /// Used for projectile trajectories and jumping animations.
        /// </summary>
        /// <param name="maxHeight">The maximum height of the arc at its peak.</param>
        /// <param name="touchdownX">The horizontal distance at which the arc returns to ground level.</param>
        /// <param name="currentX">The current horizontal position along the arc.</param>
        /// <returns>The calculated height at the current position, rounded to the nearest integer.</returns>
        public static int GetArc(double maxHeight, double touchdownX, double currentX)
        {
            // = (-4 * m / (n ^ 2) ) * x ^ 2 + (4 * m / n) * x
            // m = height, n = total time, x = current time
            double height = -4 * maxHeight * Math.Pow(currentX / touchdownX, 2) + 4 * maxHeight * (currentX / touchdownX);
            return (int)Math.Round(height);
        }

        /// <summary>
        /// Performs linear interpolation between two integer values.
        /// </summary>
        /// <param name="int1">The starting value when point is 0.</param>
        /// <param name="int2">The ending value when point is 1.</param>
        /// <param name="point">The interpolation factor, typically between 0 and 1.</param>
        /// <returns>The interpolated value, rounded to the nearest integer.</returns>
        public static int Lerp(int int1, int int2, double point)
        {
            return (int)Math.Round(int1 * (1 - point) + int2 * point);
        }
    }
}
