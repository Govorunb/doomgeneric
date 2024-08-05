using System.Runtime.InteropServices;

namespace InteropDoom.Native;
internal static partial class DoomNativeEvents
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] public delegate void SecretDiscoveredCallback(nint sector);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] public delegate void KillCallback(nint target, nint attacker);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] public delegate void PlayerTookDamageCallback(nint plr, nint dealer, int health_dmg, int armor_dmg);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] public delegate void LevelCompleteCallback(int episode, int map);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] public delegate void GameMessageCallback(string msg);

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
    private static nint _callbackPtr;

    public static void SetEventCallbacks(Callbacks callbacks)
    {
        _callbacks = callbacks;
        bool firstStart = _callbackPtr == default;
        if (firstStart)
            _callbackPtr = Marshal.AllocHGlobal(Marshal.SizeOf<Callbacks>());
        Marshal.StructureToPtr(_callbacks, _callbackPtr, !firstStart);
        Native_SetEventCallbacks(_callbackPtr);
    }
    [DllImport(DoomNative.DLL_NAME, EntryPoint = "SetEventCallbacks", CallingConvention = CallingConvention.Cdecl)]
    private static extern void Native_SetEventCallbacks(nint callbacks);
}
