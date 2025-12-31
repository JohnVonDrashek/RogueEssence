namespace AABB
{
	using System;
	using System.Collections.Generic;
    using System.Linq;
    using RogueElements;


	/// <summary>
	/// Basic spatial hashing of world's boxes for efficient collision detection.
	/// Divides the world into a grid of cells to quickly find nearby obstacles.
	/// </summary>
	public class Grid
	{
		/// <summary>
		/// Represents a single cell in the spatial hash grid that contains obstacles.
		/// </summary>
		public class Cell
		{
			/// <summary>
			/// Initializes a new cell at the specified grid coordinates.
			/// </summary>
			/// <param name="x">The x grid coordinate of the cell.</param>
			/// <param name="y">The y grid coordinate of the cell.</param>
			/// <param name="cellSize">The size of the cell in world units.</param>
            public Cell(int x, int y, int cellSize)
			{
				this.Bounds = new Rect(x * cellSize, y * cellSize, cellSize, cellSize);
			}

			/// <summary>
			/// Gets the rectangular bounds of this cell in world coordinates.
			/// </summary>
			public Rect Bounds { get; private set; }

			/// <summary>
			/// Gets the obstacles contained in this cell.
			/// </summary>
			public IEnumerable<IObstacle> Children { get { return  this.children;}}

			private List<IObstacle> children = new List<IObstacle>();

			/// <summary>
			/// Adds an obstacle to this cell.
			/// </summary>
			/// <param name="box">The obstacle to add.</param>
			public void Add(IObstacle box)
			{
				this.children.Add(box);
			}

			/// <summary>
			/// Checks if this cell contains the specified obstacle.
			/// </summary>
			/// <param name="box">The obstacle to check for.</param>
			/// <returns>True if the cell contains the obstacle; otherwise, false.</returns>
			public bool Contains(IObstacle box)
			{
				return this.children.Contains(box);
			}

			/// <summary>
			/// Removes an obstacle from this cell.
			/// </summary>
			/// <param name="box">The obstacle to remove.</param>
			/// <returns>True if the obstacle was removed; otherwise, false.</returns>
			public bool Remove(IObstacle box)
			{
				return this.children.Remove(box);
			}

			/// <summary>
			/// Gets the number of obstacles in this cell.
			/// </summary>
			/// <returns>The count of obstacles.</returns>
			public int Count()
			{
				return this.children.Count;
			}
		}

		/// <summary>
		/// Initializes a new grid with the specified dimensions and cell size.
		/// </summary>
		/// <param name="width">The number of columns in the grid.</param>
		/// <param name="height">The number of rows in the grid.</param>
		/// <param name="cellSize">The size of each cell in world units.</param>
        public Grid(int width, int height, int cellSize)
		{
			this.Cells = new Cell[width, height];
			this.CellSize = cellSize;
		}

		/// <summary>
		/// Gets or sets the size of each cell in world units.
		/// </summary>
        public int CellSize { get; set; }

		#region Size

		/// <summary>
		/// Gets the total width of the grid in world units.
		/// </summary>
        public int Width { get { return this.Columns * CellSize; } }

		/// <summary>
		/// Gets the total height of the grid in world units.
		/// </summary>
        public int Height { get { return this.Rows * CellSize; } }

		/// <summary>
		/// Gets the number of columns in the grid.
		/// </summary>
		public int Columns { get { return  this.Cells.GetLength(0);}}

		/// <summary>
		/// Gets the number of rows in the grid.
		/// </summary>
        public int Rows { get { return this.Cells.GetLength(1); } }

		#endregion

		/// <summary>
		/// Gets the 2D array of cells in this grid.
		/// </summary>
		public Cell[,] Cells { get; private set; }

		/// <summary>
		/// Queries all cells that intersect with the specified rectangular area.
		/// </summary>
		/// <param name="x">The x-coordinate of the query area.</param>
		/// <param name="y">The y-coordinate of the query area.</param>
		/// <param name="w">The width of the query area.</param>
		/// <param name="h">The height of the query area.</param>
		/// <param name="wrap">Whether to wrap coordinates around the grid edges.</param>
		/// <returns>An enumerable of cells that intersect the query area.</returns>
        public IEnumerable<Cell> QueryCells(int x, int y, int w, int h, bool wrap)
        {
            List<Cell> result = new List<Cell>();
            if (w == 0 && h == 0)
                return result;

			var minX = MathUtils.DivDown(x, this.CellSize);
			var minY = MathUtils.DivDown(y, this.CellSize);
			var maxX = MathUtils.DivUp(x + w, this.CellSize);
			var maxY = MathUtils.DivUp(y + h, this.CellSize);

			Loc size = new Loc(this.Columns, this.Rows);

			for (int ix = minX; ix < maxX; ix++)
			{
				for (int iy = minY; iy < maxY; iy++)
				{
					Loc testLoc = new Loc(ix, iy);
					if (wrap)
						testLoc = Loc.Wrap(testLoc, size);
					else if (!RogueElements.Collision.InBounds(this.Columns, this.Rows, testLoc))
						continue;

					var cell = Cells[testLoc.X, testLoc.Y];

					if (cell == null)
					{
						cell = new Cell(testLoc.X, testLoc.Y, CellSize);
						Cells[testLoc.X, testLoc.Y] = cell;
					}

					result.Add(cell);
				}
			}

			return result;

		}

		/// <summary>
		/// Queries all obstacles in cells that intersect with the specified rectangular area.
		/// </summary>
		/// <param name="x">The x-coordinate of the query area.</param>
		/// <param name="y">The y-coordinate of the query area.</param>
		/// <param name="w">The width of the query area.</param>
		/// <param name="h">The height of the query area.</param>
		/// <param name="wrap">Whether to wrap coordinates around the grid edges.</param>
		/// <returns>A distinct enumerable of obstacles found in the query area.</returns>
        public IEnumerable<IObstacle> QueryBoxes(int x, int y, int w, int h, bool wrap)
		{
			var cells = this.QueryCells(x, y, w, h, wrap);

			return cells.SelectMany((cell) => cell.Children).Distinct();
		}

		/// <summary>
		/// Adds an obstacle to all cells it intersects with.
		/// </summary>
		/// <param name="box">The obstacle to add.</param>
		/// <param name="wrap">Whether to wrap coordinates around the grid edges.</param>
		public void Add(IObstacle box, bool wrap)
		{
			var cells = this.QueryCells(box.X, box.Y, box.Width, box.Height, wrap);

			foreach (var cell in cells)
			{
				if(!cell.Contains(box))
					cell.Add(box);
			}
		}

		/// <summary>
		/// Updates an obstacle's position in the grid by removing it from its old cells and adding it to new ones.
		/// </summary>
		/// <param name="box">The obstacle to update.</param>
		/// <param name="from">The previous bounds of the obstacle.</param>
		/// <param name="wrap">Whether to wrap coordinates around the grid edges.</param>
		public void Update(IObstacle box, Rect from, bool wrap)
		{
			var fromCells = this.QueryCells(from.X, from.Y, from.Width, from.Height, wrap);
			var removed = false;
			foreach (var cell in fromCells)
			{
				removed |= cell.Remove(box);
			}

			this.Add(box, wrap);
		}

		/// <summary>
		/// Removes an obstacle from all cells it intersects with.
		/// </summary>
		/// <param name="box">The obstacle to remove.</param>
		/// <param name="wrap">Whether to wrap coordinates around the grid edges.</param>
		/// <returns>True if the obstacle was removed from at least one cell; otherwise, false.</returns>
		public bool Remove(IObstacle box, bool wrap)
		{
			var cells = this.QueryCells(box.X, box.Y, box.Width, box.Height, wrap);

			var removed = false;
			foreach (var cell in cells)
			{
				removed |= cell.Remove(box);
			}

			return removed;
		}

		/// <summary>
		/// Returns a string representation of the grid.
		/// </summary>
		/// <returns>A string containing the grid dimensions.</returns>
		public override string ToString()
		{
			return string.Format("[Grid: Width={0}, Height={1}, Columns={2}, Rows={3}]", Width, Height, Columns, Rows);
		}
	}
}

