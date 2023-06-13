namespace NetcodeIO.NET.Utils.Crypto
{
    /// <summary>The interface stream ciphers conform to.</summary>
    public interface IStreamCipher
    {
        /// <summary>
        /// Process a block of bytes from <c>input</c> putting the result into <c>output</c>.
        /// </summary>
        /// <param name="input">The input byte array.</param>
        /// <param name="inOff">
        /// The offset into <c>input</c> where the data to be processed starts.
        /// </param>
        /// <param name="length">The number of bytes to be processed.</param>
        /// <param name="output">The output buffer the processed bytes go into.</param>
        /// <param name="outOff">
        /// The offset into <c>output</c> the processed data starts at.
        /// </param>
        void ProcessBytes(byte[] input, int inOff, int length, byte[] output, int outOff);
    }
}