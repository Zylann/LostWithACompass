using SFML.Graphics;
using System.Collections.Generic;

namespace Framework
{
	public class Array2DImager<T>
	{
		public Color defaultColor = new Color(0, 0, 0, 255);
		private Dictionary<T, Color> _colormap = new Dictionary<T,Color>();

		public Array2DImager<T> MapColor(T input, Color color)
		{
			_colormap.Add(input, color);
			return this;
		}

		public Image CreateImage(Array2D<T> a)
		{
			Image image = new Image((uint)a.sizeX, (uint)a.sizeY);
			for(int y = 0; y < a.sizeY; ++y)
			{
				for(int x = 0; x < a.sizeX; ++x)
				{
					Color c = defaultColor;
					_colormap.TryGetValue(a[x, y], out c);
					image.SetPixel((uint)x, (uint)y, c);
				}
			}
			return image;
		}

		public void CreateAndSaveImage(Array2D<T> a, string path)
		{
			Image image = CreateImage(a);
			image.SaveToFile(path);
		}
	}
}
