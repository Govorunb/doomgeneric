using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using InteropDoom.Engine;
using InteropDoom.Engine.Events;
using InteropDoom.Input;
using InteropDoom.Native;

namespace InteropDoom;

partial class DoomRuntime
{
    private static Thread? _doomThread;
    private static ManualResetEventSlim ThreadStop { get; } = new(false);
    private static ManualResetEventSlim RunningEvent { get; }= new(true);

    private static readonly string[] _launchArgs = [];

    public static Action? OnDrawFrame { get; set; }

    private static void StartDoom()
    {
        if (IsStarted)
        {
            LogWarning("Tried to start more than once");
            return;
        }
        if (!DoomNative.CheckDll())
        {
            LogError("DLL check failed, exiting");
            IsStarted = true;
            ThreadStop.Set();
            Doom_Exit(-1);
            return;
        }
        Stopwatch sw = Stopwatch.StartNew();
        DoomNativeEvents.SetEventCallback(Evt_Handle);
        DoomNativeAudio.SetAudioCallbacks(GetSndCallbacks(), GetMusCallbacks());
        DoomNative.Callbacks engineCallbacks = GetEngineCallbacks();
        DoomNative.Start(engineCallbacks, _launchArgs);
        sw.Stop();
        LogWarning($"Startup took: {sw.Elapsed.TotalMilliseconds:n}ms");
        IsStarted = true;

        DoomLoop();
    }


    private static void DoomLoop()
    {
        while (!ThreadStop.IsSet)
        {
            RunningEvent.Wait();
            DoomNative.Tick();
        }
    }

    public static void Pause() => RunningEvent.Reset();
    public static void Unpause() => RunningEvent.Set();

    private static DoomNative.Callbacks GetEngineCallbacks()
    {
        return new()
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
        };
    }

    private static void Doom_Init(nint screenBuffer, int resX, int resY)
    {
        Screen.Buffer = screenBuffer;
        Screen.Width = resX;
        Screen.Height = resY;
        Engine.OnInit(Screen);
    }

    private static uint Doom_GetTicksMillis()
        => (uint)Engine.TimeProvider.GetUtcNow().ToUnixTimeMilliseconds();

    private static void Doom_SetWindowTitle(string title)
    {
        Engine.OnWindowTitleChanged(title);
    }

    private static void Doom_DrawFrame()
    {
        Engine.OnDrawFrame();
        OnDrawFrame?.Invoke();
    }

    private static void Doom_Sleep(uint millis)
    {
        Engine.TimeProvider.Delay(TimeSpan.FromMilliseconds(millis)).Wait();
    }

    private static bool Doom_GetKey(out bool pressed, out DoomKey key)
    {
        pressed = default;
        key = default;
        if (_keyQueue.TryDequeue(out var evt))
        {
            pressed = evt.Down;
            key = evt.Key;
            return true;
        }
        //LogDebug("GetKey nothing");
        return false;
    }

    private static void Doom_GetMouse(out int deltaX, out int deltaY, out bool left, out bool right, out bool middle, out int wheel)
    {
        lock (_inputSync)
        {
            deltaX = (int)Interlocked.Exchange(ref _mouseState.DeltaX, 0);
            deltaY = 0;
            wheel = Math.Sign(Interlocked.Exchange(ref _mouseState.WheelDelta, 0));
            left = _mouseState.Buttons.HasFlag(MouseButtons.Left);
            right = _mouseState.Buttons.HasFlag(MouseButtons.Right);
            middle = _mouseState.Buttons.HasFlag(MouseButtons.Middle);
        }
    }

    private static void Doom_Exit(int exitCode)
    {
        LogWarning($"Exit {exitCode}");
        if (exitCode != 0)
        {
            if (!ThreadStop.IsSet)
            {
                LogError("Doom did not exit cleanly, aborting thread");
                ThreadStop.Set();
            }
            IsStarted = false;
        }
        Engine.OnExit(exitCode);
    }

    private static void Doom_Log(string message)
    {
        LogInfo($"(Native) {message}");
    }

    internal static void LogVerbose(string message)
        => Engine.Logger.LogVerbose(message);
    internal static void LogInfo(string message)
        => Engine.Logger.LogInfo(message);
    internal static void LogDebug(string message)
        => Engine.Logger.LogDebug(message);
    internal static void LogWarning(string message)
        => Engine.Logger.LogWarning(message);
    internal static void LogError(string message)
        => Engine.Logger.LogError(message);
    internal static void LogFatal(string message)
        => Engine.Logger.LogFatal(message);

    private static DoomNativeAudio.SoundModule.Callbacks GetSndCallbacks()
    {
        return new()
        {
            Init = Snd_Init,
            Shutdown = Snd_Shutdown,
            StartSound = Snd_Start,
            StopSound = Snd_Stop,
            IsPlaying = Snd_IsPlaying,
            Update = Snd_Update,
            UpdateSoundParams = Snd_UpdateSoundParams,
            CacheSounds = Snd_CacheSounds,
        };
    }

    private static bool Snd_Init()
        => Engine.SoundEngine?.Init() ?? false;
    private static void Snd_Shutdown()
        => Engine.SoundEngine?.Shutdown();
    private static nint Snd_Start(in SfxInfo sfxinfo, int channel, int vol, int sep)
        => Engine.SoundEngine?.Start(sfxinfo, channel, vol/127f, (sep-127)/127f)
            ?? throw new InvalidOperationException("No sound engine available");
    private static void Snd_Stop(nint channel)
        => Engine.SoundEngine?.Stop(channel);
    private static void Snd_CacheSounds(SfxInfo[] sounds, int num_sounds) { } // unimplemented
    private static void Snd_UpdateSoundParams(nint channel, int vol, int sep)
        => Engine.SoundEngine?.UpdateSoundParams(channel, vol/127f, (sep-127)/127f);
    private static void Snd_Update()
        => Engine.SoundEngine?.Update();
    private static bool Snd_IsPlaying(nint channel)
        => Engine.SoundEngine?.IsPlaying(channel) ?? false;

    private static DoomNativeAudio.MusicModule.Callbacks GetMusCallbacks()
    {
        return new()
        {
            Init = Mus_Init,
            Shutdown = Mus_Shutdown,
            RegisterSong = Mus_Register,
            UnRegisterSong = Mus_Unregister,
            PlaySong = Mus_Play,
            StopSong = Mus_Stop,
            Pause = Mus_Pause,
            Resume = Mus_Resume,
            SetVolume = Mus_SetVolume,
            IsPlaying = Mus_IsPlaying,
            Poll = Mus_Poll,
        };
    }

    private static bool Mus_Init()
        => Engine.MusicEngine?.Init() ?? false;
    private static void Mus_Shutdown()
        => Engine.MusicEngine?.Shutdown();
    private static nint Mus_Register(nint midiData, int lenBytes)
    {
        Stream stream;
        unsafe
        {
            stream = new UnmanagedMemoryStream((byte*)midiData, lenBytes);
        }
        return Engine.MusicEngine?.RegisterSong(stream) ?? throw new InvalidOperationException("No music engine available");
    }
    private static void Mus_Unregister(nint handle)
        => Engine.MusicEngine?.UnRegisterSong(handle);
    private static void Mus_Play(nint handle, bool looping)
        => Engine.MusicEngine?.Play(handle, looping);
    private static void Mus_Stop()
        => Engine.MusicEngine?.Stop();
    private static void Mus_Pause()
        => Engine.MusicEngine?.Pause();
    private static void Mus_Resume()
        => Engine.MusicEngine?.Resume();
    private static void Mus_SetVolume(int volume)
        => Engine.MusicEngine?.SetVolume(volume / 127f);
    private static bool Mus_IsPlaying()
        => Engine.MusicEngine?.IsPlaying() ?? false;
    private static void Mus_Poll()
        => Engine.MusicEngine?.Update();

    private static void Evt_Handle(EventType type, nint dataPtr)
        => Engine.EventEngine?.DispatchEvent(type, dataPtr);
}
