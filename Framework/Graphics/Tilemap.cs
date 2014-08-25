using SFML.Graphics;
using SFML.Window;

namespace Framework
{
	public class Tilemap
	{
		public Array2D<int> tiles;
		public int tileSize;
		public Vector2i atlasSizePx;

		public Tilemap(int width, int height, int tileSize, Vector2u atlasSizePx)
		{
			tiles = new Array2D<int>(width, height);
			this.tileSize = tileSize;
			this.atlasSizePx = new Vector2i((int)atlasSizePx.X, (int)atlasSizePx.Y);
		}

		public int width
		{
			get { return tiles.sizeX; }
		}

		public int height
		{
			get { return tiles.sizeY; }
		}

		public void SetTile(int x, int y, int tx, int ty)
		{
			//tiles[x, y] = (tx & 0xffff) | (ty << 16);
			tiles[x, y] = tx + ty * (atlasSizePx.X / tileSize);
		}

		public void Draw(SpriteBatch batch, IntRect bounds)
		{
			int minX = bounds.Left;
			int minY = bounds.Top;
			int maxX = bounds.Left+bounds.Width;
			int maxY = bounds.Top+bounds.Height;

			if (minX < 0)
				minX = 0;
			if (minY < 0)
				minY = 0;
			if (maxX >= width)
				maxX = width-1;
			if (maxY >= height)
				maxY = height-1;

			IntRect subRect = new IntRect(0, 0, tileSize, tileSize);
			int tilesX = atlasSizePx.X / tileSize;

			for(int y = minY; y <= maxY; ++y)
			{
				for(int x = minX; x <= maxX; ++x)
				{
					int t = tiles[x,y];
					if (t != -1)
					{
						subRect.Left = tileSize * (t % tilesX);
						subRect.Top = tileSize * (t / tilesX);
						batch.Draw(new Vector2f(x * tileSize, y * tileSize), subRect);
					}
				}
			}
		}
	}
}

