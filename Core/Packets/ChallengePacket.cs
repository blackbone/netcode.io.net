using System.Runtime.InteropServices;
using NetcodeIO.NET.Utils.IO;

namespace NetcodeIO.NET.Core.Requests
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal unsafe struct ChallengePacket
    {
        public const int SIZE = PacketHeader.SIZE + 8 + Defines.CHALLENGE_ENCRYPTED_SIZE;
        
        public PacketHeader Header;
        public ulong ChallengeSequence;
        private fixed byte encryptedChallengeData[Defines.CHALLENGE_ENCRYPTED_SIZE];

        public Span<byte> EncryptedChallengeData
        {
            get
            {
                fixed (byte* p = encryptedChallengeData) return new Span<byte>(p, Defines.NONCE_SIZE);
            }
        }

        public ChallengePacket()
        {
            Header.PacketType = PacketType.Challenge;
            Header.PayloadFlags = PayloadFlags.None;
            Header.SequenceNumber = 0;
            Header.SequenceNumber = 0;
            ChallengeSequence = 0;
        }
        
        public ChallengePacket(ref PacketHeader header)
        {
            Header.PacketType = header.PacketType;
            Header.SequenceNumber = header.SequenceNumber;
            Header.PayloadFlags = header.PayloadFlags;
            Header.SequenceNumber = header.SequenceNumber;
            ChallengeSequence = 0;
        }

        public bool Read(ref ReaderWriter reader)
        {
            reader.Read(out ChallengeSequence);
            reader.Read(EncryptedChallengeData);
            
            return true;
        }
        
        public bool Write(ref ReaderWriter writer)
        {
            writer.Write(ChallengeSequence);
            writer.Write(EncryptedChallengeData);
            
            return true;
        }
    }
}