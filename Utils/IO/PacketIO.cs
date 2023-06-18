using NetcodeIO.NET.Core;
using NetcodeIO.NET.Core.Token;
using NetcodeIO.NET.Utils.Crypto;

namespace NetcodeIO.NET.Utils.IO
{
    public static class PacketIO
    {
        [ThreadStatic] private static byte[] _input;
        [ThreadStatic] private static byte[] _output;
        [ThreadStatic] private static byte[] _aed;
        [ThreadStatic] private static byte[] _nonce;
        
        public static int EncryptPrivateConnectToken(Span<byte> sourceSpan, Span<byte> aedSpan, Span<byte> nonceSpan, byte[] privateKey)
        {
            _input ??= new byte[PrivateToken.SIZE];
            _output ??= new byte[PrivateToken.SIZE];
            _aed ??= new byte[Defines.AED_LENGTH];
            _nonce ??= new byte[Defines.NONCE_SIZE];
            
            sourceSpan.CopyTo(_input);
            aedSpan.CopyTo(_aed);
            nonceSpan.CopyTo(_nonce);
            
            var result = AeadChaCha20Poly1305.Encrypt(_input, 0, Defines.PRIVATE_TOKEN_ENCRYPT_SIZE, _aed, _nonce, privateKey, _output);
            _output.CopyTo(sourceSpan);
            return result;
        }
        
        public static int DecryptPrivateConnectToken(Span<byte> sourceSpan, Span<byte> aedSpan, Span<byte> nonceSpan, byte[] privateKey)
        {
            _input ??= new byte[PrivateToken.SIZE];
            _output ??= new byte[PrivateToken.SIZE];
            _aed ??= new byte[Defines.AED_LENGTH];
            _nonce ??= new byte[Defines.NONCE_SIZE];
            
            sourceSpan.CopyTo(_input);
            aedSpan.CopyTo(_aed);
            nonceSpan.CopyTo(_nonce);
            
            var result = AeadChaCha20Poly1305.Decrypt(_input, 0, Defines.PRIVATE_TOKEN_ENCRYPT_SIZE, _aed, _nonce, privateKey, _output);
            _output.CopyTo(sourceSpan);
            return result;
        }
    }
}