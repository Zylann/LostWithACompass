using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
	public class SampledCurve
	{
		public delegate float Function(float x);

		private float[] _lookup;
		private float _min;
		private float _max;
		private float _inverseWidth;

		public SampledCurve(Function f, float min, float max, int resolution)
		{
			_lookup = new float[resolution];
			Sample(f, min, max);
		}

		public void Sample(Function f, float min, float max)
		{
			_min = min;
			_max = max;

			_inverseWidth = 1f / (float)(_max - _min);

			for (int i = 0; i < _lookup.Length; ++i)
			{
				float t = (float)i / (float)_lookup.Length;
				_lookup[i] = f(_min + t * (_max - _min));
			}
		}

		public float GetClamp(float x)
		{
			int i = Index(x);

			if (i >= _lookup.Length)
				i = _lookup.Length - 1;
			else if (i < 0)
				i = 0;

			return _lookup[i];
		}

		public float GetRepeat(float x)
		{
			int i = Mathf.Umod(Index(x), _lookup.Length); ;
			return _lookup[i];
		}

		private int Index(float x)
		{
			return (int)((float)(_lookup.Length) * (x - _min) * _inverseWidth);
		}
	}
}
