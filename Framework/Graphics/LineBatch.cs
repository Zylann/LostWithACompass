using SFML.Graphics;
using SFML.Window;
using System;

namespace Framework
{
	public class LineBatch
	{
		private Vertex[] _vertices = new Vertex[1024 * 4];
		private uint _primitiveCount;
		private RenderStates _states;

		public LineBatch()
		{
			_states = RenderStates.Default;
		}

		public void Display(RenderTarget renderTarget)
		{
			if (_primitiveCount != 0)
				renderTarget.Draw(_vertices, 0, _primitiveCount * 2, PrimitiveType.Lines, _states);
		}

		public void Flush()
		{
			_primitiveCount = 0;
		}

		public void DrawLine(Vector2f a, Vector2f b)
		{
			DrawLine(a, b, Color.White);
		}

		public void DrawLine(Vector2f a, Vector2f b, Color color)
		{
			if (_primitiveCount * 2 >= _vertices.Length)
				Array.Resize<Vertex>(ref _vertices, _vertices.Length * 2);

			uint vi = _primitiveCount * 2;

			_vertices[vi] = new Vertex(a, color);
			_vertices[vi + 1] = new Vertex(b, color);

			++_primitiveCount;
		}
	}
}
