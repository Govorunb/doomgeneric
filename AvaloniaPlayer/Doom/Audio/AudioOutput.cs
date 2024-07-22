using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace AvaloniaPlayer.Doom.Audio;
internal static class AudioOutput
{
    private static readonly WasapiOut _out = new(AudioClientShareMode.Shared, 50);
    private static readonly MixingSampleProvider _mixer = new(WaveFormat.CreateIeeeFloatWaveFormat(44100, 2));

    public static void Init()
    {
        _mixer.ReadFully = true;
        _out.Init(_mixer);
        _out.Play();
    }

    public static void Add(ISampleProvider input)
        => _mixer.AddMixerInput(input);
    public static void Remove(ISampleProvider input)
        => _mixer.RemoveMixerInput(input);
}
