using NetcodeIO.NET.Core;
using NetcodeIO.NET.Core.Requests;
using NetcodeIO.NET.Utils.IO;

namespace NetcodeIO.NET
{
    public partial class Client
    {
        private ulong sendTimer = 0;
        private ulong timeout = 0;

        private void PrepareSendingConnectionRequest()
        {
            var connectionPacket = new ConnectionPacket(ref token);
            var writer = new ReaderWriter(buffers.WriteBuffer);
            connectionPacket.Header.Write(ref writer);
            connectionPacket.Write(ref writer);
            buffers.WriteSize = writer.Position;
        }
        
        private void UpdateSendingConnectionRequest()
        {
            // checking token expiration
            if (time.Time >= token.ExpireTimestamp)
            {
                Disconnect(ClientState.ConnectTokenExpired);
                return;
            }

            // sending request process
            sendTimer += time.Delta; // send request timer
            if (sendTimer > s / options.ConnectionRequestSendRate)
            {
                socket.WriteTo(buffers.WriteBuffer, ref buffers.WriteSize, currentServer);
                sendTimer = 0;
            }

            // check timeout
            // if we don't get a response within timeout, move on.
            timeout += time.Delta;
            if (timeout > token.TimeoutSeconds * s)
            {
                pendingDisconnectState = ClientState.ConnectionRequestTimedOut;
                sendTimer = 0;
                timeout = 0;
                MoveToNextServer();
            }
        }
        
        private void PrepareSendingChallengeResponse(ChallengePacket challengeResponsePacket)
        {
            var writer = new ReaderWriter(buffers.WriteBuffer);
            challengeResponsePacket.Header.Write(ref writer);
            challengeResponsePacket.Write(ref writer);
            buffers.WriteSize = writer.Position;
        }

        private void UpdateSendingChallengeResponse()
        {
            // checking token expiration
            if (time.Time >= token.ExpireTimestamp)
            {
                Disconnect(ClientState.ConnectTokenExpired);
                return;
            }

            // sending request process
            sendTimer += time.Delta; // send request timer
            if (sendTimer > s / options.ConnectionRequestSendRate)
            {
                socket.WriteTo(buffers.WriteBuffer, ref buffers.WriteSize, currentServer);
                sendTimer = 0;
            }

            // check timeout
            // if we don't get a response within timeout, move on.
            timeout += time.Delta;
            if (timeout > token.TimeoutSeconds * s)
            {
                pendingDisconnectState = ClientState.ConnectionRequestTimedOut;
                sendTimer = 0;
                timeout = 0;
                MoveToNextServer();
            }
        }
        
        // immediate change methods
        private void MoveToNextServer()
        {
            ReleaseSocket();
            currentServerIndex++;
                
            if (!CreateSocket())
            {
                Disconnect(pendingDisconnectState);
                return;
            }
                
            State = ClientState.SendingConnectionRequest;
        }

        private void SendDisconnect()
        {
            var writer = new ReaderWriter(buffers.WriteBuffer);
            var disconnectPacket = new DisconnectPacket();
            disconnectPacket.Write(ref writer);
            buffers.WriteSize = writer.Position;

            var num = Defines.NUM_DISCONNECT_PACKETS;
            while (num-- >= 0)
                socket.WriteTo(buffers.WriteBuffer, ref buffers.WriteSize, currentServer);
        }

        private void UpdateSendingConnected()
        {
            // TODO [Dmitrii Osipov] 
        }
    }
}