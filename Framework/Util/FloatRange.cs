
namespace Framework
{
	public struct FloatRange
	{
		public float min;
		public float max;

		public FloatRange(float pmin, float pmax)
		{
			this.min = pmin;
			this.max = pmax;
			Repair();
		}

		public void Repair()
		{
			if(max < min)
			{
				float temp = max;
				max = min;
				min = temp;
			}
		}

		public float length
		{
			get { return max - min; }
		}

		public float Random()
		{
			return Framework.Random.Range(min, max);
		}
	}
}

