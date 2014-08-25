using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
	public abstract class Behaviour : Component
	{
		public override void OnCreate()
		{
			entity.world.behaviours.Add(this);
		}

		public override void OnDestroy()
		{
			entity.world.behaviours.Remove(this);
		}
	}
}
