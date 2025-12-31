namespace AABB
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	
    using RogueElements;


	/// <summary>
	/// A world implementation that uses spatial hashing with a grid for collision detection.
	/// Provides efficient box-based collision detection and movement simulation.
	/// </summary>
	public class GridWorld : IWorld
	{
		/// <summary>
		/// Initializes a new GridWorld with the specified dimensions and cell size.
		/// </summary>
		/// <param name="width">The width of the world in world units.</param>
		/// <param name="height">The height of the world in world units.</param>
		/// <param name="cellSize">The size of each grid cell.</param>
        public GridWorld(int width, int height, int cellSize)
		{
			var iwidth = MathUtils.DivUp(width, cellSize);
			var iheight = MathUtils.DivUp(height, cellSize);

			this.grid = new Grid(iwidth, iheight, cellSize);
		}

		/// <summary>
		/// Gets the rectangular bounds of this world.
		/// </summary>
        public Rect Bounds { get { return new Rect(0, 0, this.grid.Width, this.grid.Height); } }

		#region Boxes

		private Grid grid;

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
			this.grid.Add(box, false);
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

			return this.grid.QueryBoxes(x, y, w, h, false);
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
			return this.grid.Remove(box, false);
		}

		/// <summary>
		/// Updates a box's position in the world's spatial hash.
		/// </summary>
		/// <param name="box">The box to update.</param>
		/// <param name="from">The previous bounds of the box.</param>
		public void Update(IBox box, Rect from)
		{
			this.grid.Update(box, from, false);
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

				if (hit != null && (nearest == null || hit.IsNearest(nearest,origin)))
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
		/// Draws debug visualization of the world's spatial hash grid and boxes.
		/// </summary>
		/// <param name="x">The x-coordinate of the view area.</param>
		/// <param name="y">The y-coordinate of the view area.</param>
		/// <param name="w">The width of the view area.</param>
		/// <param name="h">The height of the view area.</param>
		/// <param name="drawCell">Callback to draw a grid cell.</param>
		/// <param name="drawBox">Callback to draw an obstacle box.</param>
		/// <param name="drawString">Callback to draw text.</param>
		public void DrawDebug(int x, int y, int w, int h, Action<int,int,int,int,float> drawCell, Action<IObstacle> drawBox, Action<string,int,int, float> drawString)
		{
			// Drawing boxes
			var boxes = this.grid.QueryBoxes(x, y, w, h, false);
			foreach (var box in boxes)
			{
				drawBox(box);
			}

			// Drawing cells
			var cells = this.grid.QueryCells(x, y, w, h, false);
			foreach (var cell in cells)
			{
				var count = cell.Count();
				var alpha = count > 0 ? 1f : 0.4f;
				drawCell((int)cell.Bounds.X, (int)cell.Bounds.Y, (int)cell.Bounds.Width, (int)cell.Bounds.Height, alpha);
				drawString(count.ToString(), (int)cell.Bounds.Center.X, (int)cell.Bounds.Center.Y,alpha);
			}
		}

		#endregion
	}
}

