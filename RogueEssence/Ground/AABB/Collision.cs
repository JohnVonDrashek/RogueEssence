using RogueElements;

namespace AABB
{

	/// <summary>
	/// Represents collision information between a moving box and an obstacle.
	/// Contains details about the boxes involved, their positions, and whether a collision occurred.
	/// </summary>
	public class Collision : ICollision
	{
		/// <summary>
		/// Initializes a new instance of the Collision class.
		/// </summary>
		public Collision()
		{
		}

		/// <summary>
		/// Gets or sets the moving box involved in the collision.
		/// </summary>
		public IBox Box { get; set; }

		/// <summary>
		/// Gets the obstacle that was hit, or null if no collision occurred.
		/// </summary>
		public IObstacle Other { get { return (this.Hit != null ? this.Hit.Box : null); } }

		/// <summary>
		/// Gets or sets the original position of the box before movement.
		/// </summary>
		public Rect Origin { get; set; }

		/// <summary>
		/// Gets or sets the intended destination of the box.
		/// </summary>
		public Rect Goal { get; set; }

		/// <summary>
		/// Gets or sets the hit information from the collision.
		/// </summary>
		public IHit Hit { get; set; }

		/// <summary>
		/// Gets a value indicating whether a collision occurred.
		/// </summary>
        public bool HasCollided { get { return this.Hit != null; } }
	}
}

