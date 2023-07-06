#if !UNITY
using System.Net;
using Netcode.io.IO;
using Netcode.io.Tokens;

namespace Netcode.io
{
    public sealed class TokenFactory
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
        /// <param name="keys">Pairs of client / server encryption keys</param>
        /// <param name="clientId">The unique ID to assign to the client consuming this token</param>
        /// <param name="userData">Up to 256 bytes of arbitrary user data</param>
        /// <returns>2048 byte connect token to send to client</returns>
        public byte[] GenerateConnectToken(IPEndPoint[] addressList, byte[][] keys, ulong clientId, int expirySeconds = 10, uint serverTimeout = 5, ulong sequence = 1UL, byte[] userData = null)
        {
            var result = new byte[Constants.PublicTokenMaxSize];
            GenerateConnectToken(result, addressList, keys, clientId, expirySeconds, serverTimeout, sequence, userData);
            return result;
        }

        /// <summary>
        /// Generate a new public connect token
        /// </summary>
        /// <param name="result">Where to write result token.</param>
        /// <param name="addressList">The list of public server addresses in this connect token</param>
        /// <param name="expirySeconds">The number of seconds until this token expires</param>
        /// <param name="serverTimeout">Server response time timeout</param>
        /// <param name="sequence">The token sequence number of this token</param>
        /// <param name="keys">Pairs of client / server encryption keys</param>
        /// <param name="clientId">The unique ID to assign to the client consuming this token</param>
        /// <param name="userData">Up to 256 bytes of arbitrary user data</param>
        /// <returns>2048 byte connect token to send to client</returns>
        public void GenerateConnectToken(byte[] result, IPEndPoint[] addressList, byte[][] keys, ulong clientId, int expirySeconds = 10, uint serverTimeout = 5, ulong sequence = 1UL, byte[] userData = null)
        {
            if (result == null) throw new NullReferenceException("Result array can not be null");
            if (result.Length > Constants.PublicTokenMaxSize) throw new ArgumentOutOfRangeException(nameof(result), $"Must be exactly {Constants.PublicTokenMaxSize} bytes long.");
            if (userData?.Length > Constants.UserDataSize) throw new ArgumentOutOfRangeException(nameof(userData));
            if (addressList == null) throw new NullReferenceException("Address list cannot be null");
            if (keys == null) throw new NullReferenceException("keys list cannot be null");
            if (addressList.Length == 0) throw new ArgumentOutOfRangeException(nameof(addressList));
            if (keys.Length == 0) throw new ArgumentOutOfRangeException(nameof(keys));
            if (addressList.Length > Constants.MaxServers) throw new ArgumentOutOfRangeException("Address list cannot contain more than " + 32 + " entries");
            if (keys.Length > Constants.MaxServers) throw new ArgumentOutOfRangeException("keys list cannot contain more than " + 32 + " entries");
            if (addressList.Length != keys.Length) throw new ArgumentOutOfRangeException("Address list cannot contain more than " + 32 + " entries");

            var publicToken = new PublicToken
            {
                ProtocolId = protocolId,
                Created = (ulong)DateTimeOffset.Now.UtcTicks,
                Expire = (ulong)(DateTimeOffset.Now + TimeSpan.FromSeconds(expirySeconds)).UtcTicks,
                ClientId = clientId,
                Servers = new ServerEntry[addressList.Length]
            };
            
            var numServers = addressList.Length;
            for (var i = 0; i < numServers; i++)
            {
                var clientKey = keys[i][..15];
                var serverKey = keys[i][16..];
                publicToken.Servers[i].EndPoint = addressList[i];
                publicToken.Servers[i].ClientKey = clientKey;
                GeneratePrivateToken(ref publicToken.Servers[i].PrivateTokenData, protocolId, clientId, publicToken.Created, publicToken.Expire, publicToken.Servers[i].EndPoint, privateKey, serverKey);
            }

            var rw = new ReaderWriter(result);
            publicToken.Write(ref rw);
        }
        
        private void GeneratePrivateToken(ref byte[] result, ulong protocolId, ulong clientId, ulong created, ulong expire, in Tokens.EndPoint endPoint, byte[] privateKey, byte[] serverKey)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));
            if (result.Length > Constants.PrivateKeyMaxSize) throw new ArgumentException( $"Must be up to {Constants.PrivateKeyMaxSize} bytes long.", nameof(result));

            // if (clientKey == null) throw new ArgumentNullException(nameof(clientKey));
            // if (clientKey.Length <= Constants.KeySize) throw new ArgumentException( $"Must be up to {Constants.KeySize} bytes long.", nameof(clientKey));

            var privateToken = new PrivateToken
            {
                ProtocolId = protocolId,
                ClientId = clientId,
                Created = created,
                Expire = expire,
                EndPoint = endPoint,
            };

            var rw = new ReaderWriter(result);
            privateToken.Write(ref rw);

            // TODO [Dmitrii Osipov] encrypt private token data
        }
    }
}
#endif