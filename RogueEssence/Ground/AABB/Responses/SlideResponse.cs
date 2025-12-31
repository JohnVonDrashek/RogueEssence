namespace AABB
{


    using RogueElements;


	/// <summary>
	/// A collision response that causes the moving box to slide along the collided surface.
	/// The box stops at the collision point on one axis but continues along the other axis.
	/// </summary>
    public class SlideResponse : ICollisionResponse
	{
		/// <summary>
		/// Creates a slide response for the given collision, calculating the slide destination.
		/// </summary>
		/// <param name="collision">The collision information to respond to.</param>
		public SlideResponse(ICollision collision)
		{
            var velocity = (collision.Goal.Start - collision.Origin.Start);
			var vert = collision.Hit.Normal.ToAxis() == Axis4.Vert;
            var endLoc = vert ? new Loc(collision.Goal.X, collision.Hit.Position.Y) : new Loc(collision.Hit.Position.X, collision.Goal.Y);

            this.Destination = new Rect(endLoc, collision.Goal.Size);
		}

		/// <summary>
		/// Gets the destination after sliding along the collision surface.
		/// </summary>
		public Rect Destination { get; private set; }
	}
}

