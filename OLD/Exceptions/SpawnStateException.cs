namespace Netcode.io.OLD.Exceptions
{
    /// <summary>
    /// Exception thrown when an object is not yet spawned
    /// </summary>
    public class SpawnStateException : Exception
    {
        /// <summary>
        /// Constructs a SpawnStateException
        /// </summary>
        public SpawnStateException() { }

        /// <summary>
        /// Constructs a SpawnStateException with a message
        /// </summary>
        /// <param name="message">The exception message</param>
        public SpawnStateException(string message) : base(message) { }

        /// <summary>
        /// Constructs a SpawnStateException with a message and a inner exception
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="inner">The inner exception</param>
        public SpawnStateException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>
    /// Exception thrown when a specified network channel is invalid
    /// </summary>
    public class InvalidChannelException : Exception
    {
        /// <summary>
        /// Constructs an InvalidChannelException with a message
        /// </summary>
        /// <param name="message">the message</param>
        public InvalidChannelException(string message) : base(message) { }
    }
}
