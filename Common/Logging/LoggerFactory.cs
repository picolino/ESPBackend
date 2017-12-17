namespace Common.Logging
{
    public static class LoggerFactory
    {
        public static ILogger CreateLogger()
        {
            return new Logger(new LoggingConfig());
        }
    }
}