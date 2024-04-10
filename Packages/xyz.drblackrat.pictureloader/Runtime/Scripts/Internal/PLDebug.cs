using UnityEngine;

namespace DrBlackRat
{
    public static class PLDebug
    {
        private const string logPrefix = "[<color=#8063cf>Picture Loader</color>]";
        private const string liteLogPrefix = "[<color=#637ecf>Picture Loader Lite</color>]";
        private const string urlLogPrefix = "[<color=#49509c>Picture Loader URL Input</color>]";

        // Normal
        public static void Log(object message)
        {
            Debug.Log($"{logPrefix} {message}");
        }
        public static void LogWarning(object message)
        {
            Debug.LogWarning($"{logPrefix} {message}");
        }
        public static void LogError(object message)
        {
            Debug.LogError($"{logPrefix} {message}");
        }
        // Lite
        public static void LiteLog(object message)
        {
            Debug.Log($"{liteLogPrefix} {message}");
        }
        public static void LiteLogWarning(object message)
        {
            Debug.LogWarning($"{liteLogPrefix} {message}");
        }
        public static void LiteLogError(object message)
        {
            Debug.LogError($"{liteLogPrefix} {message}");
        }        
        // URL
        public static void UrlLog(object message)
        {
            Debug.Log($"{urlLogPrefix} {message}");
        }
        public static void UrlLogWarning(object message)
        {
            Debug.LogWarning($"{urlLogPrefix} {message}");
        }
        public static void UrlLogError(object message)
        {
            Debug.LogError($"{urlLogPrefix} {message}");
        }
    }
    // Enums
    public enum PLState
    {
        Waiting,
        Loading,
        Finished,
        Error,
    }
}