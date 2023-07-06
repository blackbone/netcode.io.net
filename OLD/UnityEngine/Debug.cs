namespace Netcode.io.OLD.UnityEngine
{
    public static class Debug
    {
        private static object debugObj = new();
        
        public static void Log(object message) => debugObj.Log(message);
        public static void LogWarning(object message) => debugObj.LogWarning(message);
        public static void LogError(object message) => debugObj.LogError(message);
        public static void LogException(Exception exception) => debugObj.LogException(exception);
    }
}