using Framework;
using SFML.Graphics;
using SFML.Window;

namespace Framework
{
	public class SpriteRenderer : Renderer
	{
		private Sprite _sprite = new Sprite();

		public IntRect textureRect
		{
			get { return _sprite.TextureRect; }
			set { _sprite.TextureRect = value; }
		}

		public Vector2f position
		{
			get { return _sprite.Position; }
			set { _sprite.Position = value; }
		}

		public float rotation
		{
			get { return _sprite.Rotation; }
			set { _sprite.Rotation = value; }
		}

		public Vector2f scale
		{
			get { return _sprite.Scale; }
			set { _sprite.Scale = value; }
		}

        public Vector2f origin
        {
            get { return _sprite.Origin; }
            set { _sprite.Origin = value; }
        }

        public void CenterOrigin()
        {
            IntRect rect = _sprite.TextureRect;
            _sprite.Origin = new Vector2f(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
        }

		public FloatRect GetGlobalBounds()
		{
			return _sprite.GetGlobalBounds();
		}

		public override void Render(RenderTarget rt, RenderMode mode)
		{
			Material mat = GetMaterial(mode);
			if(mat != null)
			{
				RenderStates states = new RenderStates(mat.shader);
				states.BlendMode = mat.blendMode;
				_sprite.Texture = mat.mainTexture;
				rt.Draw(_sprite, states);
			}
		}
	}
}


