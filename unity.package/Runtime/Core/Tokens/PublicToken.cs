using System.Runtime.InteropServices;
using Netcode.io.IO;

namespace Netcode.io.Tokens
{
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public struct PublicToken
    {
        public static bool Read(byte[] data, out PublicToken token)
        {
            var rw = new ReaderWriter(data);
            token = new PublicToken();
            return token.Read(ref rw);
        }
        
        public ulong ProtocolId;
        public ulong Created;
        public ulong Expire;
        public ulong ClientId;
        public ServerEntry[] Servers;

        internal bool Read(ref ReaderWriter rw)
        {
            rw.Read(out var version, Constants.Version.Length);
            if (version != Constants.Version) return false;
            
            rw.Read(out ProtocolId);
            rw.Read(out Created);
            rw.Read(out Expire);
            rw.Read(out ClientId);
            rw.Read(out byte numServers);
            Servers = new ServerEntry[numServers];
            for (var i = 0; i < numServers; i++)
                Servers[i].Read(ref rw);

            return version == Constants.Version;
        }
        
        internal bool Write(ref ReaderWriter rw)
        {
            rw.Write(Constants.Version);
            rw.Write(ProtocolId);
            rw.Write(Created);
            rw.Write(Expire);
            rw.Write(ClientId);
            rw.Write((byte)Servers.Length);
            for (var i = 0; i < Servers.Length; i++)
                Servers[i].Write(ref rw);

            return true;
        }
    }
}