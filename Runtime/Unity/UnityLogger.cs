using System;
using UnityEngine;

namespace BIG
{
    public class UnityLogger : ILogger
    {
        public void Log(object sender,
            string message,
            LogLevel logLevel = LogLevel.Debug,
            bool withStackTrace = false,
            bool withTime = false)
        {
            string time = withTime ? $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]" : string.Empty;
            switch (logLevel)
            {
                case LogLevel.Editor:
#if UNITY_EDITOR
                    Debug.Log($"<color=cyan>{time}[{logLevel}] {sender} : {message}</color>");
#endif
                    break;
                case LogLevel.Debug:
#if UNITY_EDITOR || DEBUG
                    Debug.Log($"{time}[{logLevel}] {sender} : {message}");
#endif
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning($"{time}[{logLevel}] {sender} : {message}");
                    break;
                case LogLevel.Error:
                    Debug.LogError($"{time}[{logLevel}] {sender} : {message}");
                    break;
                default:
                    Debug.Log($"<color=green>{time}[{logLevel}] {sender} : {message}</color>");
                    break;
            }
        }
    }
}