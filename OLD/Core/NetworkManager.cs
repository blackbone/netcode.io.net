using System.Collections;
using System.Runtime.CompilerServices;
using Netcode.io.Hashing;
using Netcode.io.Logging;
using Netcode.io.Messaging;
using Netcode.io.Messaging.Messages;
using Netcode.io.OLD.Configuration;
using Netcode.io.OLD.Connection;
using Netcode.io.OLD.Metrics;
using Netcode.io.OLD.SceneManagement;
using Netcode.io.OLD.Spawning;
using Netcode.io.OLD.Timing;
using Netcode.io.OLD.Transports;
using Netcode.io.OLD.Unity;
using Netcode.io.OLD.UnityEngine;
using Netcode.io.Serialization;

namespace Netcode.io.OLD
{
    /// <summary>
    /// The main component of the library
    /// </summary>
    public class NetworkManager : MonoBehaviour
    {
#pragma warning disable IDE1006 // disable naming rule violation check

        // RuntimeAccessModifiersILPP will make this `public`
        internal delegate void RpcReceiveHandler(NetworkBehaviour behaviour, FastBufferReader reader, __RpcParams parameters);

        // RuntimeAccessModifiersILPP will make this `public`
        internal static readonly Dictionary<uint, RpcReceiveHandler> __rpc_func_table = new();

#pragma warning restore IDE1006 // restore naming rule violation check

        private const double k_TimeSyncFrequency = 1.0d; // sync every second
        private const float k_DefaultBufferSizeSec = 0.05f; // todo talk with UX/Product, find good default value for this

        internal static string PrefabDebugHelper(NetworkPrefab networkPrefab)
        {
            return $"{nameof(NetworkPrefab)} \"{networkPrefab.Prefab.name}\"";
        }

        internal NetworkBehaviourUpdater BehaviourUpdater { get; set; }

        internal void MarkNetworkObjectDirty(NetworkObject networkObject)
        {
            BehaviourUpdater.AddForUpdate(networkObject);
        }

        internal MessagingSystem MessagingSystem { get; private set; }
        public Scene Scene { get; private set; }

        private NetworkPrefabHandler m_PrefabHandler;

        internal Dictionary<ulong, ConnectionApprovalResponse> ClientsToApprove = new Dictionary<ulong, ConnectionApprovalResponse>();

        // Stores the objects that need to be shown at end-of-frame
        internal Dictionary<ulong, List<NetworkObject>> ObjectsToShowToClient = new Dictionary<ulong, List<NetworkObject>>();

        /// <summary>
        /// The <see cref="NetworkPrefabHandler"/> instance created after starting the <see cref="NetworkManager"/>
        /// </summary>
        public NetworkPrefabHandler PrefabHandler
        {
            get
            {
                if (m_PrefabHandler == null)
                {
                    m_PrefabHandler = new NetworkPrefabHandler();
                }

                return m_PrefabHandler;
            }
        }

        private bool m_ShuttingDown;
        private bool m_StopProcessingMessages;

        /// <summary>
        /// When disconnected from the server, the server may send a reason. If a reason was sent, this property will
        /// tell client code what the reason was. It should be queried after the OnClientDisconnectCallback is called
        /// </summary>
        public string DisconnectReason { get; internal set; }

        private class NetworkManagerHooks : INetworkHooks
        {
            private NetworkManager m_NetworkManager;

            internal NetworkManagerHooks(NetworkManager manager)
            {
                m_NetworkManager = manager;
            }

            public void OnBeforeSendMessage<T>(ulong clientId, ref T message, NetworkDelivery delivery) where T : INetworkMessage
            {
            }

            public void OnAfterSendMessage<T>(ulong clientId, ref T message, NetworkDelivery delivery, int messageSizeBytes) where T : INetworkMessage
            {
            }

            public void OnBeforeReceiveMessage(ulong senderId, Type messageType, int messageSizeBytes)
            {
            }

            public void OnAfterReceiveMessage(ulong senderId, Type messageType, int messageSizeBytes)
            {
            }

            public void OnBeforeSendBatch(ulong clientId, int messageCount, int batchSizeInBytes, NetworkDelivery delivery)
            {
            }

            public void OnAfterSendBatch(ulong clientId, int messageCount, int batchSizeInBytes, NetworkDelivery delivery)
            {
            }

            public void OnBeforeReceiveBatch(ulong senderId, int messageCount, int batchSizeInBytes)
            {
            }

            public void OnAfterReceiveBatch(ulong senderId, int messageCount, int batchSizeInBytes)
            {
            }

            public bool OnVerifyCanSend(ulong destinationId, Type messageType, NetworkDelivery delivery)
            {
                return !m_NetworkManager.m_StopProcessingMessages;
            }

            public bool OnVerifyCanReceive(ulong senderId, Type messageType, FastBufferReader messageContent, ref NetworkContext context)
            {
                if (messageType == typeof(ConnectionApprovedMessage))
                {
                    if (NetworkLog.CurrentLogLevel <= LogLevel.Normal)
                        NetworkLog.LogError($"A {nameof(ConnectionApprovedMessage)} was received from a client on the server side. This should not happen. Please report this to the Netcode for GameObjects team at https://github.com/Unity-Technologies/com.unity.netcode.gameobjects/issues and include the following data: Message Size: {messageContent.Length}. Message Content: {MessagingSystem.ByteArrayToString(messageContent.ToArray(), 0, messageContent.Length)}");

                    return false;
                }

                if (m_NetworkManager.PendingClients.TryGetValue(senderId, out PendingClient client) &&
                    (client.ConnectionState == PendingClient.State.PendingApproval || (client.ConnectionState == PendingClient.State.PendingConnection && messageType != typeof(ConnectionRequestMessage))))
                {
                    if (NetworkLog.CurrentLogLevel <= LogLevel.Normal) NetworkLog.LogWarning($"Message received from {nameof(senderId)}={senderId} before it has been accepted");

                    return false;
                }

                if (m_NetworkManager.ConnectedClients.TryGetValue(senderId, out NetworkClient connectedClient) && messageType == typeof(ConnectionRequestMessage))
                {
                    if (NetworkLog.CurrentLogLevel <= LogLevel.Normal)
                        NetworkLog.LogError($"A {nameof(ConnectionRequestMessage)} was received from a client when the connection has already been established. This should not happen. Please report this to the Netcode for GameObjects team at https://github.com/Unity-Technologies/com.unity.netcode.gameobjects/issues and include the following data: Message Size: {messageContent.Length}. Message Content: {MessagingSystem.ByteArrayToString(messageContent.ToArray(), 0, messageContent.Length)}");

                    return false;
                }

                return !m_NetworkManager.m_StopProcessingMessages;
            }

            public void OnBeforeHandleMessage<T>(ref T message, ref NetworkContext context) where T : INetworkMessage
            {
            }

            public void OnAfterHandleMessage<T>(ref T message, ref NetworkContext context) where T : INetworkMessage
            {
            }
        }

        private class NetworkManagerMessageSender : IMessageSender
        {
            private NetworkManager m_NetworkManager;

            public NetworkManagerMessageSender(NetworkManager manager)
            {
                m_NetworkManager = manager;
            }

            public void Send(ulong clientId, NetworkDelivery delivery, FastBufferWriter batchData)
            {
                var sendBuffer = batchData.ToTempByteArray();

                m_NetworkManager.NetworkConfig.NetworkTransport.Send(m_NetworkManager.ClientIdToTransportId(clientId), sendBuffer, delivery);
            }
        }

        /// <summary>
        /// Returns the <see cref="GameObject"/> to use as the override as could be defined within the NetworkPrefab list
        /// Note: This should be used to create <see cref="GameObject"/> pools (with <see cref="NetworkObject"/> components)
        /// under the scenario where you are using the Host model as it spawns everything locally. As such, the override
        /// will not be applied when spawning locally on a Host.
        /// Related Classes and Interfaces:
        /// <see cref="NetworkPrefabHandler"/>
        /// <see cref="INetworkPrefabInstanceHandler"/>
        /// </summary>
        /// <param name="gameObject">the <see cref="GameObject"/> to be checked for a <see cref="NetworkManager"/> defined NetworkPrefab override</param>
        /// <returns>a <see cref="GameObject"/> that is either the override or if no overrides exist it returns the same as the one passed in as a parameter</returns>
        public GameObject GetNetworkPrefabOverride(GameObject gameObject)
        {
            if (gameObject.TryGetComponent<NetworkObject>(out var networkObject))
            {
                if (NetworkConfig.Prefabs.NetworkPrefabOverrideLinks.ContainsKey(networkObject.GlobalObjectIdHash))
                {
                    switch (NetworkConfig.Prefabs.NetworkPrefabOverrideLinks[networkObject.GlobalObjectIdHash].Override)
                    {
                        case NetworkPrefabOverride.Hash:
                        case NetworkPrefabOverride.Prefab:
                            {
                                return NetworkConfig.Prefabs.NetworkPrefabOverrideLinks[networkObject.GlobalObjectIdHash].OverridingTargetPrefab;
                            }
                    }
                }
            }
            return gameObject;
        }

        /// <summary>
        /// Accessor for the <see cref="NetworkTimeSystem"/> of the NetworkManager.
        /// Prefer the use of the LocalTime and ServerTime properties
        /// </summary>
        public NetworkTimeSystem NetworkTimeSystem { get; private set; }

        /// <summary>
        /// Accessor for the <see cref="NetworkTickSystem"/> of the NetworkManager.
        /// </summary>
        public NetworkTickSystem NetworkTickSystem { get; private set; }

        /// <summary>
        /// The local <see cref="NetworkTime"/>
        /// </summary>
        public NetworkTime LocalTime => NetworkTickSystem?.LocalTime ?? default;

        /// <summary>
        /// The <see cref="NetworkTime"/> on the server
        /// </summary>
        public NetworkTime ServerTime => NetworkTickSystem?.ServerTime ?? default;

        /// <summary>
        /// The log level to use
        /// </summary>
        public LogLevel LogLevel = LogLevel.Normal;

        /// <summary>
        /// Gets the SpawnManager for this NetworkManager
        /// </summary>
        public NetworkSpawnManager SpawnManager { get; private set; }

        internal DeferredMessageManager DeferredMessageManager { get; private set; }

        internal RealTimeProvider RealTimeProvider { get; private set; }

        /// <summary>
        /// Gets the CustomMessagingManager for this NetworkManager
        /// </summary>
        public CustomMessagingManager CustomMessagingManager { get; private set; }

        /// <summary>
        /// The <see cref="NetworkSceneManager"/> instance created after starting the <see cref="NetworkManager"/>
        /// </summary>
        public NetworkSceneManager SceneManager { get; private set; }

        /// <summary>
        /// The client id used to represent the server
        /// </summary>
        public const ulong ServerClientId = 0;

        /// <summary>
        /// Gets the networkId of the server
        /// </summary>
        private ulong m_ServerTransportId => NetworkConfig.NetworkTransport?.ServerClientId ?? throw new NullReferenceException($"The transport in the active {nameof(NetworkConfig)} is null");

        /// <summary>
        /// Returns ServerClientId if IsServer or LocalClientId if not
        /// </summary>
        public ulong LocalClientId
        {
            get => m_LocalClientId;
            internal set => m_LocalClientId = value;
        }

        private ulong m_LocalClientId;

        private Dictionary<ulong, NetworkClient> m_ConnectedClients = new();

        private ulong m_NextClientId = 1;
        private Dictionary<ulong, ulong> m_ClientIdToTransportIdMap = new Dictionary<ulong, ulong>();
        private Dictionary<ulong, ulong> m_TransportIdToClientIdMap = new Dictionary<ulong, ulong>();

        private List<NetworkClient> m_ConnectedClientsList = new List<NetworkClient>();

        private List<ulong> m_ConnectedClientIds = new List<ulong>();

        /// <summary>
        /// Gets a dictionary of connected clients and their clientId keys. This is only accessible on the server.
        /// </summary>
        public IReadOnlyDictionary<ulong, NetworkClient> ConnectedClients => m_ConnectedClients;

        /// <summary>
        /// Gets a list of connected clients. This is only accessible on the server.
        /// </summary>
        public IReadOnlyList<NetworkClient> ConnectedClientsList => m_ConnectedClientsList;

        /// <summary>
        /// Gets a list of just the IDs of all connected clients. This is only accessible on the server.
        /// </summary>
        public IReadOnlyList<ulong> ConnectedClientsIds => m_ConnectedClientIds;

        /// <summary>
        /// Gets the local <see cref="NetworkClient"/> for this client.
        /// </summary>
        public NetworkClient LocalClient { get; internal set; }

        /// <summary>
        /// Gets a dictionary of the clients that have been accepted by the transport but are still pending by the Netcode. This is only populated on the server.
        /// </summary>
        public readonly Dictionary<ulong, PendingClient> PendingClients = new();

        /// <summary>
        /// Gets Whether or not we are listening for connections
        /// </summary>
        public bool IsListening { get; private set; }

        /// <summary>
        /// Can be used to determine if the <see cref="NetworkManager"/> is currently shutting itself down
        /// </summary>
        public bool ShutdownInProgress { get { return m_ShuttingDown; } }

        /// <summary>
        /// The callback to invoke once a client connects. This callback is only ran on the server and on the local client that connects.
        /// </summary>
        public event Action<ulong> OnClientConnectedCallback = null;

        internal void InvokeOnClientConnectedCallback(ulong clientId) => OnClientConnectedCallback?.Invoke(clientId);

        /// <summary>
        /// The callback to invoke when a client disconnects. This callback is only ran on the server and on the local client that disconnects.
        /// </summary>
        public event Action<ulong> OnClientDisconnectCallback = null;

        /// <summary>
        /// This callback is invoked when the local server is started and listening for incoming connections.
        /// </summary>
        public event Action OnServerStarted = null;

        /// <summary>
        /// The callback to invoke once the local client is ready
        /// </summary>
        public event Action OnClientStarted = null;

        /// <summary>
        /// This callback is invoked once the local server is stopped.
        /// </summary>
        /// <param name="arg1">The first parameter of this event will be set to <see cref="true"/> when stopping a host instance and <see cref="false"/> when stopping a server instance.</param>
        public event Action<bool> OnServerStopped = null;

        /// <summary>
        /// The callback to invoke once the local client stops
        /// </summary>
        /// <remarks>The parameter states whether the client was running in host mode</remarks>
        /// <param name="arg1">The first parameter of this event will be set to <see cref="true"/> when stopping the host client and <see cref="false"/> when stopping a standard client instance.</param>
        public event Action<bool> OnClientStopped = null;

        /// <summary>
        /// The callback to invoke if the <see cref="NetworkTransport"/> fails.
        /// </summary>
        /// <remarks>
        /// A failure of the transport is always followed by the <see cref="NetworkManager"/> shutting down. Recovering
        /// from a transport failure would normally entail reconfiguring the transport (e.g. re-authenticating, or
        /// recreating a new service allocation depending on the transport) and restarting the client/server/host.
        /// </remarks>
        public event Action OnTransportFailure = null;

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
        /// The callback to invoke during connection approval. Allows client code to decide whether or not to allow incoming client connection
        /// </summary>
        public Action<ConnectionApprovalRequest, ConnectionApprovalResponse> ConnectionApprovalCallback
        {
            get => m_ConnectionApprovalCallback;
            set
            {
                if (value != null && value.GetInvocationList().Length > 1)
                {
                    throw new InvalidOperationException($"Only one {nameof(ConnectionApprovalCallback)} can be registered at a time.");
                }
                else
                {
                    m_ConnectionApprovalCallback = value;
                }
            }
        }

        private Action<ConnectionApprovalRequest, ConnectionApprovalResponse> m_ConnectionApprovalCallback;

        /// <summary>
        /// The current NetworkConfig
        /// </summary>
        public NetworkConfig NetworkConfig;
        
        public NetworkManager(NetworkConfig config, NetworkTransport transport)
        {
            config.NetworkTransport = transport;
            NetworkConfig = config;
        }

        /// <summary>
        /// The current host name we are connected to, used to validate certificate
        /// </summary>
        public string ConnectedHostname { get; private set; }

        internal INetworkMetrics NetworkMetrics { get; private set; }

        internal static event Action OnSingletonReady;

        private void Reset()
        {
        }

        /// <summary>
        /// Adds a new prefab to the network prefab list.
        /// This can be any GameObject with a NetworkObject component, from any source (addressables, asset
        /// bundles, Resource.Load, dynamically created, etc)
        ///
        /// There are three limitations to this method:
        /// - If you have NetworkConfig.ForceSamePrefabs enabled, you can only do this before starting
        /// networking, and the server and all connected clients must all have the same exact set of prefabs
        /// added via this method before connecting
        /// - Adding a prefab on the server does not automatically add it on the client - it's up to you
        /// to make sure the client and server are synchronized via whatever method makes sense for your game
        /// (RPCs, configs, deterministic loading, etc)
        /// - If the server sends a Spawn message to a client that has not yet added a prefab for, the spawn message
        /// and any other relevant messages will be held for a configurable time (default 1 second, configured via
        /// NetworkConfig.SpawnTimeout) before an error is logged. This is intented to enable the SDK to gracefully
        /// handle unexpected conditions (slow disks, slow network, etc) that slow down asset loading. This timeout
        /// should not be relied on and code shouldn't be written around it - your code should be written so that
        /// the asset is expected to be loaded before it's needed.
        /// </summary>
        /// <param name="prefab"></param>
        /// <exception cref="Exception"></exception>
        public void AddNetworkPrefab(GameObject prefab)
        {
            if (IsListening && NetworkConfig.ForceSamePrefabs)
                throw new Exception($"All prefabs must be registered before starting {nameof(NetworkManager)} when {nameof(NetworkConfig.ForceSamePrefabs)} is enabled.");

            var networkObject = prefab.GetComponent<NetworkObject>();
            if (!networkObject)
                throw new Exception($"All {nameof(NetworkPrefab)}s must contain a {nameof(NetworkObject)} component.");

            var networkPrefab = new NetworkPrefab { Prefab = prefab };
            bool added = NetworkConfig.Prefabs.Add(networkPrefab);
            if (IsListening && added)
                DeferredMessageManager.ProcessTriggers(DeferredMessageManager.TriggerType.OnAddPrefab, networkObject.GlobalObjectIdHash);
        }

        /// <summary>
        /// Remove a prefab from the prefab list.
        /// As with AddNetworkPrefab, this is specific to the client it's called on -
        /// calling it on the server does not automatically remove anything on any of the
        /// client processes.
        ///
        /// Like AddNetworkPrefab, when NetworkConfig.ForceSamePrefabs is enabled,
        /// this cannot be called after connecting.
        /// </summary>
        /// <param name="prefab"></param>
        public void RemoveNetworkPrefab(GameObject prefab)
        {
            if (IsListening && NetworkConfig.ForceSamePrefabs)
                throw new Exception($"Prefabs cannot be removed after starting {nameof(NetworkManager)} when {nameof(NetworkConfig.ForceSamePrefabs)} is enabled.");

            var globalObjectIdHash = prefab.GetComponent<NetworkObject>().GlobalObjectIdHash;
            NetworkConfig.Prefabs.Remove(prefab);
            if (PrefabHandler.ContainsHandler(globalObjectIdHash))
                PrefabHandler.RemoveHandler(globalObjectIdHash);
        }

        internal void Initialize()
        {
            DisconnectReason = string.Empty;

            if (NetworkLog.CurrentLogLevel <= LogLevel.Developer)
                NetworkLog.LogInfo(nameof(Initialize));

            MessagingSystem = new MessagingSystem(new NetworkManagerMessageSender(this), this);
            MessagingSystem.Hook(new NetworkManagerHooks(this));
            LocalClientId = ulong.MaxValue;

            ClearClients();

            // Create spawn manager instance
            SpawnManager = new NetworkSpawnManager(this);
            DeferredMessageManager = new DeferredMessageManager(this);
            RealTimeProvider = new RealTimeProvider();
            CustomMessagingManager = new CustomMessagingManager(this);
            
            SceneManager = new NetworkSceneManager(this);
            BehaviourUpdater = new NetworkBehaviourUpdater();
            NetworkMetrics ??= new NullNetworkMetrics();

            if (NetworkConfig.NetworkTransport == null)
            {
                if (NetworkLog.CurrentLogLevel <= LogLevel.Error)
                    NetworkLog.LogError("No transport has been selected!");

                return;
            }

            NetworkConfig.NetworkTransport.NetworkMetrics = NetworkMetrics;
            NetworkTimeSystem = NetworkTimeSystem.ServerTimeSystem();

            NetworkTickSystem = new NetworkTickSystem(NetworkConfig.TickRate, NetworkTimeSystem.LocalTime, NetworkTimeSystem.ServerTime);
            NetworkTickSystem.Reset(NetworkTimeSystem.LocalTime, NetworkTimeSystem.ServerTime);
            NetworkTickSystem.Tick += OnNetworkManagerTick;

            NetworkConfig.InitializePrefabs();

            // If we have a player prefab, then we need to verify it is in the list of NetworkPrefabOverrideLinks for client side spawning.
            // if (NetworkConfig.PlayerPrefab != null)
            // {
            //     if (NetworkConfig.PlayerPrefab.TryGetComponent<NetworkObject>(out var playerPrefabNetworkObject))
            //     {
            //         //In the event there is no NetworkPrefab entry (i.e. no override for default player prefab)
            //         if (!NetworkConfig.Prefabs.NetworkPrefabOverrideLinks.ContainsKey(playerPrefabNetworkObject.GlobalObjectIdHash))
            //         {
            //             //Then add a new entry for the player prefab
            //             AddNetworkPrefab(NetworkConfig.PlayerPrefab);
            //         }
            //     }
            //     else
            //     {
            //         // Provide the name of the prefab with issues so the user can more easily find the prefab and fix it
            //         NetworkLog.LogError($"{nameof(NetworkConfig.PlayerPrefab)} (\"{NetworkConfig.PlayerPrefab.name}\") has no NetworkObject assigned to it!.");
            //     }
            // }

            NetworkConfig.NetworkTransport.OnTransportEvent += HandleRawTransportPoll;
            NetworkConfig.NetworkTransport.Initialize(this);
        }

        private void ClearClients()
        {
            PendingClients.Clear();
            m_ConnectedClients.Clear();
            m_ConnectedClientsList.Clear();
            m_ConnectedClientIds.Clear();
            LocalClient = null;
            NetworkObject.OrphanChildren.Clear();
            ClientsToApprove.Clear();
        }

        /// <summary>
        /// Starts a server
        /// </summary>
        /// <returns>(<see cref="true"/>/<see cref="false"/>) returns true if <see cref="NetworkManager"/> started in server mode successfully.</returns>
        public bool StartServer()
        {
            if (NetworkLog.CurrentLogLevel <= LogLevel.Developer)
                NetworkLog.LogInfo(nameof(StartServer));

            if (!CanStart())
                return false;

            Initialize();
            IsListening = true;
            LocalClientId = ServerClientId;

            try
            {
                // If we failed to start then shutdown and notify user that the transport failed to start
                if (NetworkConfig.NetworkTransport.StartServer())
                {
                    SpawnManager.ServerSpawnSceneObjectsOnStartSweep();

                    OnServerStarted?.Invoke();
                    return true;
                }

                IsListening = false;

                NetworkLog.LogError($"Server is shutting down due to network transport start failure of {NetworkConfig.NetworkTransport.GetType().Name}!");
                OnTransportFailure?.Invoke();
                Shutdown();
            }
            catch (Exception)
            {
                IsListening = false;
                throw;
            }

            return false;
        }

        private bool CanStart()
        {
            if (IsListening)
            {
                if (NetworkLog.CurrentLogLevel <= LogLevel.Normal)
                    NetworkLog.LogWarning("Cannot start server while an instance is already running");

                return false;
            }

            // Only if it is starting as a server or host do we need to check this
            // Clients don't invoke the ConnectionApprovalCallback
            if (NetworkConfig.ConnectionApproval)
            {
                if (ConnectionApprovalCallback == null)
                {
                    if (NetworkLog.CurrentLogLevel <= LogLevel.Normal)
                    {
                        NetworkLog.LogWarning(
                            "No ConnectionApproval callback defined. Connection approval will timeout");
                    }
                }
            }

            if (ConnectionApprovalCallback == null) return true;
            if (NetworkConfig.ConnectionApproval) return true;
            if (NetworkLog.CurrentLogLevel > LogLevel.Normal) return true;
            
            NetworkLog.LogWarning("A ConnectionApproval callback is defined but ConnectionApproval is disabled. In order to use ConnectionApproval it has to be explicitly enabled ");

            return true;
        }

        private void Awake()
        {
            NetworkConfig?.InitializePrefabs();

            UnityEngine.SceneManagement.SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        // Ensures that the NetworkManager is cleaned up before OnDestroy is run on NetworkObjects and NetworkBehaviours when unloading a scene with a NetworkManager
        private void OnSceneUnloaded(Scene scene)
        {
            if (scene == gameObject.scene)
            {
                OnDestroy();
            }
        }

        // Ensures that the NetworkManager is cleaned up before OnDestroy is run on NetworkObjects and NetworkBehaviours when quitting the application.
        protected override void OnDispose()
        {
            OnDestroy();
        }

        // Note that this gets also called manually by OnSceneUnloaded and OnApplicationQuit
        private void OnDestroy()
        {
            ShutdownInternal();

            UnityEngine.SceneManagement.SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        private void DisconnectRemoteClient(ulong clientId)
        {
            var transportId = ClientIdToTransportId(clientId);
            MessagingSystem.ProcessSendQueues();
            NetworkConfig.NetworkTransport.DisconnectRemoteClient(transportId);
        }

        /// <summary>
        /// Globally shuts down the library.
        /// Disconnects clients if connected and stops server if running.
        /// </summary>
        /// <param name="discardMessageQueue">
        /// If false, any messages that are currently in the incoming queue will be handled,
        /// and any messages in the outgoing queue will be sent, before the shutdown is processed.
        /// If true, NetworkManager will shut down immediately, and any unprocessed or unsent messages
        /// will be discarded.
        /// </param>
        public void Shutdown(bool discardMessageQueue = false)
        {
            if (NetworkLog.CurrentLogLevel <= LogLevel.Developer)
                NetworkLog.LogInfo(nameof(Shutdown));

            // If we're not running, don't start shutting down, it would only cause an immediate
            // shutdown the next time the manager is started.
            m_ShuttingDown = true;
            m_StopProcessingMessages = discardMessageQueue;

            NetworkConfig.NetworkTransport.OnTransportEvent -= HandleRawTransportPoll;
        }

        internal void ShutdownInternal()
        {
            if (NetworkLog.CurrentLogLevel <= LogLevel.Developer)
                NetworkLog.LogInfo(nameof(ShutdownInternal));

            // make sure all messages are flushed before transport disconnect clients
            MessagingSystem?.ProcessSendQueues();

            var disconnectedIds = new HashSet<ulong>();

            //Don't know if I have to disconnect the clients. I'm assuming the NetworkTransport does all the cleaning on shutdown. But this way the clients get a disconnect message from server (so long it does't get lost)

            foreach (KeyValuePair<ulong, NetworkClient> pair in ConnectedClients)
            {
                if (disconnectedIds.Contains(pair.Key)) continue;
                disconnectedIds.Add(pair.Key);

                if (pair.Key == NetworkConfig.NetworkTransport.ServerClientId)
                    continue;

                DisconnectRemoteClient(pair.Key);
            }

            foreach (var pair in PendingClients)
            {
                if (disconnectedIds.Contains(pair.Key)) continue;
                    
                disconnectedIds.Add(pair.Key);
                if (pair.Key == NetworkConfig.NetworkTransport.ServerClientId)
                    continue;

                DisconnectRemoteClient(pair.Key);
            }

            // We need to clean up NetworkObjects before we reset the IsServer
            // and IsClient properties. This provides consistency of these two
            // property values for NetworkObjects that are still spawned when
            // the shutdown cycle begins.
            if (SpawnManager != null)
            {
                SpawnManager.DespawnAndDestroyNetworkObjects();
                SpawnManager.ServerResetShudownStateForSceneObjects();
                SpawnManager = null;
            }

            if (NetworkTickSystem != null)
            {
                NetworkTickSystem.Tick -= OnNetworkManagerTick;
                NetworkTickSystem = null;
            }

            if (MessagingSystem != null)
            {
                MessagingSystem.Dispose();
                MessagingSystem = null;
            }

            if (NetworkConfig?.NetworkTransport != null)
            {
                NetworkConfig.NetworkTransport.OnTransportEvent -= HandleRawTransportPoll;
            }

            DeferredMessageManager?.CleanupAllTriggers();

            if (SceneManager != null)
            {
                // Let the NetworkSceneManager clean up its two SceneEvenData instances
                SceneManager.Dispose();
                SceneManager = null;
            }

            if (CustomMessagingManager != null)
                CustomMessagingManager = null;

            if (BehaviourUpdater != null)
                BehaviourUpdater = null;

            // This is required for handling the potential scenario where multiple NetworkManager instances are created.
            // See MTT-860 for more information
            if (IsListening)
            {
                //The Transport is set during initialization, thus it is possible for the Transport to be null
                NetworkConfig?.NetworkTransport?.Shutdown();
            }

            m_ClientIdToTransportIdMap.Clear();
            m_TransportIdToClientIdMap.Clear();

            IsListening = false;
            m_ShuttingDown = false;
            m_StopProcessingMessages = false;

            ClearClients();

            OnServerStopped?.Invoke(true);

            // This cleans up the internal prefabs list
            NetworkConfig?.Prefabs.Shutdown();

            // Reset the configuration hash for next session in the event
            // that the prefab list changes
            NetworkConfig?.ClearConfigHash();
        }

        public void Tick()
        {
            RealTimeProvider.Tick(); // this will increase tick
            
            OnNetworkEarlyUpdate();
            OnNetworkPreUpdate();
            OnNetworkPostLateUpdate();
        }
        
        private void ProcessPendingApprovals()
        {
            List<ulong> senders = null;

            foreach (var responsePair in ClientsToApprove)
            {
                var response = responsePair.Value;
                var senderId = responsePair.Key;

                if (response.Pending) continue;
                
                try
                {
                    HandleConnectionApproval(senderId, response);

                    senders ??= new List<ulong>();
                    senders.Add(senderId);
                }
                catch (Exception e)
                {
                    this.LogException(e);
                }
            }

            if (senders == null) return;
            
            foreach (var sender in senders)
                ClientsToApprove.Remove(sender);
        }

        private void OnNetworkEarlyUpdate()
        {
            if (!IsListening)
                return;

            ProcessPendingApprovals();

            NetworkEvent networkEvent;
            do
            {
                networkEvent = NetworkConfig.NetworkTransport.PollEvent(out ulong clientId, out ArraySegment<byte> payload, out float receiveTime);
                HandleRawTransportPoll(networkEvent, clientId, payload, receiveTime);
                // Only do another iteration if: there are no more messages AND (there is no limit to max events or we have processed less than the maximum)
            } while (IsListening && networkEvent != NetworkEvent.Nothing);

            MessagingSystem.ProcessIncomingMessageQueue();
            MessagingSystem.CleanupDisconnectedClients();
        }

        // TODO Once we have a way to subscribe to NetworkUpdateLoop with order we can move this out of NetworkManager but for now this needs to be here because we need strict ordering.
        private void OnNetworkPreUpdate()
        {
            if (m_ShuttingDown && m_StopProcessingMessages)
            {
                return;
            }

            // Only update RTT here, server time is updated by time sync messages
            var reset = NetworkTimeSystem.Advance(RealTimeProvider.UnscaledDeltaTime);
            if (reset) NetworkTickSystem.Reset(NetworkTimeSystem.LocalTime, NetworkTimeSystem.ServerTime);
            NetworkTickSystem.UpdateTick(NetworkTimeSystem.LocalTime, NetworkTimeSystem.ServerTime);
        }

        private void OnNetworkPostLateUpdate()
        {
            if (!m_ShuttingDown || !m_StopProcessingMessages)
            {
                // This should be invoked just prior to the MessagingSystem
                // processes its outbound queue.
                SceneManager.CheckForAndSendNetworkObjectSceneChanged();

                MessagingSystem.ProcessSendQueues();
                NetworkMetrics.UpdateNetworkObjectsCount(SpawnManager.SpawnedObjects.Count);
                NetworkMetrics.UpdateConnectionsCount(ConnectedClients.Count);
                NetworkMetrics.DispatchFrame();

                NetworkObject.VerifyParentingStatus();
            }
            DeferredMessageManager.CleanupStaleTriggers();

            if (m_ShuttingDown)
            {
                ShutdownInternal();
            }
        }

        /// <summary>
        /// This function runs once whenever the local tick is incremented and is responsible for the following (in order):
        /// - collect commands/inputs and send them to the server (TBD)
        /// - call NetworkFixedUpdate on all NetworkBehaviours in prediction/client authority mode
        /// </summary>
        private void OnNetworkManagerTick()
        {
            // Do NetworkVariable updates
            BehaviourUpdater.NetworkBehaviourUpdate(this);

            // Handle NetworkObjects to show
            foreach (var client in ObjectsToShowToClient)
            {
                ulong clientId = client.Key;
                foreach (var networkObject in client.Value)
                {
                    SpawnManager.SendSpawnCallForObject(clientId, networkObject);
                }
            }
            ObjectsToShowToClient.Clear();

            var timeSyncFrequencyTicks = (int)(k_TimeSyncFrequency * NetworkConfig.TickRate);
            if (NetworkTickSystem.ServerTime.Tick % timeSyncFrequencyTicks == 0)
                SyncTime();
        }

        private void SendConnectionRequest()
        {
            var message = new ConnectionRequestMessage
            {
                // Since only a remote client will send a connection request,
                // we should always force the rebuilding of the NetworkConfig hash value
                ConfigHash = NetworkConfig.GetConfig(false),
                ShouldSendConnectionData = NetworkConfig.ConnectionApproval,
                ConnectionData = NetworkConfig.ConnectionData,
                MessageVersions = new MessageVersionData[MessagingSystem.MessageHandlers.Length]
            };
            for (var index = 0; index < MessagingSystem.MessageHandlers.Length; index++)
            {
                if (MessagingSystem.MessageTypes[index] == null) continue;
                var type = MessagingSystem.MessageTypes[index];
                message.MessageVersions[index] = new MessageVersionData
                {
                    Hash = type.FullName.Hash32(),
                    Version = MessagingSystem.GetLocalVersion(type)
                };
            }

            SendMessage(ref message, NetworkDelivery.ReliableSequenced, ServerClientId);
            message.MessageVersions = null;
        }

        private IEnumerator ApprovalTimeout(ulong clientId)
        {
            var timeStarted = LocalTime.TimeAsFloat;
            var timedOut = false;
            var connectionApproved = false;
            var connectionNotApproved = false;
            var timeoutMarker = timeStarted + NetworkConfig.ClientConnectionBufferTimeout;

            while (IsListening && !ShutdownInProgress && !timedOut && !connectionApproved)
            {
                yield return null;
                // Check if we timed out
                timedOut = timeoutMarker < LocalTime.TimeAsFloat;

                // When the client is no longer in the pending clients list and is in the connected clients list
                // it has been approved
                connectionApproved = !PendingClients.ContainsKey(clientId) && ConnectedClients.ContainsKey(clientId);

                // For the server side, if the client is in neither list then it was declined or the client disconnected
                connectionNotApproved = !PendingClients.ContainsKey(clientId) && !ConnectedClients.ContainsKey(clientId);
            }

            // Exit coroutine if we are no longer listening or a shutdown is in progress (client or server)
            if (!IsListening || ShutdownInProgress)
            {
                yield break;
            }

            // If the client timed out or was not approved
            if (timedOut || connectionNotApproved)
            {
                // Timeout
                if (NetworkLog.CurrentLogLevel <= LogLevel.Developer)
                {
                    if (timedOut)
                    {
                        // Log a warning that the transport detected a connection but then did not receive a follow up connection request message.
                        // (hacking or something happened to the server's network connection)
                        NetworkLog.LogWarning($"Server detected a transport connection from Client-{clientId}, but timed out waiting for the connection request message.");
                    }
                    else if (connectionNotApproved)
                    {
                        NetworkLog.LogInfo($"Client-{clientId} was either denied approval or disconnected while being approved.");
                    }
                }

                DisconnectClient(clientId);
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

                    PendingClients.Add(clientId, new PendingClient()
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

        /// <summary>
        /// Handles cleaning up the transport id/client id tables after
        /// receiving a disconnect event from transport
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ulong TransportIdCleanUp(ulong clientId, ulong transportId)
        {
            // This check is for clients that attempted to connect but failed.
            // When this happens, the client will not have an entry within the
            // m_TransportIdToClientIdMap or m_ClientIdToTransportIdMap lookup
            // tables so we exit early and just return 0 to be used for the
            // disconnect event.
            clientId = TransportIdToClientId(clientId);

            m_TransportIdToClientIdMap.Remove(transportId);
            m_ClientIdToTransportIdMap.Remove(clientId);

            return clientId;
        }

        internal unsafe int SendMessage<TMessageType, TClientIdListType>(ref TMessageType message, NetworkDelivery delivery, in TClientIdListType clientIds)
            where TMessageType : INetworkMessage
            where TClientIdListType : IReadOnlyList<ulong>
        {
            var nonServerIds = stackalloc ulong[clientIds.Count];
            var newIdx = 0;
            for (var idx = 0; idx < clientIds.Count; ++idx)
            {
                if (clientIds[idx] == ServerClientId)
                    continue;

                nonServerIds[newIdx++] = clientIds[idx];
            }

            return newIdx == 0 ? 0 : MessagingSystem.SendMessage(ref message, delivery, nonServerIds, newIdx);

        }

        internal unsafe int SendMessage<T>(ref T message, NetworkDelivery delivery,
            ulong* clientIds, int numClientIds)
            where T : INetworkMessage
        {
            var nonServerIds = stackalloc ulong[numClientIds];
            var newIdx = 0;
            for (var idx = 0; idx < numClientIds; ++idx)
            {
                if (clientIds[idx] == ServerClientId)
                    continue;

                nonServerIds[newIdx++] = clientIds[idx];
            }

            return newIdx == 0 ? 0 : MessagingSystem.SendMessage(ref message, delivery, nonServerIds, newIdx);
        }

        internal unsafe int SendMessage<T>(ref T message, NetworkDelivery delivery, ref Span<ulong> clientIds)
            where T : INetworkMessage
        {
            return SendMessage(ref message, delivery, (ulong*)clientIds.GetUnsafePtr(), clientIds.Length);
        }

        internal int SendMessage<T>(ref T message, NetworkDelivery delivery, ulong clientId)
            where T : INetworkMessage
        {
            // Prevent server sending to itself
            return clientId == ServerClientId ? 0 : MessagingSystem.SendMessage(ref message, delivery, clientId);
        }

        internal int SendPreSerializedMessage<T>(in FastBufferWriter writer, int maxSize, ref T message, NetworkDelivery delivery, ulong clientId)
            where T : INetworkMessage
        {
            return MessagingSystem.SendPreSerializedMessage(writer, maxSize, ref message, delivery, clientId);
        }

        private void HandleIncomingData(ulong clientId, ArraySegment<byte> payload, float receiveTime)
        {
            MessagingSystem.HandleIncomingData(clientId, payload, receiveTime);
        }

        /// <summary>
        /// Disconnects the remote client.
        /// </summary>
        /// <param name="clientId">The ClientId to disconnect</param>
        public void DisconnectClient(ulong clientId)
        {
            DisconnectClient(clientId, null);
        }

        /// <summary>
        /// Disconnects the remote client.
        /// </summary>
        /// <param name="clientId">The ClientId to disconnect</param>
        /// <param name="reason">Disconnection reason. If set, client will receive a DisconnectReasonMessage and have the
        /// reason available in the NetworkManager.DisconnectReason property</param>
        public void DisconnectClient(ulong clientId, string reason)
        {
            if (!string.IsNullOrEmpty(reason))
            {
                var disconnectReason = new DisconnectReasonMessage
                {
                    Reason = reason
                };
                SendMessage(ref disconnectReason, NetworkDelivery.Reliable, clientId);
            }
            MessagingSystem.ProcessSendQueues();

            OnClientDisconnectFromServer(clientId);
            DisconnectRemoteClient(clientId);
        }

        private void OnClientDisconnectFromServer(ulong clientId)
        {
            PendingClients.Remove(clientId);

            if (ConnectedClients.TryGetValue(clientId, out var networkClient))
            {
                var playerObject = networkClient.PlayerObject;
                if (playerObject != null)
                {
                    if (!playerObject.DontDestroyWithOwner)
                        if (PrefabHandler.ContainsHandler(ConnectedClients[clientId].PlayerObject.GlobalObjectIdHash))
                            PrefabHandler.HandleNetworkPrefabDestroy(ConnectedClients[clientId].PlayerObject);
                        else
                            // Call despawn to assure NetworkBehaviour.OnNetworkDespawn is invoked
                            // on the server-side (when the client side disconnected).
                            // This prevents the issue (when just destroying the GameObject) where
                            // any NetworkBehaviour component(s) destroyed before the NetworkObject
                            // would not have OnNetworkDespawn invoked.
                            SpawnManager.DespawnObject(playerObject, true);
                    else
                        playerObject.RemoveOwnership();
                }

                // Get the NetworkObjects owned by the disconnected client
                var clientOwnedObjects = SpawnManager.GetClientOwnedObjects(clientId);
                if (clientOwnedObjects == null)
                {
                    // This could happen if a client is never assigned a player object and is disconnected
                    // Only log this in verbose/developer mode
                    if (LogLevel == LogLevel.Developer)
                        NetworkLog.LogWarning($"ClientID {clientId} disconnected with (0) zero owned objects!  Was a player prefab not assigned?");
                }
                else
                {
                    // Handle changing ownership and prefab handlers
                    for (var i = clientOwnedObjects.Count - 1; i >= 0; i--)
                    {
                        var ownedObject = clientOwnedObjects[i];
                        if (ownedObject == null) continue;
                        if (!ownedObject.DontDestroyWithOwner)
                            if (PrefabHandler.ContainsHandler(clientOwnedObjects[i].GlobalObjectIdHash))
                                PrefabHandler.HandleNetworkPrefabDestroy(clientOwnedObjects[i]);
                            else
                                Destroy(ownedObject.gameObject);
                        else
                            ownedObject.RemoveOwnership();
                    }
                }

                foreach (var sobj in SpawnManager.SpawnedObjectsList)
                    sobj.Observers.Remove(clientId);

                for (var i = 0; i < ConnectedClientsList.Count; i++)
                {
                    if (ConnectedClientsList[i].ClientId != clientId) continue;
                    m_ConnectedClientsList.RemoveAt(i);
                    break;
                }

                for (var i = 0; i < ConnectedClientsIds.Count; i++)
                {
                    if (ConnectedClientsIds[i] != clientId) continue;
                    m_ConnectedClientIds.RemoveAt(i);
                    break;
                }

                m_ConnectedClients.Remove(clientId);
            }

            MessagingSystem.ClientDisconnected(clientId);
        }

        private void SyncTime()
        {
            if (NetworkLog.CurrentLogLevel <= LogLevel.Developer)
                NetworkLog.LogInfo("Syncing Time To Clients");

            var message = new TimeSyncMessage
            {
                Tick = NetworkTickSystem.ServerTime.Tick
            };
            SendMessage(ref message, NetworkDelivery.Unreliable, ConnectedClientsIds);
        }

        /// <summary>
        /// Server Side: Handles the approval of a client
        /// </summary>
        /// <param name="ownerClientId">The Network Id of the client being approved</param>
        /// <param name="response">The response to allow the player in or not, with its parameters</param>
        internal void HandleConnectionApproval(ulong ownerClientId, ConnectionApprovalResponse response)
        {
            if (response.Approved)
            {
                // Inform new client it got approved
                PendingClients.Remove(ownerClientId);

                var client = new NetworkClient { ClientId = ownerClientId, };
                m_ConnectedClients.Add(ownerClientId, client);
                m_ConnectedClientsList.Add(client);
                m_ConnectedClientIds.Add(client.ClientId);

                if (response.CreatePlayerObject)
                {
                    var playerPrefabHash = response.PlayerPrefabHash;

                    // Generate a SceneObject for the player object to spawn
                    // Note: This is only to create the local NetworkObject,
                    // many of the serialized properties of the player prefab
                    // will be set when instantiated.
                    var sceneObject = new NetworkObject.SceneObject
                    {
                        OwnerClientId = ownerClientId,
                        IsPlayerObject = true,
                        IsSceneObject = false,
                        HasTransform = true,
                        Hash = playerPrefabHash,
                        TargetClientId = ownerClientId,
                        Transform = new NetworkObject.SceneObject.TransformData
                        {
                            Position = response.Position.GetValueOrDefault(),
                            Rotation = response.Rotation.GetValueOrDefault()
                        }
                    };

                    // Create the player NetworkObject locally
                    var networkObject = SpawnManager.CreateLocalNetworkObject(sceneObject);

                    // Spawn the player NetworkObject locally
                    SpawnManager.SpawnNetworkObjectLocally(
                        networkObject,
                        SpawnManager.GetNetworkObjectId(),
                        sceneObject: false,
                        playerObject: true,
                        ownerClientId,
                        destroyWithScene: false);

                    ConnectedClients[ownerClientId].PlayerObject = networkObject;
                }

                // Server doesn't send itself the connection approved message
                if (ownerClientId != ServerClientId)
                {
                    var message = new ConnectionApprovedMessage
                    {
                        OwnerClientId = ownerClientId,
                        NetworkTick = LocalTime.Tick
                    };
                    if (!NetworkConfig.EnableSceneManagement)
                        if (SpawnManager.SpawnedObjectsList.Count != 0)
                            message.SpawnedObjectsList = SpawnManager.SpawnedObjectsList;

                    message.MessageVersions = new MessageVersionData[MessagingSystem.MessageHandlers.Length];
                    for (int index = 0; index < MessagingSystem.MessageHandlers.Length; index++)
                    {
                        if (MessagingSystem.MessageTypes[index] != null)
                        {
                            var type = MessagingSystem.MessageTypes[index];
                            message.MessageVersions[index] = new MessageVersionData
                            {
                                Hash = XXHash.Hash32(type.FullName),
                                Version = MessagingSystem.GetLocalVersion(type)
                            };
                        }
                    }

                    SendMessage(ref message, NetworkDelivery.ReliableFragmentedSequenced, ownerClientId);
                    message.MessageVersions = null;

                    // If scene management is enabled, then let NetworkSceneManager handle the initial scene and NetworkObject synchronization
                    if (!NetworkConfig.EnableSceneManagement)
                    {
                        InvokeOnClientConnectedCallback(ownerClientId);
                    }
                    else
                    {
                        SceneManager.SynchronizeNetworkObjects(ownerClientId);
                    }
                }
                else // Server just adds itself as an observer to all spawned NetworkObjects
                {
                    LocalClient = client;
                    SpawnManager.UpdateObservedNetworkObjects(ownerClientId);
                }

                if (!response.CreatePlayerObject)
                    return;

                // Separating this into a contained function call for potential further future separation of when this notification is sent.
                ApprovedPlayerSpawn(ownerClientId, response.PlayerPrefabHash);
            }
            else
            {
                if (!string.IsNullOrEmpty(response.Reason))
                {
                    var disconnectReason = new DisconnectReasonMessage
                    {
                        Reason = response.Reason
                    };
                    SendMessage(ref disconnectReason, NetworkDelivery.Reliable, ownerClientId);

                    MessagingSystem.ProcessSendQueues();
                }

                PendingClients.Remove(ownerClientId);
                DisconnectRemoteClient(ownerClientId);
            }
        }

        /// <summary>
        /// Spawns the newly approved player
        /// </summary>
        /// <param name="clientId">new player client identifier</param>
        /// <param name="playerPrefabHash">the prefab GlobalObjectIdHash value for this player</param>
        internal void ApprovedPlayerSpawn(ulong clientId, uint playerPrefabHash)
        {
            foreach (var clientPair in ConnectedClients)
            {
                if (clientPair.Key == clientId ||
                    clientPair.Key == ServerClientId || // Server already spawned it
                    ConnectedClients[clientId].PlayerObject == null ||
                    !ConnectedClients[clientId].PlayerObject.Observers.Contains(clientPair.Key))
                {
                    continue; //The new client.
                }

                var message = new CreateObjectMessage
                {
                    ObjectInfo = ConnectedClients[clientId].PlayerObject.GetMessageSceneObject(clientPair.Key)
                };
                message.ObjectInfo.Hash = playerPrefabHash;
                message.ObjectInfo.IsSceneObject = false;
                message.ObjectInfo.HasParent = false;
                message.ObjectInfo.IsPlayerObject = true;
                message.ObjectInfo.OwnerClientId = clientId;
                var size = SendMessage(ref message, NetworkDelivery.ReliableFragmentedSequenced, clientPair.Key);
                NetworkMetrics.TrackObjectSpawnSent(clientPair.Key, ConnectedClients[clientId].PlayerObject, size);
            }
        }

        internal void MarkObjectForShowingTo(NetworkObject networkObject, ulong clientId)
        {
            if (!ObjectsToShowToClient.ContainsKey(clientId))
            {
                ObjectsToShowToClient.Add(clientId, new List<NetworkObject>());
            }
            ObjectsToShowToClient[clientId].Add(networkObject);
        }

        // returns whether any matching objects would have become visible and were returned to hidden state
        internal bool RemoveObjectFromShowingTo(NetworkObject networkObject, ulong clientId)
        {
            var ret = false;
            if (!ObjectsToShowToClient.ContainsKey(clientId))
            {
                return false;
            }

            // probably overkill, but deals with multiple entries
            while (ObjectsToShowToClient[clientId].Contains(networkObject))
            {
                NetworkLog.LogWarning(
                    "Object was shown and hidden from the same client in the same Network frame. As a result, the client will _not_ receive a NetworkSpawn");
                ObjectsToShowToClient[clientId].Remove(networkObject);
                ret = true;
            }

            networkObject.Observers.Remove(clientId);

            return ret;
        }
    }
}
