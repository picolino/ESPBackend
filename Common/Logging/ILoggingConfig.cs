
namespace Common.Logging
{
    public interface ILoggingConfig
    {
        LogLvl LogLevel { get; }
        string NlogConfigFileName { get; }
    }
}