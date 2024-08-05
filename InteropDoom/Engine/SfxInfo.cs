namespace InteropDoom.Engine;

public struct SfxInfo
{
    public ushort SampleRate;
    public uint NumSamples;
    public nint Data; // short*

    public readonly unsafe Stream ToStream()
    {
        var ptr = (byte*)Data + 16 * sizeof(short); // first and last 16 bytes are padding (both get expanded to 32)
        var bytesLength = NumSamples * 2; // 16-bit PCM
        return new UnmanagedMemoryStream(ptr, bytesLength);
    }
}
