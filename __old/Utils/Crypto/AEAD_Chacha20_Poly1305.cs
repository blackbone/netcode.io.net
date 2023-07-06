namespace NetcodeIO.NET.Utils.Crypto
{
    internal sealed class AeadChaCha20Poly1305
    {
        [ThreadStatic] private static byte[] buffer8;
        [ThreadStatic] private static byte[] buffer16;
        [ThreadStatic] private static byte[] buffer16_2;
        [ThreadStatic] private static byte[] buffer64;
        
        private static readonly byte[] Zeroes = new byte[15];
        private static readonly IMac Mac = new Poly1305();
        private static readonly object Mutex = new();
        private static ChaCha7539Engine _cipher;

        private static ParametersWithIv _tempParams;
        private static KeyParameter _encryptKey;
        private static KeyParameter _decryptKey;
        private static KeyParameter _macKey;

        public static int Encrypt(byte[] plaintext, int offset, int len, byte[] additionalData, byte[] nonce, byte[] key, byte[] outBuffer)
        {
            lock (Mutex)
            {
                if (_cipher == null)
                    _cipher = new ChaCha7539Engine();
                else
                    _cipher.Reset();

                if (_encryptKey == null)
                {
                    _encryptKey = new KeyParameter(key);
                }
                else
                {
                    _encryptKey.Reset();
                    _encryptKey.SetKey(key);
                }

                if (_tempParams == null)
                {
                    _tempParams = new ParametersWithIv(_encryptKey, nonce);
                }
                else
                {
                    _tempParams.Reset();
                    _tempParams.Set(_encryptKey, nonce);
                }

                _cipher.Init(_tempParams);

                buffer64 ??= new byte[64];
                var macKey = GenerateRecordMacKey(_cipher, buffer64);

                _cipher.ProcessBytes(plaintext, offset, len, outBuffer, 0);

                buffer16 ??= new byte[16];
                var macSize = CalculateRecordMac(macKey, additionalData, outBuffer, 0, len, buffer16);
                Array.Copy(buffer16, 0, outBuffer, len, macSize);

                return len + 16;
            }
        }

        public static int Decrypt(byte[] ciphertext, int offset, int len, byte[] additionalData, byte[] nonce, byte[] key, byte[] outBuffer)
        {
            lock (Mutex)
            {
                if (_cipher == null)
                    _cipher = new ChaCha7539Engine();
                else
                    _cipher.Reset();

                if (_decryptKey == null)
                {
                    _decryptKey = new KeyParameter(key);
                }
                else
                {
                    _decryptKey.Reset();
                    _decryptKey.SetKey(key);
                }

                if (_tempParams == null)
                {
                    _tempParams = new ParametersWithIv(_decryptKey, nonce);
                }
                else
                {
                    _tempParams.Reset();
                    _tempParams.Set(_decryptKey, nonce);
                }

                _cipher.Init(_tempParams);

                buffer64 ??= new byte[64];
                var macKey = GenerateRecordMacKey(_cipher, buffer64);

                var plaintextLength = len - 16;

                buffer16 ??= new byte[16];
                CalculateRecordMac(macKey, additionalData, ciphertext, offset, plaintextLength, buffer16);

                buffer16_2 ??= new byte[16];
                Array.Copy(ciphertext, offset + plaintextLength, buffer16_2, 0, buffer16_2.Length);

                var areEqual = Arrays.ConstantTimeAreEqual(buffer16, buffer16_2);

                if (!areEqual)
                    throw new Exception("bad_record_mac");

                _cipher.ProcessBytes(ciphertext, offset, plaintextLength, outBuffer, 0);
                return plaintextLength;
            }
        }

        private static KeyParameter GenerateRecordMacKey(IStreamCipher cipher, byte[] firstBlock)
        {
            cipher.ProcessBytes(firstBlock, 0, firstBlock.Length, firstBlock, 0);

            if (_macKey == null)
            {
                _macKey = new KeyParameter(firstBlock, 0, 32);
            }
            else
            {
                _macKey.Reset();
                _macKey.SetKey(firstBlock, 0, 32);
            }

            Arrays.Fill(firstBlock, 0);
            return _macKey;
        }

        private static int CalculateRecordMac(ICipherParameters macKey, byte[] additionalData, byte[] buf, int off, int len, byte[] outMac)
        {
            Mac.Reset();
            Mac.Init(macKey);

            UpdateRecordMacText(Mac, additionalData, 0, additionalData.Length);
            UpdateRecordMacText(Mac, buf, off, len);
            UpdateRecordMacLength(Mac, additionalData.Length);
            UpdateRecordMacLength(Mac, len);

            return MacUtilities.DoFinalOut(Mac, outMac);
        }

        private static void UpdateRecordMacLength(IMac mac, int len)
        {
            buffer8 ??= new byte[8];
            Pack.UInt64_To_LE((ulong)len, buffer8);
            mac.BlockUpdate(buffer8, 0, buffer8.Length);
        }

        private static void UpdateRecordMacText(IMac mac, byte[] buf, int off, int len)
        {
            mac.BlockUpdate(buf, off, len);

            var partial = len % 16;
            if (partial != 0) mac.BlockUpdate(Zeroes, 0, 16 - partial);
        }
    }
}