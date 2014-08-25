
namespace Framework
{
	public class ApplicationSettings
	{
		public bool debug = true;
		public uint pixelSize = 3;
		public int minDeltaMs = 1000 / 70;
		public int maxDeltaMs = 1000 / 30;
		public bool pixelPerfect = true;
		public bool fixedSize = false;
		public bool showSystemCursor = false;
		public bool smoothDeltas = true;
		public bool vsync = true;
	}
}


