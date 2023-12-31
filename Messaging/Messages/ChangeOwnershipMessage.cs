using Netcode.io.Serialization;

namespace Netcode.io.Messaging.Messages
{
    internal struct ChangeOwnershipMessage : INetworkMessage, INetworkSerializeByMemcpy
    {
        public int Version => 0;

        public ulong NetworkObjectId;
        public ulong OwnerClientId;

        public void Serialize(FastBufferWriter writer, int targetVersion)
        {
            BytePacker.WriteValueBitPacked(writer, NetworkObjectId);
            BytePacker.WriteValueBitPacked(writer, OwnerClientId);
        }

        public bool Deserialize(FastBufferReader reader, ref NetworkContext context, int receivedMessageVersion) => false;

        public void Handle(ref NetworkContext context)
        {
            var networkManager = (OLD.NetworkManager)context.SystemOwner;
            var networkObject = networkManager.SpawnManager.SpawnedObjects[NetworkObjectId];
            var originalOwner = networkObject.OwnerClientId;

            networkObject.OwnerClientId = OwnerClientId;

            // We are current owner.
            if (originalOwner == networkManager.LocalClientId)
                networkObject.InvokeBehaviourOnLostOwnership();

            // We are new owner.
            if (OwnerClientId == networkManager.LocalClientId)
                networkObject.InvokeBehaviourOnGainedOwnership();

            // For all other clients that are neither the former or current owner, update the behaviours' properties
            if (OwnerClientId != networkManager.LocalClientId && originalOwner != networkManager.LocalClientId)
                for (var i = 0; i < networkObject.ChildNetworkBehaviours.Count; i++)
                    networkObject.ChildNetworkBehaviours[i].UpdateNetworkProperties();

            networkManager.NetworkMetrics.TrackOwnershipChangeReceived(context.SenderId, networkObject, context.MessageSize);
        }
    }
}
