using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AvaloniaPlayer.Doom.Input;

namespace AvaloniaPlayer.Doom;

internal static partial class DoomNative
{
    public const string DLL_NAME = "doomgeneric.dll";
    private static readonly string DIR = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
    public static readonly string DLL_PATH = Path.Combine(DIR, DLL_NAME);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void InitCallback(int resX, int resY);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void DrawFrameCallback(IntPtr screenBuffer, int bufferBytes);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void SleepCallback(uint millis);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate uint GetTicksMillisCallback();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate bool GetKeyCallback(out bool pressed, out DoomKey doomKey);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void GetMouseCallback(out int deltax, out int deltay, out bool left, out bool right, out bool middle, out int wheel);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void SetWindowTitleCallback([MarshalAs(UnmanagedType.LPStr)] string title);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void Exit(int exitCode);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void Log([In, MarshalAs(UnmanagedType.LPStr)] string message);

    [StructLayout(LayoutKind.Sequential)]
    public struct Callbacks
    {
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
    private static PixelSize _res;
    private static bool _created;

    // keep them pinned
    private static Callbacks _callbacks;
    private static IntPtr _callbackPtr;

    public static void Start(Callbacks callbacks, params string[] args)
    {
        callbacks.Validate();
        _callbacks = WrapCallbacks(callbacks);
        bool firstStart = _callbackPtr == IntPtr.Zero;
        if (firstStart)
            _callbackPtr = Marshal.AllocHGlobal(Marshal.SizeOf<Callbacks>());
        Marshal.StructureToPtr(_callbacks, _callbackPtr, !firstStart);
        NativeSetCallbacks(_callbackPtr);
        if (_created)
        {
            // maybe throw instead?
            callbacks.Init(_res.Width, _res.Height);
        }
        else
        {
            NativeAddIWADPath(DIR); // player can put their own wads into this folder
            //NativeSetFallbackIWADPath(FALLBACK_WAD_PATH);
            string[] argv = [Path.GetFullPath(DLL_PATH), ..args];
            NativeCreate(argv.Length, argv);
        }
    }
    public static bool CheckDll() => File.Exists(DLL_PATH);

    public static void Tick() => NativeTick();

    private static Callbacks WrapCallbacks(Callbacks callbacks)
    {
        return callbacks with
        {
            Init = WrapInitCallback(callbacks.Init),
            Exit = WrapExitCallback(callbacks.Exit),
        };
    }

    private static InitCallback WrapInitCallback(InitCallback inner)
    {
        return (resX, resY) =>
        {
            _res = new(resX, resY);
            inner(resX, resY);
            _created = true;
        };
    }

    private static Exit WrapExitCallback(Exit inner)
    {
        return (code) =>
        {
            inner(code);
            if (code == 0) return;
            // destroy and free
            _res = default;
            _created = false;
            Marshal.DestroyStructure<Callbacks>(_callbackPtr);
            Marshal.FreeHGlobal(_callbackPtr);
            _callbackPtr = IntPtr.Zero;
        };
    }

    // our interface
    [LibraryImport(DLL_NAME, EntryPoint = "SetCallbacks")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial void NativeSetCallbacks(IntPtr callbacks);

    [LibraryImport(DLL_NAME, EntryPoint = "Create")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial void NativeCreate(int argc, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0, ArraySubType = UnmanagedType.LPStr)] string[] argv);
    [LibraryImport(DLL_NAME, EntryPoint = "AddIWADPath", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial void NativeAddIWADPath(string path);
    [LibraryImport(DLL_NAME, EntryPoint = "SetFallbackIWADPath", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial void NativeSetFallbackIWADPath(string path);

    [LibraryImport(DLL_NAME, EntryPoint = "Tick")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    private static partial void NativeTick();
}
