using System.Runtime.CompilerServices;

namespace UrlShortener.Core.Services.Interfaces
{
    public interface ILoggerManager
    {
        void LogTrace(string message, Exception? ex = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", params object[]? args);
        void LogDebug(string message, Exception? ex = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", params object[]? args);
        void LogInfo(string message, Exception? ex = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", params object[]? args);
        void LogWarn(string message, Exception? ex = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", params object[]? args);
        void LogError(string message, Exception? ex = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", params object[]? args);
        void LogCritical(string message, Exception? ex = null, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", params object[]? args);
    }

}
