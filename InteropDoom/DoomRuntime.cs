using System.Collections.Concurrent;
using System.Numerics;
using InteropDoom.Engine;
using InteropDoom.Input;

namespace InteropDoom;

public static partial class DoomRuntime
{
    public static ScreenBuffer Screen { get; private set; } = new();

    /// <summary>
    /// Whether the player has been initialized at least once.
    /// </summary>
    public static bool IsInitialized => _doomThread is { ThreadState: not ThreadState.Unstarted };
    /// <summary>
    /// Set to <see langword="true"/> when doom starts, and back to <see langword="false"/> when doom exits.
    /// </summary>
    public static bool IsStarted { get; private set; }
    public static DoomEngine Engine { get; set; } = new DefaultDoomEngine();

    public static void Initialize()
    {
        if (IsInitialized) return;

        _doomThread = new(StartDoom)
        {
            Name = "Doom",
            Priority = ThreadPriority.BelowNormal,
            IsBackground = true
        };
        _doomThread.Start();
    }

    internal static void RunOnMainThread(Action action)
    {
        if (Engine.Dispatcher.IsOnMainThread())
            action();
        else
            Engine.Dispatcher.PostToMainThread(action);
    }

    public static void OnKeyEvent(bool down, DoomKey key)
        => _keyQueue.Enqueue(new(down, key));

    private static readonly object _inputSync = new();
    private record struct KeyEvent(bool Down, DoomKey Key);
    // holding input events until the doom thread consumes them
    private static readonly ConcurrentQueue<KeyEvent> _keyQueue = [];

    public static void OnMouseMove(double deltaX, double deltaY)
    {
        lock (_inputSync)
        {
            _mouseState.DeltaX += deltaX;
            // Y axis controls forward/back movement which feels mega weird
            //_mouseState.DeltaY = y;
        }
    }
    public static void OnMouseScroll(double delta)
    {
        lock (_inputSync)
        {
            _mouseState.WheelDelta += delta;
        }
    }

    public static void UpdateMouseButtons(MouseButtons buttons)
    {
        lock (_inputSync)
        {
            _mouseState.Buttons = buttons;
        }
    }

    private static readonly MouseState _mouseState = new();
}
