using Netcode.io.Serialization;

namespace Netcode.io.Messaging
{
    /// <summary>
    /// Header placed at the start of each message batch
    /// </summary>
    internal struct BatchHeader : INetworkSerializeByMemcpy
    {
        internal const ushort MagicValue = 0x1160;
        /// <summary>
        /// A magic number to detect corrupt messages.
        /// Always set to k_MagicValue
        /// </summary>
        public ushort Magic;

        /// <summary>
        /// Total number of bytes in the batch.
        /// </summary>
        public int BatchSize;

        /// <summary>
        /// Hash of the message to detect corrupt messages.
        /// </summary>
        public ulong BatchHash;

        /// <summary>
        /// Total number of messages in the batch.
        /// </summary>
        public ushort BatchCount;
    }
}
