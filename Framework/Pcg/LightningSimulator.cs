using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
	public class LightningSimulator
	{
		public float interval = 2f;
		public float maxDistance = 20f;
		public Vector2f origin;

		public List<Vector2f> points = new List<Vector2f>();

		public void Simulate()
		{
			Vector2f pos = origin;
			float distance = 0;

			points.Clear();

			while(distance < maxDistance)
			{
				pos.X += Random.Range(-interval, interval);
				pos.Y += Random.Range(-interval, interval);
				points.Add(pos);
				distance += interval;
			}
		}

		public void BendTo(Vector2f dst)
		{
			float distanceToOrigin = dst.DistanceTo(origin);

			for(int i = points.Count-1; i >= 0; --i)
			{
				Vector2f p = points[i];
				
				//float d = p.DistanceTo(dst);
				float k = (float)i / (float)points.Count;

				p = Mathf.Lerp(p, dst, k);

				points[i] = p;
			}
		}
	}
}
