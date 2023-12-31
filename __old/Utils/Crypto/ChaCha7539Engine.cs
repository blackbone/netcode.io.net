﻿namespace NetcodeIO.NET.Utils.Crypto
{
    /// <summary>
    /// Implementation of Daniel J. Bernstein's ChaCha stream cipher.
    /// </summary>
    internal sealed  class ChaCha7539Engine
        : Salsa20Engine
    {
        /// <summary>
        /// Creates a 20 rounds ChaCha engine.
        /// </summary>
        public ChaCha7539Engine()
        {
        }

        public override string AlgorithmName => "ChaCha7539" + Rounds;

        protected override int NonceSize => 12;

        protected override void AdvanceCounter()
        {
            if (++EngineState[12] == 0)
                throw new InvalidOperationException("attempt to increase counter past 2^32.");
        }

        protected override void ResetCounter()
        {
            EngineState[12] = 0;
        }

        protected override void SetKey(byte[] keyBytes, byte[] ivBytes)
        {
            if (keyBytes != null)
            {
                if (keyBytes.Length != 32)
                    throw new ArgumentException(AlgorithmName + " requires 256 bit key");

                PackTauOrSigma(keyBytes.Length, EngineState, 0);

                // Key
                Pack.LE_To_UInt32(keyBytes, 0, EngineState, 4, 8);
            }

            // IV
            Pack.LE_To_UInt32(ivBytes, 0, EngineState, 13, 3);
        }

        protected override void GenerateKeyStream(byte[] output)
        {
            ChaChaEngine.ChachaCore(Rounds, EngineState, X);
            Pack.UInt32_To_LE(X, output, 0);
        }
    }
}