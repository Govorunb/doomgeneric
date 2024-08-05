using System.Runtime.InteropServices;

namespace InteropDoom;

/// <summary>
/// Wrapper for a screen buffer.
/// </summary>
/// <param name="buffer">Buffer pointer.</param>
/// <param name="width">Width in pixels.</param>
/// <param name="height">Height in pixels.</param>
public sealed class ScreenBuffer
{
    internal ScreenBuffer() { }

    public nint Buffer { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int TotalPixels => Width * Height;
    public bool IsInitialized => Buffer != default;

    public uint GetPixel(int x, int y)
    {
        int i = y * Width + x;
        if (i < 0 || i >= TotalPixels)
            throw new ArgumentOutOfRangeException(null, "Coordinates out of bounds");
        return (uint)Marshal.ReadInt32(Buffer + i * sizeof(uint));
    }
}
