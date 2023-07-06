using NetcodeIO.NET.Core.Token;
using NetcodeIO.NET.Utils;
using NetcodeIO.NET.Utils.IO;

namespace NetcodeIO.NET.Core.Requests
{
    internal unsafe struct ConnectionPacket
    {
        public const int SIZE = PacketHeader.SIZE + 8 + 8 + Defines.NONCE_SIZE + PrivateToken.SIZE;

        public PacketHeader Header;
        
        public ulong ProtocolId; // 8
        public ulong ExpireTimestamp; // 8
        private fixed byte nonce[Defines.NONCE_SIZE]; // 12
        private fixed byte privateKeyData[PrivateToken.SIZE]; // 512

        public Span<byte> Nonce
        {
            get
            {
                fixed (byte* p = nonce) return new Span<byte>(p, Defines.NONCE_SIZE);
            }
        }
        
        public Span<byte> PrivateKeyData
        {
            get
            {
                fixed (byte* p = privateKeyData) return new Span<byte>(p, PrivateToken.SIZE);
            }
        }

        public ConnectionPacket(ref PublicToken token)
        {
            Header = new PacketHeader(PacketType.ConnectionRequest);
            
            ProtocolId = token.ProtocolId;
            ExpireTimestamp = token.ExpireTimestamp;
            
            fixed (byte* from = token.Nonce)
            fixed (byte* to = Nonce)
                KeyUtils.CopyKey(new Span<byte>(from, Defines.NONCE_SIZE), new Span<byte>(to, Defines.NONCE_SIZE));
            
            fixed (byte* from = token.PrivateKeyData)
            fixed (byte* to = PrivateKeyData)
                KeyUtils.CopyKey(new Span<byte>(from, Defines.NONCE_SIZE), new Span<byte>(to, PrivateToken.SIZE));
        }

        public bool Read(ref ReaderWriter reader)
        {
            reader.Read(out var version, 13);
            if (version != Defines.NETCODE_VERSION_INFO_STR) return false;

            reader.Read(out ProtocolId);
            reader.Read(out ExpireTimestamp);
            reader.Read(Nonce);
            reader.Read(PrivateKeyData);
            
            return true;
        }

        public bool Write(ref ReaderWriter writer)
        {
            writer.Write(Defines.NETCODE_VERSION_INFO_STR);
            
            writer.Write(ProtocolId);
            writer.Write(ExpireTimestamp);
            writer.Write(Nonce);
            writer.Write(PrivateKeyData);
            
            return true;
        }
    }
}