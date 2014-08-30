using SFML.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
    public class AudioParams
    {
        public float volume = 1;
        public float pitch = 1f;
        public int category = AudioCategory.NONE;
        public bool loop = false;

        public AudioParams SetVolume(float v)
        {
            volume = v;
            return this;
        }

        public AudioParams SetPitch(float p)
        {
            pitch = p;
            return this;
        }

        public AudioParams SetCategory(int cat)
        {
            category = cat;
            return this;
        }

        public AudioParams Loop()
        {
            loop = true;
            return this;
        }

        public void Apply(Sound s)
        {
            s.Volume = volume;
            s.Pitch = pitch;
            s.Loop = loop;
        }
    }
}
