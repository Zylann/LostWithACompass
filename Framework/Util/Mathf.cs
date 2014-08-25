using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
	public static class Mathf
	{
		public const float PI = (float)Math.PI;
		public const float DEG2RAD = PI / 180f;
		public const float RAD2DEG = 180f / PI;
		public const float EPSILON = 0.0001f;

		public static float Cos(float x)
		{
			return (float)Math.Cos(x);
		}

		public static float Sin(float x)
		{
			return (float)Math.Sin(x);
		}

		public static float CosDeg(float x)
		{
			return (float)Math.Cos(x * DEG2RAD);
		}

		public static float SinDeg(float x)
		{
			return (float)Math.Sin(x * DEG2RAD);
		}

		public static int Umod(int x, int d)
		{
			if (x > 0)
				return x % d;
			else
				return (x % d) + d - 1;
		}

		public static float Sqrt(float x)
		{
			return (float)Math.Sqrt(x);
		}

        public static float Atan2(float y, float x)
        {
            return (float)Math.Atan2(y, x);
        }

		public static float Magnitude(float x, float y)
		{
			return (float)(Math.Sqrt(x * x + y * y));
		}

		public static float Magnitude(Vector2f v)
		{
			return Magnitude(v.X, v.Y);
		}

		public static bool Approximately(float x, float v)
		{
			return x > v - EPSILON && x < v + EPSILON;
		}

		public static bool IsZero(float v)
		{
			return v > -EPSILON && v < EPSILON;
		}

		public static float Floor(float x)
		{
			return (float)Math.Floor(x);
		}

		public static float Round(float p)
		{
			return (float)Math.Floor(p + 0.5f);
		}

		public static float Abs(float p)
		{
			return (float)Math.Abs(p);
		}

		public static float Ceil(float p)
		{
			return (float)Math.Ceiling(p);
		}

		public static float Clamp01(float p)
		{
			if (p > 1f) return 1f;
			if (p < 0f) return 0f;
			return p;
		}

		public static float Lerp(float src, float dst, float t)
		{
			return src + (dst - src) * t;
		}

		public static Vector2f Lerp(Vector2f src, Vector2f dst, float t)
		{
			return src + (dst - src) * t;
		}

		public static float Sq(float x)
		{
			return x * x;
		}

        public static float Pow4(float x)
        {
            return x * x * x * x;
        }

        public static float Clamp(float v, float min, float max)
        {
            if (v < min)
                return min;
            if (v > max)
                return max;
            return v;
        }

        public static float Pow3(float p)
        {
            return p * p * p;
        }
    }
}

