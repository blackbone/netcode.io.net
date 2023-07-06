namespace NetcodeIO.NET.Utils.Crypto
{
    internal class Check
    {
        internal static void DataLength(byte[] buf, int off, int len, string msg)
        {
            if (off + len > buf.Length)
                throw new Exception(msg);
        }

        internal static void OutputLength(byte[] buf, int off, int len, string msg)
        {
            if (off + len > buf.Length)
                throw new Exception(msg);
        }
    }
}