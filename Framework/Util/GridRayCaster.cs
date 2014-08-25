using SFML.Window;

namespace Framework
{
	public class GridRayCaster
	{
		public struct Hit
		{
			public bool isHit;
			public Vector2i pos; // Position of the cell that stopped the ray
			public Vector2i prevPos; // Previously crossed cell position (to find which face we have hit)
		};

		public delegate bool HitPredicate(int x, int y);

		// Result
		public Hit hit = new Hit(); // Result of ray casting. Contains last ray state if not.

		// Casts a ray, returns true if a collision occurred.
		// Options of the raycaster are used.
		// Important : you must normalize the ray direction,
		// otherwise the crossed distance will not be accurate.
		public Hit Cast(Vector2f origin, Vector2f direction, HitPredicate hitFunc, float maxDistance)
		{
			float infinite = 9999999;

			// Equation : p + v*t
			// p : ray start position (ray.pos)
			// v : ray orientation vector (ray.dir)
			// t : parametric variable = a distance if v is normalized

			// This raycasting technique is described here :
			// http://www.cse.yorku.ca/~amana/research/grid.pdf

			// Note : the grid is assumed to have 1-unit square cells.

			//
			// Initialisation
			//

			hit.isHit = false;

			// Cell position
			hit.pos.X = (int)Mathf.Floor(origin.X);
			hit.pos.Y = (int)Mathf.Floor(origin.Y);
			hit.prevPos = hit.pos;

			// Voxel step (will not change)
			int xi_step = direction.X > 0 ? 1 : direction.X < 0 ? -1 : 0;
			int yi_step = direction.Y > 0 ? 1 : direction.Y < 0 ? -1 : 0;

			// Parametric voxel step (will not change)
			float tdelta_x = xi_step != 0 ? 1f / Mathf.Abs(direction.X) : infinite;
			float tdelta_y = yi_step != 0 ? 1f / Mathf.Abs(direction.Y) : infinite;

			// Parametric grid-cross
			float tcross_x; // At which value of T we will cross a vertical line?
			float tcross_y; // At which value of T we will cross a horizontal line?

			// X initialization
			if (xi_step != 0)
			{
				if (xi_step == 1)
					tcross_x = (Mathf.Ceil(origin.X) - origin.X) * tdelta_x;
				else
					tcross_x = (origin.X - Mathf.Floor(origin.X)) * tdelta_x;
			}
			else
				tcross_x = infinite; // Will never cross on X

			// Y initialization
			if (yi_step != 0)
			{
				if (yi_step == 1)
					tcross_y = (Mathf.Ceil(origin.Y) - origin.Y) * tdelta_y;
				else
					tcross_y = (origin.Y - Mathf.Floor(origin.Y)) * tdelta_y;
			}
			else
				tcross_y = infinite; // Will never cross on X

			//
			// Iteration
			//

			do
			{
				hit.prevPos = hit.pos;
				if (tcross_x < tcross_y)
				{
					// X collision
					//hit.prevPos.x = hit.pos.x;
					hit.pos.X += xi_step;
					if (tcross_x > maxDistance)
						return hit;
					tcross_x += tdelta_x;
				}
				else
				{
					// Y collision
					//hit.prevPos.y = hit.pos.y;
					hit.pos.Y += yi_step;
					if (tcross_y > maxDistance)
						return hit;
					tcross_y += tdelta_y;
				}

			} while (!hitFunc(hit.pos.X, hit.pos.Y));

			return hit;
		}
	}
}


