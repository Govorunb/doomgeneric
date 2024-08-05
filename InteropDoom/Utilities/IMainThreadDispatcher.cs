namespace InteropDoom.Utilities;

/// <summary>
/// Dispatcher for controlling access to the main thread.
/// </summary>
public interface IMainThreadDispatcher
{
    /// <summary>
    /// Check if the current thread is the main thread.
    /// </summary>
    bool IsOnMainThread();
    /// <summary>
    /// Queue an <paramref name="action"/> to be executed on the main thread.
    /// </summary>
    void PostToMainThread(Action action);
}
