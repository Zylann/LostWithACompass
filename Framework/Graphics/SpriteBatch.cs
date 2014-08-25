using SFML.Graphics;
using SFML.Window;
using System;

namespace Framework
{
	public class SpriteBatch
	{
		private static SampledCurve __sin;
		private static SampledCurve __cos;
		private const float E = 0.01f; // slight negative padding applied to texture coordinates

		private Vertex[] _vertices = new Vertex[1024 * 4];
		private uint _primitiveCount;
		private RenderStates _states;

		public SpriteBatch()
		{
			_states = RenderStates.Default;

			if (__sin == null)
				__sin = new SampledCurve(Mathf.SinDeg, 0f, 360f, 360);
			if (__cos == null)
				__cos = new SampledCurve(Mathf.CosDeg, 0f, 360f, 360);
		}

		public void Display(RenderTarget renderTarget, RenderStates states)
		{
			if (_primitiveCount == 0)
				return;
			renderTarget.Draw(_vertices, 0, _primitiveCount * 4, PrimitiveType.Quads, states);
		}

		public void Display(RenderTarget renderTarget, Texture texture)
		{
			_states = RenderStates.Default;
			_states.Texture = texture;
			Display(renderTarget, _states);
		}

		public void Flush()
		{
			_primitiveCount = 0;
		}

		public void Draw(Sprite sprite)
		{
			Draw(sprite.Position, sprite.TextureRect, sprite.Color, sprite.Scale, sprite.Origin, sprite.Rotation);
		}

		public void Draw(Vector2f position, IntRect subRect)
		{
			Draw(position, subRect, Color.White);
		}

		public void DrawCentered(Vector2f position, IntRect subRect, Color color, Vector2f scale)
		{
			Draw(position, subRect, color, scale, new Vector2f((float)subRect.Width / 2f, (float)subRect.Height / 2f));
		}

		public void DrawCentered(Vector2f position, IntRect subRect, Color color, Vector2f scale, float rotation)
		{
			Draw(position, subRect, color, scale, new Vector2f((float)subRect.Width / 2f, (float)subRect.Height / 2f), rotation);
		}

		public void Draw(Vector2f position, IntRect subRect, Color color)
		{
			if (_primitiveCount * 4 >= _vertices.Length)
				Array.Resize<Vertex>(ref _vertices, _vertices.Length * 2);

			uint vi = _primitiveCount * 4;

			Vertex v = new Vertex();
			v.Color = color;

			v.Position = position;
			v.TexCoords.X = subRect.Left+E;
			v.TexCoords.Y = subRect.Top+E;
			_vertices[vi] = v;

			v.Position.X += subRect.Width;
			v.TexCoords.X += subRect.Width-2f*E;
			_vertices[vi + 1] = v;

			v.Position.Y += subRect.Height;
			v.TexCoords.Y += subRect.Height-2f*E;
			_vertices[vi + 2] = v;

			v.Position.X -= subRect.Width;
			v.TexCoords.X -= subRect.Width-2f*E;
			_vertices[vi + 3] = v;

			++_primitiveCount;
		}

		public void Draw(Vector2f position, IntRect subRect, Color color, Vector2f scale, Vector2f origin)
		{
			if (_primitiveCount * 4 >= _vertices.Length)
				Array.Resize<Vertex>(ref _vertices, _vertices.Length * 2);

			origin.X *= scale.X;
			origin.Y *= scale.Y;
			scale.X *= subRect.Width;
			scale.Y *= subRect.Height;

			Vertex v = new Vertex();
			v.Color = color;

			float pX, pY;
			uint vi = _primitiveCount * 4;

			pX = -origin.X;
			pY = -origin.Y;
			v.Position.X = pX + position.X;
			v.Position.Y = pY + position.Y;
			v.TexCoords.X = subRect.Left+E;
			v.TexCoords.Y = subRect.Top+E;
			_vertices[vi] = v;

			pX += scale.X;
			v.Position.X = pX + position.X;
			v.Position.Y = pY + position.Y;
			v.TexCoords.X += subRect.Width-2f*E;
			_vertices[vi + 1] = v;

			pY += scale.Y;
			v.Position.X = pX + position.X;
			v.Position.Y = pY + position.Y;
			v.TexCoords.Y += subRect.Height-2f*E;
			_vertices[vi + 2] = v;

			pX -= scale.X;
			v.Position.X = pX + position.X;
			v.Position.Y = pY + position.Y;
			v.TexCoords.X -= subRect.Width-2f*E;
			_vertices[vi + 3] = v;

			++_primitiveCount;
		}

		public void Draw(Vector2f position, IntRect subRect, Color color, Vector2f scale, Vector2f origin, float rotation)
		{
			if (_primitiveCount*4 >= _vertices.Length)
				Array.Resize<Vertex>(ref _vertices, _vertices.Length * 2);

			float sin = __sin.GetRepeat(rotation);
			float cos = __cos.GetRepeat(rotation);

			origin.X *= scale.X;
			origin.Y *= scale.Y;
			scale.X *= subRect.Width;
			scale.Y *= subRect.Height;

			Vertex v = new Vertex();
			v.Color = color;

			float pX, pY;
			uint vi = _primitiveCount * 4;

			pX = -origin.X;
			pY = -origin.Y;
			v.Position.X = pX * cos - pY * sin + position.X;
			v.Position.Y = pX * sin + pY * cos + position.Y;
			v.TexCoords.X = subRect.Left+E;
			v.TexCoords.Y = subRect.Top+E;
			_vertices[vi] = v;

			pX += scale.X;
			v.Position.X = pX * cos - pY * sin + position.X;
			v.Position.Y = pX * sin + pY * cos + position.Y;
			v.TexCoords.X += subRect.Width-2f*E;
			_vertices[vi + 1] = v;

			pY += scale.Y;
			v.Position.X = pX * cos - pY * sin + position.X;
			v.Position.Y = pX * sin + pY * cos + position.Y;
			v.TexCoords.Y += subRect.Height-2f*E;
			_vertices[vi + 2] = v;

			pX -= scale.X;
			v.Position.X = pX * cos - pY * sin + position.X;
			v.Position.Y = pX * sin + pY * cos + position.Y;
			v.TexCoords.X -= subRect.Width-2f*E;
			_vertices[vi + 3] = v;

			++_primitiveCount;
		}
	}
}


