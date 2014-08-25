using SFML.Window;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{
	/// <summary>
	/// A* path finder optimized for grids.
	/// Inspired on a similar implementation here : 
	/// http://www.codeguru.com/csharp/csharp/cs_misc/designtechniques/article.php/c12527/AStar-A-Implementation-in-C-Path-Finding-PathFinder.htm
	/// </summary>
	public class PathFinder
	{
		public struct PathFinderNode
		{
			public int cost; // goneCost + heuristic (F)
			public int goneCost; // (G)
			public int heuristic;
			public int x;
			public int y;
			public int parentX;
			public int parentY;
			public byte state;
		}

		// Smaller version for internal use
		internal struct PathFinderNodeFast
		{
			public int cost; // goneCost + heuristic (F)
			public int goneCost; // (G)
			public int parentX;
			public int parentY;
			public byte state;
		}

		//---------------------------------------------------------

		internal class ComparePFNode : IComparer<int>
		{
			PathFinderNodeFast[] _grid;

			public ComparePFNode(PathFinderNodeFast[] grid)
			{
				_grid = grid;
			}

			public int Compare(int a, int b)
			{
				if (_grid[a].cost > _grid[b].cost)
					return 1;
				else if (_grid[a].cost < _grid[b].cost)
					return -1;
				return 0;
			}
		}

		//---------------------------------------------------------

		private byte[] _grid; // 0 means uncrossable, 1 means crossable, more means crossable at higher cost etc.
		private ushort _gridSizeX;
		private ushort _gridSizeY;
		private byte _openNodeValue = 1;
		private byte _closeNodeValue = 2;
		private int _searchLimit = 2000;
		private bool _diagonals = true;
		private bool _heavyDiagonals = true;
		private bool _avoidDiagonalCross = true;

		private PathFinderNodeFast[] _calcGrid;
		private PriorityQueueB<int> _open = null;
		private List<PathFinderNode> _close = new List<PathFinderNode>();

		private sbyte[][] _directions = {
		// Cardinals
			new sbyte[] {0,-1},
			new sbyte[] {1, 0},
			new sbyte[] {0, 1},
			new sbyte[] {-1,0},
		// Diagonals
			new sbyte[] {1,-1},
			new sbyte[] {1,1},
			new sbyte[] {-1,1},
			new sbyte[] {-1,-1}
		};

		public PathFinder(byte[] grid, int gridSizeX, int gridSizeY)
		{
			if (grid == null)
				throw new Exception("Grid cannot be null");
			if (grid.Length != gridSizeX * gridSizeY)
				throw new Exception("Grid dimensions are inconsistent");

			_grid = grid;

			_gridSizeX = (ushort)gridSizeX;
			_gridSizeY = (ushort)gridSizeY;

			_calcGrid = new PathFinderNodeFast[_gridSizeX * _gridSizeY];

			_open = new PriorityQueueB<int>(new ComparePFNode(_calcGrid));
		}

		public int SearchLimit
		{
			get { return _searchLimit; }
			set { _searchLimit = value; }
		}

		public bool Diagonals
		{
			get { return _diagonals; }
			set { _diagonals = value; }
		}

		public bool HeavyDiagonals
		{
			get { return _heavyDiagonals; }
			set { _heavyDiagonals = value; }
		}

		public bool AvoidDiagonalCross
		{
			get { return _avoidDiagonalCross; }
			set { _avoidDiagonalCross = value; }
		}

		private int EncodeLocation(int x, int y)
		{
			return y * _gridSizeX + x;
		}

		private void DecodeLocation(int l, ref ushort x, ref ushort y)
		{
			x = (ushort)(l % (int)_gridSizeX);
			y = (ushort)(l / (int)_gridSizeX);
		}

		public List<PathFinderNode> FindPath(Vector2i start, Vector2i end)
		{
			bool found = false;
			bool stop = false;
			int heuristicEstimate = 2;
			int closeNodeCounter = 0;
			int directionCount = _diagonals ? 8 : 4;

			// Instead of clearing the grid each time, I change node state values and simply ignore the other values.
			// It's faster than clearing the grid (not much, but it is).
			_openNodeValue += 2;
			_closeNodeValue += 2;

			_open.Clear();
			_close.Clear();

			int location = EncodeLocation(start.X, start.Y);
			int endLocation = EncodeLocation(end.X, end.Y);
			ushort locationX = 0, locationY = 0;

			_calcGrid[location].goneCost = 0;
			_calcGrid[location].cost = 0 + heuristicEstimate;
			_calcGrid[location].parentX = (ushort)start.X;
			_calcGrid[location].parentY = (ushort)start.Y;
			_calcGrid[location].state = _openNodeValue;

			_open.Push(location);

			while (_open.Count > 0 && !stop)
			{
				location = _open.Pop();

				// Is it in closed list? means this node was already processed
				if (_calcGrid[location].state == _closeNodeValue)
				{
					continue;
				}

				DecodeLocation(location, ref locationX, ref locationY);

				if (location == endLocation)
				{
					_calcGrid[location].state = _closeNodeValue;
					found = true;
					break;
				}

				if (closeNodeCounter > _searchLimit)
				{
					// Evaluated nodes exceeded limit : path not found
					Console.WriteLine("Pathfinder searchLimit exceed");
					return null;
				}

				// Let's calculate each successors
				for (int i = 0; i < directionCount; ++i)
				{
					ushort newLocationX = (ushort)(locationX + _directions[i][0]);
					ushort newLocationY = (ushort)(locationY + _directions[i][1]);
					int newLocation = EncodeLocation(newLocationX, newLocationY);
					int newGoneCost;

					// Outside the grid?
					if (newLocationX >= _gridSizeX || newLocationY >= _gridSizeY)
						continue;

					// Not crossable?
					if (_grid[newLocation] == 0)
						continue;

					if (_avoidDiagonalCross)
					{
						//
						//   +----+----+----+
						//   |    |    | 3  | 
						//   |    |    |    | 
						//   +----+----+----+
						//   |    | 2  |XXXX|   Diagonals are allowed, 
						//   |    |    |XXXX|   but going through 1, 2 then 3 should be avoided,
						//   +----+----+----+   because there are contiguous uncrossable cells.
						//   |    | 1  |XXXX|   (A square object cannot go from 2 to 3 for example, it will have to bypass the corner).
						//   |    |    |XXXX| 
						//   +----+----+----+
						//

						if (i > 3)
						{
							if (_grid[EncodeLocation(locationX + _directions[i][0], locationY)] == 0 ||
								_grid[EncodeLocation(locationX, locationY + _directions[i][1])] == 0)
							{
								continue;
							}
						}
					}

					if (_heavyDiagonals && i > 3)
					{
						newGoneCost = _calcGrid[location].goneCost + (int)(_grid[newLocation] * 2.41f);
					}
					else
					{
						newGoneCost = _calcGrid[location].goneCost + _grid[newLocation];
					}

					//Is it open or closed?
					if (_calcGrid[newLocation].state == _openNodeValue || _calcGrid[newLocation].state == _closeNodeValue)
					{
						// The current node has less code than the previous? then skip this node
						if (_calcGrid[newLocation].goneCost <= newGoneCost)
						{
							continue;
						}
					}

					_calcGrid[newLocation].parentX = locationX;
					_calcGrid[newLocation].parentY = locationY;
					_calcGrid[newLocation].goneCost = newGoneCost;

					// Heuristic : manhattan distance
					int heuristic = heuristicEstimate * (Math.Abs(newLocationX - end.X) + Math.Abs(newLocationY - end.Y));

					_calcGrid[newLocation].cost = newGoneCost + heuristic;

					//It is faster if we leave the open node in the priority queue
					//When it is removed, it will be already closed, it will be ignored automatically
					_open.Push(newLocation);

					_calcGrid[newLocation].state = _openNodeValue;
				}

				closeNodeCounter++;
				_calcGrid[location].state = _closeNodeValue;
			}

			if (found)
			{
				_close.Clear();

				int posX = end.X;
				int posY = end.Y;

				PathFinderNodeFast tmpNode = _calcGrid[EncodeLocation(end.X, end.Y)];

				PathFinderNode node = new PathFinderNode();
				node.cost = tmpNode.cost;
				node.goneCost = tmpNode.goneCost;
				node.heuristic = 0;
				node.parentX = tmpNode.parentX;
				node.parentY = tmpNode.parentY;
				node.x = end.X;
				node.y = end.Y;

				while (node.x != node.parentX || node.y != node.parentY)
				{
					_close.Add(node);

					posX = node.parentX;
					posY = node.parentY;

					tmpNode = _calcGrid[EncodeLocation(posX, posY)];
					node.cost = tmpNode.cost;
					node.goneCost = tmpNode.goneCost;
					node.heuristic = 0;
					node.parentX = tmpNode.parentX;
					node.parentY = tmpNode.parentY;
					node.x = posX;
					node.y = posY;
				}

				_close.Add(node);

				// Path found
				return _close;
			}

			// Path not found
			Console.WriteLine("Pathfinder path not found");
			return null;
		}

	}
}




