namespace NetcodeIO.NET.Utils.Crypto
{
    public class ParametersWithIv
        : ICipherParameters
    {
        private ICipherParameters parameters;
        private byte[] iv;

        public ParametersWithIv(ICipherParameters parameters, byte[] iv) : this(parameters, new Span<byte>(iv, 0, iv.Length))
        {
        }

        public ParametersWithIv(ICipherParameters parameters, Span<byte> iv)
        {
            // NOTE: 'parameters' may be null to imply key re-use
            if (iv == null)
                throw new ArgumentNullException(nameof(iv));

            this.parameters = parameters;
            this.iv = new byte[iv.Length];
            iv.CopyTo(this.iv);
        }

        public void Set(ICipherParameters parameters, Span<byte> iv)
        {
            this.parameters = parameters;
            ByteBufferPool.GetBuffer(iv.Length, out this.iv);
            iv.CopyTo(this.iv);
        }
        
        public void Set(ICipherParameters parameters, byte[] iv)
        {
            this.parameters = parameters;
            ByteBufferPool.GetBuffer(iv.Length, out this.iv);
            Array.Copy(iv, 0, this.iv, 0, this.iv.Length);
        }

        public void Reset()
        {
            ByteBufferPool.ReturnBuffer(ref iv);
            iv = null;
        }

        public byte[] GetIv() => iv;

        public ICipherParameters Parameters => parameters;
    }
}