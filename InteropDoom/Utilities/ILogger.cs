namespace InteropDoom.Utilities;
public interface ILogger
{
    void LogVerbose(string message);
    void LogDebug(string message);
    void LogInfo(string message);
    void LogWarning(string message);
    void LogError(string message);
    void LogFatal(string message);
}
