using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Pcg
{
	public static class Noise
	{
		// These values have been set at random.
		private static int RAND_SEQ_X = 72699;
		private static int RAND_SEQ_Y = 31976;
		private static int RAND_SEQ_SEED = 561;
		private static int RAND_SEQ1 = 11126;
		private static int RAND_SEQ2 = 98756;
		private static int RAND_SEQ3 = 423005601;

		/// <summary>
		/// Generates a random positive value between 0 and maxint.
		/// The same parameters will return the same value.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="seed"></param>
		/// <returns>Value between 0 and MAXINT</returns>
		public static int Get(int x, int y, int seed)
		{
			int n = RAND_SEQ_X * x + RAND_SEQ_Y * y + RAND_SEQ_SEED * seed;
			n &= 0x7fffffff;
			n = (n >> 13) ^ n;
			n = n * (n * n * RAND_SEQ1 + RAND_SEQ2) + RAND_SEQ3;
			n &= 0x7fffffff;
			return n;
		}

		/// <summary>
		/// Uses get to generate a random float value between 0 and 1.
		/// The same parameters will return the same value.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="seed"></param>
		/// <returns>Value between 0 and 1</returns>
		public static float Getf(int x, int y, int seed)
		{
			return (float)Get(x, y, seed) / 0x7fffffff;
		}

		private static float GetGradient(float x, float y, int seed)
		{
			// Calculate the integer coordinates
			int x0 = (x > 0.0 ? (int)x : (int)x - 1);
			int y0 = (y > 0.0 ? (int)y : (int)y - 1);

			// Calculate the remaining part of the coordinates
			float xl = x - (float)x0;
			float yl = y - (float)y0;

			// Get values for corners of square
			float v00 = Getf(x0, y0, seed);
			float v10 = Getf(x0 + 1, y0, seed);
			float v01 = Getf(x0, y0 + 1, seed);
			float v11 = Getf(x0 + 1, y0 + 1, seed);

			// Interpolate
			return BiLinearInterpolation(v00, v10, v01, v11, xl, yl);
		}

		/// <summary>
		/// Generates 2D Perlin noise
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="seed"></param>
		/// <param name="octaves"></param>
		/// <param name="persistence"></param>
		/// <param name="period"></param>
		/// <returns>Value between 0 and 1</returns>
		public static float GetPerlin(float x, float y, int seed, int octaves, float persistence, float period)
		{
			if (octaves < 1)
				return 0;

			x /= period;
			y /= period;

			float noise = 0; // noise
			float f = 1.0f;
			float amp = 1.0f; // amplitude of an octave
			float ampMax = 0; // total amplitude

			for (int i = 0; i < octaves; i++)
			{
				noise += amp * GetGradient(x * f, y * f, seed + i);
				ampMax += amp;
				f *= 2.0f;
				amp *= persistence; // reduce next amplitude
			}

			return noise / ampMax;
		}

		private static float SmoothCurve(float x)
		{
			return 6f * x * x * x * x * x - 15f * x * x * x * x + 10f * x * x * x;
			//return 6f * Mathf.Pow (x, 5f) - 15f * Mathf.Pow (x, 4f) + 10f * Mathf.Pow (x, 3f);
		}

		private static float BiLinearInterpolation(
				float x0y0, float x1y0,
				float x0y1, float x1y1,
				float x, float y)
		{
			float tx = SmoothCurve(x);
			float ty = SmoothCurve(y);

			float u = Mathf.Lerp(x0y0, x1y0, tx);
			float v = Mathf.Lerp(x0y1, x1y1, tx);

			return Mathf.Lerp(u, v, ty);
		}
	}
}
