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

    public interface ILogger
    {
        void Log(object sender, string message, Category category = Category.Default, LogLevel logLevel = LogLevel.Debug, bool withStackTrace = false);
    }

    public static class Logger
    {
        private static ILogger _logger = null!;

        public static void InitLogger(ILogger logger)
        {
            _logger = logger;
        }

        public static void Log(this object sender, string message, Category category = Category.Default, LogLevel logLevel = LogLevel.Debug, bool withStackTrace = false)
        {
            _logger?.Log(sender, message, category, logLevel, withStackTrace);
        }
    }
}