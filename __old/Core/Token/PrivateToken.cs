using System.Net;
using System.Runtime.InteropServices;
using NetcodeIO.NET.Utils.IO;

namespace NetcodeIO.NET.Core.Token
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal unsafe struct PrivateToken
    {
        public const int SIZE = 8 + 4 + Defines.KEY_SIZE + Defines.KEY_SIZE + Defines.USER_DATA_SIZE + 2 + ServerEntry.SIZE * Defines.MAX_SERVERS;
        
        public ulong ClientId; // 8
        public uint TimeoutSeconds; // 4
        private fixed byte clientToServerKey[Defines.KEY_SIZE]; // 32
        private fixed byte serverToClientKey[Defines.KEY_SIZE]; // 32
        private fixed byte userData[Defines.USER_DATA_SIZE]; // 256
        // at this point we have 334b
        
        public ushort NumServers; // in 1 - 10, 2 bytes
        // at this point we have 334b and (512 - 334) 178 bytes for servers addresses which is 10 slots
        // size of also can vary in size from 7 to 19 bytes but for optimization purposes we'll use 17 bytes explicit layout
        // due to C# limitations define them explicitly 
        private ServerEntry server1; // 19
        private ServerEntry server2; // 19
        private ServerEntry server3; // 19
        private ServerEntry server4; // 19
        private ServerEntry server5; // 19
        private ServerEntry server6; // 19
        private ServerEntry server7; // 19
        private ServerEntry server8; // 19
        private ServerEntry server9; // 19

        public Span<byte> ClientToServerKey
        {
            get
            {
                fixed(byte* p = clientToServerKey)
                    return new Span<byte>(p, Defines.KEY_SIZE);
            }
        }
        
        public Span<byte> ServerToClientKey
        {
            get
            {
                fixed(byte* p = serverToClientKey)
                    return new Span<byte>(p, Defines.KEY_SIZE);
            }
        }

        public Span<byte> UserData
        {
            get
            {
                fixed(byte* p = userData)
                    return new Span<byte>(p, Defines.USER_DATA_SIZE);
            }
        }

        // and give access by method
        public ServerEntry GetServer(int index)
        {
            switch (index)
            {
                case 0: return server1;
                case 1: return server2;
                case 2: return server3;
                case 3: return server4;
                case 4: return server5;
                case 5: return server6;
                case 6: return server7;
                case 7: return server8;
                case 8: return server9;
                default: throw new IndexOutOfRangeException();
            }
        }
        
        public void SetServer(int index, IPEndPoint endPoint)
        {
            switch (index)
            {
                case 0: server1.Fill(endPoint); break;
                case 1: server2.Fill(endPoint); break;
                case 2: server3.Fill(endPoint); break;
                case 3: server4.Fill(endPoint); break;
                case 4: server5.Fill(endPoint); break;
                case 5: server6.Fill(endPoint); break;
                case 6: server7.Fill(endPoint); break;
                case 7: server8.Fill(endPoint); break;
                case 8: server9.Fill(endPoint); break;
                default: throw new IndexOutOfRangeException();
            }
        }

        public bool Read(ref ReaderWriter reader)
        {
            reader.Read(out ClientId);
            reader.Read(out TimeoutSeconds);
            reader.Read(ClientToServerKey);
            reader.Read(ServerToClientKey);
            reader.Read(UserData);
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

        public bool Write(ref ReaderWriter writer)
        {
            writer.Write(ClientId);
            writer.Write(TimeoutSeconds);
            writer.Write(ClientToServerKey);
            writer.Write(ServerToClientKey);
            writer.Write(UserData);
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
            return true;
        }
    }
}