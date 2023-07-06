using System.Runtime.InteropServices;
using NetcodeIO.NET.Utils.IO;

namespace NetcodeIO.NET.Core.Requests
{
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    internal unsafe struct PayloadPacket
    {
        public PacketHeader Header;
        private fixed byte payload[Defines.PACKET_PAYLOAD_DATA_SIZE];
        private int? payloadSize;

        public Span<byte> Payload
        {
            get
            {
                fixed (byte* p = payload) return new Span<byte>(p, payloadSize ?? Defines.PACKET_PAYLOAD_DATA_SIZE);
            }
        }

        public int? PayloadSize
        {
            get => payloadSize;
            set => payloadSize = value;
        }
        
        public PayloadPacket(ref PacketHeader header)
        {
            Header.PacketType = header.PacketType;
            Header.SequenceNumber = header.SequenceNumber;
            Header.PayloadFlags = header.PayloadFlags;
            Header.SequenceNumber = header.SequenceNumber;
        }

        public bool Read(ref ReaderWriter reader)
        {
            reader.Read(Payload, reader.Remaining);
            
            return true;
        }

        public bool Write(ref ReaderWriter writer)
        {
            writer.Write(Payload, PayloadSize ?? 0);
            
            return true;
        }
    }
}