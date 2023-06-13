using System.Runtime.InteropServices;
using NetcodeIO.NET.Utils.IO;

namespace NetcodeIO.NET.Core.Token
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct PublicNetcodeToken
    {
        public const int SIZE = 1024; // 841 actually

        // 13 bytes for version
        public ulong ProtocolId; // 8
        public ulong CreateTimestamp; // 8
        public ulong ExpireTimestamp; // 8
        public ulong ConnectTokenSequence; // 8
        public fixed byte Nonce[Defines.NONCE_SIZE]; // 24
        public fixed byte PrivateKeyData[Defines.PRIVATE_TOKEN_SIZE]; // 512
        public uint TimeoutSeconds; // 4
        public fixed byte ClientToServerKey[Defines.KEY_SIZE]; // 32
        public fixed byte ServerToClientKey[Defines.KEY_SIZE]; // 32
        public ushort NumServers; // in 1 - 10, 2
        // at this point we have 334b and (512 - 334) 178 bytes for servers addresses which is 10 slots
        // size of also can vary in size from 7 to 19 bytes but for optimization purposes we'll use 17 bytes explicit layout
        // due to C# limitations define them explicitly 
        private NetcodeServerEntry server1; // 19
        private NetcodeServerEntry server2; // 19
        private NetcodeServerEntry server3; // 19
        private NetcodeServerEntry server4; // 19
        private NetcodeServerEntry server5; // 19
        private NetcodeServerEntry server6; // 19
        private NetcodeServerEntry server7; // 19
        private NetcodeServerEntry server8; // 19
        private NetcodeServerEntry server9; // 19
        private NetcodeServerEntry server10; // 19
        // struct layout contains 833 bytes but we'll simply round it to 1024
        
        // and give access by method
        public NetcodeServerEntry GetServer(int index)
        {
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
                       9 => server10,
                       _ => throw new IndexOutOfRangeException()
                   };
        }
        
        public void SetServer(int index, in NetcodeServerEntry value)
        {
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
                case 9: server10 = value; return;
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
            fixed (byte* p = Nonce) reader.Read(p, Defines.NONCE_SIZE);
            fixed (byte* p = PrivateKeyData) reader.Read(p, Defines.PRIVATE_TOKEN_SIZE);
            reader.Read(out TimeoutSeconds);
            fixed(byte* p = ClientToServerKey) reader.Read(p, Defines.KEY_SIZE);
            fixed(byte* p = ServerToClientKey) reader.Read(p, Defines.KEY_SIZE);
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
            server10.Read(ref reader);
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
            fixed (byte* p = Nonce) writer.Write(p, Defines.NONCE_SIZE);
            fixed (byte* p = PrivateKeyData) writer.Write(p, Defines.PRIVATE_TOKEN_SIZE);
            writer.Write(TimeoutSeconds);
            fixed(byte* p = ClientToServerKey) writer.Write(p, Defines.KEY_SIZE);
            fixed(byte* p = ServerToClientKey) writer.Write(p, Defines.KEY_SIZE);
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
            server10.Write(ref writer);
        }
    }
}