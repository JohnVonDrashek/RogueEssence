namespace AABB
{
    using RogueElements;

	/// <summary>
	/// A collision response that causes the moving box to bounce off the collided surface.
	/// The box reflects its velocity based on the collision normal.
	/// </summary>
    public class BounceResponse : ICollisionResponse
	{
		/// <summary>
		/// Creates a bounce response for the given collision, calculating the bounced destination.
		/// </summary>
		/// <param name="collision">The collision information to respond to.</param>
		public BounceResponse(ICollision collision)
		{
            var velocity = (collision.Goal.Start - collision.Origin.Start);
            var vert = collision.Hit.Normal.ToAxis() == Axis4.Vert;
            var diff = velocity * collision.Hit.Amount.Numerator / collision.Hit.Amount.Denominator;
            var bouncePos = collision.Origin.Start + diff * 2 - velocity;
            var endLoc = vert ? new Loc(collision.Goal.X, bouncePos.Y) : new Loc(bouncePos.X, collision.Goal.Y);

            this.Destination = new Rect(endLoc, collision.Goal.Size);
		}

		/// <summary>
		/// Gets the destination after bouncing off the collision surface.
		/// </summary>
		public Rect Destination { get; private set; }
	}
}

