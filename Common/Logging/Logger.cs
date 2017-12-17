using System;
using System.Collections.Generic;
using System.IO;
using NLog;
using NLog.Config;

namespace Common.Logging
{
    public class Logger : ILogger
    {
        private readonly LogFactory loggerFactory;
        private readonly LogLvl desiredLogLevel;
        private const string DefaultSource = "Unknown";
        private readonly NLog.Logger variableLogger;
        private readonly NLog.Logger variableErrorLogger;

        public Logger(ILoggingConfig config)
        {
            desiredLogLevel = config.LogLevel;
            
            var normalizer = new PathNormalizer();
            var path = normalizer.Normalize(config.NlogConfigFileName);
            loggerFactory = new LogFactory(new XmlLoggingConfiguration(path));
            variableLogger = loggerFactory.GetLogger("messageLogger");
            variableErrorLogger = loggerFactory.GetLogger("errorLogger");
            
        }


        public void Debug(string sourceType, string sourceMethod, string message, params object[] args)
        {
            TraceMessageInternal(LogLvl.Debug, FormatMessageWithSource(sourceType, sourceMethod, message, args));
        }

        public void Info(string sourceType, string sourceMethod, string message, params object[] args)
        {
            TraceMessageInternal(LogLvl.Info, FormatMessageWithSource(sourceType, sourceMethod, message, args));
        }

        public void Warning(string sourceType, string sourceMethod, string message, params object[] args)
        {
            TraceMessageInternal(LogLvl.Warning, FormatMessageWithSource(sourceType, sourceMethod, message, args));
        }

        public void Error(string type, string method, Exception exception)
        {
            var source = FormatSource(type, method);
            TraceErrorInternal(exception, source: source);
        }

        public void Error(string type, string method, Exception exception, string message)
        {
            var source = FormatSource(type, method);
            TraceErrorInternal(exception, source: source, additionalMessage: message);
        }

        private void TraceMessageInternal(LogLvl level, string message)
        {
            if (level > desiredLogLevel)
            {
                return;
            }

            switch (level)
            {
                case LogLvl.Debug:
                    variableLogger?.Debug(message);
                    break;
                case LogLvl.Info:
                    variableLogger?.Info(message);
                    break;
                case LogLvl.Warning:
                    variableLogger?.Warn(message);
                    break;
                case LogLvl.Error:
                    variableErrorLogger?.Error(message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }

        private void TraceErrorInternal(Exception ex, string additionalMessage = null, string source = null,
                                        Dictionary<string, string> actionArguments = null,
                                        Dictionary<string, object> customProperties = null)
        {
            var eventInfo = new LogEventInfo
            {
                Exception = ex,
                Level = NLog.LogLevel.Error,
                LoggerName = variableErrorLogger.Name,
                Message = additionalMessage
            };
            if (!string.IsNullOrEmpty(source))
            {
                eventInfo.Properties.Add("Source", source);
            }
            if (actionArguments != null)
            {
                eventInfo.Properties.Add("ActionArguments", actionArguments);
            }
            if (customProperties != null)
            {
                foreach (var customProperty in customProperties)
                {
                    eventInfo.Properties.Add(customProperty.Key, customProperty.Value);
                }
            }
            if (ex?.Data.Count > 0)
            {
                foreach (var key in ex.Data.Keys)
                {
                    eventInfo.Properties.Add(key, ex.Data[key]);
                }
            }

            variableErrorLogger.Log(eventInfo);
        }
        private string FormatMessageWithSource(string sourceType, string sourceMethod, string message, params object[] args)
        {
            var sourceDeclaration = sourceMethod == null ? $"[{sourceType}]" : $"[{sourceType}/{sourceMethod}]";

            return $"{sourceDeclaration}: {string.Format(message, args)}";
        }

        private string FormatSource(string source1, string source2)
        {
            var checkedSource = DefaultSource;
            if (!string.IsNullOrEmpty(source1))
            {
                checkedSource = source1;
            }
            if (!string.IsNullOrEmpty(source2))
            {
                checkedSource = checkedSource + $" / {source2}";
            }

            checkedSource = $"[{checkedSource}]: ";
            return checkedSource;
        }
    }
}
