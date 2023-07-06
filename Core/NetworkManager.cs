using Netcode.io.Configuration;
using Netcode.io.Connection;
using Netcode.io.Logging;
using Netcode.io.Messaging;
using Netcode.io.Timing;
using Netcode.io.Transports;
using UnityEngine;

namespace Netcode.io
{
    /// <summary>
    /// Connection Approval Request
    /// </summary>
    public struct ConnectionApprovalRequest
    {
        /// <summary>
        /// The connection data payload
        /// </summary>
        public byte[] Payload;
        /// <summary>
        /// The Network Id of the client we are about to handle
        /// </summary>
        public ulong ClientNetworkId;
    }

    /// <summary>
    /// Connection Approval Response
    /// </summary>
    public class ConnectionApprovalResponse
    {
        /// <summary>
        /// Whether or not the client was approved
        /// </summary>
        public bool Approved;
        /// <summary>
        /// If true, a player object will be created. Otherwise the client will have no object.
        /// </summary>
        public bool CreatePlayerObject;
        /// <summary>
        /// The prefabHash to use for the client. If createPlayerObject is false, this is ignored. If playerPrefabHash is null, the default player prefab is used.
        /// </summary>
        public uint PlayerPrefabHash;
        /// <summary>
        /// The position to spawn the client at. If null, the prefab position is used.
        /// </summary>
        public Vector3? Position;
        /// <summary>
        /// The rotation to spawn the client with. If null, the prefab position is used.
        /// </summary>
        public Quaternion? Rotation;
        /// <summary>
        /// If the Approval decision cannot be made immediately, the client code can set Pending to true, keep a reference to the ConnectionApprovalResponse object and write to it later. Client code must exercise care to setting all the members to the value it wants before marking Pending to false, to indicate completion. If the field is set as Pending = true, we'll monitor the object until it gets set to not pending anymore and use the parameters then.
        /// </summary>
        public bool Pending;

        /// <summary>
        /// Optional reason. If Approved is false, this reason will be sent to the client so they know why they
        /// were not approved.
        /// </summary>
        public string Reason;
    }
    
    /// <summary>
    /// The main component of the library
    /// </summary>
    public class NetworkManager
    {
        private ulong m_NextClientId = 1;
        private readonly Dictionary<ulong, ulong> m_ClientIdToTransportIdMap = new();
        private readonly Dictionary<ulong, ulong> m_TransportIdToClientIdMap = new();
        private readonly Dictionary<ulong, NetworkClient> m_ConnectedClients = new();
        private readonly Dictionary<ulong, PendingClient> m_PendingClients = new();
        private Dictionary<ulong, ConnectionApprovalResponse> m_ClientsToApprove = new ();
        
        internal RealTimeProvider RealTimeProvider { get; private set; }
        internal MessagingSystem MessagingSystem { get; private set; }
        public bool IsListening { get; private set; }
        public IReadOnlyDictionary<ulong, NetworkClient> ConnectedClients => m_ConnectedClients;

        private readonly NetworkTransport transport;
        
        public NetworkManager(NetworkConfig networkConfig, NetworkTransport transport)
        {
        }

        public void StartServer()
        {
            
        }

        public void Shutdown()
        {
            
        }

        public void Tick()
        {
            RealTimeProvider.Tick(); // this will increase tick
            
            OnNetworkEarlyUpdate();
            //OnNetworkPreUpdate();
            //OnNetworkPostLateUpdate();
        }
        
        private void OnNetworkEarlyUpdate()
        {
            if (!IsListening)
                return;

            //ProcessPendingApprovals();

            NetworkEvent networkEvent;
            do
            {
                networkEvent = transport.PollEvent(out var clientId, out var payload, out var receiveTime);
                HandleRawTransportPoll(networkEvent, clientId, payload, receiveTime);
                // Only do another iteration if: there are no more messages AND (there is no limit to max events or we have processed less than the maximum)
            } while (IsListening && networkEvent != NetworkEvent.Nothing);

            MessagingSystem.ProcessIncomingMessageQueue();
            MessagingSystem.CleanupDisconnectedClients();
        }
        
        private void HandleRawTransportPoll(NetworkEvent networkEvent, ulong clientId, ArraySegment<byte> payload, float receiveTime)
        {
            var transportId = clientId;
            switch (networkEvent)
            {
                case NetworkEvent.Connect:

                    // Assumptions:
                    // - When server receives a connection, it *must be* a client
                    // - When client receives one, it *must be* the server
                    // Client's can't connect to or talk to other clients.
                    // Server is a sentinel so only one exists, if we are server, we can't be
                    // connecting to it.
                    clientId = m_NextClientId++;
                    m_ClientIdToTransportIdMap[clientId] = transportId;
                    m_TransportIdToClientIdMap[transportId] = clientId;

                    MessagingSystem.ClientConnected(clientId);
                    if (NetworkLog.CurrentLogLevel <= LogLevel.Developer)
                        NetworkLog.LogInfo("Client Connected");

                    m_PendingClients.Add(clientId, new PendingClient
                    {
                        ClientId = clientId,
                        ConnectionState = PendingClient.State.PendingConnection
                    });

                    StartCoroutine(ApprovalTimeout(clientId));

                    break;
                case NetworkEvent.Data:
                    {
                        clientId = TransportIdToClientId(clientId);

                        HandleIncomingData(clientId, payload, receiveTime);
                        break;
                    }
                case NetworkEvent.Disconnect:
                    clientId = TransportIdCleanUp(clientId, transportId);

                    if (NetworkLog.CurrentLogLevel <= LogLevel.Developer)
                        NetworkLog.LogInfo($"Disconnect Event From {clientId}");

                    // Process the incoming message queue so that we get everything from the server disconnecting us
                    // or, if we are the server, so we got everything from that client.
                    MessagingSystem.ProcessIncomingMessageQueue();

                    try
                    {
                        OnClientDisconnectCallback?.Invoke(clientId);
                    }
                    catch (Exception exception)
                    {
                        this.LogException(exception);
                    }

                    OnClientDisconnectFromServer(clientId);
                    break;

                case NetworkEvent.TransportFailure:
                    NetworkLog.LogError($"Shutting down due to network transport failure of {NetworkConfig.NetworkTransport.GetType().Name}!");
                    OnTransportFailure?.Invoke();
                    Shutdown(true);
                    break;
            }
        }
        
        
        internal ulong TransportIdToClientId(ulong transportId)
        {
            return transportId == m_ServerTransportId ? ServerClientId : m_TransportIdToClientIdMap[transportId];
        }

        internal ulong ClientIdToTransportId(ulong clientId)
        {
            return clientId == ServerClientId ? m_ServerTransportId : m_ClientIdToTransportIdMap[clientId];
        }

    }
}
