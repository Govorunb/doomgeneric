using Avalonia.Input;

namespace AvaloniaPlayer.Doom.Input;

internal static class KeyCodeConverter
{
    private static readonly Dictionary<PhysicalKey, DoomKey> _physToDoom = new()
    {
        { PhysicalKey.ArrowRight, DoomKey.RightArrow },
        { PhysicalKey.ArrowLeft, DoomKey.LeftArrow },
        { PhysicalKey.ArrowUp, DoomKey.UpArrow },
        { PhysicalKey.ArrowDown, DoomKey.DownArrow },
        { PhysicalKey.W, DoomKey.UpArrow },
        { PhysicalKey.A, DoomKey.StrafeLeft },
        { PhysicalKey.S, DoomKey.DownArrow },
        { PhysicalKey.D, DoomKey.StrafeRight },
        { PhysicalKey.E, DoomKey.Use },
        //{ PhysicalKey.Mouse0, DoomKey.Fire }, // we use the mouse natively now
        { PhysicalKey.ControlRight, DoomKey.Fire },
        { PhysicalKey.Escape, DoomKey.Escape },
        { PhysicalKey.Enter, DoomKey.Enter },
        { PhysicalKey.Space, DoomKey.Enter },
        { PhysicalKey.Tab, DoomKey.Tab },

        { PhysicalKey.F1, DoomKey.F1 },
        { PhysicalKey.F2, DoomKey.F2 },
        { PhysicalKey.F3, DoomKey.F3 },
        { PhysicalKey.F4, DoomKey.F4 },
        { PhysicalKey.F5, DoomKey.F5 },
        { PhysicalKey.F6, DoomKey.F6 },
        { PhysicalKey.F7, DoomKey.F7 },
        { PhysicalKey.F8, DoomKey.F8 },
        { PhysicalKey.F9, DoomKey.F9 },
        { PhysicalKey.F10, DoomKey.F10 },
        { PhysicalKey.F11, DoomKey.F11 },
        { PhysicalKey.F12, DoomKey.F12 },

        { PhysicalKey.Backspace, DoomKey.Backspace },
        { PhysicalKey.Pause, DoomKey.Pause },
        { PhysicalKey.Equal, DoomKey.Equals },
        { PhysicalKey.Minus, DoomKey.Minus },

        { PhysicalKey.ShiftRight, DoomKey.RShift },
        { PhysicalKey.ShiftLeft, DoomKey.RShift },
        //{ PhysicalKey.RightControl, DoomKey.RCtrl },
        { PhysicalKey.ControlLeft, DoomKey.RCtrl },
        { PhysicalKey.AltRight, DoomKey.RAlt },

        { PhysicalKey.AltLeft, DoomKey.LAlt },

        { PhysicalKey.CapsLock, DoomKey.CapsLock },
        { PhysicalKey.NumLock, DoomKey.NumLock },
        { PhysicalKey.ScrollLock, DoomKey.ScrollLock },
        { PhysicalKey.PrintScreen, DoomKey.PrintScreen },

        { PhysicalKey.Home, DoomKey.Home },
        { PhysicalKey.End, DoomKey.End },
        { PhysicalKey.PageUp, DoomKey.PageUp },
        { PhysicalKey.PageDown, DoomKey.PageDown },
        { PhysicalKey.Insert, DoomKey.Insert },
        { PhysicalKey.Delete, DoomKey.Delete },

        { PhysicalKey.NumPad0, DoomKey.NumPad0 },
        { PhysicalKey.NumPad1, DoomKey.NumPad1 },
        { PhysicalKey.NumPad2, DoomKey.NumPad2 },
        { PhysicalKey.NumPad3, DoomKey.NumPad3 },
        { PhysicalKey.NumPad4, DoomKey.NumPad4 },
        { PhysicalKey.NumPad5, DoomKey.NumPad5 },
        { PhysicalKey.NumPad6, DoomKey.NumPad6 },
        { PhysicalKey.NumPad7, DoomKey.NumPad7 },
        { PhysicalKey.NumPad8, DoomKey.NumPad8 },
        { PhysicalKey.NumPad9, DoomKey.NumPad9 },

        { PhysicalKey.NumPadDivide, DoomKey.NumPadDivide },
        { PhysicalKey.NumPadAdd, DoomKey.NumPadPlus },
        { PhysicalKey.NumPadSubtract, DoomKey.NumPadMinus },
        { PhysicalKey.NumPadMultiply, DoomKey.NumPadMultiply },
        { PhysicalKey.NumPadDecimal, DoomKey.NumPadPeriod },
        { PhysicalKey.NumPadEqual, DoomKey.NumPadEquals },
        { PhysicalKey.NumPadEnter, DoomKey.NumPadEnter }
    };

    private static readonly Dictionary<DoomKey, PhysicalKey> _doomToPhys = _physToDoom
        // there are duplicate DoomKey enum members
        .GroupBy(pair => pair.Value, pair => pair.Key)
        .ToDictionary(pair => pair.Key, pair => pair.First());

    public static PhysicalKey ToPhysicalKey(this DoomKey doomKey)
    {
        if (_doomToPhys.TryGetValue(doomKey, out var physKey))
            return physKey;

        var keyName = char.ToUpper((char)doomKey);
        if (Enum.TryParse(keyName.ToString(), out physKey))
            return physKey;

        return default;
    }

    public static DoomKey ToDoomKey(this PhysicalKey physKey)
    {
        if (_physToDoom.TryGetValue(physKey, out DoomKey doomKey))
            return doomKey;

        string? name = physKey.ToQwertyKeySymbol();
        if (name?.Length == 1)
        {
            // letter key
            return (DoomKey)char.ToLower(name[0]);
        }
        return default;
    }

    public static IEnumerable<(PhysicalKey, DoomKey)> GetAllKeys() => _physToDoom.Select(pair => (pair.Key, pair.Value));

    private static readonly Dictionary<char, char> _shiftDigits = new()
    {
        { '!', '1' },
        { '@', '2' },
        { '#', '3' },
        { '$', '4' },
        { '%', '5' },
        { '^', '6' },
        { '&', '7' },
        { '*', '8' },
        { '(', '9' },
        { ')', '0' },
        { '_', '-' },
        { '+', '=' },
    };
    public static DoomKey ConvertKey(byte key)
    {
        char c = (char)key;
        if (_shiftDigits.TryGetValue(c, out char digit))
            key = (byte)digit;
        return (DoomKey)key;
    }
}
