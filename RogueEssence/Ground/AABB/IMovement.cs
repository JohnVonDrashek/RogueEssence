namespace AABB
{
    using System.Collections.Generic;
    using RogueElements;


	/// <summary>
	/// Represents the result of a movement simulation with collision detection.
	/// Contains information about hits encountered and the final destination.
	/// </summary>
	public interface IMovement
	{
		/// <summary>
		/// Gets the collection of hits that occurred during the movement.
		/// </summary>
		IEnumerable<IHit> Hits { get; }

		/// <summary>
		/// Gets a value indicating whether any collisions occurred during the movement.
		/// </summary>
		bool HasCollided { get; }

		/// <summary>
		/// Gets the original position before the movement.
		/// </summary>
		Rect Origin { get; }

		/// <summary>
		/// Gets the intended destination of the movement.
		/// </summary>
		Rect Goal { get; }

		/// <summary>
		/// Gets the actual final position after collision resolution.
		/// </summary>
		Rect Destination { get; }
	}
}

