namespace InteropDoom.Utilities;

internal class AssumeMainThread : IMainThreadDispatcher
{
    public bool IsOnMainThread() => true;
    public void PostToMainThread(Action action) => action();

    public static AssumeMainThread Instance { get; } = new();
    private AssumeMainThread() { }
}
