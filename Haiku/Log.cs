namespace Haiku
{
    public enum LogLevel
    {
        Debug,
        Trace,
        Warning,
        Error
    }

    public static class Log
    {
        public static Logging Logger = new NullLogger();
        public static LogLevel Level = LogLevel.Debug;
        public static uint Flags = 0x00000001;

        public static void Debug(string message, uint flags = 0x00000001)
        {
#if DEBUG
            if (Level == LogLevel.Debug
                && (flags & Flags) != 0)
            {
                Logger.WriteLine(message);
            }
#endif
        }

        public static void Trace(string message, uint flags = 0x00000001)
        {
            if (Level <= LogLevel.Trace
                && (flags & Flags) != 0)
            {
                Logger.WriteLine(message);
            }
        }

        public static void Warning(string message, uint flags = 0x00000001)
        {
            if (Level <= LogLevel.Warning
                && (flags & Flags) != 0)
            {
                Logger.WriteLine(message);
            }
        }

        public static void Error(string message, uint flags = 0x00000001)
        {
            if (Level <= LogLevel.Error
                && (flags & Flags) != 0)
            {
                Logger.WriteLine(message);
            }
        }
    }
}
