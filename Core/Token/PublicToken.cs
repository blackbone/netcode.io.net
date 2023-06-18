using System.Runtime.InteropServices;
using NetcodeIO.NET.Utils.IO;

namespace NetcodeIO.NET.Core.Token
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct PublicToken
    {
        public const int SIZE = 13 + 8 + 8 + 8 + 8 + Defines.NONCE_SIZE + PrivateToken.SIZE + 4 + Defines.KEY_SIZE + Defines.KEY_SIZE + 2 + ServerEntry.SIZE * Defines.MAX_SERVERS; // 841 actually

        // 13 bytes for version
        public ulong ProtocolId; // 8
        public ulong CreateTimestamp; // 8
        public ulong ExpireTimestamp; // 8
        public ulong ConnectTokenSequence; // 8
        private fixed byte nonce[Defines.NONCE_SIZE]; // 12
        private fixed byte privateKeyData[PrivateToken.SIZE]; // 505
        public uint TimeoutSeconds; // 4
        private fixed byte clientToServerKey[Defines.KEY_SIZE]; // 32
        private fixed byte serverToClientKey[Defines.KEY_SIZE]; // 32
        public ushort NumServers; // in 1 - 10, 2
        // due to C# limitations define them explicitly
        // also it'll be possible to avoid duplication of public token but we're sending only 
        private ServerEntry server1; // 19
        private ServerEntry server2; // 19
        private ServerEntry server3; // 19
        private ServerEntry server4; // 19
        private ServerEntry server5; // 19
        private ServerEntry server6; // 19
        private ServerEntry server7; // 19
        private ServerEntry server8; // 19
        private ServerEntry server9; // 19
        // struct layout contains 833 bytes but we'll simply round it to 1024
        
        public Span<byte> ClientToServerKey
        {
            get
            {
                fixed(byte* p = clientToServerKey) return new Span<byte>(p, Defines.KEY_SIZE);
            }
        }
        
        public Span<byte> ServerToClientKey
        {
            get
            {
                fixed(byte* p = serverToClientKey) return new Span<byte>(p, Defines.KEY_SIZE);
            }
        }
        
        public Span<byte> Nonce
        {
            get
            {
                fixed(byte* p = nonce) return new Span<byte>(p, Defines.NONCE_SIZE);
            }
        }
        
        public Span<byte> PrivateKeyData
        {
            get
            {
                fixed(byte* p = privateKeyData) return new Span<byte>(p, PrivateToken.SIZE);
            }
        }
        
        public Span<byte> Aed
        {
            get
            {
                fixed(byte* p = privateKeyData) return new Span<byte>(p, Defines.AED_LENGTH);
            }
        }
        
        // and give access by method
        public ServerEntry GetServer(byte index)
        {
            if (index >= NumServers) throw new IndexOutOfRangeException();
            
            return index switch
                   {
                       0 => server1,
                       1 => server2,
                       2 => server3,
                       3 => server4,
                       4 => server5,
                       5 => server6,
                       6 => server7,
                       7 => server8,
                       8 => server9,
                       _ => throw new IndexOutOfRangeException()
                   };
        }
        
        public void SetServer(byte index, in ServerEntry value)
        {
            if (index >= NumServers) throw new IndexOutOfRangeException();

            switch (index)
            {
                case 0: server1 = value; return;
                case 1: server2 = value; return;
                case 2: server3 = value; return;
                case 3: server4 = value; return;
                case 4: server5 = value; return;
                case 5: server6 = value; return;
                case 6: server7 = value; return;
                case 7: server8 = value; return;
                case 8: server9 = value; return;
                default:
                    throw new IndexOutOfRangeException();
            }
        }
        
        public bool Read(ref ReaderWriter reader)
        {
            reader.Read(out var version, 13);
            if (version != Defines.NETCODE_VERSION_INFO_STR) return false;
            reader.Read(out ProtocolId);
            reader.Read(out ExpireTimestamp);
            // written above will be used to form AED primitive for de- and encryption, it's 13 + 8 + 8 = 29 bytes
            
            reader.Read(out CreateTimestamp);
            reader.Read(out ConnectTokenSequence);
            reader.Read(Nonce);
            reader.Read(PrivateKeyData);
            reader.Read(out TimeoutSeconds);
            reader.Read(ClientToServerKey);
            reader.Read(ServerToClientKey);
            reader.Read(out NumServers);
            
            // because we always have bytes but sometimes filled with zeroes we can simply bypass this thing
            server1.Read(ref reader);
            server2.Read(ref reader);
            server3.Read(ref reader);
            server4.Read(ref reader);
            server5.Read(ref reader);
            server6.Read(ref reader);
            server7.Read(ref reader);
            server8.Read(ref reader);
            server9.Read(ref reader);
            return true;
        }

        public void Write(ref ReaderWriter writer)
        {
            writer.Write(Defines.NETCODE_VERSION_INFO_STR);
            writer.Write(ProtocolId);
            writer.Write(ExpireTimestamp);
            // written above will be used to form AED primitive for de- and encryption
            
            writer.Write(CreateTimestamp);
            writer.Write(ConnectTokenSequence);
            writer.Write(Nonce);
            writer.Write(PrivateKeyData);
            writer.Write(TimeoutSeconds);
            writer.Write(ClientToServerKey);
            writer.Write(ServerToClientKey);
            writer.Write(NumServers);
            
            // because we always have bytes but sometimes filled with zeroes we can simply bypass this thing
            server1.Write(ref writer);
            server2.Write(ref writer);
            server3.Write(ref writer);
            server4.Write(ref writer);
            server5.Write(ref writer);
            server6.Write(ref writer);
            server7.Write(ref writer);
            server8.Write(ref writer);
            server9.Write(ref writer);
        }
    }
}