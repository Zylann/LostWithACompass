using SFML.Audio;
using SFML.Window;

namespace Framework
{
	public class FootstepsSound : Behaviour
	{
		private SoundBuffer[] _vars;
		private float _stepInterval = 1f;
		private Vector2f _lastPosition;
		private float _distanceBeforeNextStep;
		private float _volume = 0.5f;

		public FootstepsSound SetStep(float distanceInterval)
		{
			_stepInterval = distanceInterval;
			return this;
		}

		public FootstepsSound SetVars(string baseName, int min, int max)
		{
			_vars = new SoundBuffer[max-min+1];
			for (int i = 0; i < _vars.Length; ++i)
			{
				_vars[i] = Assets.soundBuffers[baseName + (min+i)];
			}
			return this;
		}

		public FootstepsSound SetVars(string[] soundVars)
		{
			_vars = new SoundBuffer[soundVars.Length];
			for (int i = 0; i < _vars.Length; ++i)
			{
				_vars[i] = Assets.soundBuffers[soundVars[i]];
			}
			return this;
		}

		public FootstepsSound SetVolume(float volume)
		{
			_volume = volume;
			return this;
		}

		public override void OnUpdate()
		{
			Vector2f pos = entity.body.position;

			float d = Mathf.Magnitude(entity.body.position - _lastPosition);

			if (Mathf.Approximately(d, 0))
				_distanceBeforeNextStep = _stepInterval * 0.25f;

			_distanceBeforeNextStep -= d;
			if(_distanceBeforeNextStep <= 0)
			{
				PlayStep();
				_distanceBeforeNextStep = _stepInterval * Random.Range(0.9f, 1.1f);
			}

			_lastPosition = entity.body.position;
		}

		private void PlayStep()
		{
			int i = Random.Range(0, _vars.Length);
			float pitch = Random.Range(0.9f, 1.1f);
			entity.audio.Play(_vars[i], _volume, pitch);
		}
	}
}

