using System.Diagnostics.CodeAnalysis;
using Melanchall.DryWetMidi.Multimedia;
using MidiFile = Melanchall.DryWetMidi.Core.MidiFile;

namespace AvaloniaPlayer.Doom.Audio;

internal static class MusicOutput
{
    private static NAudioOutputDevice _midiOut = null!;
    private static MidiFile? _songData;
    private static Playback? _song;

    public static int Pos { get; set; }

    internal static DoomNativeAudio.MusicModule.Callbacks GetCallbacks()
    {
        return new()
        {
            Init = Init,
            Shutdown = Shutdown,
            IsPlaying = IsPlaying,
            PlaySong = Play,
            StopSong = Stop,
            Pause = Pause,
            Resume = Resume,
            RegisterSong = RegisterSong,
            UnRegisterSong = UnRegisterSong,
            Poll = Poll,
            SetVolume = SetVolume,
        };
    }

    private static bool Init()
    {
        _midiOut = new(0);
        return true;
    }

    private static void Shutdown()
    {
        _midiOut?.Dispose();
    }
    [MemberNotNullWhen(true, nameof(_songData))]
    private static bool IsPlaying() => _song is { IsRunning: true };
    private static void Play(nint handle, bool looping)
    {
        _midiOut.Reset();
        _song!.Loop = looping;
        _song.MoveToStart();
        _song.Start();
    }
    private static void Stop()
    {
        _song!.Stop();
        _song.MoveToStart();
        _midiOut.Reset();
    }

    private static void Pause()
    {
        _song?.Stop();
    }
    private static void Resume()
    {
        _song?.Play();
    }

    private static unsafe nint RegisterSong(nint midiData, int len)
    {
        var stream = new UnmanagedMemoryStream((byte*)midiData.ToPointer(), len);
        _songData = MidiFile.Read(stream);
        _song = _songData.GetPlayback();
        _song.OutputDevice = _midiOut;
        return midiData;
    }
    private static void UnRegisterSong(nint handle)
    {
        _song?.Dispose();
    }

    private static void Poll()
    {
    }

    private static void SetVolume(int volume)
    {
        // the `midiOutSetVolume` windows API (used by MidiOut) actually sets the audio mixer volume (lol)
        // _midiOut.Volume = volume * 65536 / 127;
        _midiOut.MidiVolume = volume;
    }
}