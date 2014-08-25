using SFML.Graphics;
using SFML.Window;
using System.Collections.Generic;

namespace Framework
{
	public class PhysicsSystem
	{
		public World world;

		private List<BasicBody> _bodies = new List<BasicBody>();
		private List<Collider> _colliders = new List<Collider>();
		private GridRayCaster _raycaster;

		public PhysicsSystem(World world)
		{
			this.world = world;
			_raycaster = new GridRayCaster();
		}

		public void Register(Collider collider)
		{
			_colliders.Add(collider);
		}

		public void Unregister(Collider collider)
		{
			_colliders.Remove(collider);
		}

		public void Register(BasicBody body)
		{
			_bodies.Add(body);
		}

		public void Unregister(BasicBody body)
		{
			_bodies.Remove(body);
		}

		public bool Collides(FloatRect rect)
		{
			// TODO include bodies and collision layers
			foreach(Collider c in _colliders)
			{
				if (c.Collides(rect))
					return true;
			}
			return false;
		}

		public bool Collides(int x, int y)
		{
			Vector2f pos = new Vector2f(x,y);
			foreach(Collider c in _colliders)
			{
				if (c.Collides(pos))
					return true;
			}
			return false;
		}

		public GridRayCaster.Hit RayCast(Vector2f origin, Vector2f direction, float maxDistance)
		{
			// TODO include bodies and collision layers
			return _raycaster.Cast(origin, direction, Collides, maxDistance);
		}

		public void Update()
		{
			foreach (BasicBody body in _bodies)
			{
				body.OnUpdate();
			}
		}
	}
}

