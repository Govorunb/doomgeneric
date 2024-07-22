using Avalonia.Logging;

namespace AvaloniaPlayer.Doom;

partial class DoomEngine
{
    private const string LOG_SOURCE = "DOOM";

    internal static void Log(LogEventLevel level, string message)
    {
        var logger = Logger.TryGet(level, LOG_SOURCE);
        logger?.Log(null, message);
    }

    internal static void LogDebug(string message) => Log(LogEventLevel.Debug, message);
    internal static void LogInfo(string message) => Log(LogEventLevel.Information, message);
    internal static void LogVerbose(string message) => Log(LogEventLevel.Verbose, message);
    internal static void LogWarning(string message) => Log(LogEventLevel.Warning, message);
    internal static void LogError(string message) => Log(LogEventLevel.Error, message);
    internal static void LogFatal(string message) => Log(LogEventLevel.Fatal, message);
}
