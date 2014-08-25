using Framework;
using SFML.Graphics;
using SFML.Window;

namespace Framework
{
	public class ParticleRenderer : Renderer
	{
		public ParticleSimulator simulator = new ParticleSimulator();

		private SpriteBatch _batch = new SpriteBatch();

		public override void OnUpdate()
		{
		}

		public override void Render(RenderTarget rt, RenderMode renderMode = RenderMode.BASE)
		{
			// Simulate
			float dt = world.delta;
			simulator.Update(dt);

			Material m = GetMaterial(renderMode);
			Texture tex = m.mainTexture;

			IntRect subRect = new IntRect(0, 0, (int)tex.Size.X, (int)tex.Size.Y);
			Vector2f scale = new Vector2f(1, 1);

			foreach(Particle p in simulator.particles)
			{
				_batch.DrawCentered(p.position, subRect, p.color, scale);
			}

			_batch.Display(rt, new RenderStates(tex));

			_batch.Flush();
		}
	}
}

