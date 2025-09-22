using System.Numerics;

namespace Atlantis
{
    public class Camera
    {
        public Vector2 Position = Vector2.Zero;

        private float angle = 0.0f;

        /// <summary>
        /// Angle in radians between 0 and PI*2
        /// </summary>
        public float Angle
        {
            get
            {
                return angle;
            }
            set
            {
                angle = value;
                if (angle < 0.0f)
                {
                    angle = MathF.PI * 2.0f + angle;
                }
                angle %= MathF.PI * 2.0f;
            }
        }

        /// <summary>
        /// Not used. Viewport size
        /// </summary>
        public Vector2 Viewport = Vector2.One;
    }
}


