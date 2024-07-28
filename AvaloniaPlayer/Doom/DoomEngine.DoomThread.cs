using AvaloniaPlayer.Doom.Audio;
using AvaloniaPlayer.Doom.Input;
using SkiaSharp;

namespace AvaloniaPlayer.Doom;

partial class DoomEngine
{
    private static readonly Stopwatch _gameClock = new();
    private static readonly Thread _doomThread = new(StartDoom);
    private static readonly ManualResetEventSlim _threadStop = new(false);
    private static readonly ManualResetEventSlim _runningEvent = new(true);

    private static readonly string[] _launchArgs = [];

    public static Action? OnDrawFrame;

    private static void StartDoom()
    {
        if (IsStarted)
        {
            LogWarning("Tried to start doom more than once");
            return;
        }
        if (!DoomNative.CheckDll())
        {
            LogError("DLL check failed, exiting");
            IsStarted = true;
            _threadStop.Set();
            Doom_Exit(-1);
            return;
        }
        _gameClock.Start();
        Stopwatch sw = Stopwatch.StartNew();
        // TODO: abstract these function pointers away
        DoomNativeEvents.SetEventCallbacks(new()
        {
            OnSecretDiscovered = (_) => LogWarning("Discovered a secret!"),
            // OnKill = (_, _) => LogWarning("Map thing died"),
            OnPlayerTookDamage = (_, _, dmgHealth, dmgArmor) => LogInfo($"Player took {dmgHealth+dmgArmor} damage ({dmgHealth}HP, {dmgArmor}AP)"),
            OnLevelComplete = (ep, map) => LogWarning($"E{ep}M{map} complete"),
            OnGameMessage = (msg) => LogWarning($"(Game) {msg}"),
        });
        DoomNativeAudio.SetAudioCallbacks(SfxOutput.GetCallbacks(), MusicOutput.GetCallbacks());
        AudioOutput.Init();
        DoomNative.Start(new()
        {
            Init = Doom_Init,
            DrawFrame = Doom_DrawFrame,
            Sleep = Doom_Sleep,
            GetTicksMillis = Doom_GetTicksMillis,
            GetKey = Doom_GetKey,
            GetMouse = Doom_GetMouse,
            SetWindowTitle = Doom_SetWindowTitle,
            Exit = Doom_Exit,
            Log = Doom_Log,
        }, _launchArgs);
        sw.Stop();
        StartupTime = (float) sw.Elapsed.TotalMilliseconds;
        LogWarning($"Startup took: {StartupTime:n}ms");
        IsStarted = true;
        if (IsOnMainThread())
        {
            LogError("Should not be on main thread here");
            return;
        }

        DoomLoop();
    }

    private static void DoomLoop()
    {
        while (!_threadStop.IsSet)
        {
            _runningEvent.Wait();
            DoomNative.Tick();
        }
    }

    private static void Doom_Init(int resX, int resY)
    {
        LogWarning($"Init {resX}x{resY}");
        ScreenResolution = new(resX, resY);
    }

    private static uint Doom_GetTicksMillis()
    {
        CurrentTick = (int) _gameClock.ElapsedMilliseconds;
        //LogDebug($"GetTicksMillis {CurrentTick}");
        return (uint) CurrentTick;
    }

    private static void Doom_SetWindowTitle(string title)
    {
        LogWarning($"SetWindowTitle {title}");
        WindowTitle = title;
        WindowTitleChanged?.Invoke();
    }

    private static IntPtr _screenBuffer;
    private static void Doom_DrawFrame(IntPtr screenBuffer, int bufferBytes)
    {
        if (_screenBuffer == IntPtr.Zero)
        {
            _screenBuffer = screenBuffer;
            Screen.InstallPixels(new(ScreenResolution.Width, ScreenResolution.Height, SKColorType.Bgra8888, SKAlphaType.Opaque), screenBuffer);
        }
        OnDrawFrame?.Invoke();
    }

    private static void Doom_Sleep(uint millis)
    {
        if (IsOnMainThread())
        {
            LogError($"Should not be on main thread in {nameof(Doom_Sleep)}");
            return;
        }
        //LogDebug($"Sleep {millis}");
        // millis *= 100;
        Thread.Sleep(TimeSpan.FromMilliseconds(millis));
    }

    private static bool Doom_GetKey(out bool pressed, out DoomKey key)
    {
        pressed = default;
        key = default;
        lock (_inputSync)
        {
            if (_keyQueue.TryDequeue(out var evt))
            {
                pressed = evt.Down;
                key = evt.Key;
                return true;
            }
        }
        //LogDebug("GetKey nothing");
        return false;
    }

    private static void Doom_GetMouse(out int deltaX, out int deltaY, out bool left, out bool right, out bool middle, out int wheel)
    {
        lock (_inputSync)
        {
            (deltaX, deltaY) = ((int)_mouseDelta.X, (int)_mouseDelta.Y);
            _mouseDelta = Vector.Zero;
            wheel = _mouseWheelDelta == 0 ? 0 : double.Sign(_mouseWheelDelta);
            _mouseWheelDelta = 0;
            left = _mouseButtons[0];
            right = _mouseButtons[1];
            middle = _mouseButtons[2];
        }
    }

    private static void Doom_Exit(int exitCode)
    {
        LogWarning($"Exit {exitCode}");
        LastExitCode = exitCode;
        if (exitCode != 0)
        {
            if (!_threadStop.IsSet)
            {
                LogError("Doom did not exit cleanly, aborting thread");
                _threadStop.Set();
            }
            IsStarted = false;
        }
        RunOnMainThread(() => App.Current.MainWindow?.Close());
    }

    private static void Doom_Log(string message)
    {
        LogInfo($"(Native) {message}");
    }
}
