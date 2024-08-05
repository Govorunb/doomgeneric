using System.Diagnostics;

namespace InteropDoom.Utilities;

internal class TraceLogger : ILogger
{
    private static void Log(string level, string message)
        => Trace.WriteLine($"[DOOM | {level}] {message}");

    public void LogVerbose(string message) => Log("Verbose", message);
    public void LogDebug(string message) => Log("Debug", message);
    public void LogInfo(string message) => Log("Information", message);
    public void LogWarning(string message) => Log("Warning", message);
    public void LogError(string message) => Log("Error", message);
    public void LogFatal(string message) => Log("Fatal", message);

    public static TraceLogger Instance { get; } = new();
    private TraceLogger() { }
}
