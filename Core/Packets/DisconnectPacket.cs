using NetcodeIO.NET.Utils.IO;

namespace NetcodeIO.NET.Core.Requests
{
    internal struct DisconnectPacket
    {
        public const int SIZE = 2;
        
        public PacketHeader Header;

        public DisconnectPacket() => Header = new PacketHeader(PacketType.Disconnect);
        public bool Read(ref ReaderWriter reader) => Header.Read(ref reader);
        public bool Write(ref ReaderWriter writer) => Header.Write(ref writer);
    }
}