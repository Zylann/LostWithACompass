using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
    public class AudioCategory
    {
        public const int NONE = -1;

        public int maxInstances = -1;

        public AudioCategory SetMaxInstances(int max)
        {
            maxInstances = max;
            return this;
        }
    }
}


