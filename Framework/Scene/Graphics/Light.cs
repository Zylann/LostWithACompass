using SFML.Graphics;
using SFML.Window;

namespace Framework
{
	/// <summary>
	/// Pixel-shader-based light.
	/// </summary>
	public class Light : Component
	{
		public Vector2f worldPosition;
		public Color color = Color.White;
		public int layerMask;

		private float _intensity = 1f;
		private float _radius = 250f;

		//public RenderTexture filterMap;
		//public RenderTexture lightMap;
		public bool needUpdate;

		private RectangleShape _fillRect;
		private View _view;

		public int atlasX;
		public int atlasY;

        private Sprite _cookieSprite;

		// TODO implement dynamic shader compilation
		//public const int HARD_RESOLUTION = 128; // Size of the light map texture
		public int atlasIndex;

		public float intensity
		{
			get { return _intensity; }
			set { _intensity = Mathf.Clamp01(value); }
		}

		public override void OnCreate()
		{
			needUpdate = true;
			_view = new View();
			_fillRect = new RectangleShape();
			entity.world.lightSystem.RegisterLight(this);
		}

        public void SetCookie(string textureName)
        {
            Texture cookie = Assets.textures[textureName];
            _cookieSprite = new Sprite(cookie);
            _cookieSprite.Origin = new Vector2f(cookie.Size.X / 2, cookie.Size.Y / 2);
        }

        public void SetCookieRotation(float rotation)
        {
            _cookieSprite.Rotation = rotation;
        }

		public void Poke()
		{
			needUpdate = true;
		}

		public override void OnDestroy()
		{
			entity.world.lightSystem.UnregisterLight(this);
		}

		public float radius
		{
			get { return _radius; }
		}

		public FloatRect bounds
		{
			get
			{
				return new FloatRect(
					worldPosition.X - _radius,
					worldPosition.Y - _radius,
					2f * _radius,
					2f * _radius
				);
			}
		}

		public Light AddLayer(int layerIndex)
		{
			layerMask |= (1 << layerIndex);
			return this;
		}

		public Light RemoveLayer(int layerIndex)
		{
			layerMask &= ~(1 << layerIndex);
			return this;
		}

		public Light AddLayersUnder(int layerIndex)
		{
			while(layerIndex >= 0)
			{
				AddLayer(layerIndex--);
			}
			return this;
		}

		public void UpdateLight()
		{
			RenderTexture filterMap = entity.world.lightSystem.filterFrame;
			RenderTexture lightAtlas = entity.world.lightSystem.lightAtlas;

			// Render filtermap under the light
			_view.Center = worldPosition;
			_view.Size = new Vector2f(2f * _radius, 2f * _radius);
			_view.Viewport = new FloatRect(0, 0, 1, 1);
			filterMap.SetView(_view);
			filterMap.Clear(Color.White);
			entity.world.graphics.Render(filterMap, RenderMode.LIGHT_FILTER, layerMask);
			filterMap.Display();

			// Calculate light atlas viewport
			int framesX = LightSystem.ATLAS_SIZE / LightSystem.ATLAS_FRAME_SIZE;
			int framesY = LightSystem.ATLAS_SIZE / LightSystem.ATLAS_FRAME_SIZE;
			int frameX = atlasIndex % framesX;
			int frameY = atlasIndex / framesX;
			FloatRect viewport = new FloatRect(
				(float)frameX / (float)framesX,
				(float)frameY / (float)framesY,
				1f / (float)framesX,
				1f / (float)framesY
			);
			_view.Reset(new FloatRect(
				0, 0, LightSystem.ATLAS_FRAME_SIZE, LightSystem.ATLAS_FRAME_SIZE
			));
			_view.Viewport = viewport;

			// Configure light shader
			Shader shader = entity.world.lightSystem.lightShader;
			shader.SetParameter("lightPos", new Vector2f(filterMap.Size.X/2, filterMap.Size.Y/2));
			shader.SetParameter("filterMap", filterMap.Texture);
			shader.SetParameter("lightColor", color);
			shader.SetParameter("fragOffset", new Vector2f(
				frameX * LightSystem.ATLAS_FRAME_SIZE,
				LightSystem.ATLAS_SIZE - LightSystem.ATLAS_FRAME_SIZE - frameY * LightSystem.ATLAS_FRAME_SIZE
			));

			// Render the light in a sub-rectangle of the lightAtlas
			RenderStates lightRenderStates = new RenderStates(shader);
			lightRenderStates.BlendMode = BlendMode.None;
			_fillRect.Size = _view.Size;
			_fillRect.FillColor = new Color(255, 255, 255, 255);
			lightAtlas.SetView(_view);
			lightAtlas.Draw(_fillRect, lightRenderStates);

            // Draw cookie
            if(_cookieSprite != null)
            {
                _cookieSprite.Position = _view.Size / 2;
                lightAtlas.Draw(_cookieSprite, new RenderStates(BlendMode.Multiply));
            }

			needUpdate = false;
		}

		private Vector2f GetNewPosition()
		{
			if (entity.sprite != null)
			{
				FloatRect b = entity.sprite.GetGlobalBounds();
				Vector2f p = new Vector2f(
					b.Left + b.Width / 2f,
					b.Top + b.Height / 2f
				);
				return p;
			}
			else if (entity.body != null)
			{
				return entity.body.pixelPosition;
			}
			else
			{
				return worldPosition;
			}
		}

		public override void OnUpdate()
		{
			Vector2f newPos = GetNewPosition();
			Vector2f delta = worldPosition - newPos;
			
			if(!Mathf.IsZero(delta.X) || !Mathf.IsZero(delta.Y))
			{
				// Match position
				worldPosition = newPos;
				needUpdate = true;
			}

			if(Keyboard.IsKeyPressed(Keyboard.Key.P))
			{
				needUpdate = true;
			}

            if(_cookieSprite != null)
            {
                // TODO Optimize only when the rotation changes...
                // Nonono it's LUDUM DARE #30 48h, we never optimize while we can play :'D
                needUpdate = true;
            }
		}

		public Light SetRadius(float radius)
		{
			this._radius = radius;
			return this;
		}

		public Light SetColor(Color color)
		{
			this.color = color;
			return this;
		}
	}
}

