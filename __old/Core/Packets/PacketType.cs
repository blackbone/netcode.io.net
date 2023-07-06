namespace NetcodeIO.NET.Core.Requests
{
    /// <summary>
    /// Packet type code
    /// </summary>
    internal enum PacketType : byte
    {
        /// <summary>
        /// Connection request (sent from client to server)
        /// </summary>
        ConnectionRequest = 0,

        /// <summary>
        /// Connection denied (sent from server to client)
        /// </summary>
        Denied = 1,

        /// <summary>
        /// Connection challenge (sent from server to client)
        /// </summary>
        Challenge = 2,

        /// <summary>
        /// Challenge response (sent from client to server)
        /// </summary>
        ChallengeResponse = 3,

        /// <summary>
        /// Connection keep-alive (sent by both client and server)
        /// </summary>
        KeepAlive = 4,

        /// <summary>
        /// Connection payload (sent by both client and server)
        /// </summary>
        Payload = 5,

        /// <summary>
        /// Connection disconnect (sent by both client and server)
        /// </summary>
        Disconnect = 6,

        /// <summary>
        /// Invalid packet
        /// </summary>
        InvalidPacket = 7
    }

    internal enum PayloadFlags : byte
    {
        None = 0b0000_0000,
        Ordered = 0b0000_0001,
        Reliable = 0b0000_0010
    }
}