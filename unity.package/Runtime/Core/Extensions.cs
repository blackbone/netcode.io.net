using System;
using System.Runtime.CompilerServices;

namespace Netcode.io
{
    public static class Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetBytesCount(ref this ulong number)
        {
            switch (number)
            {
                case <= 255UL: return 1;
                case <= 65535UL: return 2;
                case <= 16777215UL: return 3;
                case <= 4294967295UL: return 4;
                case <= 1099511627775UL: return 5;
                case <= 281474976710655UL: return 6;
                case <= 72057594037927935UL: return 7;
                default: return 8;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EqualsTo(this byte[] a1, byte[] a2)
            => EqualsTo((Span<byte>)a1, (Span<byte>)a2);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool EqualsTo(this Span<byte> a1, Span<byte> a2)
        {
            if (a1 == null || a2 == null || a1.Length != a2.Length)
                return false;
            
            fixed (byte* p1 = a1, p2 = a2)
            {
                byte* x1 = p1, x2 = p2;
                var l = a1.Length;
                for (var i = 0; i < l / 8; i++, x1 += 8, x2 += 8)
                    if (*(long*) x1 != *(long*) x2)
                        return false;
                
                if ((l & 4) != 0)
                {
                    if (*(int*) x1 != *(int*) x2) return false;
                    x1 += 4;
                    x2 += 4;
                }
                
                if ((l & 2) != 0)
                {
                    if (*(short*) x1 != *(short*) x2) return false;
                    x1 += 2;
                    x2 += 2;
                }

                if ((l & 1) == 0) return true;
                return *x1 == *x2;
            }
        }
    }
}