using SFML.Graphics;
using SFML.Window;
using System.Collections.Generic;
using System;

namespace Framework
{
	public class LightSystem
    {
        #region game jam shit
        public bool jamFlash = false;
        #endregion// Useless region since I might have fucked up other things at random in the framework

        /// <summary>
		/// Reference to the world this light system is in 
		/// </summary>
		public World world;

		/// <summary>
		/// Lightmap to render on the screen
		/// </summary>
		public RenderTexture screenShadowMap;

		/// <summary>
		/// List of all lights.
		/// </summary>
		public List<Light> lights = new List<Light>();

		/// <summary>
		/// Enables the light system or hides it.
		/// </summary>
		public bool enabled = true;

		/// <summary>
		/// Pre-rendered region of the scene where colors define how much light can travel through objects.
		/// It is shared between the lights for performance.
		/// </summary>
		public RenderTexture filterFrame;

		/// <summary>
		/// Atlas of lights. Each light is stored in a sub-frame.
		/// </summary>
		public RenderTexture lightAtlas;

		public const int ATLAS_SIZE = 4096;
		public const int ATLAS_FRAME_SIZE = 256;

		/// <summary>
		/// Maps atlas frames to the lights. -1 means no light is assigned.
		/// values are stored with [x+y*w] convention.
		/// </summary>
		private int[] _atlasFrames;

		/// <summary>
		/// Shader used to compute a light.
		/// </summary>
		public Shader lightShader;

		/// <summary>
		/// Sprite used to display the final lighting on the screen
		/// </summary>
		private Sprite _screenShadowSprite;

		/// <summary>
		/// Sprite used to render one light during the baking process
		/// </summary>
		private Sprite _lightSprite;

		public LightSystem(World worldRef, Vector2u screenSize)
		{
			world = worldRef;

			lightShader = Assets.shaders["light"];

			_screenShadowSprite = new Sprite();
			_screenShadowSprite.Origin = new Vector2f(0, 0);

			_lightSprite = new Sprite();

			_atlasFrames = new int[(ATLAS_SIZE / ATLAS_FRAME_SIZE) * (ATLAS_SIZE / ATLAS_FRAME_SIZE)];
			for (int i = 0; i < _atlasFrames.Length; ++i)
				_atlasFrames[i] = -1;

			lightAtlas = new RenderTexture(ATLAS_SIZE, ATLAS_SIZE, false);
			lightAtlas.Clear(Color.Black);
			lightAtlas.Display();
			lightAtlas.Smooth = false;

			filterFrame = new RenderTexture(ATLAS_FRAME_SIZE, ATLAS_FRAME_SIZE, false);
			filterFrame.Clear(Color.White);
			filterFrame.Display();
			filterFrame.Smooth = false;

			OnScreenResized(screenSize);
		}

		public void RegisterLight(Light light)
		{
			for(int i = 0; i < _atlasFrames.Length; ++i)
			{
				if(_atlasFrames[i] == -1)
				{
					_atlasFrames[i] = i;
					light.atlasIndex = i;
					lights.Add(light);
					return;
				}
			}

			throw new LightSystemException("Light count exceeded");
		}

		public void UnregisterLight(Light light)
		{
			_atlasFrames[light.atlasIndex] = -1;
			lights.Remove(light);
		}

		public void OnScreenResized(Vector2u size)
		{
			if (screenShadowMap != null)
			{
				screenShadowMap.Dispose();
			}

			screenShadowMap = new RenderTexture(size.X, size.Y);
			screenShadowMap.Clear(Color.Black);
			screenShadowMap.Display();
			screenShadowMap.Smooth = false;

			_screenShadowSprite = new Sprite();
			_screenShadowSprite.Texture = screenShadowMap.Texture;
		}

		public void Update()
		{
			foreach (Light light in lights)
			{
				light.OnUpdate();
			}
		}

		public void Draw(RenderTarget rt, int layerMask)
		{
			View view = rt.GetView();
			Vector2f viewOrigin = view.Center - view.Size/2f;
			Vector2f viewSize = view.Size;
			FloatRect viewRect = new FloatRect(viewOrigin.X, viewOrigin.Y, viewSize.X, viewSize.Y);
			screenShadowMap.SetView(view);

			int count = 0;

			_lightSprite.Texture = lightAtlas.Texture;

			// Draw self-illuminated elements
			world.graphics.Render(screenShadowMap, RenderMode.LIGHT_MAP, layerMask);

			// Draw lights
			foreach (Light light in lights)
			{
				FloatRect rect = light.bounds;

				if (viewRect.Intersects(rect))
				{
					if (light.needUpdate)
					{
						light.UpdateLight();
						light.needUpdate = false;
						lightAtlas.Display();
					}

					if(!Mathf.IsZero(light.intensity))
					{
						int framesX = ATLAS_SIZE / ATLAS_FRAME_SIZE;
						//int framesY = ATLAS_SIZE / ATLAS_FRAME_SIZE;

						_lightSprite.Origin = new Vector2f(ATLAS_FRAME_SIZE / 2, ATLAS_FRAME_SIZE / 2);
						_lightSprite.Position = light.worldPosition;
						_lightSprite.TextureRect = new IntRect(
							ATLAS_FRAME_SIZE * (light.atlasIndex % framesX),
							ATLAS_FRAME_SIZE * (light.atlasIndex / framesX),
							ATLAS_FRAME_SIZE,
							ATLAS_FRAME_SIZE
						);
						_lightSprite.Scale = new Vector2f(
							2f * light.radius / (float)ATLAS_FRAME_SIZE,
							2f * light.radius / (float)ATLAS_FRAME_SIZE
						);
						byte c = (byte)(light.intensity * 255f);
						_lightSprite.Color = new Color(c, c, c, 255);

						screenShadowMap.Draw(_lightSprite, new RenderStates(BlendMode.Add));

						++count;
					}
				}
			}

			// Update lightmap rt
			screenShadowMap.Display();

			// Blends the light map on the base render
			_screenShadowSprite.Position = viewOrigin;
			_screenShadowSprite.Texture = screenShadowMap.Texture;

			DebugOverlay.instance.Line("Render lights", count);
			DebugOverlay.instance.Line("Screen size", screenShadowMap.Size.X +"x"+ screenShadowMap.Size.Y);

			_screenShadowSprite.Color = Color.White;
			rt.Draw(_screenShadowSprite, new RenderStates(BlendMode.Multiply));
            if(jamFlash)
                rt.Draw(_screenShadowSprite, new RenderStates(BlendMode.Add));
            //_screenLightSprite.Color = new Color(32,32,32);
			//rt.Draw(_screenLightSprite, new RenderStates(BlendMode.Add));

			screenShadowMap.Clear(Color.White);
		}
	}

	class LightSystemException : Exception
	{
		private string _msg;

		public LightSystemException(string msg)
		{
			_msg = msg;
		}

		public override string Message
		{
			get { return "Light system error: " + _msg; }
		}
	}
}

