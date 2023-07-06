namespace NetcodeIO.NET
{
    /// <summary>
    /// Entire state machine
    /// </summary>
    internal enum State : sbyte
    {
        /// <summary>
        /// The connect token has expired
        /// </summary>
        ConnectTokenExpired = -6,

        /// <summary>
        /// The connect token is invalid
        /// </summary>
        InvalidConnectionToken = -5,

        /// <summary>
        /// Connection timed out while connected to a server
        /// </summary>
        ConnectionTimedOut = -4,

        /// <summary>
        /// Connection timed out while sending challenge response
        /// </summary>
        ChallengeResponseTimedOut = -3,

        /// <summary>
        /// Connection timed out while sending connection request
        /// </summary>
        ConnectionRequestTimedOut = -2,

        /// <summary>
        /// The connection request was denied by the server
        /// </summary>
        ConnectionDenied = -1,

        /// <summary>
        /// Client is not currently connected
        /// </summary>
        Disconnected = 0,

        /// <summary>
        /// Client is currently sending a connection request
        /// </summary>
        SendingConnectionRequest = 1,

        /// <summary>
        /// Client is currently sending a connection response to a server
        /// </summary>
        SendingChallengeResponse = 2,

        /// <summary>
        /// The client is connected to a server
        /// </summary>
        Connected = 3
    }

    /// <summary>
    /// State of a client object on server side
    /// </summary>
    public enum ServerState : sbyte
    {
        /// <summary>
        /// Client is not currently connected
        /// </summary>
        Disconnected = 0,

        /// <summary>
        /// Connection token accepted, now sending challenge request
        /// </summary>
        SendingChallengeRequest = 1,
        
        /// <summary>
        /// Challenge succeed, now waiting for keep alive packet from client to ensure that both sides now can send and receive messages.
        /// </summary>
        WaitingForKeepAliveVerification = 2,
        
        /// <summary>
        /// The client is connected to a server
        /// </summary>
        Connected = 3
    }
    
    
    /// <summary>
    /// State of a client object on client side
    /// </summary>
    public enum ClientState : sbyte
    {
        /// <summary>
        /// The connect token has expired
        /// </summary>
        ConnectTokenExpired = State.ConnectTokenExpired,

        /// <summary>
        /// The connect token is invalid
        /// </summary>
        InvalidConnectionToken = State.InvalidConnectionToken,

        /// <summary>
        /// Connection timed out while connected to a server
        /// </summary>
        ConnectionTimedOut = State.ConnectionTimedOut,

        /// <summary>
        /// Connection timed out while sending challenge response
        /// </summary>
        ChallengeResponseTimedOut = State.ChallengeResponseTimedOut,

        /// <summary>
        /// Connection timed out while sending connection request
        /// </summary>
        ConnectionRequestTimedOut = State.ConnectionRequestTimedOut,

        /// <summary>
        /// The connection request was denied by the server
        /// </summary>
        ConnectionDenied = State.ConnectionDenied,

        /// <summary>
        /// Client is not currently connected
        /// </summary>
        Disconnected = State.Disconnected,

        /// <summary>
        /// Client is currently sending a connection request
        /// </summary>
        SendingConnectionRequest = State.SendingConnectionRequest,

        /// <summary>
        /// Client is currently sending a connection response to a server
        /// </summary>
        SendingChallengeResponse = State.SendingChallengeResponse,

        /// <summary>
        /// The client is connected to a server
        /// </summary>
        Connected = State.Connected
    }
}