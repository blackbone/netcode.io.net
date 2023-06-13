namespace NetcodeIO.NET.Utils.Crypto
{
    /// <summary> General array utilities.</summary>
    public abstract class Arrays
    {
        /// <summary>
        /// A constant time equals comparison - does not terminate early if
        /// test will fail.
        /// </summary>
        /// <param name="a">first array</param>
        /// <param name="b">second array</param>
        /// <returns>true if arrays equal, false otherwise.</returns>
        public static bool ConstantTimeAreEqual(in byte[] a, in byte[] b)
        {
            var i = a.Length;
            if (i != b.Length)
                return false;
            var cmp = 0;
            while (i != 0)
            {
                --i;
                cmp |= a[i] ^ b[i];
            }

            return cmp == 0;
        }

        public static void Fill(in byte[] buf, in byte b)
        {
            var i = buf.Length;
            while (i > 0) buf[--i] = b;
        }
    }
}