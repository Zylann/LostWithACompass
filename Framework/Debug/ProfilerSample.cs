using System;

namespace Framework
{
	public class ProfilerSample
	{
		public string name;
		public float beginTime;
		public float endTime;
		public bool recording;

		public ProfilerSample(string name)
		{
			this.name = name;
		}

		public float duration
		{
			get { return (recording ? Environment.TickCount : endTime) - beginTime; }
		}

		public void Begin()
		{
			beginTime = Environment.TickCount;
			recording = true;
		}

		public void End()
		{
			endTime = Environment.TickCount;
			recording = false;
		}
	}
}

