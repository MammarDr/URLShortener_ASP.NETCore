using NLog;
using System.Runtime.CompilerServices;
using UrlShortener.Core.Services.Interfaces;

namespace UrlShortener.Core.Services.Implementations
{
    public class LoggerManager : ILoggerManager
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public void LogTrace(string message,
                            Exception? ex = null,
                            [CallerMemberName] string memberName = "",
                            [CallerFilePath] string filePath = "",
                            params object[]? args) =>
            LogWithCallerInfo(LogLevel.Trace, message, ex, args, memberName, filePath);

        public void LogDebug(string message,
                            Exception? ex = null,
                            [CallerMemberName] string memberName = "",
                            [CallerFilePath] string filePath = "",
                            params object[]? args) =>
            LogWithCallerInfo(LogLevel.Debug, message, ex, args, memberName, filePath);

        public void LogInfo(string message,
                            Exception? ex = null,
                            [CallerMemberName] string memberName = "",
                            [CallerFilePath] string filePath = "",
                            params object[]? args) =>
            LogWithCallerInfo(LogLevel.Info, message, ex, args, memberName, filePath);

        public void LogError(string message,
                            Exception? ex = null,
                            [CallerMemberName] string memberName = "",
                            [CallerFilePath] string filePath = "",
                            params object[]? args) =>
            LogWithCallerInfo(LogLevel.Error, message, ex, args, memberName, filePath);

        public void LogWarn(string message,
                            Exception? ex = null, 
                            [CallerMemberName] string memberName = "",
                            [CallerFilePath] string filePath = "",
                            params object[]? args)
            => LogWithCallerInfo(LogLevel.Warn, message, ex, args, memberName, filePath);

        public void LogCritical(string message,
                            Exception? ex = null,
                            [CallerMemberName] string memberName = "",
                            [CallerFilePath] string filePath = "",
                            params object[]? args)
            => LogWithCallerInfo(LogLevel.Fatal, message, ex, args, memberName, filePath);

        private void LogWithCallerInfo(
            LogLevel level,
            string message,
            Exception? ex,
            object[]? args,
            string memberName = "",
            string filePath = ""
        )
        {
            // Extract just the class name from file path
            var className = Path.GetFileNameWithoutExtension(filePath);
            var caller = $"{className}.{memberName}"; // e.g. PlanService.GetByIDs

            var logEvent = new LogEventInfo(level, caller, message)
            {
                Exception = ex,
                Parameters = args
            };

            // Still tell NLog that LoggerManager is a wrapper
            logger.Log(typeof(LoggerManager), logEvent);
        }


    }

}
