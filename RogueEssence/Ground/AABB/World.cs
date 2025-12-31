namespace AABB
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    
    using RogueElements;


	/// <summary>
	/// A simple world implementation that uses linear search for collision detection.
	/// Suitable for worlds with a small number of objects where spatial hashing is not needed.
	/// </summary>
    public class World : IWorld
    {
		/// <summary>
		/// Initializes a new World with the specified dimensions.
		/// </summary>
		/// <param name="width">The width of the world.</param>
		/// <param name="height">The height of the world.</param>
        public World(int width, int height)
        {
            Width = width;
            Height = height;

            boxes = new List<IBox>();
        }

		/// <summary>
		/// Gets the rectangular bounds of this world.
		/// </summary>
        public Rect Bounds { get { return new Rect(0, 0, Width, Height); } }

        #region Boxes

		/// <summary>
		/// Gets the width of the world.
		/// </summary>
        public int Width { get; private set; }

		/// <summary>
		/// Gets the height of the world.
		/// </summary>
        public int Height { get; private set; }

        private List<IBox> boxes;

		/// <summary>
		/// Creates a new box at the specified position and adds it to the world.
		/// </summary>
		/// <param name="x">The x-coordinate of the box.</param>
		/// <param name="y">The y-coordinate of the box.</param>
		/// <param name="width">The width of the box.</param>
		/// <param name="height">The height of the box.</param>
		/// <returns>The newly created box.</returns>
        public IBox Create(int x, int y, int width, int height)
        {
            var box = new Box(this, x, y, width, height);
            boxes.Add(box);
            return box;
        }

		/// <summary>
		/// Finds all obstacles that may possibly intersect with the specified area.
		/// </summary>
		/// <param name="x">The x-coordinate of the query area.</param>
		/// <param name="y">The y-coordinate of the query area.</param>
		/// <param name="w">The width of the query area.</param>
		/// <param name="h">The height of the query area.</param>
		/// <returns>An enumerable of obstacles in the area.</returns>
        public IEnumerable<IObstacle> FindPossible(int x, int y, int w, int h)
        {
            x = Math.Max(0, Math.Min(x, this.Bounds.Right - w));
            y = Math.Max(0, Math.Min(y, this.Bounds.Bottom - h));

            Rect testRect = new Rect(x, y, w, h);
            return boxes.Where((box) => testRect.Intersects(box.Bounds));
        }

		/// <summary>
		/// Finds all obstacles that may possibly intersect with the specified area.
		/// </summary>
		/// <param name="area">The rectangular area to query.</param>
		/// <returns>An enumerable of obstacles in the area.</returns>
        public IEnumerable<IObstacle> FindPossible(Rect area)
        {
            return this.FindPossible(area.X, area.Y, area.Width, area.Height);
        }

		/// <summary>
		/// Removes a box from the world.
		/// </summary>
		/// <param name="box">The box to remove.</param>
		/// <returns>True if the box was removed; otherwise, false.</returns>
        public bool Remove(IBox box)
        {
            return boxes.Remove(box);
        }

		/// <summary>
		/// Updates a box's position in the world. No-op for this simple implementation.
		/// </summary>
		/// <param name="box">The box to update.</param>
		/// <param name="from">The previous bounds of the box.</param>
        public void Update(IBox box, Rect from)
        {
            //no need to update any caches
        }

        #endregion

        #region Hits

		/// <summary>
		/// Tests if a point hits any obstacle in the world.
		/// </summary>
		/// <param name="point">The point to test.</param>
		/// <param name="ignoring">Optional obstacles to ignore in the test.</param>
		/// <returns>Hit information if a collision occurred; otherwise, null.</returns>
        public IHit Hit(Loc point, IEnumerable<IObstacle> ignoring = null)
        {
            var boxes = this.FindPossible(point.X, point.Y, 0, 0);

            if (ignoring != null)
            {
                boxes = boxes.Except(ignoring);
            }

            foreach (var other in boxes)
            {
                var hit = AABB.Hit.Resolve(point, other);

                if (hit != null)
                {
                    return hit;
                }
            }

            return null;
        }

		/// <summary>
		/// Tests if a ray from origin to destination hits any obstacle.
		/// </summary>
		/// <param name="origin">The starting point of the ray.</param>
		/// <param name="destination">The ending point of the ray.</param>
		/// <param name="ignoring">Optional obstacles to ignore in the test.</param>
		/// <returns>Hit information for the nearest collision; otherwise, null.</returns>
        public IHit Hit(Loc origin, Loc destination, IEnumerable<IObstacle> ignoring = null)
        {
            var min = Loc.Min(origin, destination);
            var max = Loc.Max(origin, destination);

            var wrap = new Rect(min, max - min);
            var boxes = this.FindPossible(wrap.X, wrap.Y, wrap.Width, wrap.Height);

            if (ignoring != null)
            {
                boxes = boxes.Except(ignoring);
            }

            IHit nearest = null;

            foreach (var other in boxes)
            {
                var hit = AABB.Hit.Resolve(origin, destination, other);

                if (hit != null && (nearest == null || hit.IsNearest(nearest, origin)))
                {
                    nearest = hit;
                }
            }

            return nearest;
        }

		/// <summary>
		/// Tests if a rectangle moving from origin to destination hits any obstacle.
		/// </summary>
		/// <param name="origin">The starting bounds of the rectangle.</param>
		/// <param name="destination">The ending bounds of the rectangle.</param>
		/// <param name="ignoring">Optional obstacles to ignore in the test.</param>
		/// <returns>Hit information for the nearest collision; otherwise, null.</returns>
        public IHit Hit(Rect origin, Rect destination, IEnumerable<IObstacle> ignoring = null)
        {
            var wrap = new Rect(origin, destination);
            var boxes = this.FindPossible(wrap.X, wrap.Y, wrap.Width, wrap.Height);

            if (ignoring != null)
            {
                boxes = boxes.Except(ignoring);
            }

            IHit nearest = null;

            foreach (var other in boxes)
            {
                var hit = AABB.Hit.Resolve(origin, destination, other);

                if (hit != null && (nearest == null || hit.IsNearest(nearest, origin.Start)))
                {
                    nearest = hit;
                }
            }

            return nearest;
        }

        #endregion

        #region Movements

		/// <summary>
		/// Simulates moving a box to the specified coordinates with collision detection.
		/// </summary>
		/// <param name="box">The box to simulate movement for.</param>
		/// <param name="x">The target x-coordinate.</param>
		/// <param name="y">The target y-coordinate.</param>
		/// <param name="filter">A function that determines how to respond to collisions.</param>
		/// <returns>The movement result containing collision information and final destination.</returns>
        public IMovement Simulate(IBox box, int x, int y, Func<ICollision, ICollisionResponse> filter)
        {
            var origin = box.Bounds;
            var destination = new Rect(x, y, box.Width, box.Height);

            var hits = new List<IHit>();

            var result = new Movement()
            {
                Origin = origin,
                Goal = destination,
                Destination = this.Simulate(hits, new List<IObstacle>() { box }, box, origin, destination, filter),
                Hits = hits,
            };

            return result;
        }

        private Rect Simulate(List<IHit> hits, List<IObstacle> ignoring, IBox box, Rect origin, Rect destination, Func<ICollision, ICollisionResponse> filter)
        {
            var nearest = this.Hit(origin, destination, ignoring);

            if (nearest != null)
            {
                hits.Add(nearest);

                var impact = new Rect(nearest.Position, origin.Size);
                var collision = new Collision() { Box = box, Hit = nearest, Goal = destination, Origin = origin };
                var response = filter(collision);

                if (response != null && destination != response.Destination)
                {
                    ignoring.Add(nearest.Box);
                    return this.Simulate(hits, ignoring, box, impact, response.Destination, filter);
                }
            }

            return destination;
        }

        #endregion

        #region Diagnostics

		/// <summary>
		/// Draws debug visualization of the world's boxes.
		/// </summary>
		/// <param name="x">The x-coordinate of the view area.</param>
		/// <param name="y">The y-coordinate of the view area.</param>
		/// <param name="w">The width of the view area.</param>
		/// <param name="h">The height of the view area.</param>
		/// <param name="drawCell">Callback to draw a grid cell (not used in simple world).</param>
		/// <param name="drawBox">Callback to draw an obstacle box.</param>
		/// <param name="drawString">Callback to draw text (not used in simple world).</param>
        public void DrawDebug(int x, int y, int w, int h, Action<int, int, int, int, float> drawCell, Action<IObstacle> drawBox, Action<string, int, int, float> drawString)
        {
            // Drawing boxes
            foreach (var box in boxes)
                drawBox(box);
        }

        #endregion
    }
}

