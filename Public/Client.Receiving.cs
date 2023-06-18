using System.Net;
using NetcodeIO.NET.Core.Requests;
using NetcodeIO.NET.Utils.IO;

namespace NetcodeIO.NET
{
    public partial class Client
    {
        private PacketHeader readPacketHeader;
        
        private void UpdateReceivingChallengeRequest()
        {
            EndPoint remote = currentServer;
            while (socket.ReadFrom(buffers.ReadBuffer, out var data, ref remote))
            {
                if (!Equals(remote, currentServer))
                    continue;

                var reader = new ReaderWriter(data);
                if (!readPacketHeader.Read(ref reader))
                    continue;
                    
                switch (readPacketHeader.PacketType)
                {
                    case PacketType.Challenge:
                        // because the layout is same - use same packet type
                        readPacketHeader.PacketType = PacketType.ChallengeResponse;
                        var packet = new ChallengePacket(ref readPacketHeader);
                        if (!packet.Read(ref reader))
                            continue;

                        PrepareSendingChallengeResponse(packet);
                        state = ClientState.SendingChallengeResponse;
                        break;

                    case PacketType.ConnectionRequest:
                    case PacketType.ChallengeResponse:
                    case PacketType.KeepAlive:
                    case PacketType.Payload:
                    case PacketType.Disconnect:
                    case PacketType.InvalidPacket:
                    case PacketType.Denied:
                        state = ClientState.Disconnected;
                        MoveToNextServer();
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
                    
                reader.Reset();
            }
        }

        private void UpdateReceivingConnectionConfirmation()
        {
            EndPoint remote = currentServer;
            while (socket.IsAllocated && socket.ReadFrom(buffers.ReadBuffer, out var data, ref remote))
            {
                if (!Equals(remote, currentServer))
                    continue;

                var reader = new ReaderWriter(data);
                if (!readPacketHeader.Read(ref reader))
                    continue;
                    
                switch (readPacketHeader.PacketType)
                {
                    case PacketType.KeepAlive:
                        // because the layout is same - use same packet type
                        readPacketHeader.PacketType = PacketType.ChallengeResponse;
                        var packet = new KeepAlivePacket(ref readPacketHeader);
                        if (!packet.Read(ref reader))
                            continue;

                        state = ClientState.Connected;
                        break;

                    case PacketType.ConnectionRequest:
                    case PacketType.Challenge:
                    case PacketType.ChallengeResponse:
                    case PacketType.Payload:
                    case PacketType.Disconnect:
                    case PacketType.InvalidPacket:
                    case PacketType.Denied:
                        state = ClientState.Disconnected;
                        MoveToNextServer();
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
                    
                reader.Reset();
            }
        }

        private void UpdateReceivingConnected()
        {
            EndPoint remote = currentServer;
            while (socket.IsAllocated && socket.ReadFrom(buffers.ReadBuffer, out var data, ref remote))
            {
                if (!Equals(remote, currentServer))
                    continue;

                var reader = new ReaderWriter(data);
                if (!readPacketHeader.Read(ref reader))
                    continue;
                    
                switch (readPacketHeader.PacketType)
                {
                    case PacketType.KeepAlive:
                        // because the layout is same - use same packet type
                        readPacketHeader.PacketType = PacketType.ChallengeResponse;
                        var packet = new PayloadPacket(ref readPacketHeader);
                        if (!packet.Read(ref reader))
                            continue;

                        // TODO [Dmitrii Osipov] packet processors
                        break;

                    case PacketType.ConnectionRequest:
                    case PacketType.Challenge:
                    case PacketType.ChallengeResponse:
                    case PacketType.Payload:
                    case PacketType.Disconnect:
                    case PacketType.InvalidPacket:
                    case PacketType.Denied:
                        state = ClientState.Disconnected;
                        MoveToNextServer();
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
                    
                reader.Reset();
            }
        }
    }
}