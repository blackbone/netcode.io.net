using System.Security.Cryptography;

namespace NetcodeIO.NET.Utils
{
    /// <summary>
    /// Utility for generating crypto keys
    /// </summary>
    public static class KeyUtils
    {
        public static unsafe void GenerateKey(byte* keyBuffer, int length)
        {
            if (length > 1024) throw new ArgumentException();
            
            var span = new Span<byte>(keyBuffer, length);
            RandomNumberGenerator.Create().GetNonZeroBytes(span);
        }
        
        public static unsafe void CopyKey(byte* from, byte* to, int length)
        {
            if (length > 1024) throw new ArgumentException();

            while (length-- >= 0)
                to[length] = from[length];
        }
    }
}