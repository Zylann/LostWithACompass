using SFML.Audio;
using SFML.Window;
using System.Collections.Generic;

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

        class Source
        {
            public int category = 0;
            public Sound sound = new Sound();
        }

		private Source[] _sources;
        private List<AudioCategory> _categories = new List<AudioCategory>();

		private AudioSystem()
		{
			_sources = new Source[MAX_SOURCES];
			for(int i = 0; i < _sources.Length; ++i)
			{
				_sources[i] = new Source();
			}

			Listener.Direction = new Vector3f(0, 0, -1);
			Listener.Position = new Vector3f(0, 0, 0);
		}

		public int maxSources
		{
			get { return _sources.Length; }
		}

        public AudioCategory AddCategory(int ID)
        {
            while(_categories.Count <= ID)
            {
                _categories.Add(new AudioCategory());
            }
            AudioCategory category = new AudioCategory();
            _categories.Add(category);
            return category;
        }

		public int GetUsedSourcesCount()
		{
			int count = 0;
			for(int i = 0; i < _sources.Length; ++i)
			{
                Source s = _sources[i];
				if(s.sound != null && s.sound.Status != SoundStatus.Stopped)
				{
					++count;
				}
			}
			return count;
		}

        public int GetUsedSourcesCount(int category)
        {
            int count = 0;
            for (int i = 0; i < _sources.Length; ++i)
            {
                Source s = _sources[i];
                if (s.sound != null && s.sound.Status != SoundStatus.Stopped && s.category == category)
                {
                    ++count;
                }
            }
            return count;
        }

		private Sound RequestFreeSource(int categoryID)
		{
			for(int i = 0; i < _sources.Length; ++i)
			{
				if(_sources[i].sound.Status == SoundStatus.Stopped)
				{
                    Source freeSource = _sources[i];

                    if(categoryID == AudioCategory.NONE)
                    {
    					return freeSource.sound;
                    }
                    else
                    {
                        AudioCategory category = _categories[categoryID];

                        if(category.maxInstances < 0)
                        {
                            return freeSource.sound;
                        }

                        int playingCount = GetUsedSourcesCount(categoryID);
                        if(playingCount < category.maxInstances)
                        {
                            return freeSource.sound;
                        }
                    }
				}
			}

			return null;
		}

		public Sound Play(SoundBuffer buffer, float volume = 1f, float pitch = 1f, bool loop=false, int category=0)
		{
            return Play(buffer, new Vector3f(0, 0, 0), volume, pitch, loop, false, category);
		}

		public Sound Play(SoundBuffer buffer, Vector3f position, float volume = 1f, float pitch = 1f, bool loop=false, bool relativeToListener=false, int category=0)
		{
			Sound source = RequestFreeSource(category);
			if (source == null)
            {
                Log.Debug("Sound play ignored (cat=" + category + ")");
                return null;
            }

            if(!relativeToListener && buffer.ChannelCount != 1)
            {
                Log.Warn("Stereo sound will not be spatialized");
            }

			source.SoundBuffer = buffer;
			source.Pitch = pitch;
			source.Volume = volume*100f;
            source.RelativeToListener = relativeToListener;
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

