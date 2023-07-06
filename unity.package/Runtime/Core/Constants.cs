namespace Netcode.io
{
    internal static class Constants
    {
        public const string Version = "NCSTD1.00";
        public const int MaxServers = 9;
        public const int UserDataSize = 256;
        
        public const int DatagramReadBufferSize = 2048;
        public const int DatagramWriteBufferSize = 2048;
        
        public const int PrivateKeyMaxSize = 512;
        public const int PublicTokenMaxSize = 1024;
        public const int ChallengeEncryptedSize = 300;
    }
}