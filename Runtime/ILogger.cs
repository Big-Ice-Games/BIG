namespace BIG
{
    public enum LogLevel
    {
        Info,
        Debug,
        Warning,
        Error,
        ToDo,
        Editor
    }

    public enum Category
    {
        Default,
        Sound,
        UI,
        IO,
        Networking,
        Benchmark,
        Services,
        Gameplay
    }

    /// <summary>
    /// This logger is implemented by end user like Unity Logger implementation.
    /// </summary>
    public interface ILogger
    {
        void Log(object sender, string message, Category category = Category.Default, LogLevel logLevel = LogLevel.Debug, bool withStackTrace = false);
    }

    /// <summary>
    /// This logger is and extension method that can be used by every instance or called on static functions arguments.
    /// It simplify logging to this.Log(...) calls.
    /// </summary>
    public static class Logger
    {
        private static ILogger LOGGER = null!;

        internal static void InitLogger(ILogger logger)
        {
            LOGGER = logger;
        }

        public static void Log(this object sender, string message, Category category = Category.Default, LogLevel logLevel = LogLevel.Debug, bool withStackTrace = false)
        {
            LOGGER?.Log(sender, message, category, logLevel, withStackTrace);
        }

        public static void Log(this object sender, string message, LogLevel logLevel = LogLevel.Debug, bool withStackTrace = false)
        {
            LOGGER?.Log(sender, message, Category.Default, logLevel, withStackTrace);
        }
    }
}