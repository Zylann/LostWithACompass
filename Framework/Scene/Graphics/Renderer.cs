using Framework;
using SFML.Graphics;

namespace Framework
{
	public abstract class Renderer : Component
	{
		public int drawOrder;

		private Material[] _materials = new Material[1];
		private uint _layer;

		public abstract void Render(RenderTarget rt, RenderMode renderMode = RenderMode.BASE);

		public override void OnCreate()
		{
			base.OnCreate();
			world.graphics.Register(this);
		}

		public override void OnDestroy()
		{
			base.OnDestroy();
			world.graphics.Unregister(this);
		}

		public Renderer SetTexture(string name, RenderMode mode=RenderMode.BASE)
		{
			Texture tex = Assets.textures[name];
			return SetTexture(tex, mode);
		}

		public Renderer SetTexture(Texture tex, RenderMode mode=RenderMode.BASE)
		{
			Material m = GetOrCreateMaterial(mode);
			m.mainTexture = tex;
			return this;
		}

		public Renderer SetMaterial(Material mat, RenderMode mode=RenderMode.BASE)
		{
			int modeI = (int)mode;
			if(modeI >= _materials.Length)
			{
				System.Array.Resize<Material>(ref _materials, modeI+1);
			}
			_materials[modeI] = mat;
			return this;
		}

		private Material GetOrCreateMaterial(RenderMode mode)
		{
			int modeI = (int)mode;
			if (modeI >= _materials.Length)
			{
				System.Array.Resize<Material>(ref _materials, modeI + 1);
			}
			if (_materials[modeI] == null)
				_materials[modeI] = new Material();
			return _materials[modeI];
		}

		public Material GetMaterial(RenderMode mode = RenderMode.BASE)
		{
			int modeI = (int)mode;
			if(modeI >= _materials.Length)
				return null;
			return _materials[modeI];
		}

		public Texture mainTexture
		{
			get
			{
				Material m = GetMaterial(RenderMode.BASE);
				return m == null ? null : m.mainTexture;
			}
		}

		public Renderer SetLayer(uint layerIndex)
		{
			this.layer = layerIndex;
			return this;
		}

		public uint layer
		{
			get { return _layer; }
			set { _layer = value; }
		}
	}
}

