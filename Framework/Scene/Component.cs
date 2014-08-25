
namespace Framework
{
	public abstract class Component
	{
		public int updateOrder = 0;

		public Entity entity;

		public virtual void OnCreate() { }
		public virtual void OnUpdate() { }
		public virtual void OnDestroy() { }

		public virtual void OnComponentAdded(Component c) { }
		public virtual void OnComponentRemoved(Component c) { }

		public World world
		{
			get { return entity.world; }
		}
	}
}

