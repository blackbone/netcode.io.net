using Netcode.io.Serialization;

namespace Netcode.io.Messaging.Messages
{
    internal struct CreateObjectMessage : INetworkMessage
    {
        public int Version => 0;

        public NetworkObject.SceneObject ObjectInfo;
        private FastBufferReader m_ReceivedNetworkVariableData;

        public void Serialize(FastBufferWriter writer, int targetVersion)
        {
            ObjectInfo.Serialize(writer);
        }

        public bool Deserialize(FastBufferReader reader, ref NetworkContext context, int receivedMessageVersion) => false;

        public void Handle(ref NetworkContext context)
        {
            var networkManager = (OLD.NetworkManager)context.SystemOwner;
            var networkObject = NetworkObject.AddSceneObject(ObjectInfo, m_ReceivedNetworkVariableData, networkManager);

            networkManager.NetworkMetrics.TrackObjectSpawnReceived(context.SenderId, networkObject, context.MessageSize);
        }
    }
}
