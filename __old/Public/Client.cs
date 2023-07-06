#if COMPACT
#define SHARED_BUFFER
#endif

using System.Net;
using System.Runtime.InteropServices;
using NetcodeIO.NET.Core;
using NetcodeIO.NET.Core.Buffers;
using NetcodeIO.NET.Core.Token;
using NetcodeIO.NET.Utils.IO;

namespace NetcodeIO.NET
{
    public partial class Client : IDisposable
    {
        private const ulong ms = 10000;
        private const ulong s = ms * 1000;

#if SHARED_BUFFER
        [ThreadStatic] private static SocketBuffers buffers;
#else
        private CilentBuffers buffers;
#endif
        
        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public class Options
        {
            public ulong ConnectionRequestSendRate = 10; // number per second
        }
        
        private Options options;
        
        private ClientState state;
        private ClientState pendingDisconnectState;
        
        private TimeState time;
        private PublicToken token;
        
        
        private UdpSocketContext socket;
        private byte currentServerIndex;
        private IPEndPoint currentServer;
        
        private bool isRunning;

        private ReplayProtection replayProtection;

        private ClientState State
        {
            set
            {
                if (state == value) return;

                state = value;
                OnStateChanged(state);
            }
        }

        public Client(Options options)
        {
            this.options = options;
            
            state = ClientState.Disconnected;
            currentServerIndex = 0;
        }

        public void Dispose()
        {
        }

        protected virtual void OnStateChanged(ClientState state)
        {
            switch (state)
            {
                case ClientState.ConnectTokenExpired: break;
                case ClientState.InvalidConnectionToken: break;
                case ClientState.ConnectionTimedOut: break;
                case ClientState.ChallengeResponseTimedOut: break;
                case ClientState.ConnectionRequestTimedOut: break;
                case ClientState.ConnectionDenied: break;
                case ClientState.Disconnected: break;
                case ClientState.SendingConnectionRequest: PrepareSendingConnectionRequest(); break;
                case ClientState.SendingChallengeResponse: 
                    break;
                case ClientState.Connected:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
        
        public void Connect(byte[] connectionToken)
        {
            if (state != ClientState.Disconnected) throw new InvalidOperationException();
            if (connectionToken.Length != PublicToken.SIZE)
            {
                State = ClientState.InvalidConnectionToken;
                return;
            }

            var tokenReader = new ReaderWriter(connectionToken);
            if (!token.Read(ref tokenReader))
            {
                State = ClientState.InvalidConnectionToken;
                return;
            }
            
            if (token.CreateTimestamp >= token.ExpireTimestamp)
            {
                State = ClientState.InvalidConnectionToken;
                return;
            }

            // reset state
            time.Reset();
            replayProtection.Reset();

            // set socket running
            isRunning = true;
            if (!CreateSocket())
            {
                State = ClientState.InvalidConnectionToken;
                return;
            }
            
            State = ClientState.SendingConnectionRequest;
        }

        public void Disconnect() => Disconnect(ClientState.Disconnected);

        public int Update(ulong time)
        {
            this.time.Delta = time - this.time.Time;
            this.time.Time = time;

            switch (state)
            {
                case ClientState.ConnectTokenExpired: break;
                case ClientState.InvalidConnectionToken: break;
                case ClientState.ConnectionTimedOut: break;
                case ClientState.ChallengeResponseTimedOut: break;
                case ClientState.ConnectionRequestTimedOut: break;
                case ClientState.ConnectionDenied: break;
                case ClientState.Disconnected: break;
                case ClientState.SendingConnectionRequest:
                    UpdateReceivingChallengeRequest();
                    UpdateSendingConnectionRequest();
                    break;
                case ClientState.SendingChallengeResponse:
                    UpdateReceivingConnectionConfirmation();
                    UpdateSendingChallengeResponse();
                    break;
                case ClientState.Connected:
                    UpdateReceivingConnected();
                    UpdateSendingConnected();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return 0;
        }

        private bool CreateSocket()
        {
            currentServer = token.GetServer(currentServerIndex).AsIPEndPoint();
            if (currentServer == null)
                return false;
            
            UdpSocketContext.GetClient(ref socket, currentServer);
            return true;
        }

        private void ReleaseSocket()
        {
            if (!socket.IsAllocated) return;

            socket.Close();
            UdpSocketContext.Release(ref socket);
        }

        private void Disconnect(ClientState disconnectState)
        {
            if (state == ClientState.Connected)
                SendDisconnect();

            isRunning = false;
            pendingDisconnectState = ClientState.Disconnected;
            State = disconnectState;
            ReleaseSocket();
        }

        private void Reset()
        {
            currentServerIndex = 0;
        }
    }
}