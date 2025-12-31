namespace AABB
{
    using RogueElements;


	/// <summary>
	/// A collision response that stops the moving box at the collision point.
	/// The box ends its movement exactly where it first touched the obstacle.
	/// </summary>
	public class TouchResponse : ICollisionResponse
	{
		/// <summary>
		/// Creates a touch response for the given collision, stopping at the collision point.
		/// </summary>
		/// <param name="collision">The collision information to respond to.</param>
		public TouchResponse(ICollision collision)
		{
			this.Destination = new Rect(collision.Hit.Position, collision.Goal.Size);
		}

		/// <summary>
		/// Gets the destination at the collision point.
		/// </summary>
		public Rect Destination { get; private set; }
	}
}

