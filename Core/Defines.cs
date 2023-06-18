#if !COMPACT
#define HEADER_COMPACT_LAYOUT
#endif

using System.Runtime.CompilerServices;
using NetcodeIO.NET.Core.Token;

namespace NetcodeIO.NET.Core
{
    internal static class Defines
    {
        public const string NETCODE_VERSION_INFO_STR = "NETCODE 1.02\0";
        public const int NETCODE_VERSION_INFO_STR_SIZE = 13;
        public const int AED_LENGTH = NETCODE_VERSION_INFO_STR_SIZE + 8 + 8;
        public const int PRIVATE_TOKEN_ENCRYPT_SIZE = PrivateToken.SIZE - MAC_SIZE;
        
        public const int MAX_SERVERS = 9;
        public const int NONCE_SIZE = 12; // REQUIRES EXACTLY IT
        public const int KEY_SIZE = 32;
        public const int USER_DATA_SIZE = 256;
        public const int NUM_DISCONNECT_PACKETS = 8;
        
        public const int DATAGRAM_READ_BUFFER_SIZE = 2048;
        public const int DATAGRAM_WRITE_BUFFER_SIZE = 2048;
        
        public const int MAC_SIZE = 16;

        public const int CHALLENGE_ENCRYPTED_SIZE = 300;
        public const int PACKET_PAYLOAD_DATA_SIZE = 1200;
        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetBytesCount(ref this ulong number)
        {
            switch (number)
            {
                case <= 0x00000000000000FFUL: return 1;
                case <= 0x000000000000FFFFUL: return 2;
                case <= 0x0000000000FFFFFFUL: return 3;
                case <= 0x00000000FFFFFFFFUL: return 4;
                case <= 0x000000FFFFFFFFFFUL: return 5;
                case <= 0x0000FFFFFFFFFFFFUL: return 6;
                case <= 0x00FFFFFFFFFFFFFFUL: return 7;
                default: return 8;
            }
        }
    }
}