using Netcode.io.IO;

namespace Netcode.io.Tokens
{
    public struct ServerEntry
    {
        public EndPoint EndPoint;
        public byte[] ClientKey;
        public byte[] PrivateTokenData;
        
        internal bool Read(ref ReaderWriter rw)
        {
            EndPoint.Read(ref rw);
            ClientKey = new byte[16];
            rw.Read(ClientKey);
            rw.Read(out byte len);
            PrivateTokenData = new byte[len];
            rw.Read(PrivateTokenData);
            return true;
        }

        internal bool Write(ref ReaderWriter rw)
        {
            EndPoint.Write(ref rw);
            rw.Write(PrivateTokenData.Length);
            rw.Write(ClientKey);
            rw.Write(PrivateTokenData);
            return true;
        }
    }
}