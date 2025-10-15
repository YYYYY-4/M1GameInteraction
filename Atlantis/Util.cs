using System.Numerics;

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

        public static double DegToRad(this double degrees)
        {
            return degrees * (Math.PI / 180.0);
        }

        public static double RadToDeg(this double radians)
        {
            return radians * (180.0 / Math.PI);
        }

        public static Vector2 AngleToDirection(this float angleRadians)
        {
            return new Vector2(MathF.Cos(angleRadians), MathF.Sin(angleRadians));
        }

        public static double NanToZero(double d)
        {
            return double.IsNaN(d) ? 0.0 : d;
        }

    }
}
