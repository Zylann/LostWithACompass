
namespace Framework
{
	public class Random
	{
		private static System.Random _random = new System.Random();

		public static float Range(float min, float max)
		{
			return min + ((float)_random.NextDouble()) * (max-min);
		}

		public static int Range(int min, int max)
		{
			if (max <= min)
				return min;
			return min + _random.Next() % (max-min);
		}

        public static bool Chance(float p)
        {
            return _random.NextDouble() < (double)p;
        }
    }
}

