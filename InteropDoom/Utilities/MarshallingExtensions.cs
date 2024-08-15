using System.Numerics;
using System.Runtime.CompilerServices;
using InteropDoom.Native.Definitions;
using System.Runtime.InteropServices;

namespace InteropDoom.Utilities;

internal static class MarshallingExtensions
{
    public static unsafe T ToStruct<T>(this nint dataPtr)
        where T : unmanaged
    {
        return Unsafe.AsRef<T>((T*)dataPtr);
    }

    public static void ThrowIfNullPtr(this nint ptr)
    {
        if (ptr == default)
            throw new AccessViolationException();
    }

    public static byte ReadByte(this nint ptr, int offset) => Marshal.ReadByte(ptr, offset);
    public static short ReadShort(this nint ptr, int offset) => Marshal.ReadInt16(ptr, offset);
    public static int ReadInt32(this nint ptr, int offset) => Marshal.ReadInt32(ptr, offset);
    public static nint ReadPtr(this nint ptr, int offset) => Marshal.ReadIntPtr(ptr, offset);
    public unsafe static T ReadArray<T>(this nint ptr, int offset, int index)
        where T : unmanaged
    {
        ptr.ThrowIfNullPtr();
        ReadOnlySpan<T> span = new((T*)(ptr + offset), index);
        return span[index];
    }
    public static unsafe Vector3 ReadVector3(this nint ptr, int offset)
    {
        ptr.ThrowIfNullPtr();
        ReadOnlySpan<int> raw = new((int*)(ptr + offset), 3);
        return new(
            FixedPoint.ToFloat(raw[0]),
            FixedPoint.ToFloat(raw[1]),
            FixedPoint.ToFloat(raw[2])
        );
    }
}
