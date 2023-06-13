namespace NetcodeIO.NET.Core
{
    internal static class Defines
    {
        public const string NETCODE_VERSION_INFO_STR = "NETCODE 1.02\0";
        public const int NETCODE_VERSION_INFO_STR_SIZE = 13;
        public static int AED_LENGTH = NETCODE_VERSION_INFO_STR_SIZE + 8 + 8;
        public static int PRIVATE_TOKEN_ENCRYPT_SIZE = PRIVATE_TOKEN_SIZE - MAC_SIZE;
        
        public const int MAX_SERVERS = 10;
        public const int NONCE_SIZE = 12; // REQUIRES EXACTLY IT
        public const int KEY_SIZE = 32;
        public const int PRIVATE_TOKEN_SIZE = Token.PrivateNetcodeToken.SIZE;
        public const int PUBLIC_TOKEN_SIZE = Token.PublicNetcodeToken.SIZE;
        public const int USER_DATA_SIZE = 256;
        
        
        
        public const int DATAGRAM_READ_BUFFER_SIZE = 2048;
        
        public const int NETCODE_CONNECT_TOKEN_PRIVATE_BYTES = 1024;
        public const int NETCODE_CONNECT_TOKEN_PUBLIC_BYTES = 2048;
        public const int MAC_SIZE = 16;
        public const int MAX_PAYLOAD_SIZE = 1200;

        public const int NETCODE_VERSION_INFO_BYTES = 13;

        public const int NUM_DISCONNECT_PACKETS = 10;

        public const int NETCODE_TIMEOUT_SECONDS = 10;
        public const int CHALLENGE_TOKEN_SIZE = 32;
    }
}