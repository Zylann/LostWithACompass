using Framework;
using SFML.Graphics;
using System;
using System.Collections;

namespace Framework
{
	/// <summary>
	/// Flat-array version of a 2D array.
	/// It is useful if you compile with .NET 2.0 or want a different approach.
	/// </summary>
	public class Array2D<T>
	{
		public int sizeX;
		public int sizeY;

		public T[] data;

		public Array2D(T[] pData, int width, int height)
		{
			if (pData.Length != width * height)
			{
				Log.Error("Array length and 2D size mismatch");
			}

			data = pData;
			sizeX = width;
			sizeY = height;
		}

		public Array2D(int width, int height)
		{
			sizeX = width;
			sizeY = height;
			data = new T[sizeX * sizeY];
		}

		public Array2D(uint width, uint height)
		{
			sizeX = (int)width;
			sizeY = (int)height;
			data = new T[sizeX * sizeY];
		}

		public void Create()
		{
			data = new T[sizeX * sizeY];
		}

		public void Create(int newSizeX, int newSizeY)
		{
			sizeX = newSizeX;
			sizeY = newSizeY;
			data = new T[sizeX * sizeY];
		}

		public int EncodeLocation(int x, int y)
		{
			return x + y * sizeX;
		}

		public void DecodeLocation(int l, out int x, out int y)
		{
			x = l % sizeX;
			y = l / sizeX;
		}

		public void Fill(T v)
		{
			for (int i = 0; i < data.Length; ++i)
			{
				data[i] = v;
			}
		}

		public void Fill(T v, int subX, int subY, int subW, int subH)
		{
			for (int y = 0; y < subH; ++y)
			{
				for (int x = 0; x < subW; ++x)
				{
					data[EncodeLocation(subX + x, subY + y)] = v;
				}
			}
		}

		public T Get(int x, int y)
		{
			return data[EncodeLocation(x, y)];
		}

		public void Set(int x, int y, T v)
		{
			data[EncodeLocation(x, y)] = v;
		}

		public bool Contains(int x, int y)
		{
			return x >= 0 && y >= 0 && x < sizeX && y < sizeY;
		}

		public override string ToString()
		{
			return "{sizeX=" + sizeX + ", " + "sizeY=" + sizeY + "}";
		}

		public T this[int x, int y]
		{
			get { return data[EncodeLocation(x, y)]; }
			set { Set(x, y, value); }
		}

		public void Outline(T value)
		{
			int lastRow = sizeX * (sizeY - 1);
			for(int x = 0; x < sizeX; ++x)
			{
				data[x] = value;
				data[x + lastRow] = value;
			}

			lastRow = sizeX * (sizeY - 1);
			int lastColumn = sizeX - 1;
			for (int i = sizeX; i < lastRow; i += sizeX)
			{
				data[i] = value;
				data[i + lastColumn] = value;
			}
		}

		public Array2D<T> Pad(T borderValue)
		{
			int newSizeX = sizeX + 2;
			int newSizeY = sizeY + 2;

			T[] newData = new T[newSizeX * newSizeY];

			Array2D<T> newArray = new Array2D<T>(newData, newSizeX, newSizeY);

			newArray.Outline(borderValue);

			for (int y = 0; y < sizeY; ++y)
			{
				for (int x = 0; x < sizeX; ++x)
				{
					newArray[x+1,y+1] = data[EncodeLocation(x, y)];
				}
			}

			return newArray;
		}

		/// <summary>
		/// Clones the array.
		/// Note: if T is a reference type, it will be copied, not duplicated !
		/// </summary>
		public Array2D<T> Clone()
		{
			T[] newData = new T[data.Length];
			for (int i = 0; i < newData.Length; ++i)
			{
				newData[i] = data[i];
			}
			Array2D<T> newArray = new Array2D<T>(newData, sizeX, sizeY);
			return newArray;
		}

		public void MirrorY()
		{
			for (int y = 0; y < sizeY / 2; ++y)
			{
				for(int x = 0; x < sizeX; ++x)
				{
					int src = EncodeLocation(x, y);
					int dst = EncodeLocation(x, sizeY-y-1);
					T temp = data[src];
					data[src] = data[dst];
					data[dst] = temp;
				}
			}
		}

		public class NeighborInfo
		{
			public T value;
			public int dir;
			public int x;
			public int y;
		}

		public delegate bool NeighborCallback(NeighborInfo n);
		private NeighborInfo __ninfo = new NeighborInfo();

		public bool EvaluateNeighbors(int x, int y, int ndirs, NeighborCallback cb)
		{
			// Note: this is an optimization to avoid too much memory allocations.
			// However, it introduces a limit:
			// - Don't call this function in a multithread environnment
			// - Don't call this function inside itself (in the callback)
			NeighborInfo n = __ninfo;

			for(n.dir = 0; n.dir < ndirs; ++n.dir)
			{
				n.x = x + Dir.vec2i[n.dir].X;
				n.y = y + Dir.vec2i[n.dir].Y;
				if(Contains(n.x, n.y))
				{
					n.value = this[n.x, n.y];
					if(cb(n))
					{
						return true;
					}
				}
			}

			return false;
		}

		public void Contour(T contourValue, Predicate<T> match, int ndirs=4)
		{
			for(int y = 0; y < sizeY; ++y)
			{
				for(int x = 0; x < sizeX; ++x)
				{
					if(!match(this[x,y]))
					{
						if (EvaluateNeighbors(x, y, ndirs, (NeighborInfo n) => { return match(n.value); }))
						{
							this[x, y] = contourValue;
						}
					}
				}
			}
		}

		public Array2D<T> Crop(int x0, int y0, int w, int h)
		{
			Array2D<T> croppedArray = new Array2D<T>(w, h);
			for(int y = 0; y < h; ++y)
			{
				for(int x = 0; x < w; ++x)
				{
					croppedArray[x, y] = this[x0 + x, y0 + y];
				}
			}
			return croppedArray;
		}

		public Array2D<T> Crop(IntRect rect)
		{
			return Crop(rect.Left, rect.Top, rect.Width, rect.Height);
		}

		public bool ContainsValue(int x0, int y0, int w, int h, Predicate<T> match)
		{
			for(int y = 0; y < h; ++y)
			{
				for(int x = 0; x < w; ++x)
				{
					if (match(this[x, y]))
						return true;
				}
			}
			return false;
		}

		public IntRect FindBounds(Predicate<T> match)
		{
			IntRect rect = new IntRect(0,0,sizeX,sizeY);

			// Top
			for (; rect.Top < sizeY; ++rect.Top)
			{
				if (ContainsValue(0, rect.Top, rect.Width, 1, match))
				{
					break;
				}
			}
			rect.Height = sizeY - rect.Top;

			// Left
			for (; rect.Left < sizeX; ++rect.Left)
			{
				if (ContainsValue(rect.Left, rect.Top, 1, sizeY-rect.Top, match))
				{
					break;
				}
			}
			rect.Width = sizeX - rect.Left;

			// Bottom
			for (int y = sizeY - 1; y >= rect.Top; --y)
			{
				if(ContainsValue(rect.Left, y, rect.Width, 1, match))
				{
					break;
				}
			}

			// Right
			for (int x = sizeX - 1; x >= rect.Left; --x)
			{
				if (ContainsValue(x, rect.Top, 1, rect.Height, match))
				{
					break;
				}
			}

			return rect;
		}
	}
}


