namespace Netcode.io.OLD.UnityEngine
{
    public static class Time
    {
        [ThreadStatic] private static DateTimeOffset startupTime;
        private static DateTimeOffset lastTickTime;
        
        public static float realtimeSinceStartup { get; private set; }
        public static float unscaledTime { get; private set; }
        public static float deltaTime { get; private set; }
        public static float unscaledDeltaTime { get; private set; }

        static Time()
        {
            startupTime = DateTimeOffset.UtcNow;
        }

        // TODO [Dmitrii Osipov] need to call it somewhere
        public static void Tick()
        {
            realtimeSinceStartup = unscaledTime = (float)(DateTimeOffset.UtcNow - startupTime).TotalSeconds;
            deltaTime = unscaledDeltaTime = (float)(DateTimeOffset.UtcNow - lastTickTime).TotalSeconds;
            lastTickTime = DateTimeOffset.UtcNow;
        }
    }
}