using UnityEngine;

namespace ShmupBaby
{
    /// <summary>
    /// Manages the time for the game.
    /// </summary>
    public static class TimeManager
    {
        /// <summary>
        /// The default time scale for the game.
        /// </summary>
        private static float defaultTimeScale = 1;

        /// <summary>
        /// The default time scale for the game.
        /// </summary>
        public static float DefaultTimeScale
        {
            get { return defaultTimeScale; }
            set { defaultTimeScale = value; }
        }

        /// <summary>
        /// Pause time in the game.
        /// </summary>
        public static void Pause()
        {
            Time.timeScale = 0;
        }

        /// <summary>
        /// Sets the time scale to the default time scale.
        /// </summary>
        public static void UnPause()
        {
            Time.timeScale = DefaultTimeScale;
        }
    }

}