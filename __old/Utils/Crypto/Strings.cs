using System.Text;

namespace NetcodeIO.NET.Utils.Crypto
{
    /// <summary> General string utilities.</summary>
    public abstract class Strings
    {
        public static string FromAsciiByteArray(in byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes, 0, bytes.Length);
        }

        public static byte[] ToAsciiByteArray(in string s)
        {
            return Encoding.ASCII.GetBytes(s);
        }
    }
}