using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AvaloniaPlayer.Doom;
internal static partial class DoomNativeEvents
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void SecretDiscoveredCallback(IntPtr sector);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void KillCallback(IntPtr target, IntPtr attacker);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void PlayerTookDamageCallback(IntPtr plr, IntPtr dealer, int health_dmg, int armor_dmg);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void LevelCompleteCallback(int episode, int map);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GameMessageCallback(string msg);

    [StructLayout(LayoutKind.Sequential)]
    public struct Callbacks
    {
        public SecretDiscoveredCallback? OnSecretDiscovered;
        public KillCallback? OnKill;
        public PlayerTookDamageCallback? OnPlayerTookDamage;
        public LevelCompleteCallback? OnLevelComplete;
        public GameMessageCallback? OnGameMessage;
    }

    // keep them pinned
    private static Callbacks _callbacks;
    private static IntPtr _callbackPtr;

    public static void SetEventCallbacks(Callbacks callbacks)
    {
        _callbacks = callbacks;
        bool firstStart = _callbackPtr == IntPtr.Zero;
        if (firstStart)
            _callbackPtr = Marshal.AllocHGlobal(Marshal.SizeOf<Callbacks>());
        Marshal.StructureToPtr(_callbacks, _callbackPtr, !firstStart);
        NativeSetEventCallbacks(_callbackPtr);
    }

    [LibraryImport(DoomNative.DLL_NAME, EntryPoint = "SetEventCallbacks")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial void NativeSetEventCallbacks(IntPtr callbacks);
}
