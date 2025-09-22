namespace Atlantis
{
    public class KeyState
    {
        /// <summary>
        /// Pressed this frame
        /// </summary>
        public bool pressedNow = false;

        public float pressedAt = -1.0f;

        /// <summary>
        /// Released this frame
        /// </summary>
        public bool releasedNow = false;

        public float releasedAt = -1.0f;

        /// <summary>
        /// true if held down
        /// </summary>
        public bool isPressed = false;
    }
}


