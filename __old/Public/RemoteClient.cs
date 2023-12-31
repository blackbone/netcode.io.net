using System.Runtime.InteropServices;

namespace NetcodeIO.NET
{
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public struct RemoteClient
    {
        public bool Allocated;
        public ServerState State;

        public void Allocate()
        {
            Allocated = true;
            State = ServerState.SendingChallengeRequest;
        }

        public void Update()
        {
            switch (State)
            {
                case ServerState.Disconnected:
                    break;
                case ServerState.SendingChallengeRequest:
                    break;
                case ServerState.Connected:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Disconnect()
        {
            
        }
    }
}