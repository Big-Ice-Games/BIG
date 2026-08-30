namespace BIG
{
    public enum LogLevel : byte { Editor, Debug, Warning, Error }

    public interface ILogger
    {
        void Log(
            object sender,
            string message,
            LogLevel logLevel = LogLevel.Debug,
            bool withStackTrace = false,
            bool withTime = false);
    }

    public static class Logger
    {
        private static ILogger? _instance;
        private static bool _active;

        internal static void InitLogger(ILogger? logger)
        {
            _instance = logger;
            SetActive(logger != null);
        }

        public static void SetActive(bool active) => _active = active;

        public static void LogEditor(this object sender, string message, bool withStackTrace = false, bool withTime = false)
            => Log(sender, message, LogLevel.Editor, withStackTrace, withTime);
        public static void LogWarning(this object sender, string message, bool withStackTrace = false, bool withTime = false)
            => Log(sender, message, LogLevel.Warning, withStackTrace, withTime);

        public static void LogError(this object sender, string message, bool withStackTrace = false, bool withTime = false)
            => Log(sender, message, LogLevel.Error, withStackTrace, withTime);

        public static void Log(
            this object sender,
            string message,
            LogLevel logLevel = LogLevel.Debug,
            bool withStackTrace = false,
            bool withTime = false)
        {
            if (_active)
                _instance?.Log(sender, message, logLevel, withStackTrace, withTime);
        }
    }
}