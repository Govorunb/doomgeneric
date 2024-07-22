using System.Collections.Concurrent;
using Avalonia.Input;
using AvaloniaPlayer.Doom.Input;
using SkiaSharp;

namespace AvaloniaPlayer.Doom;

internal static partial class DoomEngine
{
    public static SKBitmap Screen { get; private set; } = new();
    public static PixelSize ScreenResolution { get; private set; }

    /// <summary>
    /// Whether the player has been initialized at least once.
    /// </summary>
    public static bool IsInitialized => _doomThread.ThreadState != System.Threading.ThreadState.Unstarted;
    /// <summary>
    /// Whether the game has started.
    /// </summary>
    [Reactive]
    public static bool IsStarted { get; private set; }
    public static string? WindowTitle { get; private set; }
    public static Action? WindowTitleChanged;

    internal static float StartupTime { get; private set; }
    internal static int LastExitCode { get; private set; }
    internal static int CurrentTick { get; private set; }

    public static void Initialize()
    {
        if (IsInitialized) return;

        _doomThread.Name = "Doom";
        _doomThread.Priority = ThreadPriority.BelowNormal;
        _doomThread.IsBackground = true;
        if (!IsOnMainThread())
            throw new InvalidOperationException("Doom must be initialized from the main thread");
        _doomThread.Start();
    }

    private static bool IsOnMainThread() => UIThread.CheckAccess();

    internal static void RunOnMainThread(Action action)
    {
        if (IsOnMainThread())
            action();
        else
            UIThread.Post(action);
    }

    public static void OnKeyEvent(bool down, PhysicalKey key)
    {
        DoomKey doomKey = key.ToDoomKey();
        _keyQueue.Enqueue(new(down, doomKey));
    }

    private static readonly object _inputSync = new();
    // holding input events until the doom thread consumes them
    private record struct KeyEvent(bool Down, DoomKey Key);
    private static readonly ConcurrentQueue<KeyEvent> _keyQueue = [];

    public static void OnMouseMove(Point delta)
    {
        // Y axis controls forward/back movement which feels mega weird
        _mouseDelta += delta.WithY(0);
    }

    public static void OnScroll(double delta)
    {
        _mouseWheelDelta += delta;
    }
    public static void UpdateMouseButtons(bool left, bool right, bool middle)
    {
        _mouseButtons[0] = left;
        _mouseButtons[1] = right;
        _mouseButtons[2] = middle;
    }

    private static Vector _mouseDelta;
    private static double _mouseWheelDelta;
    // index is MouseButton - 1
    private static bool[] _mouseButtons = new bool[3];
}
