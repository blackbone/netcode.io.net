using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using NetcodeIO.NET.Utils.IO;

namespace NetcodeIO.NET.Core.Token
{
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    internal unsafe struct ServerEntry
    {
        public const int SIZE = 19;
        
        private fixed byte addressBytes[16];
        public ushort Port;
        public AddressType AddressType;

        public Span<byte> AddressBytes
        {
            get
            {
                fixed(byte* p = addressBytes) return new Span<byte>(p, (int)AddressType);
            }
        }

        public void Read(ref ReaderWriter reader)
        {
            reader.Read(out AddressType);
            if (AddressType == 0)
            {
                reader.Skip(18);
                return;
            }
            
            reader.Read(AddressBytes);
            reader.Read(out Port);
        }
        
        public void Write(ref ReaderWriter writer)
        {
            if (AddressType == 0)
            {
                writer.Skip(19);
                return;
            }
            
            writer.Write(AddressType);
            writer.Write(AddressBytes);
            writer.Write(Port);
        }

        public void Fill(in IPEndPoint endPoint)
        {
            switch (endPoint.AddressFamily)
            {
                case AddressFamily.InterNetwork: AddressType = AddressType.IPv4; break;
                case AddressFamily.InterNetworkV6: AddressType = AddressType.IPv6; break;
                default: throw new ArgumentOutOfRangeException(nameof(endPoint.AddressFamily));
            }

            var span = AddressBytes;
            var bytes = endPoint.Address.GetAddressBytes();
            bytes.CopyTo(span);
            Port = (ushort)endPoint.Port;
        }

        public IPEndPoint AsIPEndPoint() => new(new IPAddress(AddressBytes), Port);
    }
}