using System.Net;

namespace NetcodeIO.NET.Core.Datagram
{
    internal struct Datagram
    {
        public EndPoint Sender;
        public unsafe fixed byte Payload[Defines.DATAGRAM_READ_BUFFER_SIZE];
        public int PayloadSize;
    }
}