#if !UNITY
using System.Runtime.InteropServices;
using Netcode.io.IO;

namespace Netcode.io.Tokens
{
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    internal struct PrivateToken
    {
        public ulong ProtocolId;
        public ulong ClientId;
        public ulong Created;
        public ulong Expire;
        public EndPoint EndPoint;

        public bool Read(ref ReaderWriter rw)
        {
            rw.Read(out var version, Constants.Version.Length);
            if (version != Constants.Version) return false;
            
            rw.Read(out ProtocolId);
            rw.Read(out ClientId);
            rw.Read(out Created);
            rw.Read(out Expire);
            EndPoint.Read(ref rw);

            return version == Constants.Version;
        }
        
        public bool Write(ref ReaderWriter rw)
        {
            rw.Write(Constants.Version);
            rw.Write(ProtocolId);
            rw.Write(ClientId);
            rw.Write(Created);
            rw.Write(Expire);
            EndPoint.Write(ref rw);

            return true;
        }
    }
}
#endif