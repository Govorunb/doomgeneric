using System.Runtime.InteropServices;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace AvaloniaPlayer.Doom.Audio;

internal static class SfxOutput
{
    private static readonly MixingSampleProvider _mixer = new(WaveFormat.CreateIeeeFloatWaveFormat(44100, 2));
    private static readonly SfxChannel[] _channels = new SfxChannel[32];

    internal static DoomNativeAudio.SoundModule.Callbacks GetCallbacks()
    {
        return new()
        {
            Init = Init,
            Shutdown = Shutdown,
            IsPlaying = IsPlaying,
            StartSound = Start,
            StopSound = Stop,
            UpdateSoundParams = UpdateSoundParams,
            Update = Update,
            CacheSounds = null!,
        };
    }

    private static bool Init()
    {
        Splat.LogHost.Default.Info("Sfx Init");
        if (!_mixer.MixerInputs.Any())
        {
            for (var i = 0; i < _channels.Length; i++)
            {
                _channels[i] = new();
                _mixer.AddMixerInput(_channels[i]);
            }
        }
        _mixer.ReadFully = true;
        AudioOutput.Add(_mixer);

        return true;
    }

    private static void Shutdown()
    {
        Splat.LogHost.Default.Info("Sfx Shutdown");
        AudioOutput.Remove(_mixer);
    }

    private static bool IsPlaying(nint channel)
        => _channels[(int)channel] is { IsPlaying: true };

    private static nint Start(IntPtr sfxinfo, int channel, int vol, int sep)
    {
        DoomNativeAudio.SoundModule.SfxInfo sfxInfo = Marshal.PtrToStructure<DoomNativeAudio.SoundModule.SfxInfo>(sfxinfo);
        var player = _channels[channel];
        player.SetSfx(sfxInfo);
        UpdateSoundParams(channel, vol, sep);
        player.Play();
        _mixer.RemoveMixerInput(player);
        _mixer.AddMixerInput(player);
        return channel;
    }

    private static void Stop(nint channel)
    {
        _channels[channel].Stop();
    }

    private static void Update() { }
    private static void UpdateSoundParams(nint channel, int vol, int sep)
    {
        _channels[channel].Volume = vol / 127f;
        _channels[channel].Pan = (sep - 127) / 127f;
    }
}
