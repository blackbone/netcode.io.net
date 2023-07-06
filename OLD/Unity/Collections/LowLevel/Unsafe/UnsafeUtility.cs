using System.Runtime.InteropServices;

namespace Netcode.io.OLD.Unity.Collections.LowLevel.Unsafe
{
    public static class UnsafeUtility
    {
        public static unsafe void MemCpy(void* from, void* to, int count)
            => new Span<byte>(from, count).CopyTo(new Span<byte>(to, count));
        
        public static unsafe int MemCmp(void* from, void* to, int count)
            => new Span<byte>(from, count).SequenceCompareTo(new Span<byte>(to, count));
        
        public static unsafe void* AddressOf<T>(ref T val) where T: struct
        {
            fixed (void* p = &val) return p;
        }

        public static unsafe void* Malloc(int size, int alignment, Allocator allocator) => Marshal.AllocHGlobal(size).ToPointer();
        public static unsafe void Free(void* handle, Allocator allocator) => Marshal.FreeHGlobal(new IntPtr(handle));
        
        public static unsafe int SizeOf<T>() where T : struct => sizeof (T);
        public static int AlignOf<T>() where T : struct => SizeOf<AlignOfHelper<T>>() - SizeOf<T>();
        
        private struct AlignOfHelper<T> where T : struct
        {
            public byte dummy;
            public T data;
        }
    }
}