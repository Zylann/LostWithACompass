using System.Collections.Generic;

namespace Framework
{
	public class EntityList<T> where T : Entity
	{
		private List<T> _all = new List<T>();
		private List<T> _destroyed = new List<T>();

		public void Add(T e)
		{
			_all.Add(e);
		}

		public void UpdateAll()
		{
			for(int i = 0; i < _all.Count; ++i)
			{
                Entity e = _all[i];
				e.Update();
                if (e.destroyLaterFlag)
				{
					Log.Debug("Destroying entity " + e.name);
					e.DestroyNow();
				}
			}

			_all.RemoveAll(IsDestroyed);
		}

		//public void RenderAll(RenderTarget rt)
		//{
		//	for (int i = 0; i < _all.Count; ++i)
		//	{
		//		_all[i].Render(rt);
		//	}
		//}

		private static bool IsDestroyed(T e)
		{
			return e.Destroyed;
		}

		public void Clear()
		{
			foreach(Entity e in _all)
			{
				e.DestroyNow();
			}

			_all.Clear();
			_destroyed.Clear();
		}
	}
}

