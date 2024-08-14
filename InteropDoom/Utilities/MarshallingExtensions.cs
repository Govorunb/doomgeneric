using System.Runtime.CompilerServices;

namespace InteropDoom.Utilities;

internal static class MarshallingExtensions
{
    public static unsafe T ToStruct<T>(this nint dataPtr)
        where T : unmanaged
    {
        return Unsafe.AsRef<T>((T*)dataPtr);
    }
}
