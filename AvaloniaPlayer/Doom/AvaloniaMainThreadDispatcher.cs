using InteropDoom.Utilities;

namespace AvaloniaPlayer.Doom;
internal class AvaloniaMainThreadDispatcher : IMainThreadDispatcher
{
    public bool IsOnMainThread() => UIThread.CheckAccess();
    public void PostToMainThread(Action action) => UIThread.Post(action);
}
