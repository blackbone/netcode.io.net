using Netcode.io.OLD.Unity.Collections;
using Netcode.io.Serialization;

namespace Netcode.io.Messaging.Messages
{
    internal static class RpcMessageHelpers
    {
        public static unsafe void Serialize(ref FastBufferWriter writer, ref RpcMetadata metadata, ref FastBufferWriter payload)
        {
            BytePacker.WriteValueBitPacked(writer, metadata.NetworkObjectId);
            BytePacker.WriteValueBitPacked(writer, metadata.NetworkBehaviourId);
            BytePacker.WriteValueBitPacked(writer, metadata.NetworkRpcMethodId);
            writer.WriteBytesSafe(payload.GetUnsafePtr(), payload.Length);
        }

        public static unsafe bool Deserialize(ref FastBufferReader reader, ref NetworkContext context, ref RpcMetadata metadata, ref FastBufferReader payload)
        {
            ByteUnpacker.ReadValueBitPacked(reader, out metadata.NetworkObjectId);
            ByteUnpacker.ReadValueBitPacked(reader, out metadata.NetworkBehaviourId);
            ByteUnpacker.ReadValueBitPacked(reader, out metadata.NetworkRpcMethodId);

            var networkManager = (OLD.NetworkManager)context.SystemOwner;
            if (!networkManager.SpawnManager.SpawnedObjects.ContainsKey(metadata.NetworkObjectId))
            {
                networkManager.DeferredMessageManager.DeferMessage(DeferredMessageManager.TriggerType.OnSpawn, metadata.NetworkObjectId, reader, ref context);
                return false;
            }

            var networkObject = networkManager.SpawnManager.SpawnedObjects[metadata.NetworkObjectId];
            var networkBehaviour = networkManager.SpawnManager.SpawnedObjects[metadata.NetworkObjectId].GetNetworkBehaviourAtOrderIndex(metadata.NetworkBehaviourId);
            if (networkBehaviour == null)
            {
                return false;
            }

            if (!OLD.NetworkManager.__rpc_func_table.ContainsKey(metadata.NetworkRpcMethodId))
            {
                return false;
            }

            payload = new FastBufferReader(reader.GetUnsafePtrAtCurrentPosition(), Allocator.None, reader.Length - reader.Position);
            return true;
        }

        public static void Handle(ref NetworkContext context, ref RpcMetadata metadata, ref FastBufferReader payload, ref __RpcParams rpcParams)
        {
            var networkManager = (OLD.NetworkManager)context.SystemOwner;
            if (!networkManager.SpawnManager.SpawnedObjects.TryGetValue(metadata.NetworkObjectId, out var networkObject))
            {
                throw new InvalidOperationException($"An RPC called on a {nameof(NetworkObject)} that is not in the spawned objects list. Please make sure the {nameof(NetworkObject)} is spawned before calling RPCs.");
            }
            var networkBehaviour = networkObject.GetNetworkBehaviourAtOrderIndex(metadata.NetworkBehaviourId);

            try
            {
                OLD.NetworkManager.__rpc_func_table[metadata.NetworkRpcMethodId](networkBehaviour, payload, rpcParams);
            }
            catch (Exception ex)
            {
                ((object)null).LogException(new Exception("Unhandled RPC exception!", ex));
            }
        }
    }

    internal struct RpcMetadata : INetworkSerializeByMemcpy
    {
        public ulong NetworkObjectId;
        public ushort NetworkBehaviourId;
        public uint NetworkRpcMethodId;
    }

    internal struct ServerRpcMessage : INetworkMessage
    {
        public int Version => 0;

        public RpcMetadata Metadata;

        public FastBufferWriter WriteBuffer;
        public FastBufferReader ReadBuffer;

        public unsafe void Serialize(FastBufferWriter writer, int targetVersion)
        {
            RpcMessageHelpers.Serialize(ref writer, ref Metadata, ref WriteBuffer);
        }

        public unsafe bool Deserialize(FastBufferReader reader, ref NetworkContext context, int receivedMessageVersion)
        {
            return RpcMessageHelpers.Deserialize(ref reader, ref context, ref Metadata, ref ReadBuffer);
        }

        public void Handle(ref NetworkContext context)
        {
            var rpcParams = new __RpcParams
            {
                Server = new ServerRpcParams
                {
                    Receive = new ServerRpcReceiveParams
                    {
                        SenderClientId = context.SenderId
                    }
                }
            };
            RpcMessageHelpers.Handle(ref context, ref Metadata, ref ReadBuffer, ref rpcParams);
        }
    }

    internal struct ClientRpcMessage : INetworkMessage
    {
        public int Version => 0;

        public RpcMetadata Metadata;

        public FastBufferWriter WriteBuffer;
        public FastBufferReader ReadBuffer;

        public void Serialize(FastBufferWriter writer, int targetVersion)
        {
            RpcMessageHelpers.Serialize(ref writer, ref Metadata, ref WriteBuffer);
        }

        public bool Deserialize(FastBufferReader reader, ref NetworkContext context, int receivedMessageVersion)
        {
            return RpcMessageHelpers.Deserialize(ref reader, ref context, ref Metadata, ref ReadBuffer);
        }

        public void Handle(ref NetworkContext context)
        {
            var rpcParams = new __RpcParams
            {
                Client = new ClientRpcParams
                {
                    Receive = new ClientRpcReceiveParams
                    {
                    }
                }
            };
            RpcMessageHelpers.Handle(ref context, ref Metadata, ref ReadBuffer, ref rpcParams);
        }
    }
}
