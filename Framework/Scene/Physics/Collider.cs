using SFML.Graphics;
using SFML.Window;

namespace Framework
{
	/// <summary>
	/// Static collider.
	/// </summary>
	public abstract class Collider : Component
	{
		public override void OnCreate()
		{
			world.physics.Register(this);
		}

		public override void OnDestroy()
		{
			world.physics.Unregister(this);
		}

		public abstract FloatRect bounds { get; }

		public abstract bool Collides(Vector2f point);

		public abstract bool Collides(FloatRect box);

		public IntRect PixelBounds()
		{
			float ts = entity.world.TS;
			FloatRect b = this.bounds;
			return new IntRect(
				(int)(ts * b.Left),
				(int)(ts * b.Top),
				(int)(ts * b.Width),
				(int)(ts * b.Height)
			);
		}

		public Vector2f size
		{
			get
			{
				FloatRect b = this.bounds;
				return new Vector2f(b.Width, b.Height);
			}
		}

	}
}

