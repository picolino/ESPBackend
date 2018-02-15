#region Usings

using System;

#endregion

namespace Common.Logging
{
    public interface ILogger
    {
        void Info(string sourceType, string sourceMethod, string message, params object[] args);
        void InfoWithIp(string sourceType, string sourceMethod, string message, params object[] args);
        void Warning(string sourceType, string sourceMethod, string message, params object[] args);
        void Error(string type, string method, Exception exception);
        void Error(string type, string method, Exception exception, string message);
        void Debug(string sourceType, string sourceMethod, string message, params object[] args);
    }
}