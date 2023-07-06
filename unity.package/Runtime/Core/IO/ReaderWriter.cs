using System;
using System.Text;
using Netcode.io.Tokens;

namespace Netcode.io.IO
{
    internal ref partial struct ReaderWriter
    {
        private readonly Span<byte> payload;
        private int position;

        public int Position => position;
        public int Size => payload.Length;
        public int Remaining => Size - Position;

        /// <summary>
        /// Part of data related to position
        /// </summary>
        public Span<byte> Data => payload[..position];

        public ReaderWriter(Span<byte> payload, int position = 0)
        {
            this.payload = payload;
            this.position = position;
        }
        
        public ReaderWriter(byte[] payload, int position = 0)
        {
            this.payload = payload;
            this.position = position;
        }
        
        public void Skip(uint count) => position += (int)count;
        public void Reset() => position = 0;
        
        public void Read(out AddressType value) => value = (AddressType)payload[position++];
        public void Write(in AddressType value) => payload[position++] = (byte)value;
        
        public void Read(out byte value) => value = payload[position++];
        public void Write(in byte value) => payload[position++] = value;

        public void Read(out ushort value)
        {
            value = 0;
            for (var i = 0; i < sizeof(ushort); i++)
                value |= (ushort)(payload[position++] << (i << 3));
        }
        
        public void Write(ushort value)
        {
            for (var i = 0; i < sizeof(ushort); i++)
            {
                payload[position++] = (byte)(value & 0xFF);
                value >>= 8;
            }
        }
        
        public void Read(out short value)
        {
            value = 0;
            for (var i = 0; i < sizeof(short); i++)
                value |= (short)(payload[position++] << (i << 3));
        }
        public void Write(short value)
        {
            for (var i = 0; i < sizeof(short); i++)
            {
                payload[position++] = (byte)(value & 0xFF);
                value >>= 8;
            }
        }
        
        public void Read(out uint value)
        {
            value = 0;
            for (var i = 0; i < sizeof(uint); i++)
                value |= (uint)payload[position++] << (i << 3);
        }
        public void Write(uint value)
        {
            for (var i = 0; i < sizeof(uint); i++)
            {
                payload[position++] = (byte)(value & 0xFF);
                value >>= 8;
            }
        }
        
        public void Read(out int value)
        {
            value = 0;
            for (var i = 0; i < sizeof(int); i++)
                value |= payload[position++] << (i << 3);
        }
        
        public void Write(int value)
        {
            for (var i = 0; i < sizeof(int); i++)
            {
                payload[position++] = (byte)(value & 0xFF);
                value >>= 8;
            }
        }
        
        public void Read(out ulong value)
        {
            value = 0;
            for (var i = 0; i < sizeof(ulong); i++)
                value |= (ulong)payload[position++] << (i << 3);
        }
        
        public void Write(ulong value)
        {
            for (var i = 0; i < sizeof(ulong); i++)
            {
                payload[position++] = (byte)(value & 0xFF);
                value >>= 8;
            }
        }
        
        public void Read(out long value)
        {
            value = 0;
            for (var i = 0; i < sizeof(long); i++)
                value |= (long)payload[position++] << (i << 3);
        }
        
        public void Write(long value)
        {
            for (var i = 0; i < sizeof(long); i++)
            {
                payload[position++] = (byte)(value & 0xFF);
                value >>= 8;
            }
        }
        
        public void Read(in Span<byte> target)
        {
            payload[new Range(position, position + target.Length)].CopyTo(target[new Range(0, target.Length)]);
            position += target.Length;
        }
        
        public void Write(in Span<byte> target)
        {
            target[new Range(0, target.Length)].CopyTo(payload[new Range(position, position + target.Length)]);
            position += target.Length;
        }

        public void Read(Span<byte> target, int count)
        {
            payload[new Range(position, position + count)].CopyTo(target[new Range(0, count)]);
            position += count;
        }
        
        public void Write(Span<byte> target, int count)
        {
            target[new Range(0, count)].CopyTo(payload[new Range(position, position + count)]);
            position += count;
        }

        public void Read(out string value, int length)
        {
            value = Encoding.ASCII.GetString(payload[new Range(position, position + length)]);
            position += length;
        }

        public void Write(in string value)
        {
            foreach (var c in value)
                payload[position++] = (byte)(c & 0xFF);
        }
    }
}