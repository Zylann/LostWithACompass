using SFML.Audio;
using SFML.Window;
using System.Collections.Generic;

namespace Framework
{
	public class AudioEmitter : Component
	{
		public Vector2f position;
        public bool spatialized;

        private List<Sound> _sources = new List<Sound>();

		public override void OnCreate()
		{
			entity.world.behaviours.Add(this);
		}

		public override void OnDestroy()
		{
			Stop();
			entity.world.behaviours.Remove(this);
		}

		public void Stop()
		{
			foreach (Sound s in _sources)
			{
				s.Stop();
			}

			// Release all finished sounds
			_sources.RemoveAll(s => s.Status == SoundStatus.Stopped);
		}

        public AudioEmitter Spatialize()
        {
            spatialized = true;
            return this;
        }

		public override void OnUpdate()
		{
			if (entity.body != null)
			{
				position = entity.body.position;
			}
			else if (entity.sprite != null)
			{
				position = entity.sprite.position / (float)entity.world.TS;
			}

			UpdatePosition(position);

			// Release all finished sounds
			_sources.RemoveAll(s => s.Status == SoundStatus.Stopped);
		}

		private void UpdatePosition(Vector2f pos)
		{
			Vector3f pos3d = new Vector3f(pos.X, pos.Y, 0);
			foreach (Sound s in _sources)
			{
				s.Position = pos3d;
			}
		}

        public void Play(SoundBuffer buffer, float volume = 1f, float pitch = 1f, bool loop = false, int category = AudioCategory.NONE)
		{
			Vector3f pos3d = new Vector3f(position.X, position.Y, 0);

			Sound s = AudioSystem.instance.Play(buffer, pos3d, volume, pitch, loop, !spatialized, category);

			if(s != null)
			{
				_sources.Add(s);
			}
		}

		public void Play(string soundName, float volume=1f, float pitch=1f, bool loop=false, int category=AudioCategory.NONE)
		{
			Play(Assets.soundBuffers[soundName], volume, pitch, loop, category);
		}
	}
}




