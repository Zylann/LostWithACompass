using SFML.Window;

namespace Framework
{
	public static class Vector2fExtensions
	{
		public static float Magnitude(this Vector2f a)
		{
			return Mathf.Sqrt(a.X * a.X + a.Y * a.Y);
		}

		public static float DistanceTo(this Vector2f a, Vector2f b)
		{
			return Mathf.Magnitude(a - b);
		}

		public static Vector2f Normalized(this Vector2f a)
		{
			float mag = a.Magnitude();
			if (Mathf.IsZero(mag))
				return new Vector2f(1, 0);
			else
				return a / mag;
		}

        public static float Angle(this Vector2f a)
        {
            Vector2f n = a.Normalized();
            return Mathf.Atan2(n.Y, n.X);
        }

        public static float Dot(this Vector2f a, Vector2f b)
        {
            return a.X * b.X + a.Y * b.Y;
        }
    }
}


