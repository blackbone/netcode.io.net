using System.Net;
using System.Net.Sockets;

namespace NetcodeIO.NET.Utils
{
    internal class BufferPool<T>
    {
        private static readonly Dictionary<int, Queue<T[]>> Pool = new();
        
        /// <summary>
        /// Retrieve a buffer of the given size
        /// </summary>
        public static void GetBuffer(int size, out T[] value)
        {
            lock (Pool)
            {
                if (Pool.ContainsKey(size) && Pool[size].Count > 0)
                    value = Pool[size].Dequeue();
            }

            value = new T[size];
        }

        /// <summary>
        /// Return a buffer to the pool
        /// </summary>
        public static void ReturnBuffer(ref T[] buffer)
        {
            lock (Pool)
            {
                if (!Pool.ContainsKey(buffer.Length))
                    Pool.Add(buffer.Length, new Queue<T[]>());

                Array.Clear(buffer, 0, buffer.Length);
                Pool[buffer.Length].Enqueue(buffer);
            }

            buffer = null;
        }
    }
    
    /// <summary>
    /// Helper methods for allocating temporary buffers
    /// </summary>
    internal sealed class ByteBufferPool : BufferPool<byte> { }
    
    /// <summary>
    /// Helper methods for allocating temporary buffers
    /// </summary>
    internal sealed class CharBufferPool : BufferPool<char> { }

    internal static class IPEndPointPool
    {
        private static readonly Queue<IPEndPoint> Pool = new();

        public static void GetDefault<T>(AddressFamily addressFamily, out T value) where T : EndPoint
        {
            switch (addressFamily)
            {
                case AddressFamily.InterNetwork:
                    Get(IPAddress.Any, 0, out value);
                    return;
                case AddressFamily.InterNetworkV6:
                    Get(IPAddress.IPv6Any, 0, out value);
                    return;
                default:
                    throw new ArgumentException(nameof(addressFamily));
            }
        }

        public static void Get<T>(IPAddress ip, ushort port, out T value) where T : EndPoint
        {
            lock (Pool)
            {
                if (Pool.Count > 0)
                {
                    var result = Pool.Dequeue();
                    result.Address = ip;
                    result.Port = port;
                    value = result as T;
                }

                value = new IPEndPoint(ip, port) as T;
            }
        }

        public static void Release(ref EndPoint value)
        {
            if (value is IPEndPoint ipEndPoint)
            {
                ipEndPoint.Address = null;
                ipEndPoint.Port = 0;
                lock (Pool)
                {
                    Pool.Enqueue(ipEndPoint);
                }
            }

            value = null;
        }
    }
    
    internal static class SocketPool
    {
        private static Queue<Socket> ipv4SocketPool;
        private static Queue<Socket> ipv6SocketPool;

        public static void Get(AddressFamily addressFamily, out Socket value)
        {
            switch (addressFamily)
            {
                case AddressFamily.InterNetwork:
                    lock (ipv4SocketPool)
                    {
                        if (ipv4SocketPool == null || ipv4SocketPool.Count <= 0)
                            value = new Socket(addressFamily, SocketType.Dgram, ProtocolType.Udp);
                        else
                            value = ipv4SocketPool.Dequeue();
                    }
                    return;
                case AddressFamily.InterNetworkV6:
                    lock (ipv6SocketPool)
                    {
                        if (ipv6SocketPool == null || ipv6SocketPool.Count <= 0)
                            value = new Socket(addressFamily, SocketType.Dgram, ProtocolType.Udp);
                        else
                            value = ipv6SocketPool.Dequeue();
                    }
                    return;
                default:
                    throw new ArgumentOutOfRangeException(nameof(addressFamily));
            }
        }

        public static void Release(ref Socket value)
        {
            switch (value.AddressFamily)
            {
                case AddressFamily.InterNetwork:
                    lock (ipv4SocketPool)
                    {
                        ipv4SocketPool ??= new Queue<Socket>();
                        ipv4SocketPool.Enqueue(value);
                    }
                    return;
                case AddressFamily.InterNetworkV6:
                    lock (ipv6SocketPool)
                    {
                        ipv6SocketPool ??= new Queue<Socket>();
                        ipv6SocketPool.Enqueue(value);
                    }
                    return;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value.AddressFamily));
            }
        }
    }
}