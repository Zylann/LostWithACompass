using SFML.Graphics;
using SFML.Window;

namespace Framework
{
	public class Camera : Component
	{
		public View view;
		public int layerMask;
		public Color clearColor;
		public bool doClear;
		public bool enableLighting;

		private RenderTarget _target;
		private int _depth;

		public override void OnCreate()
		{
			base.OnCreate();
			world.graphics.Register(this);

			OnScreenResized(Application.instance.ScreenSize);
		}

		public RenderTarget target
		{
			get
			{
				return _target;
			}
			set
			{
				_target = value;
				OnScreenResized(_target.Size);
			}
		}

		public override void OnDestroy()
		{
			base.OnDestroy();
			world.graphics.Unregister(this);
		}

		public Camera SetDepth(int depth)
		{
			_depth = depth;
			return this;
		}

		public int depth
		{
			get { return _depth; }
		}

		public Camera AddLayer(int layerIndex)
		{
			layerMask |= (1 << layerIndex);
			return this;
		}

		public Camera RemoveLayer(int layerIndex)
		{
			layerMask &= ~(1 << layerIndex);
			return this;
		}

		public Camera AddLayersUnder(int layerIndex)
		{
			while (layerIndex >= 0)
			{
				AddLayer(layerIndex--);
			}
			return this;
		}

		public void OnScreenResized(Vector2u newSize)
		{
			if (view == null)
			{
				view = new View(new FloatRect(0,0,newSize.X, newSize.Y));
			}
			view.Size = new Vector2f(newSize.X, newSize.Y);
		}
	}
}


