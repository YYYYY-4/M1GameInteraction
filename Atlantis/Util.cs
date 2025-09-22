using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis
{
    public static class Util
    {
        public static float DegToRad(this float degrees)
        {
            return degrees * (MathF.PI / 180.0f);
        }

        public static float RadToDeg(this float radians)
        {
            return radians * (180.0f / MathF.PI);
        }

        public static Vector2 AngleToDirection(this float angleRadians)
        {
            return new Vector2(MathF.Cos(angleRadians), MathF.Sin(angleRadians));
        }

    }
}
