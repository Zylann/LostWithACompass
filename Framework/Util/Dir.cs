using SFML.Window;
using System.Collections;

namespace Framework
{
	public class Dir
	{
		public const int LEFT = 0;
		public const int RIGHT = 1;
		public const int DOWN = 2;
		public const int UP = 3;
		public const int NONE = -1;

		public static Vector2i[] vec2i = new Vector2i[] {
			new Vector2i(-1,0),
			new Vector2i(1,0),
			new Vector2i(0,1),
			new Vector2i(0,-1),

			new Vector2i(-1,-1),
			new Vector2i(1,-1),
			new Vector2i(-1,1),
			new Vector2i(1,1)
		};

		public static Vector2f[] vec2f = new Vector2f[] {
			new Vector2f(-1,0),
			new Vector2f(1,0),
			new Vector2f(0,1),
			new Vector2f(0,-1),

			new Vector2f(-1,-1),
			new Vector2f(1,-1),
			new Vector2f(-1,1),
			new Vector2f(1,1)
		};

		public static bool[] horizontal = new bool[] { true, true, false, false, false, false };
		public static bool[] vertical = new bool[] { false, false, true, true, false, false };

		public static int[] left = new int[] { DOWN, UP, RIGHT, LEFT };
		public static int[] right = new int[] { UP, DOWN, LEFT, RIGHT };

		public static int[] opposite = new int[] { RIGHT, LEFT, UP, DOWN };

		public static string ToString(int d)
		{
			switch (d)
			{
				case Dir.LEFT: return "left";
				case Dir.RIGHT: return "right";
				case Dir.DOWN: return "down";
				case Dir.UP: return "up";
				default: return "none";
			}
		}

		public static int FromVector(Vector2f v)
		{
			if (Mathf.Approximately(v.X, 0) && Mathf.Approximately(v.Y, 0))
			{
				return NONE;
			}
			else if (Mathf.Approximately(0, v.Y))
			{
				if (v.X > 0)
				{
					return RIGHT;
				}
				else
				{
					return LEFT;
				}
			}
			else if (Mathf.Approximately(0, v.X))
			{
				if (v.Y > 0)
				{
					return UP;
				}
				else
				{
					return DOWN;
				}
			}
			else
			{
				if (Mathf.Abs(v.X) > Mathf.Abs(v.Y))
				{
					if (v.X > 0)
					{
						return RIGHT;
					}
					else
					{
						return LEFT;
					}
				}
				else
				{
					if (v.Y > 0)
					{
						return UP;
					}
					else
					{
						return DOWN;
					}
				}
			}
		}

	}
}

