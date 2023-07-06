using Netcode.io.OLD.Timing;
using Netcode.io.Serialization;

namespace Netcode.io.Messaging.Messages
{
    internal struct TimeSyncMessage : INetworkMessage, INetworkSerializeByMemcpy
    {
        public int Version => 0;

        public int Tick;

        public void Serialize(FastBufferWriter writer, int targetVersion)
        {
            BytePacker.WriteValueBitPacked(writer, Tick);
        }

        public bool Deserialize(FastBufferReader reader, ref NetworkContext context, int receivedMessageVersion) => false;

        public void Handle(ref NetworkContext context)
        {
            var networkManager = (OLD.NetworkManager)context.SystemOwner;
            var time = new NetworkTime(networkManager.NetworkTickSystem.TickRate, Tick);
            networkManager.NetworkTimeSystem.Sync(time.Time, networkManager.NetworkConfig.NetworkTransport.GetCurrentRtt(context.SenderId) / 1000d);
        }
    }
}
