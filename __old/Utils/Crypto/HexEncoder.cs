namespace NetcodeIO.NET.Utils.Crypto
{
    public class HexEncoder
        : IEncoder
    {
        private readonly byte[] encodingTable =
        {
            (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)'7',
            (byte)'8', (byte)'9', (byte)'a', (byte)'b', (byte)'c', (byte)'d', (byte)'e', (byte)'f'
        };

        /*
         * set up the decoding table.
         */
        private readonly byte[] decodingTable = new byte[128];

        private void InitialiseDecodingTable()
        {
            Arrays.Fill(decodingTable, (byte)0xff);

            for (int i = 0; i < encodingTable.Length; i++)
            {
                decodingTable[encodingTable[i]] = (byte)i;
            }

            decodingTable['A'] = decodingTable['a'];
            decodingTable['B'] = decodingTable['b'];
            decodingTable['C'] = decodingTable['c'];
            decodingTable['D'] = decodingTable['d'];
            decodingTable['E'] = decodingTable['e'];
            decodingTable['F'] = decodingTable['f'];
        }

        public HexEncoder()
        {
            InitialiseDecodingTable();
        }

        /**
        * encode the input data producing a Hex output stream.
        *
        * @return the number of bytes produced.
        */
        public int Encode(
            byte[] data,
            int off,
            int length,
            Stream outStream)
        {
            for (var i = off; i < off + length; i++)
            {
                int v = data[i];

                outStream.WriteByte(encodingTable[v >> 4]);
                outStream.WriteByte(encodingTable[v & 0xf]);
            }

            return length * 2;
        }

        private static bool Ignore(char c)
        {
            return c is '\n' or '\r' or '\t' or ' ';
        }

        /**
        * decode the Hex encoded byte data writing it to the given output stream,
        * whitespace characters will be ignored.
        *
        * @return the number of bytes produced.
        */
        public int Decode(
            byte[] data,
            int off,
            int length,
            Stream outStream)
        {
            var outLen = 0;
            var end = off + length;

            while (end > off)
            {
                if (!Ignore((char)data[end - 1]))
                {
                    break;
                }

                end--;
            }

            var i = off;
            while (i < end)
            {
                while (i < end && Ignore((char)data[i]))
                {
                    i++;
                }

                var b1 = decodingTable[data[i++]];

                while (i < end && Ignore((char)data[i]))
                {
                    i++;
                }

                var b2 = decodingTable[data[i++]];

                if ((b1 | b2) >= 0x80)
                    throw new IOException("invalid characters encountered in Hex data");

                outStream.WriteByte((byte)((b1 << 4) | b2));

                outLen++;
            }

            return outLen;
        }

        /**
        * decode the Hex encoded string data writing it to the given output stream,
        * whitespace characters will be ignored.
        *
        * @return the number of bytes produced.
        */
        public int DecodeString(
            string data,
            Stream outStream)
        {
            var length = 0;
            var end = data.Length;

            while (end > 0)
            {
                if (!Ignore(data[end - 1]))
                {
                    break;
                }

                end--;
            }

            var i = 0;
            while (i < end)
            {
                while (i < end && Ignore(data[i]))
                {
                    i++;
                }

                var b1 = decodingTable[data[i++]];

                while (i < end && Ignore(data[i]))
                {
                    i++;
                }

                var b2 = decodingTable[data[i++]];

                if ((b1 | b2) >= 0x80)
                    throw new IOException("invalid characters encountered in Hex data");

                outStream.WriteByte((byte)((b1 << 4) | b2));

                length++;
            }

            return length;
        }
    }
}