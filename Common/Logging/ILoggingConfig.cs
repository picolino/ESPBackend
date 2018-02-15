namespace Shared.Logging
{
    public interface ILoggingConfig
    {
        LogLvl LogLevel { get; }
        string NlogConfigFileName { get; }
    }
}