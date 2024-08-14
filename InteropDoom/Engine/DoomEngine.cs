using InteropDoom.Utilities;

namespace InteropDoom.Engine;

/// <summary>
/// Callbacks that need to be implemented for native interop.
/// </summary>
public abstract class DoomEngine
{
    protected internal abstract void OnInit(ScreenBuffer screen);
    protected internal abstract void OnExit(int exitCode);
    
    protected internal abstract void OnDrawFrame();
    protected internal abstract void OnWindowTitleChanged(string title);

    public TimeProvider TimeProvider { get; set; } = TimeProvider.System;
    public ILogger Logger { get; set; } = TraceLogger.Instance;
    public IMainThreadDispatcher Dispatcher { get; set; } = AssumeMainThread.Instance;
    public ISoundEngine? SoundEngine { get; set; }
    public IMusicEngine? MusicEngine { get; set; }
    public EventEngine? EventEngine { get; set; }
}

internal sealed class DefaultDoomEngine : DoomEngine
{
    protected internal override void OnDrawFrame()
    {
        //Logger.LogDebug(nameof(OnDrawFrame));
    }

    protected internal override void OnExit(int exitCode)
    {
        Logger.LogDebug($"{nameof(OnExit)}: {exitCode}");
    }

    protected internal override void OnInit(ScreenBuffer screen)
    {
        Logger.LogDebug($"{nameof(OnInit)}: {screen.Width}x{screen.Height}");
    }

    protected internal override void OnWindowTitleChanged(string title)
    {
        Logger.LogDebug($"{nameof(OnWindowTitleChanged)}: {title}");
    }
}
