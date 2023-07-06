using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Netcode.io.IO;

namespace Netcode.io.Tokens
{
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public unsafe struct EndPoint
    {
        private AddressType AddressType;
        private fixed byte addressBytes[16];
        private ushort Port;

        internal void Read(ref ReaderWriter rw)
        {
            rw.Read(out AddressType);
            switch (AddressType)
            {
                case AddressType.IPv4: fixed(byte* p = addressBytes) rw.Read(new Span<byte>(p, 4)); break;
                case AddressType.IPv6: fixed(byte* p = addressBytes) rw.Read(new Span<byte>(p, 16)); break;
                case AddressType.None: 
                default:
                    throw new ArgumentOutOfRangeException();
            }
            rw.Read(out Port);
        }
        
        internal void Write(ref ReaderWriter rw)
        {
            if (AddressType == 0)
            {
                rw.Skip(19);
                return;
            }
            
            rw.Write(AddressType);
            var len = AddressType == AddressType.IPv4 ? 4 : 16;
            fixed(byte* p = addressBytes) rw.Write(new Span<byte>(p, len));
            rw.Write(Port);
        }
        
        public static implicit operator IPEndPoint(EndPoint endPoint)
        {
            ReadOnlySpan<byte> span;
            switch (endPoint.AddressType)
            {
                case AddressType.IPv4: span = new ReadOnlySpan<byte>(endPoint.addressBytes, 4); break;
                case AddressType.IPv6: span = new ReadOnlySpan<byte>(endPoint.addressBytes, 16); break;
                case AddressType.None:
                default: return null;
            }

            return new IPEndPoint(new IPAddress(span), endPoint.Port);
        }
        
        public static implicit operator EndPoint(IPEndPoint endPoint)
        {
            var result = new EndPoint();
            Span<byte> span;
            switch (endPoint.AddressFamily)
            {
                case AddressFamily.InterNetwork:
                    result.AddressType = AddressType.IPv4;
                    span = new Span<byte>(result.addressBytes, 4);
                    break;
                case AddressFamily.InterNetworkV6:
                    span = new Span<byte>(result.addressBytes, 16);
                    result.AddressType = AddressType.IPv6;
                    break;
                default: return default;
            }
            
            endPoint.Address.GetAddressBytes().CopyTo(span);
            result.Port = (ushort)endPoint.Port;
            return result;
        }
    }
}