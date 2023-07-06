namespace Netcode.io.Timing
{
    internal sealed class RealTimeProvider
    {
        private readonly DateTimeOffset startupTime;
        private DateTimeOffset lastTickTime;

        public float RealTimeSinceStartup { get; private set; }
        public float UnscaledTime { get; private set; }
        public float UnscaledDeltaTime { get; private set; }
        public float DeltaTime { get; private set; }

        public RealTimeProvider() => startupTime = lastTickTime = DateTimeOffset.UtcNow;

        public void Tick()
        {
            var now = DateTimeOffset.UtcNow;
            RealTimeSinceStartup = UnscaledTime = (float)(now - startupTime).TotalSeconds;
            DeltaTime = UnscaledDeltaTime = (float)(now - lastTickTime).TotalSeconds;
            lastTickTime = now;
        }
    }
}
