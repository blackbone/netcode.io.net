using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace NetcodeIO.NET.Core
{
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    internal struct UdpSocketContext
    {
        public bool IsAllocated;
        private object mutex;
        private Socket internalSocket;
        
        public static void GetServer(ref UdpSocketContext socket, IPEndPoint endPoint)
        {
            if (socket.IsAllocated) throw new InvalidOperationException();
            
            socket.internalSocket = new Socket(endPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            socket.mutex = new();
            switch (endPoint.AddressFamily)
            {
                case AddressFamily.InterNetwork: socket.internalSocket.Bind(endPoint); break;
                case AddressFamily.InterNetworkV6: socket.internalSocket.Bind(new IPEndPoint(IPAddress.IPv6Any, 0)); break;
                default: throw new ArgumentOutOfRangeException(nameof(endPoint.AddressFamily));
            }
            socket.IsAllocated = true;
        }
        
        public static void GetClient(ref UdpSocketContext socket, IPEndPoint endPoint)
        {
            if (socket.IsAllocated) throw new InvalidOperationException();
            
            socket.internalSocket = new Socket(endPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            socket.mutex = new();
            switch (endPoint.AddressFamily)
            {
                case AddressFamily.InterNetwork: socket.internalSocket.Bind(new IPEndPoint(IPAddress.Any, 0)); break;
                case AddressFamily.InterNetworkV6: socket.internalSocket.Bind(new IPEndPoint(IPAddress.IPv6Any, 0)); break;
                default: throw new ArgumentOutOfRangeException(nameof(endPoint.AddressFamily));
            }
            socket.IsAllocated = true;
        }
        
        public static void Release(ref UdpSocketContext socket)
        {
            if (!socket.IsAllocated) throw new InvalidOperationException();
            
            socket.internalSocket.Dispose();
            socket.internalSocket = null;
            socket.mutex = null;
            socket.IsAllocated = false;
        }
        
        public unsafe bool ReadFrom(Span<byte> buffer, out Span<byte> frame, ref EndPoint remoteEp)
        {
            lock (mutex)
            {
                var size = internalSocket.ReceiveFrom(buffer, SocketFlags.None, ref remoteEp);
                fixed(byte* p = &buffer.GetPinnableReference())
                    frame = new Span<byte>(p, size);
                return size > 0;
            }
        }
        
        public unsafe bool WriteTo(Span<byte> buffer, ref int size, EndPoint remoteEp)
        {
            lock (mutex)
            {
                ReadOnlySpan<byte> frame;
                fixed (byte* p = &buffer.GetPinnableReference())
                    frame = new ReadOnlySpan<byte>(p, size);
                size = internalSocket.SendTo(frame, SocketFlags.None, remoteEp);
                return size > 0;
            }
        }
        
        public void Close()
        {
            lock (mutex)
            {
                internalSocket.Close();
            }
        }
        
    }
}