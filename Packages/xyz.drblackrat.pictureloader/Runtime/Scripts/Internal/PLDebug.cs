using UnityEngine;

namespace DrBlackRat.VRC.PictureLoader
{
    public static class PLDebug
    {
        private const string logPrefix = "[<color=#8063cf>Picture Loader</color>]";
        private const string liteLogPrefix = "[<color=#637ecf>Picture Loader Lite</color>]";
        private const string urlLogPrefix = "[<color=#63bfcf>Picture Loader URL Input</color>]";
        private const string persistenceLogPrefix = "[<color=#63cf9d>Picture Loader Persistence</color>]";

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
        // Persistence
        public static void PersistenceLog(object message)
        {
            Debug.Log($"{persistenceLogPrefix} {message}");
        }
        public static void PersistenceLogWarning(object message)
        {
            Debug.LogWarning($"{persistenceLogPrefix} {message}");
        }
        public static void PersistenceLogError(object message)
        {
            Debug.LogError($"{persistenceLogPrefix} {message}");
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