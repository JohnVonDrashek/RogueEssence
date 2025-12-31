namespace AABB
{
    using RogueElements;
	using System.Collections.Generic;
	using System.Linq;


	/// <summary>
	/// Implementation of IMovement that represents the result of a movement simulation.
	/// Contains information about collisions and the final destination after collision resolution.
	/// </summary>
	public class Movement : IMovement
	{
		/// <summary>
		/// Initializes a new instance of the Movement class with no hits.
		/// </summary>
		public Movement()
		{
			this.Hits = new IHit[0];
		}

		/// <summary>
		/// Gets or sets the collection of hits that occurred during movement.
		/// </summary>
		public IEnumerable<IHit> Hits { get; set; }

		/// <summary>
		/// Gets a value indicating whether any collisions occurred during the movement.
		/// </summary>
		public bool HasCollided { get { return this.Hits.Any(); } }

		/// <summary>
		/// Gets or sets the original position before the movement.
		/// </summary>
		public Rect Origin { get; set; }

		/// <summary>
		/// Gets or sets the actual final position after collision resolution.
		/// </summary>
		public Rect Destination { get; set; }

		/// <summary>
		/// Gets or sets the intended destination of the movement.
		/// </summary>
		public Rect Goal { get; set; }
	}
}

