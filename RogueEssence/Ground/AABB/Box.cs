namespace AABB
{
    using System;
    using RogueElements;



    /// <summary>
    /// Represents a movable axis-aligned bounding box that can detect collisions with other obstacles in the world.
    /// This class implements IBox and provides movement simulation and collision detection capabilities.
    /// </summary>
    public class Box : IBox
	{
		#region Constructors 

        /// <summary>
        /// Creates a new Box in the specified world at the given position and size.
        /// </summary>
        /// <param name="world">The world this box belongs to.</param>
        /// <param name="x">The x-coordinate of the box's top-left corner.</param>
        /// <param name="y">The y-coordinate of the box's top-left corner.</param>
        /// <param name="width">The width of the box.</param>
        /// <param name="height">The height of the box.</param>
        public Box(IWorld world, int x, int y, int width, int height)
		{
			this.world = world;
			this.bounds = new Rect(x, y, width, height);
		}

		#endregion

		#region Fields

		private IWorld world;

		private Rect bounds;

		#endregion

		#region Properties

        /// <summary>
        /// Gets the rectangular bounds of this box.
        /// </summary>
		public Rect Bounds
		{
			get { return bounds; }
		}

        /// <summary>
        /// Gets the height of the box.
        /// </summary>
        public int Height { get { return Bounds.Height; } }

        /// <summary>
        /// Gets the width of the box.
        /// </summary>
        public int Width { get { return Bounds.Width; } }

        /// <summary>
        /// Gets the x-coordinate of the box's top-left corner.
        /// </summary>
        public int X { get { return Bounds.X; } }

        /// <summary>
        /// Gets the y-coordinate of the box's top-left corner.
        /// </summary>
        public int Y { get { return Bounds.Y; } }

		#endregion

		#region Movements

        /// <summary>
        /// Simulates moving the box to the specified coordinates without actually moving it.
        /// </summary>
        /// <param name="x">The target x-coordinate.</param>
        /// <param name="y">The target y-coordinate.</param>
        /// <param name="filter">A function that determines how to respond to collisions.</param>
        /// <returns>The movement result containing collision information and final destination.</returns>
        public IMovement Simulate(int x, int y, Func<ICollision, ICollisionResponse> filter)
		{
			return world.Simulate(this, x, y, filter);
		}

        /// <summary>
        /// Moves the box to the specified coordinates with collision detection.
        /// </summary>
        /// <param name="x">The target x-coordinate.</param>
        /// <param name="y">The target y-coordinate.</param>
        /// <param name="filter">A function that determines how to respond to collisions.</param>
        /// <returns>The movement result containing collision information and final destination.</returns>
        public IMovement Move(int x, int y, Func<ICollision, ICollisionResponse> filter)
		{
			var movement = this.Simulate(x, y, filter);
			this.bounds.X = movement.Destination.X;
			this.bounds.Y = movement.Destination.Y;
			this.world.Update(this, movement.Origin);
			return movement;
		}

		#endregion

        /// <summary>
        /// Gets or sets the collision tags for this box, used to categorize collision behavior.
        /// </summary>
        public uint Tags { get; set; }
	}
}

