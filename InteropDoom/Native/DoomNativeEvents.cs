using System.Runtime.InteropServices;
using InteropDoom.Engine;

namespace InteropDoom.Native;
internal static partial class DoomNativeEvents
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] public delegate void EventCallback(EventType event_type, nint data);
    private static nint _callbackPtr;

    public static void SetEventCallback(EventCallback callback)
    {
        _callbackPtr = Marshal.GetFunctionPointerForDelegate(callback);
        Native_SetEventCallback(_callbackPtr);
    }
    [DllImport(DoomNative.DLL_NAME, EntryPoint = "SetEventCallback", CallingConvention = CallingConvention.Cdecl)]
    private static extern void Native_SetEventCallback(nint callback);
}
