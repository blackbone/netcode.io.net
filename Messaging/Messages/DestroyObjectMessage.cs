using Netcode.io.Serialization;

namespace Netcode.io.Messaging.Messages
{
    internal struct DestroyObjectMessage : INetworkMessage, INetworkSerializeByMemcpy
    {
        public int Version => 0;

        public ulong NetworkObjectId;
        public bool DestroyGameObject;

        public void Serialize(FastBufferWriter writer, int targetVersion)
        {
            BytePacker.WriteValueBitPacked(writer, NetworkObjectId);
            writer.WriteValueSafe(DestroyGameObject);
        }

        public bool Deserialize(FastBufferReader reader, ref NetworkContext context, int receivedMessageVersion) => false;

        public void Handle(ref NetworkContext context)
        {
            var networkManager = (OLD.NetworkManager)context.SystemOwner;
            if (!networkManager.SpawnManager.SpawnedObjects.TryGetValue(NetworkObjectId, out var networkObject))
            {
                // This is the same check and log message that happens inside OnDespawnObject, but we have to do it here
                return;
            }

            networkManager.NetworkMetrics.TrackObjectDestroyReceived(context.SenderId, networkObject, context.MessageSize);
            networkManager.SpawnManager.OnDespawnObject(networkObject, DestroyGameObject);
        }
    }
}
