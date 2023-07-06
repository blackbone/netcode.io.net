using System.Runtime.InteropServices;

namespace NetcodeIO.NET.Core.Buffers
{
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    internal unsafe struct SocketBuffers
    {
        private fixed byte readBuffer[Defines.DATAGRAM_READ_BUFFER_SIZE];
        private fixed byte writeBuffer[Defines.DATAGRAM_WRITE_BUFFER_SIZE];
        
        public int ReadSize;
        public int WriteSize;

        public Span<byte> ReadBuffer
        {
            get
            {
                fixed (byte* p = readBuffer) return new Span<byte>(p, Defines.DATAGRAM_READ_BUFFER_SIZE);
            }
        }
        
        public Span<byte> WriteBuffer
        {
            get
            {
                fixed (byte* p = writeBuffer) return new Span<byte>(p, Defines.DATAGRAM_WRITE_BUFFER_SIZE);
            }
        }
    }
}