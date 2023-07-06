namespace NetcodeIO.NET.Utils.Crypto
{
    internal static class Pack
    {
        internal static void UInt32_To_LE(in uint n, byte[] bs, int off = 0)
        {
            bs[off] = (byte)n;
            bs[off + 1] = (byte)(n >> 8);
            bs[off + 2] = (byte)(n >> 16);
            bs[off + 3] = (byte)(n >> 24);
        }

        internal static void UInt32_To_LE(uint[] ns, byte[] bs, int off)
        {
            for (var i = 0; i < ns.Length; ++i)
            {
                UInt32_To_LE(ns[i], bs, off);
                off += 4;
            }
        }
        
        internal static uint LE_To_UInt32(byte[] bs, int off)
        {
            return bs[off]
                   | ((uint)bs[off + 1] << 8)
                   | ((uint)bs[off + 2] << 16)
                   | ((uint)bs[off + 3] << 24);
        }

        internal static void LE_To_UInt32(byte[] bs, int bOff, uint[] ns, int nOff, int count)
        {
            for (var i = 0; i < count; ++i)
            {
                ns[nOff + i] = LE_To_UInt32(bs, bOff);
                bOff += 4;
            }
        }

        internal static uint[] LE_To_UInt32(byte[] bs, int off, int count)
        {
            var ns = new uint[count];
            for (var i = 0; i < ns.Length; ++i)
            {
                ns[i] = LE_To_UInt32(bs, off);
                off += 4;
            }

            return ns;
        }

        internal static void UInt64_To_LE(ulong n, byte[] bs)
        {
            UInt32_To_LE((uint)n, bs);
            UInt32_To_LE((uint)(n >> 32), bs, 4);
        }
    }
}