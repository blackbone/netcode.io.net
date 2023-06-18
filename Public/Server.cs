#if COMPACT
#define SHARED_BUFFER
#endif

using System.Net;
using System.Text;
using NetcodeIO.NET.Core;
using NetcodeIO.NET.Core.Buffers;
using NetcodeIO.NET.Core.Requests;
using NetcodeIO.NET.Core.Token;
using NetcodeIO.NET.Utils;
using NetcodeIO.NET.Utils.IO;

namespace NetcodeIO.NET
{
    public class Server
    {
        public class Options
        {
            public int MaxClients;
            public IPEndPoint EndPoint;
            public readonly byte[] ServerPrivateKey;

            public Options(int maxClients, IPEndPoint endPoint)
            {
                MaxClients = maxClients;
                EndPoint = endPoint;
                
                ServerPrivateKey = new byte[Defines.KEY_SIZE];
                KeyUtils.GenerateKey(new Span<byte>(ServerPrivateKey));
            }
        }
        
        private readonly Options options;
        
#if SHARED_BUFFER
        [ThreadStatic] private static SocketBuffers buffers;
#else
        private SocketBuffers buffers;
#endif
        private UdpSocketContext socket;
        private RemoteClient[] clients;
        private int connectedClients;
        
        public bool IsStarted { get; private set; }

        public int ConnectedClients => connectedClients;
        public int MaxClients => options.MaxClients;
        public string Key => Convert.ToBase64String(options.ServerPrivateKey);
        
        public Server(Options options)
        {
            this.options = options;
            
            clients = new RemoteClient[options.MaxClients];
        }

        public void Start()
        {
            IsStarted = true;
            connectedClients = 0;
            UdpSocketContext.GetServer(ref socket, options.EndPoint);
        }
        
        public int Update()
        {
            EndPoint receiveEp = null;
            socket.ReadFrom(buffers.ReadBuffer, out var frame, ref receiveEp);
            PacketHeader header = default;
            var reader = new ReaderWriter(frame);
            header.Read(ref reader);

            switch (header.PacketType)
            {
                case PacketType.ConnectionRequest:
                    ProcessConnectionRequest(ref reader, receiveEp);
                    return 0;
            }

            int? firstFreeSlotIndex = null;
            for (var i = 0; i < options.MaxClients; i++)
            {
                if (!clients[i].Allocated)
                {
                    firstFreeSlotIndex = i;
                    continue;
                }
                clients[i].Update();
            }

            return 0;
        }

        public void Stop()
        {
            for (var i = 0; i < options.MaxClients; i++)
            {
                if (!clients[i].Allocated) continue;
                clients[i].Disconnect();
            }
            UdpSocketContext.Release(ref socket);
            IsStarted = false;
        }
        
        private void ProcessConnectionRequest(ref ReaderWriter reader, EndPoint receiveEp)
        {
            ConnectionPacket packet = default;
            packet.Read(ref reader);
            var freeIndex = -1;
            for (var i = 0; i < options.MaxClients; i++)
                if (!clients[i].Allocated)
                {
                    freeIndex = i;
                    break;
                }

            if (freeIndex == -1)
            {
                DenyConnectionRequest();
                return;
            }

            ConnectionPacket connectionPacket = default;
            connectionPacket.Read(ref reader);

            PrivateToken privateToken = default;
            PacketIO.DecryptPrivateConnectToken(connectionPacket.PrivateKeyData, )
            
            clients[freeIndex].Allocated = true;
            clients[freeIndex].State = ServerState.SendingChallengeRequest;

            void DenyConnectionRequest()
            {
                var deniedPacket = new DeniedPacket();
                var writer = new ReaderWriter(buffers.WriteBuffer);
                deniedPacket.Header.Write(ref writer);
                deniedPacket.Write(ref writer);
                buffers.WriteSize = writer.Position;
                var num = Defines.NUM_DISCONNECT_PACKETS;
                while (num-- >= 0)
                    socket.WriteTo(buffers.WriteBuffer, ref buffers.WriteSize, receiveEp);
            }
        }

        // protected virtual void OnClientConnected(ref RemoteClient client)
        // {
        //     
        // }
        //
        // protected virtual void OnClientDataReceived(ref RemoteClient client)
        // {
        //     
        // }
        //
        // protected virtual void OnClientDataReceived(ref RemoteClient client)
        // {
        //     
        // }
        //
        // protected virtual void OnClientDisconnected(ref RemoteClient client)
        // {
        //     
        // }
    }
}