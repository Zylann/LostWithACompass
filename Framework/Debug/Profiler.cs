using System;
using System.Collections.Generic;

namespace Framework
{
	public class Profiler
	{
		public static Dictionary<string, ProfilerSample> samples = new Dictionary<string,ProfilerSample>();
		private static Stack<ProfilerSample> _sampleStack = new Stack<ProfilerSample>();

		public static void BeginSample(string sampleName)
		{
			ProfilerSample sample;
			if(!samples.TryGetValue(sampleName, out sample))
			{
				sample = new ProfilerSample(sampleName);
			}
			sample.Begin();
			_sampleStack.Push(sample);
		}

		public static float EndSample()
		{
			ProfilerSample sample = _sampleStack.Pop();
			sample.End();
			return sample.duration;
		}
	}
}

