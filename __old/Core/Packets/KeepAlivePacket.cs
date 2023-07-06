using NetcodeIO.NET.Utils.IO;

namespace NetcodeIO.NET.Core.Requests
{
    internal struct KeepAlivePacket
    {
        public PacketHeader Header;
        public ushort ClientIndex;
        public ushort MaxClients;

        public KeepAlivePacket(ref PacketHeader header)
        {
            Header.PacketType = header.PacketType;
            Header.SequenceNumber = header.SequenceNumber;
            Header.PayloadFlags = header.PayloadFlags;
            Header.SequenceNumber = header.SequenceNumber;
        }

        public bool Read(ref ReaderWriter reader)
        {
            reader.Read(out ClientIndex);
            reader.Read(out MaxClients);
            
            return true;
        }

        public bool Write(ref ReaderWriter writer)
        {
            writer.Write(ClientIndex);
            writer.Write(MaxClients);
            
            return true;
        }
    }
}