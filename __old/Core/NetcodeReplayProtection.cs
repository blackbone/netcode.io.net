namespace NetcodeIO.NET.Core
{
    // TODO [Dmitrii Osipov] refactor to fixed size
    
    /// <summary>
    /// Helper class for protecting against packet replay
    /// </summary>
    internal unsafe struct ReplayProtection
    {
        private const int NetcodeReplayProtectionBufferSize = 256;
        
        private ulong mostRecentSequence;
        private fixed ulong receivedPackets[NetcodeReplayProtectionBufferSize];

        /// <summary>
        /// Reset the packet replay buffer
        /// </summary>
        public void Reset()
        {
            mostRecentSequence = 0;
            for (var i = 0; i < NetcodeReplayProtectionBufferSize; i++)
                receivedPackets[i] = ulong.MaxValue;
        }

        /// <summary>
        /// Check if the given packet was already received. If not, store it in the replay buffer.
        /// </summary>
        public bool IsAlreadyReceived(ulong sequence)
        {
            if ((sequence & ((ulong)1 << 63)) != 0)
                return false;

            if (sequence + NetcodeReplayProtectionBufferSize <= mostRecentSequence)
                return true;

            if (sequence > mostRecentSequence)
                mostRecentSequence = sequence;

            var index = (int)(sequence % NetcodeReplayProtectionBufferSize);
            if (receivedPackets[index] == 0xFFFFFFFFFFFFFFFF)
            {
                receivedPackets[index] = sequence;
                return false;
            }

            if (receivedPackets[index] >= sequence)
                return true;

            receivedPackets[index] = sequence;
            return false;
        }
    }
}