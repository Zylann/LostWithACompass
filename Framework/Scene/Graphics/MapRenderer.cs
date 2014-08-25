using Framework;
using SFML.Graphics;
using SFML.Window;
using System;

namespace Framework
{
	public class MapRenderer : Renderer
	{
		private Tilemap _tilemap;
		public Tiler tiler;
		public int tileSize;

		private SpriteBatch _spriteBatch;

		public MapRenderer()
		{
			_spriteBatch = new SpriteBatch();
			tiler = new Tiler();
		}

		public override void OnCreate()
		{
			base.OnCreate();
			tileSize = world.TS;
		}

		public void RecalculateTiles(Array2D<int> cells)
		{
			// Update tilemap
			Texture mainTexture = GetMaterial(RenderMode.BASE).mainTexture;
			_tilemap = new Tilemap(cells.sizeX, cells.sizeY, tileSize, mainTexture.Size);
			tiler.Process(cells, _tilemap.tiles);
		}

		public int width
		{
			get { return _tilemap.tiles.sizeX; }
		}

		public int height
		{
			get { return _tilemap.tiles.sizeY; }
		}

		public override void Render(RenderTarget rt, RenderMode renderMode=RenderMode.BASE)
		{
			if (_tilemap == null)
				return;
			Material mat = GetMaterial(renderMode);
			if (mat == null)
				return;

			Vector2f viewCenter = rt.GetView().Center;
			Vector2f viewSize = rt.GetView().Size;

			IntRect bounds = new IntRect(
				(int)(viewCenter.X - viewSize.X / 2f) / tileSize,
				(int)(viewCenter.Y - viewSize.Y / 2f) / tileSize,
				(int)(viewSize.X) / tileSize + 1,
				(int)(viewSize.Y) / tileSize + 1
			);

			_tilemap.Draw(_spriteBatch, bounds);

			RenderStates states = new RenderStates(mat.mainTexture);
			states.BlendMode = mat.blendMode;
			states.Shader = mat.shader;
			_spriteBatch.Display(rt, states);

			_spriteBatch.Flush();
		}
	}
}


