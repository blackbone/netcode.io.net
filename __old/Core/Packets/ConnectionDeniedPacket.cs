using System.Runtime.InteropServices;
using NetcodeIO.NET.Utils.IO;

namespace NetcodeIO.NET.Core.Requests
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct DeniedPacket
    {
        public const int SIZE = PacketHeader.SIZE;
        
        public PacketHeader Header;
        
        public DeniedPacket() => Header = new PacketHeader(PacketType.Denied);

        public bool Read(ref ReaderWriter reader) => true;

        public bool Write(ref ReaderWriter writer) => Header.Write(ref writer);
    }
}