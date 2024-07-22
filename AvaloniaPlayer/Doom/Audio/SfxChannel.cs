using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace AvaloniaPlayer.Doom.Audio;
internal class SfxChannel : ISampleProvider
{
    private UnmanagedMemoryStream _stream = null!;
    private RawSourceWaveStream _waveStream = null!;
    private ISampleProvider _samples = null!;
    private VolumeSampleProvider _volume = null!;
    private PanningSampleProvider _stereo = null!;
    private ISampleProvider? _out;

    public float Volume { get => _volume.Volume; set => _volume.Volume = value; }
    /// <summary>
    /// From -1 (100% left) to 1 (100% right); 0 is center (both 100%).
    /// </summary>
    public float Pan { get => _stereo.Pan; set => _stereo.Pan = value; }
    public WaveFormat WaveFormat { get; } = WaveFormat.CreateIeeeFloatWaveFormat(44100, 2);
    public bool IsPlaying { get; private set; }

    public void SetSfx(DoomAudioNative.SoundModule.SfxInfo sfxInfo)
    {
        unsafe
        {
            byte* ptr = (byte*)sfxInfo.Data + 16 * sizeof(short); // first and last 16 bytes are padding (both get expanded to 32)
            uint bytesLength = sfxInfo.NumSamples * 2;
            _stream = new UnmanagedMemoryStream(ptr, bytesLength);
        }
        _waveStream = new(_stream, new(sfxInfo.SampleRate, 1));
        _samples = new WaveFormatConversionProvider(new(44100, 1), _waveStream).ToSampleProvider();
        _volume = new(_samples);
        _stereo = new(_volume);
        _out = _stereo;
    }

    public int Read(float[] buffer, int offset, int sampleCount)
    {
        if (!IsPlaying || _out is null) return 0;

        return _out.Read(buffer, offset, sampleCount);
    }

    public void Play() => IsPlaying = true;
    public void Stop() => IsPlaying = false;
}
