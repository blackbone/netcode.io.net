using System.Runtime.CompilerServices;

namespace NetcodeIO.NET.Utils
{
    /// <summary>
    /// Utility for generating crypto keys
    /// </summary>
    public static class KeyUtils
    {
        [ThreadStatic] private static Random random;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GenerateKey(in Span<byte> keyBuffer)
        {
            if (keyBuffer.Length > 1024) throw new ArgumentException();
            
            random ??= new Random((int)DateTimeOffset.Now.UtcTicks);
            random.NextBytes(keyBuffer);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyKey(Span<byte> from, Span<byte> to)
        {
            if (from.Length > to.Length) throw new ArgumentOutOfRangeException(nameof(from.Length));
            from.TryCopyTo(to);
        }
    }
}