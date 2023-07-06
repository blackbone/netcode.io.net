using Netcode.io.Hashing;
using Netcode.io.Messaging;
using Netcode.io.Messaging.Messages;
using Netcode.io.OLD.Transports;
using Netcode.io.OLD.Unity.Collections;
using Netcode.io.OLD.UnityEngine;
using Netcode.io.Serialization;

namespace Netcode.io.OLD.Configuration
{
    /// <summary>
    /// The configuration object used to start server, client and hosts
    /// </summary>
    [Serializable]
    public class NetworkConfig
    {
        /// <summary>
        /// The protocol version. Different versions doesn't talk to each other.
        /// </summary>
        public ushort ProtocolVersion = 0;

        /// <summary>
        /// The transport hosts the sever uses
        /// </summary>
        public NetworkTransport NetworkTransport = null;

        /// <summary>
        /// The default player prefab
        /// </summary>
        public GameObject PlayerPrefab2;
        public NetworkPrefabs Prefabs = new();

        /// <summary>
        /// The tickrate of network ticks. This value controls how often netcode runs user code and sends out data.
        /// </summary>
        public uint TickRate = 30;

        /// <summary>
        /// The amount of seconds for the server to wait for the connection approval handshake to complete before the client is disconnected.
        ///
        /// If the timeout is reached before approval is completed the client will be disconnected.
        /// </summary>
        /// <remarks>
        /// The period begins after the <see cref="NetworkEvent.Connect"/> is received on the server.
        /// The period ends once the server finishes processing a <see cref="ConnectionRequestMessage"/> from the client.
        ///
        /// This setting is independent of any Transport-level timeouts that may be in effect. It covers the time between
        /// the connection being established on the Transport layer, the client sending a
        /// <see cref="ConnectionRequestMessage"/>, and the server processing that message through <see cref="ConnectionApproval"/>.
        ///
        /// This setting is server-side only.
        /// </remarks>
        public int ClientConnectionBufferTimeout = 10;

        /// <summary>
        /// Whether or not to use connection approval
        /// </summary>
        public bool ConnectionApproval = false;

        /// <summary>
        /// The data to send during connection which can be used to decide on if a client should get accepted
        /// </summary>
        public byte[] ConnectionData = new byte[0];

        /// <summary>
        /// If your logic uses the NetworkTime, this should probably be turned off. If however it's needed to maximize accuracy, this is recommended to be turned on
        /// </summary>
        public bool EnableTimeResync = false;

        /// <summary>
        /// If time re-sync is turned on, this specifies the interval between syncs in seconds.
        /// </summary>
        public int TimeResyncInterval = 30;

        /// <summary>
        /// Whether or not to ensure that NetworkVariables can be read even if a client accidentally writes where its not allowed to. This costs some CPU and bandwidth.
        /// </summary>
        public bool EnsureNetworkVariableLengthSafety = false;

        /// <summary>
        /// Enables scene management. This will allow network scene switches and automatic scene difference corrections upon connect.
        /// SoftSynced scene objects wont work with this disabled. That means that disabling SceneManagement also enables PrefabSync.
        /// </summary>
        public bool EnableSceneManagement = true;

        /// <summary>
        /// Whether or not the netcode should check for differences in the prefabs at connection.
        /// If you dynamically add prefabs at runtime, turn this OFF
        /// </summary>
        public bool ForceSamePrefabs = true;

        /// <summary>
        /// If true, NetworkIds will be reused after the NetworkIdRecycleDelay.
        /// </summary>
        public bool RecycleNetworkIds = true;

        /// <summary>
        /// The amount of seconds a NetworkId has to be unused in order for it to be reused.
        /// </summary>
        public float NetworkIdRecycleDelay = 120f;

        /// <summary>
        /// Decides how many bytes to use for Rpc messaging. Leave this to 2 bytes unless you are facing hash collisions
        /// </summary>
        public HashSize RpcHashSize = HashSize.VarIntFourBytes;

        /// <summary>
        /// The amount of seconds to wait for all clients to load or unload a requested scene
        /// </summary>
        public int LoadSceneTimeOut = 120;

        /// <summary>
        /// The amount of time a message should be buffered if the asset or object needed to process it doesn't exist yet. If the asset is not added/object is not spawned within this time, it will be dropped.
        /// </summary>
        public float SpawnTimeout = 1f;

        /// <summary>
        /// Whether or not to enable network logs.
        /// </summary>
        public bool EnableNetworkLogs = true;

        /// <summary>
        /// The number of RTT samples that is kept as an average for calculations
        /// </summary>
        public const int RttAverageSamples = 5; // number of RTT to keep an average of (plus one)

        /// <summary>
        /// The number of slots used for RTT calculations. This is the maximum amount of in-flight messages
        /// </summary>
        public const int RttWindowSize = 64; // number of slots to use for RTT computations (max number of in-flight packets)

        /// <summary>
        /// Returns a base64 encoded version of the configuration
        /// </summary>
        /// <returns></returns>
        public string ToBase64()
        {
            NetworkConfig config = this;
            var writer = new FastBufferWriter(MessagingSystem.NON_FRAGMENTED_MESSAGE_MAX_SIZE, Allocator.Temp);
            using (writer)
            {
                writer.WriteValueSafe(config.ProtocolVersion);
                writer.WriteValueSafe(config.TickRate);
                writer.WriteValueSafe(config.ClientConnectionBufferTimeout);
                writer.WriteValueSafe(config.ConnectionApproval);
                writer.WriteValueSafe(config.LoadSceneTimeOut);
                writer.WriteValueSafe(config.EnableTimeResync);
                writer.WriteValueSafe(config.EnsureNetworkVariableLengthSafety);
                writer.WriteValueSafe(config.RpcHashSize);
                writer.WriteValueSafe(ForceSamePrefabs);
                writer.WriteValueSafe(EnableSceneManagement);
                writer.WriteValueSafe(RecycleNetworkIds);
                writer.WriteValueSafe(NetworkIdRecycleDelay);
                writer.WriteValueSafe(EnableNetworkLogs);

                // Allocates
                return Convert.ToBase64String(writer.ToArray());
            }
        }

        /// <summary>
        /// Sets the NetworkConfig data with that from a base64 encoded version
        /// </summary>
        /// <param name="base64">The base64 encoded version</param>
        public void FromBase64(string base64)
        {
            NetworkConfig config = this;
            byte[] binary = Convert.FromBase64String(base64);
            using var reader = new FastBufferReader(binary, Allocator.Temp);
            using (reader)
            {
                reader.ReadValueSafe(out config.ProtocolVersion);
                reader.ReadValueSafe(out config.TickRate);
                reader.ReadValueSafe(out config.ClientConnectionBufferTimeout);
                reader.ReadValueSafe(out config.ConnectionApproval);
                reader.ReadValueSafe(out config.LoadSceneTimeOut);
                reader.ReadValueSafe(out config.EnableTimeResync);
                reader.ReadValueSafe(out config.EnsureNetworkVariableLengthSafety);
                reader.ReadValueSafe(out config.RpcHashSize);
                reader.ReadValueSafe(out config.ForceSamePrefabs);
                reader.ReadValueSafe(out config.EnableSceneManagement);
                reader.ReadValueSafe(out config.RecycleNetworkIds);
                reader.ReadValueSafe(out config.NetworkIdRecycleDelay);
                reader.ReadValueSafe(out config.EnableNetworkLogs);
            }
        }


        private ulong? m_ConfigHash = null;

        /// <summary>
        /// Clears out the configuration hash value generated for a specific network session
        /// </summary>
        internal void ClearConfigHash()
        {
            m_ConfigHash = null;
        }

        /// <summary>
        /// Gets a SHA256 hash of parts of the NetworkConfig instance
        /// </summary>
        /// <param name="cache"></param>
        /// <returns></returns>
        public ulong GetConfig(bool cache = true)
        {
            if (m_ConfigHash != null && cache)
            {
                return m_ConfigHash.Value;
            }

            var writer = new FastBufferWriter(MessagingSystem.NON_FRAGMENTED_MESSAGE_MAX_SIZE, Allocator.Temp, int.MaxValue);
            using (writer)
            {
                writer.WriteValueSafe(ProtocolVersion);
                writer.WriteValueSafe(NetworkConstants.PROTOCOL_VERSION);

                if (ForceSamePrefabs)
                {
                    var sortedDictionary = Prefabs.NetworkPrefabOverrideLinks.OrderBy(x => x.Key);
                    foreach (var sortedEntry in sortedDictionary)

                    {
                        writer.WriteValueSafe(sortedEntry.Key);
                    }
                }

                writer.WriteValueSafe(TickRate);
                writer.WriteValueSafe(ConnectionApproval);
                writer.WriteValueSafe(ForceSamePrefabs);
                writer.WriteValueSafe(EnableSceneManagement);
                writer.WriteValueSafe(EnsureNetworkVariableLengthSafety);
                writer.WriteValueSafe(RpcHashSize);

                if (cache)
                {
                    m_ConfigHash = XXHash.Hash64(writer.ToArray());
                    return m_ConfigHash.Value;
                }

                return XXHash.Hash64(writer.ToArray());
            }
        }

        /// <summary>
        /// Compares a SHA256 hash with the current NetworkConfig instances hash
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public bool CompareConfig(ulong hash)
        {
            return hash == GetConfig();
        }

        internal void InitializePrefabs()
        {
            if (HasOldPrefabList())
                MigrateOldNetworkPrefabsToNetworkPrefabsList();

            Prefabs.Initialize();
        }

        [NonSerialized]
        private bool m_DidWarnOldPrefabList = false;

        private void WarnOldPrefabList()
        {
            if (m_DidWarnOldPrefabList) return;
            
            Debug.LogWarning("Using Legacy Network Prefab List. Consider Migrating.");
            m_DidWarnOldPrefabList = true;
        }

        /// <summary>
        /// Returns true if the old List&lt;NetworkPrefab&gt; serialized data is present.
        /// </summary>
        /// <remarks>
        /// Internal use only to help migrate projects. <seealso cref="MigrateOldNetworkPrefabsToNetworkPrefabsList"/></remarks>
        internal bool HasOldPrefabList() => OldPrefabList?.Count > 0;

        /// <summary>
        /// Migrate the old format List&lt;NetworkPrefab&gt; prefab registration to the new NetworkPrefabsList ScriptableObject.
        /// </summary>
        /// <remarks>
        /// OnAfterDeserialize cannot instantiate new objects (e.g. NetworkPrefabsList SO) since it executes in a thread, so we have to do it later.
        /// Since NetworkConfig isn't a Unity.Object it doesn't get an Awake callback, so we have to do this in NetworkManager and expose this API.
        /// </remarks>
        internal NetworkPrefabsList MigrateOldNetworkPrefabsToNetworkPrefabsList()
        {
            if (OldPrefabList == null || OldPrefabList.Count == 0)
                return null;

            if (Prefabs == null)
                throw new Exception("Prefabs field is null.");

            Prefabs.NetworkPrefabsLists.Add(ScriptableObject.CreateInstance<NetworkPrefabsList>());

            if (OldPrefabList?.Count > 0)
            {
                // Migrate legacy types/fields
                foreach (var networkPrefab in OldPrefabList)
                    Prefabs.NetworkPrefabsLists[^1].Add(networkPrefab);
            }

            OldPrefabList = null;
            return Prefabs.NetworkPrefabsLists[^1];
        }

        internal List<NetworkPrefab> OldPrefabList;
    }
}
