using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework
{
    public static class Vector2iExtensions
    {
        public static float Magnitude(this Vector2i a)
        {
            return Mathf.Sqrt(a.X * a.X + a.Y * a.Y);
        }

        public static float DistanceTo(this Vector2i a, Vector2i b)
        {
            return (a - b).Magnitude();
        }
    }
}


