using System.Diagnostics.CodeAnalysis;
using Melanchall.DryWetMidi.Multimedia;
using MidiFile = Melanchall.DryWetMidi.Core.MidiFile;

namespace AvaloniaPlayer.Doom.Audio;

internal class MusicOutput : IMusicEngine
{
    private NAudioOutputDevice _midiOut = null!;
    private MidiFile? _songData;
    private Playback? _song;
    private static readonly List<MidiFile> _songs = [];

    public int Pos { get; set; }

    public bool Init()
    {
        _midiOut = new(0);
        return true;
    }
    public void Shutdown()
    {
        _midiOut?.Dispose();
        _songs.Clear();
    }

    [MemberNotNullWhen(true, nameof(_songData))]
    public bool IsPlaying() => _song is { IsRunning: true };

    public void Play(nint handle, bool looping)
    {
        _midiOut.Reset();
        _song!.Loop = looping;
        _song.MoveToStart();
        _song.Start();
    }
    public void Stop()
    {
        _song!.Stop();
        _song.MoveToStart();
        _midiOut.Reset();
    }

    public void Pause() => _song?.Stop();
    public void Resume() => _song?.Play();

    public nint RegisterSong(Stream midiStream)
    {
        _songData = MidiFile.Read(midiStream);
        _song = _songData.GetPlayback();
        _song.OutputDevice = _midiOut;
        return 0;
    }

    public void UnRegisterSong(nint handle) => _song?.Dispose();

    public void Update() { }

    public void SetVolume(float volume)
    {
        // the `midiOutSetVolume` windows API (used by MidiOut) actually sets the audio mixer volume (lol)
        // _midiOut.Volume = volume;
        _midiOut.MidiVolume = volume;
    }
}