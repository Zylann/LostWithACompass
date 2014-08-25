using SFML.Graphics;

namespace Framework
{
	public static class ColorExtensions
	{
		//public static bool Equals(this Color a, Color b)
		//{
		//	return a.R == b.R 
		//		&& a.G == b.G 
		//		&& a.B == b.B 
		//		&& a.A == b.A;
		//}

		public static bool EqualsRGB(this Color c, int h)
		{
			return c.R == ((h >> 16) & 0xff)
				&& c.G == ((h >> 8) & 0xff)
				&& c.B == (h & 0xff);
		}
	}
}

