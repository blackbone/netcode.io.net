using System.Buffers;
using System.Net;
using NetcodeIO.NET.Core;
using NetcodeIO.NET.Core.Token;
using NetcodeIO.NET.Utils;
using NetcodeIO.NET.Utils.IO;

namespace NetcodeIO.NET
{
    /// <summary>
    /// Helper class for generating connect tokens
    /// </summary>
    public class TokenFactory
    {
        private readonly ulong protocolId;
        private readonly byte[] privateKey;

        public TokenFactory(ulong protocolId, byte[] privateKey)
        {
            this.protocolId = protocolId;
            this.privateKey = privateKey;
        }

        /// <summary>
        /// Generate a new public connect token
        /// </summary>
        /// <param name="addressList">The list of public server addresses in this connect token</param>
        /// <param name="expirySeconds">The number of seconds until this token expires</param>
        /// <param name="serverTimeout">Server response time timeout</param>
        /// <param name="sequence">The token sequence number of this token</param>
        /// <param name="clientId">The unique ID to assign to the client consuming this token</param>
        /// <param name="userData">Up to 256 bytes of arbitrary user data</param>
        /// <returns>2048 byte connect token to send to client</returns>
        public byte[] GenerateConnectToken(IPEndPoint[] addressList, ulong clientId, int expirySeconds = 10, uint serverTimeout = 5, ulong sequence = 1UL, byte[] userData = null)
        {
            if (userData?.Length > Defines.USER_DATA_SIZE) throw new ArgumentOutOfRangeException(nameof(addressList));
            if (addressList == null) throw new NullReferenceException("Address list cannot be null");
            if (addressList.Length == 0) throw new ArgumentOutOfRangeException(nameof(addressList));
            if (addressList.Length > Defines.MAX_SERVERS) throw new ArgumentOutOfRangeException("Address list cannot contain more than " + 32 + " entries");

            // start of creation Private Token
            var privateConnectToken = new PrivateNetcodeToken
            {
                ClientId = clientId,
                TimeoutSeconds = serverTimeout
            };

            unsafe
            {
                KeyUtils.GenerateKey(privateConnectToken.ClientToServerKey, Defines.KEY_SIZE);
                KeyUtils.GenerateKey(privateConnectToken.ServerToClientKey, Defines.KEY_SIZE);
                
                var len = userData?.Length ?? 0;
                for (var i = 0; i < len; i++)
                    // ReSharper disable once PossibleNullReferenceException
                    privateConnectToken.UserData[i] = userData[i];
            }

            for (var i = 0; i < addressList.Length; i++)
                privateConnectToken.SetServer(i, addressList[i]);
            // end of creation Private Token
            
            // start of creation Public Token
            var createTimestamp = (ulong)DateTimeOffset.Now.UtcTicks;
            var expireTimestamp = expirySeconds >= 0 ? createTimestamp + (ulong)expirySeconds : 0xFFFFFFFFFFFFFFFFUL;
            var publicConnectionToken = new PublicNetcodeToken
            {
                ProtocolId = protocolId,
                CreateTimestamp = createTimestamp,
                ExpireTimestamp = expireTimestamp,
                ConnectTokenSequence = sequence,
                NumServers = privateConnectToken.NumServers,
                TimeoutSeconds = serverTimeout
            };

            for (var i = 0; i < publicConnectionToken.NumServers; i++)
                publicConnectionToken.SetServer(i, privateConnectToken.GetServer(i));
            
            unsafe
            {
                KeyUtils.GenerateKey(publicConnectionToken.Nonce, Defines.NONCE_SIZE);
                KeyUtils.CopyKey(privateConnectToken.ClientToServerKey, publicConnectionToken.ClientToServerKey, Defines.KEY_SIZE);
                KeyUtils.CopyKey(privateConnectToken.ServerToClientKey, publicConnectionToken.ServerToClientKey,Defines.KEY_SIZE);
                
                // serialize private token in place directly into public token layout
                using (var privateTokeWriter = new ReaderWriter(publicConnectionToken.PrivateKeyData, Defines.PRIVATE_TOKEN_SIZE))
                {
                    var refWriter = privateTokeWriter;
                    privateConnectToken.Write(ref refWriter);
                }

                var source = new Span<byte>(publicConnectionToken.PrivateKeyData, Defines.PRIVATE_TOKEN_SIZE);
                var aed = new Span<byte>(publicConnectionToken.PrivateKeyData, Defines.AED_LENGTH);
                var nonce = new Span<byte>(publicConnectionToken.Nonce, Defines.NONCE_SIZE);
                
                // encrypt private token in place directly in public token field
                PacketIO.EncryptPrivateConnectToken(source, aed, nonce, privateKey);
            }

            var result = new byte[Defines.PUBLIC_TOKEN_SIZE];
            using var publicTokenWriter = new ReaderWriter(result);
            {
                var refWriter = publicTokenWriter;
                publicConnectionToken.Write(ref refWriter);
            }

            return result;
        }
    }
}