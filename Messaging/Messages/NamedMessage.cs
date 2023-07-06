using Netcode.io.Serialization;

namespace Netcode.io.Messaging.Messages
{
    internal struct NamedMessage : INetworkMessage
    {
        public int Version => 0;

        public ulong Hash;
        public FastBufferWriter SendData;

        private FastBufferReader m_ReceiveData;

        public unsafe void Serialize(FastBufferWriter writer, int targetVersion)
        {
            writer.WriteValueSafe(Hash);
            writer.WriteBytesSafe(SendData.GetUnsafePtr(), SendData.Length);
        }

        public bool Deserialize(FastBufferReader reader, ref NetworkContext context, int receivedMessageVersion)
        {
            reader.ReadValueSafe(out Hash);
            m_ReceiveData = reader;
            return true;
        }

        public void Handle(ref NetworkContext context)
        {
            ((OLD.NetworkManager)context.SystemOwner).CustomMessagingManager.InvokeNamedMessage(Hash, context.SenderId, m_ReceiveData, context.SerializedHeaderSize);
        }
    }
}
