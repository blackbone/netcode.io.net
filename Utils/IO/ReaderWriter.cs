using System.Text;
using NetcodeIO.NET.Core.Token;

namespace NetcodeIO.NET.Utils.IO
{
    internal unsafe struct ReaderWriter : IDisposable
    {
        private byte* payload;
        private int size;
        private int position;
        
        public ReaderWriter(byte* payload, int size, int position = 0)
        {
            this.payload = payload;
            this.size = size;
            this.position = position;
        }

        public ReaderWriter(byte[] payload, int position = 0)
        {
            this.size = payload.Length;
            this.position = position;
            fixed (byte* p = &payload[position])
                this.payload = p;
        }


        public void Skip(int count) => position += count;
        
        public void Read(out NetcodeAddressType value) => value = (NetcodeAddressType)payload[position++];
        public void Write(in NetcodeAddressType value) => payload[position++] = (byte)value;
        
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
            for (var i = 0; i < sizeof(short); i++)
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
            for (var i = 0; i < target.Length; i++)
                target[i] = payload[position++];
        }
        
        public void Write(in Span<byte> target)
        {
            for (var i = 0; i < target.Length; i++)
                payload[position++] = target[i];
        }

        public void Read(byte* target, int count)
        {
            for (var i = 0; i < count; i++)
                target[i] = payload[position++];
        }
        
        public void Write(byte* target, int count)
        {
            for (var i = 0; i < count; i++)
                payload[position++] = target[i];
        }

        public void Read(out string value, int length)
        {
            value = Encoding.ASCII.GetString(&payload[position], length);
            position += length;
        }

        public void Write(in string value)
        {
            foreach (var c in value)
                payload[position++] = (byte)(c & 0xFF);
        } 
        
        public void Dispose()
        {
            payload = null;
            size = 0;
            position = 0;
        }
    }
}