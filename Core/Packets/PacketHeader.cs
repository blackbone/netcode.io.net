#if COMPACT
#define HEADER_COMPACT_LAYOUT
#endif

using System.Runtime.InteropServices;
using NetcodeIO.NET.Utils.IO;

namespace NetcodeIO.NET.Core.Requests
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct PacketHeader
    {
#if HEADER_COMPACT_LAYOUT
        public const int SIZE = 9; // for compact layout it's max possible value
#else
        public const int SIZE = 10; // for strict layout it's constant value
#endif
        
        public PacketType PacketType; // 00000111 <- these bytes used because we have only 8 states
        public PayloadFlags PayloadFlags;
        public byte NumSequenceBytes;
        public ulong SequenceNumber; // 1-8 bytes

        public PacketHeader(PacketType packetType)
        {
            PacketType = packetType;
            PayloadFlags = PayloadFlags.None;
            NumSequenceBytes = 0;
            SequenceNumber = 0;
        }

        public bool Read(ref ReaderWriter reader)
        {
#if HEADER_COMPACT_LAYOUT
            reader.Read(out byte prefixByte); // 1
            int prefix = prefixByte;
            PacketType = (PacketType)(prefix & 0b00000111);
            if (PacketType == PacketType.ConnectionRequest)
                return true;

            var numSequenceBytes = ((prefix >> 3) & 0b00000111) + 1; // 0-7 -> 1-8, even don't need to check

            if (PacketType == PacketType.Payload)
                PayloadFlags = (PayloadFlags)((prefix >> 6) & 0b0000011);

            NumSequenceBytes = (byte)numSequenceBytes;
            SequenceNumber = 0; // reset sequence number

            for (var i = 0; i < numSequenceBytes; i++)
            {
                reader.Read(out byte current);
                SequenceNumber |= (ulong)current << (i * 8);
            }
#else
            reader.Read(out PacketType);
            if (PacketType == PacketType.ConnectionRequest) return true;
            reader.Read(out SequenceNumber);
            if (PacketType == PacketType.Payload)
                reader.Read(out PayloadFlags); // 1
#endif


            return true;
        }

        public bool Write(ref ReaderWriter writer)
        {
#if HEADER_COMPACT_LAYOUT
            byte prefixByte = 0;
            if (PacketType == PacketType.ConnectionRequest)
            {
                writer.Write(prefixByte);
                return true;
            }

            var sequrenceBytesCount = (byte)SequenceNumber.GetBytesCount();
            prefixByte |= (byte)PacketType; // 00000111
            prefixByte |= (byte)(sequrenceBytesCount << 3); // 00111000

            if (PacketType == PacketType.Payload)
                prefixByte |= (byte)((byte)PayloadFlags << 6); // 11000000

            writer.Write(prefixByte); // 1
            var sequenceNumber = SequenceNumber;
            for (var i = 0; i < sequrenceBytesCount; i++)
                writer.Write((byte)(sequenceNumber >> (i * 8)));
#else

            writer.Write(PacketType);
            if (PacketType == PacketType.ConnectionRequest) return true;
            writer.Write(SequenceNumber);
            if (PacketType != PacketType.Payload) return true;
            writer.Write(PayloadFlags);
#endif

            return true;
        }
    }
}