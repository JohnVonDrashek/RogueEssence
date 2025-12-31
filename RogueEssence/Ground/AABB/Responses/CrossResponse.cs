namespace AABB
{
    using RogueElements;

	/// <summary>
	/// A collision response that allows the moving box to pass through the obstacle.
	/// The box continues to its intended destination ignoring the collision.
	/// </summary>
	public class CrossResponse : ICollisionResponse
	{
		/// <summary>
		/// Creates a cross response for the given collision, allowing passage through the obstacle.
		/// </summary>
		/// <param name="collision">The collision information to respond to.</param>
		public CrossResponse(ICollision collision)
		{
			this.Destination = collision.Goal;
		}


		/// <summary>
		/// Gets the destination, which is the original intended goal (passes through obstacle).
		/// </summary>
		public Rect Destination { get; private set; }
	}
}

