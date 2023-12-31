namespace Netcode.io.Logging
{
    /// <summary>
    /// Helper class for logging
    /// </summary>
    public static class NetworkLog
    {
        private static readonly object netObj = new();
        
        /// <summary>
        /// Gets the current log level.
        /// </summary>
        /// <value>The current log level.</value>
        public static LogLevel CurrentLogLevel => LogLevel.Developer;

        // internal logging

        /// <summary>
        /// Locally logs a info log with Netcode prefixing.
        /// </summary>
        /// <param name="message">The message to log</param>
        public static void LogInfo(string message) => netObj.Log($"[Netcode] {message}");

        /// <summary>
        /// Locally logs a warning log with Netcode prefixing.
        /// </summary>
        /// <param name="message">The message to log</param>
        public static void LogWarning(string message) => netObj.LogWarning($"[Netcode] {message}");

        /// <summary>
        /// Locally logs a error log with Netcode prefixing.
        /// </summary>
        /// <param name="message">The message to log</param>
        public static void LogError(string message) => netObj.LogError($"[Netcode] {message}");

        /// <summary>
        /// Logs an info log locally and on the server if possible.
        /// </summary>
        /// <param name="message">The message to log</param>
        public static void LogInfoServer(string message) => LogServer(message, LogType.Info);

        /// <summary>
        /// Logs a warning log locally and on the server if possible.
        /// </summary>
        /// <param name="message">The message to log</param>
        public static void LogWarningServer(string message) => LogServer(message, LogType.Warning);

        /// <summary>
        /// Logs an error log locally and on the server if possible.
        /// </summary>
        /// <param name="message">The message to log</param>
        public static void LogErrorServer(string message) => LogServer(message, LogType.Error);

        private static void LogServer(string message, LogType logType)
        {
            // Get the sender of the local log
            ulong localId = 0;

            switch (logType)
            {
                case LogType.Info: LogInfoServerLocal(message, localId); break;
                case LogType.Warning: LogWarningServerLocal(message, localId); break;
                case LogType.Error: LogErrorServerLocal(message, localId); break;
            }

            // if (NetworkManager.Singleton != null && !NetworkManager.Singleton.IsServer && NetworkManager.Singleton.NetworkConfig.EnableNetworkLogs)
            // {
            //     var networkMessage = new ServerLogMessage
            //     {
            //         LogType = logType,
            //         Message = message
            //     };
            //     var size = NetworkManager.Singleton.SendMessage(ref networkMessage, NetworkDelivery.ReliableFragmentedSequenced, NetworkManager.ServerClientId);
            //
            //     NetworkManager.Singleton.NetworkMetrics.TrackServerLogSent(NetworkManager.ServerClientId, (uint)logType, size);
            // }
        }

        internal static void LogInfoServerLocal(string message, ulong sender) => netObj.Log($"[Netcode-Server Sender={sender}] {message}");
        internal static void LogWarningServerLocal(string message, ulong sender) => netObj.LogWarning($"[Netcode-Server Sender={sender}] {message}");
        internal static void LogErrorServerLocal(string message, ulong sender) => netObj.LogError($"[Netcode-Server Sender={sender}] {message}");

        internal enum LogType : byte
        {
            Info,
            Warning,
            Error,
            None
        }
    }
}
