using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using InteropDoom.Input;

namespace InteropDoom.Native;

internal static class DoomNative
{
    public const string DLL_NAME = "doomgeneric.dll";
    private static readonly string DIR = AppContext.BaseDirectory;
    public static readonly string DLL_PATH = Path.Combine(DIR, DLL_NAME);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] public delegate void InitCallback(nint screenBuffer, int resX, int resY);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] public delegate void DrawFrameCallback();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] public delegate void SleepCallback(uint millis);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] public delegate uint GetTicksMillisCallback();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] public delegate bool GetKeyCallback(out bool pressed, out DoomKey doomKey);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] public delegate void GetMouseCallback(out int deltax, out int deltay, out bool left, out bool right, out bool middle, out int wheel);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] public delegate void SetWindowTitleCallback(string title);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] public delegate void Exit(int exitCode);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] public delegate void Log(string message);

    [StructLayout(LayoutKind.Sequential)]
    public struct Callbacks
    {
        // note: using function pointers is (supposedly) more performant (according to MS)
        // but (a) this is doom, there are no perf issues on even semi-modern hardware
        // and (b) they make you give up on parameter names:
        //public delegate* unmanaged[Cdecl]<out int, out int, out bool, out bool, out bool, out int> GetMouse;
        // i mean just look at that, yuck
        public InitCallback Init;
        public DrawFrameCallback DrawFrame;
        public SleepCallback Sleep;
        public GetTicksMillisCallback GetTicksMillis;
        public GetKeyCallback GetKey;
        public GetMouseCallback? GetMouse;
        public SetWindowTitleCallback? SetWindowTitle;
        public Exit Exit;
        public Log? Log;

        public readonly void Validate()
        {
            ValidateCallback(Init);
            ValidateCallback(DrawFrame);
            ValidateCallback(Sleep);
            ValidateCallback(GetTicksMillis);
            ValidateCallback(GetKey);
            // GetMouse is optional
            // SetWindowTitle is optional
            ValidateCallback(Exit);
            // Log is optional
        }
        private static void ValidateCallback(Delegate callback, [CallerArgumentExpression(nameof(callback))] string name = "")
        {
            if (callback is null)
                throw new InvalidOperationException($"Required callback {name} is missing");
        }
    }

    // keep them pinned
    private static Callbacks _callbacks;
    private static nint _callbackPtr;

    public static void Start(Callbacks callbacks, params string[] args)
    {
        if (_callbackPtr != default)
            throw new InvalidOperationException("DOOM has already been initialized");
        SetCallbacks(callbacks);

        Native_AddIWADPath(DIR); // player can put their own wads into this folder
        string[] argv = [Path.GetFullPath(DLL_PATH), .. args]; // add the expected "0th arg" (path of the file being executed)
        Native_Create(argv.Length, argv);
    }

    internal static void SetCallbacks(Callbacks callbacks)
    {
        callbacks.Validate();
        _callbacks = WrapCallbacks(callbacks);
        _callbackPtr = Marshal.AllocHGlobal(Marshal.SizeOf<Callbacks>());
        Marshal.StructureToPtr(_callbacks, _callbackPtr, false);
        Native_SetCallbacks(_callbackPtr);
    }

    public static bool CheckDll() => File.Exists(DLL_PATH);
    public static void Tick() => Native_Tick();

    private static Callbacks WrapCallbacks(Callbacks callbacks)
    {
        return callbacks with
        {
            Exit = (code) =>
            {
                callbacks.Exit(code);
                if (code == 0) return;
                // destroy and free
                Marshal.DestroyStructure<Callbacks>(_callbackPtr);
                Marshal.FreeHGlobal(_callbackPtr);
                _callbackPtr = default;
            },
        };
    }

    [DllImport(DLL_NAME, EntryPoint = "SetCallbacks", CallingConvention = CallingConvention.Cdecl)]
    private static extern void Native_SetCallbacks(nint callbacks);

    [DllImport(DLL_NAME, EntryPoint = "Create",
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi, BestFitMapping = false, ExactSpelling = true)]
    private static extern void Native_Create(int argc, string[] argv);

    [DllImport(DLL_NAME, EntryPoint = "AddIWADPath",
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi, BestFitMapping = false, ExactSpelling = true)]
    private static extern void Native_AddIWADPath(string path);

    [DllImport(DLL_NAME, EntryPoint = "SetFallbackIWADPath",
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi, BestFitMapping = false, ExactSpelling = true)]
    private static extern void Native_SetFallbackIWADPath(string path);

    [DllImport(DLL_NAME, EntryPoint = "Tick", CallingConvention = CallingConvention.Cdecl)]
    private static extern void Native_Tick();
}
