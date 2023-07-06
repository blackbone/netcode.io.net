namespace NetcodeIO.NET.Utils.Crypto
{
    /// <remarks>
    ///  Utility class for creating HMac object from their names/Oids
    /// </remarks>
    public static class MacUtilities
    {
        private static byte[] DoFinal(IMac mac)
        {
            var b = new byte[mac.GetMacSize()];
            mac.DoFinal(b, 0);
            return b;
        }

        public static int DoFinalOut(IMac mac, byte[] outBuffer)
        {
            mac.DoFinal(outBuffer, 0);
            return mac.GetMacSize();
        }

        public static byte[] DoFinal(IMac mac, byte[] input)
        {
            mac.BlockUpdate(input, 0, input.Length);
            return DoFinal(mac);
        }
    }
}