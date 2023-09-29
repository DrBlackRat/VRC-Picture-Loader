using UnityEngine;

namespace DrBlackRat
{
    public static class PLDebug
    {
        private const string logPrefix = "[<color=#8063cf>Picture Loader</color>]";
        private const string liteLogPrefix = "[<color=#637ecf>Picture Loader Lite</color>]";

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
    }
}