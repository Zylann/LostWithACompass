using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Scene.Graphics
{
    /// <summary>
    /// Renderer displaying sprites through a SpriteBatch
    /// </summary>
    public abstract class SpriteBatchRenderer : Renderer
    {
        private SpriteBatch _spriteBatch;

        public SpriteBatchRenderer()
		{
			_spriteBatch = new SpriteBatch();
		}

		public override void OnCreate()
		{
			base.OnCreate();
		}

		public override void Render(RenderTarget rt, RenderMode renderMode=RenderMode.BASE)
		{
			Material mat = GetMaterial(renderMode);
			if (mat == null)
				return;

            OnDrawSprites(_spriteBatch);

			RenderStates states = new RenderStates(mat.mainTexture);
			states.BlendMode = mat.blendMode;
			states.Shader = mat.shader;
			_spriteBatch.Display(rt, states);

			_spriteBatch.Flush();
		}

        protected abstract void OnDrawSprites(SpriteBatch batch);
    }
}

