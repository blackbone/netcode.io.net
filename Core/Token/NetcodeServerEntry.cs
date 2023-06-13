using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using NetcodeIO.NET.Core.Datagram;
using NetcodeIO.NET.Utils.IO;

namespace NetcodeIO.NET.Core.Token
{
    [StructLayout(LayoutKind.Explicit)]
    internal unsafe struct NetcodeServerEntry
    {
        [FieldOffset(0)] public NetcodeAddressType AddressType;
        [FieldOffset(1)] public fixed byte AddressBytes[16];
        [FieldOffset(17)] public ushort Port;
        
        public void Read(ref ReaderWriter reader)
        {
            reader.Read(out AddressType);
            if (AddressType == 0)
            {
                reader.Skip(18);
                return;
            }
            
            fixed(byte* p = AddressBytes) reader.Read(p, 16);
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
            fixed(byte* p = AddressBytes) writer.Write(p, 16);
            writer.Write(Port);
        }

        public void Fill(IPEndPoint endPoint)
        {
            switch (endPoint.AddressFamily)
            {
                case AddressFamily.InterNetwork: AddressType = NetcodeAddressType.IPv4; break;
                case AddressFamily.InterNetworkV6: AddressType = NetcodeAddressType.IPv6; break;
                default: throw new ArgumentOutOfRangeException(nameof(endPoint.AddressFamily));
            }

            var addressLen = AddressType switch
                             {
                                 NetcodeAddressType.IPv4 => 4,
                                 NetcodeAddressType.IPv6 => 16,
                                 _ => throw new ArgumentOutOfRangeException(nameof(AddressType))
                             };

            var bytes = endPoint.Address.GetAddressBytes();
            for (var i = 0; i < addressLen; i++)
                AddressBytes[i] = bytes[i];

            Port = (ushort)endPoint.Port;
        }
    }
}