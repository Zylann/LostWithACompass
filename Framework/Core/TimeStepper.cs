using System;

namespace Framework
{
	class TimeStepper
	{
		private int _fps;
		private int _recordedFPS;
		private int _lastFPS;

		private int _minDelta;
		private int _maxDelta;
		private int _timeBefore;
		private int _rawDelta;
		private int _storedDelta;
		private int _startTime;

		public TimeStepper()
		{
			_fps = 0;
			_recordedFPS = 60;
			_minDelta = 1000 / 120;
			_maxDelta = 1000 / 60;
			_rawDelta = _maxDelta;
			_startTime = this.time;
		}

		public int time
		{
			get { return Environment.TickCount; }
		}

		public int rawDelta
		{
			get { return _rawDelta; }
		}

		public void SetDeltaRange(int minDelta, int maxDelta)
		{
			_minDelta = minDelta;
			_maxDelta = maxDelta;
		}

		public int minDelta
		{
			get { return _minDelta; }
		}

		public int maxDelta
		{
			get { return _maxDelta; }
		}

		public int RecordedFPS
		{
			get { return _recordedFPS; }
		}

		public void OnBeginFrame()
		{
			int t = this.time;

			_timeBefore = t;

			// Record FPS
			if(t - _lastFPS > 1000)
			{
				_lastFPS = t;
				_recordedFPS = _fps;
				_fps = 0;
			}
			++_fps;
		}

		public void OnEndFrame()
		{
			_rawDelta = this.time - _timeBefore;
			if (_rawDelta > _maxDelta)
			{
				_rawDelta = _maxDelta;
			}
		}

		public delegate void StepCallback(int delta);

		public int CallSteps(StepCallback stepFunc)
		{
			// Get current delta
			int delta = _rawDelta;
			if(_recordedFPS != 0)
			{
				delta = 1000 / _recordedFPS;
			}

			int updatesCount = 0;

			// Accumulate delta time
			_storedDelta += delta;

			// If the accumulated delta time is enough to trigger an update step
			if(_storedDelta >= _minDelta)
			{
				// Call update steps one or more times to match the elapsed time
				int cycles = _storedDelta / _maxDelta;
				for (int i = 0; i < cycles; ++i)
				{
					stepFunc(_maxDelta);
					++updatesCount;
				}

				// Update remainder time
				int remainder = _storedDelta % _maxDelta;
				if(remainder > _minDelta)
				{
					stepFunc(remainder);
					_storedDelta = 0;
					++updatesCount;
				}
				else
				{
					_storedDelta = remainder;
				}
			}

			return updatesCount;
		}
	}
}


