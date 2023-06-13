using NetcodeIO.NET.Core;
using NetcodeIO.NET.Utils.Crypto;

namespace NetcodeIO.NET.Utils.IO
{
    public static class PacketIO
    {
        public static int EncryptPrivateConnectToken(Span<byte> sourceSpan, Span<byte> aedSpan, Span<byte> nonceSpan, byte[] privateKey)
        {
            ByteBufferPool.GetBuffer(Defines.PRIVATE_TOKEN_SIZE, out var input);
            sourceSpan.CopyTo(input);
            
            ByteBufferPool.GetBuffer(Defines.AED_LENGTH, out var aed);
            aedSpan.CopyTo(aed);
            
            ByteBufferPool.GetBuffer(Defines.NONCE_SIZE, out var nonce);
            nonceSpan.CopyTo(nonce);
            
            ByteBufferPool.GetBuffer(Defines.PRIVATE_TOKEN_SIZE, out var output);
            var result = AeadChaCha20Poly1305.Encrypt(input, 0, Defines.PRIVATE_TOKEN_ENCRYPT_SIZE, aed, nonce, privateKey, output);
            output.CopyTo(sourceSpan);
            
            ByteBufferPool.ReturnBuffer(ref nonce);
            ByteBufferPool.ReturnBuffer(ref aed);
            ByteBufferPool.ReturnBuffer(ref input);
            
            return result;
        }
    }
}