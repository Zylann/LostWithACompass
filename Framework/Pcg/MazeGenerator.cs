using SFML.Graphics;
using SFML.Window;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{
	public static class Maze
	{
		// Constants
		public const int LEFT_BIT = 1 << Dir.LEFT;
		public const int RIGHT_BIT = 1 << Dir.RIGHT;
		public const int DOWN_BIT = 1 << Dir.DOWN;
		public const int UP_BIT = 1 << Dir.UP;
		public const int ANY_DIRS_BITS = LEFT_BIT | RIGHT_BIT | DOWN_BIT | UP_BIT;
		public const int UNVISITED_BIT = 1 << 6; // Note: choosed 6 because directions could be 3D
		public const int DISABLED_BIT = 1 << 7;
	}

	/// <summary>
	/// Constructs a maze in a 2D square-cell grid.
	/// </summary>
	public class MazeGenerator
	{
		public int corridorLengthMin = 1;
		public int corridorLengthMax = 8;

		// Note: you must create or assign the grid before to call Generate.
		// Each cell of the grid is a bitmask (refer to constants)
		public Array2D<int> grid;

		/// <summary>
		/// Generates a maze from one starting point.
		/// All corridors will be connected to this point,
		/// and no one will make any loop.
		/// </summary>
		/// <param name="seedX">Seed x.</param>
		/// <param name="seedY">Seed y.</param>
		public void Generate(int seedX, int seedY)
		{
			if (grid.sizeX <= 0 || grid.sizeY <= 0)
			{
				Log.Error("Invalid grid size " + grid);
				return;
			}

			// By default, the grid is full of blocking cells (0).
			//grid.Create();

			Vector2i pos = new Vector2i(seedX, seedY);
			int dir = -1;
			List<Vector2i> startNodes = new List<Vector2i>();
			startNodes.Add(new Vector2i(seedX, seedY));

			// Initialize bits of the grid (do not touch the others because we could specify disabled cells before)
			for (int i = 0; i < grid.data.Length; ++i)
			{
				grid.data[i] &= ~Maze.ANY_DIRS_BITS; // Clear directions
				grid.data[i] |= Maze.UNVISITED_BIT; // Set unvisited bit
			}

			// While there is registered positions where we can start a new cooridor (start nodes)
			while (startNodes.Count > 0)
			{
				// Choose at random from the available start nodes
				int j = Random.Range(0, startNodes.Count);
				pos = startNodes[j];
				startNodes.RemoveAt(j);

				// Randomize the maximum length of the corridor
				int corridorLength = Random.Range(corridorLengthMin, corridorLengthMax);

				// Generate the corridor by iteration
				for (int i = 0; i < corridorLength; ++i)
				{
					int l = grid.EncodeLocation(pos.X, pos.Y);

					List<int> availableDirs = UnvisitedDirections(pos.X, pos.Y);

					// If there is at least one available direction
					if (availableDirs.Count != 0)
					{
						// If there is more than one direction available
						// Note: going back will never happen because the cell we are on will
						// be be marked as visited, and so will not occur in the list.
						if (availableDirs.Count > 1)
						{
							// Memorize the position to come back later
							startNodes.Add(pos);
						}

						// Choose a random direction
						dir = availableDirs[Random.Range(0, availableDirs.Count)];

						// Add the available direction to cell's mask (from)
						if ((grid.data[l] & Maze.UNVISITED_BIT) != 0)
							grid.data[l] = 0;
						grid.data[l] |= (1 << dir);

						// Advance
						pos.X += Dir.vec2i[dir].X;
						pos.Y += Dir.vec2i[dir].Y;

						// Add the available direction to cell's mask (to)
						l = grid.EncodeLocation(pos.X, pos.Y);
						if ((grid.data[l] & Maze.UNVISITED_BIT) != 0)
							grid.data[l] = 0;
						grid.data[l] |= (1 << Dir.opposite[dir]);

						if (i + 1 == corridorLength)
						{
							// If it's the last iteration, add the current position in start nodes.
							// If there are no direction available, the position will be discarded in further iterations.
							startNodes.Add(pos);
						}
					}
					else
					{
						// No directions available, finish corridor
						break;
					}
				}
			}
		}

		/// <summary>
		/// This function creates loops at random in the maze by joining corridors together.
		/// </summary>
		/// <param name="chance">Probability for a dead-end to join.</param>
		public void ConnectRandomNodes(float chance)
		{
			bool gen = false;
			for (int y = 0; y < grid.sizeY; ++y)
			{
				for (int x = 0; x < grid.sizeX; ++x)
				{
					int c = grid.Get(x, y);

					// If the cell is a dead-end
					if (c == Maze.LEFT_BIT || c == Maze.RIGHT_BIT || c == Maze.DOWN_BIT || c == Maze.UP_BIT)
					{
						// Connecting can fail, so we try to do it here and at the next iteration until it works.
						// In that case, gen is set to false.

						gen = gen | (Random.Range(0f, 1f) < chance);

						if (gen)
						{
							List<int> unavailableDirs = new List<int>();

							if ((c & Maze.LEFT_BIT) == 0)
							{
								unavailableDirs.Add(Dir.LEFT);
							}
							if ((c & Maze.RIGHT_BIT) == 0)
							{
								unavailableDirs.Add(Dir.RIGHT);
							}
							if ((c & Maze.DOWN_BIT) == 0)
							{
								unavailableDirs.Add(Dir.DOWN);
							}
							if ((c & Maze.UP_BIT) == 0)
							{
								unavailableDirs.Add(Dir.UP);
							}

							if (unavailableDirs.Count > 0)
							{
								int dir = unavailableDirs[Random.Range(0, unavailableDirs.Count)];

								int nx = x + Dir.vec2i[dir].X;
								int ny = y + Dir.vec2i[dir].Y;

								if (grid.Contains(nx, ny))
								{
									grid.data[grid.EncodeLocation(x, y)] |= (1 << dir);
									grid.data[grid.EncodeLocation(nx, ny)] |= (1 << Dir.opposite[dir]);
									gen = false;
								}
							}
						}
					}
				}
			}
		}

		private List<int> UnvisitedDirections(int x, int y)
		{
			List<int> dirs = new List<int>();

			for (int d = 0; d < 4; ++d)
			{
				int nx = x + Dir.vec2i[d].X;
				int ny = y + Dir.vec2i[d].Y;

				if (grid.Contains(nx, ny))
				{
					int c = grid.Get(nx, ny);
					// Is the cell unvisited and enabled?
					if ((c & Maze.UNVISITED_BIT) != 0 && (c & Maze.DISABLED_BIT) == 0)
					{
						dirs.Add(d);
					}
				}
			}

			return dirs;
		}

		/// <summary>
		/// Bakes a texture that visually represents the generated maze.
		/// Useful for debug purpose.
		/// </summary>
		/// <returns>A new texture</returns>
		public Image BakeImage()
		{
			Image image = new Image((uint)grid.sizeX * 3, (uint)grid.sizeY * 3, Color.Black);

			// Draw paths
			for (uint y = 0; y < grid.sizeY; ++y)
			{
				for (uint x = 0; x < grid.sizeX; ++x)
				{
					uint px = x * 3;
					uint py = y * 3;

					int c = grid.Get((int)x, (int)y);

					// Background
					Color color;
					if ((c & Maze.DISABLED_BIT) != 0)
					{
						color = new Color(128, 128, 128);
					}
					else if ((c & Maze.UNVISITED_BIT) == 0)
					{
						color = Color.Black;
					}
					else
					{
						color = new Color(0, 0, 0, 128);
					}
					for (uint x2 = px; x2 < px + 3; ++x2)
					{
						for (uint y2 = py; y2 < py + 3; ++y2)
						{
							image.SetPixel(x2, y2, color);
						}
					}

					// Path
					if ((c & 0xf) != 0)
					{
						image.SetPixel(px + 1, py + 1, Color.White);

						if ((c & Maze.LEFT_BIT) != 0)
						{
							image.SetPixel(px, py + 1, Color.White);
						}
						if ((c & Maze.RIGHT_BIT) != 0)
						{
							image.SetPixel(px + 2, py + 1, Color.White);
						}
						if ((c & Maze.DOWN_BIT) != 0)
						{
							image.SetPixel(px + 1, py + 2, Color.White);
						}
						if ((c & Maze.UP_BIT) != 0)
						{
							image.SetPixel(px + 1, py, Color.White);
						}
					}
				}
			}

			return image;
		}
	}
}

