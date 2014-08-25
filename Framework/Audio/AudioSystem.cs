using SFML.Audio;
using SFML.Window;

namespace Framework
{
	public class AudioSystem
	{
		private const int MAX_SOURCES = 28;
		private static AudioSystem __instance;
		public static AudioSystem instance
		{
			get
			{
				if (__instance == null)
					__instance = new AudioSystem();
				return __instance;
			}
		}

		private Sound[] _sources;

		private AudioSystem()
		{
			_sources = new Sound[MAX_SOURCES];
			for(int i = 0; i < _sources.Length; ++i)
			{
				_sources[i] = new Sound();
			}

			Listener.Direction = new Vector3f(0, 0, -1);
			Listener.Position = new Vector3f(0, 0, 0);
		}

		public int maxSources
		{
			get { return _sources.Length; }
		}

		public int GetUsedSourcesCount()
		{
			int count = 0;
			for(int i = 0; i < _sources.Length; ++i)
			{
				if(_sources[i] != null && _sources[i].Status != SoundStatus.Stopped)
				{
					++count;
				}
			}
			return count;
		}

		private Sound GetFreeSource()
		{
			for(int i = 0; i < _sources.Length; ++i)
			{
				if(_sources[i].Status == SoundStatus.Stopped)
				{
					return _sources[i];
				}
			}
			return null;
		}

		public Sound Play(SoundBuffer buffer, float volume = 1f, float pitch = 1f, bool loop=false)
		{
			Sound source = GetFreeSource();
			if (source == null)
				return null;

			source.SoundBuffer = buffer;
			source.Pitch = pitch;
			source.Volume = volume*100f;
			source.RelativeToListener = true;
			source.Position = new Vector3f(0, 0, 0);
			source.Attenuation = 0.5f;
			source.MinDistance = 1f;
            source.Loop = loop;
			source.Play();
			return source;
		}

		public Sound Play(SoundBuffer buffer, Vector3f position, float volume = 1f, float pitch = 1f, bool loop=false)
		{
			Sound source = GetFreeSource();
			if (source == null)
				return null;

			source.SoundBuffer = buffer;
			source.Pitch = pitch;
			source.Volume = volume*100f;
			source.RelativeToListener = false;
			source.Position = position;
			source.Attenuation = 0.5f;
			source.MinDistance = 1f;
            source.Loop = loop;
            source.Play();
			return source;
		}

		public void SetListenerPosition(Vector2f v)
		{
			//Listener.Position = new Vector3f(0, 0, 0);
			Listener.Position = new Vector3f(v.X, v.Y, 1f);
		}
	}
}

