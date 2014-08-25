using SFML.Graphics;
using SFML.Window;

namespace Framework
{
	/// <summary>
	/// Static collider composed of a grid of hit boxes.
	/// </summary>
	public class MapCollider : Collider
	{
		/// <summary>
		/// Which type of collision values are found out of this map
		/// </summary>
		public int defaultCollisionValue = 0;

		/// <summary>
		/// How to map user-defined cells to collision values
		/// </summary>
		public int[] cell2CollisionValue;

		/// <summary>
		/// Collision values for each cell of the map.
		/// 0 means no collision.
		/// </summary>
		private Array2D<int> _collisionMap;

		public override FloatRect bounds
		{
			get { return new FloatRect(0,0,_collisionMap.sizeX, _collisionMap.sizeY); }
		}

		public int GetCollisionValue(int x, int y)
		{
			if (_collisionMap.Contains(x, y))
				return _collisionMap[x, y];
			else
				return defaultCollisionValue;
		}

		public void SetCollisionValue(int x, int y, int collisionMapValue)
		{
			if (_collisionMap.Contains(x, y))
				_collisionMap[x, y] = collisionMapValue;
		}

		public void RecalculateCollisionMap(Array2D<int> cells)
		{
			_collisionMap = new Array2D<int>(cells.sizeX, cells.sizeY);
			for(int i = 0; i < cells.data.Length; ++i)
			{
				int c = cells.data[i];
				if (c >= 0 && c < cell2CollisionValue.Length)
					_collisionMap.data[i] = cell2CollisionValue[c];
				else
					_collisionMap.data[i] = defaultCollisionValue;
			}
		}

		public int width
		{
			get { return _collisionMap.sizeX; }
		}

		public int height
		{
			get { return _collisionMap.sizeY; }
		}

		public override bool Collides(Vector2f point)
		{
			int x = (int)point.X;
			int y = (int)point.Y;
			return Collides(x, y);
		}

		public bool Collides(int x, int y)
		{
			int c = GetCollisionValue(x, y);
			if (c >= 0 && c < cell2CollisionValue.Length)
			{
				return GetCollisionValue(x, y) > 0;
			}
			return false;
		}

		public override bool Collides(FloatRect box)
		{
			int minX = (int)box.Left;
			int minY = (int)box.Top;
			int maxX = (int)(box.Left + box.Width);
			int maxY = (int)(box.Top + box.Height);

			for (int y = minY; y <= maxY; ++y)
			{
				for (int x = minX; x <= maxX; ++x)
				{
					if (_collisionMap.Contains(x, y))
					{
						int c = _collisionMap[x, y];
						if (c > 0)
						{
							return true;
						}
					}
				}
			}

			return false;
		}
	}
}
