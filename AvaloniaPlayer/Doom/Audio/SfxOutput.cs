using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace AvaloniaPlayer.Doom.Audio;

internal class SfxOutput : ISoundEngine
{
    private readonly MixingSampleProvider _mixer = new(WaveFormat.CreateIeeeFloatWaveFormat(44100, 2));
    private readonly SfxChannel[] _channels = new SfxChannel[32];

    public bool Init()
    {
        Splat.LogHost.Default.Info("Sfx Init");
        _mixer.ReadFully = true;
        if (!_mixer.MixerInputs.Any())
        {
            for (var i = 0; i < _channels.Length; i++)
            {
                _channels[i] = new();
                _mixer.AddMixerInput(_channels[i]);
            }
        }
        AudioOutput.Add(_mixer);

        return true;
    }

    public void Shutdown()
    {
        Splat.LogHost.Default.Info("Sfx Shutdown");
        AudioOutput.Remove(_mixer);
    }

    public bool IsPlaying(nint channel)
        => GetChannel(channel) is { IsPlaying: true };

    public nint Start(SfxInfo sfxInfo, int channel, float volume, float pan)
    {
        channel = FindFreeChannel(); // use our own channel limits
        var player = GetChannel(channel);
        player.SetSfx(sfxInfo);
        UpdateSfx(player, volume, pan);
        player.Play();
        _mixer.RemoveMixerInput(player); // remove if it was playing before (mixer's backing storage is a list so we would add twice)
        _mixer.AddMixerInput(player);
        return channel;
    }

    public void Stop(nint handle)
    {
        GetChannel(handle).Stop();
    }

    public void Update() { }
    public void UpdateSoundParams(nint handle, float volume, float pan)
    {
        UpdateSfx(GetChannel(handle), volume, pan);
    }

    private static void UpdateSfx(SfxChannel player, float volume, float pan)
    {
        player.Volume = volume;
        player.Pan = pan;
    }

    private int FindFreeChannel()
        => Array.FindIndex(_channels, c => !c.IsPlaying);

    private SfxChannel GetChannel(nint handle)
        => _channels[handle % _channels.Length];
}
