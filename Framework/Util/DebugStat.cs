using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
	public class DebugStat : Drawable
	{
		private Vector2f _position;
		private VertexArray _vertices;
		private float[] _values;
		private float _height;
		private int _index;
		private float _min;
		private float _max;

		public DebugStat(uint width, uint height, float min, float max, Vector2f position)
		{
			_position = position;
			_vertices = new VertexArray(PrimitiveType.Lines, 2 * width);
			_height = height;
			_values = new float[width];
			_min = min;
			_max = max;

			for (int i = 0; i < _values.Length; ++i)
				_values[i] = _min;
		}

		public void Push(float value)
		{
			if (value < _min)
				value = _min;
			if (value > _max)
				value = _max;
			_values[_index++] = value;
			if (_index == _values.Length)
				_index = 0;
		}

		public void Draw(RenderTarget rt, RenderStates states)
		{
			for (uint i = 0; i < _values.Length; ++i)
			{
				float h = _height * (_values[i] - _min) / (_max - _min);
				Color color = new Color(128, 128, 128);
				if (i == _index)
				{
					color = Color.White;
				}
				_vertices[2*i] = new Vertex(new Vector2f(i, _height), color);
				_vertices[2*i+1] = new Vertex(new Vector2f(i, _height-h), color);
			}

			states.Transform.Translate(_position);
			rt.Draw(_vertices, states);
		}
	}
}
