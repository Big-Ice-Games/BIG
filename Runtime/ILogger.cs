#region license

// Copyright (c) 2025, Big Ice Games
// All rights reserved.

#endregion

namespace BIG
{
    public enum LogLevel
    {
        Info,
        Debug,
        Warning,
        Error,
        ToDo,
        Editor,
        NetworkInfo,
        NetworkError
    }

    /// <summary>
    /// This logger is implemented by end user like Unity Logger implementation.
    /// </summary>
    public interface ILogger
    {
        void Log(
            object sender,
            string message,
            LogLevel logLevel = LogLevel.Debug,
            bool withStackTrace = false,
            bool withTime = false);
    }

    /// <summary>
    /// This logger is and extension method that can be used by every instance or called on static functions arguments.
    /// It simplifies logging to this.Log(...) calls.
    /// </summary>
    public static class Logger
    {
        private static ILogger _instance;
        private static ILogger LOGGER = _instance ??= God.Ask().WithLogger(new UnityLogger()).CreateWorld().Ask<ILogger>();
        
        internal static void InitLogger(ILogger logger)
        {
            _instance = logger;
        }

        public static void Log(
            this object sender,
            string message,
            LogLevel logLevel = LogLevel.Debug,
            bool withStackTrace = false,
            bool withTime = false)
        {
            LOGGER?.Log(sender, message, logLevel, withStackTrace, withTime);
        }
    }
}