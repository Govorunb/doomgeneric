# doomgeneric interop fork
This [doomgeneric](https://github.com/ozkl/doomgeneric) fork is focused on embedding Doom into managed hosts through callbacks. Feel free to read [the original repo's README](https://github.com/ozkl/doomgeneric/blob/master/README.md) for more information on doomgeneric.

# How do I start?
Implement these callbacks in your host environment.

|Function|Description|Notes|
|--------|-----------|-----|
|`void Init(byte* screen_buffer, *int resX, int resY)`|Initialize your platform (create window, framebuffer, etc...)|
|`void DrawFrame()`|Frame is ready in the screen buffer (given to you in `Init`). Copy it to your platform's screen.|You may need to flip your screen depending on your renderer.|
|`void Sleep(uint32 millis)`|Sleep in milliseconds, then return.|You may want to run the Doom loop in a separate thread so you can suspend it without affecting the rest of your application.|
|`uint32 GetTicksMillis()`|Milliseconds elapsed since a fixed point (e.g. launch).||
|`bool GetKey(out bool* pressed, out char* key)`|Provide keyboard events.|If there's a new key press/release event to process, assign the `pressed` (`true` for down, `false` for up) and `key` ([DOOMKEY](https://github.com/Govorunb/doomgeneric/blob/1dbfbcd3eac86984944eef27398c27e0486cff86/doomgeneric/doomkeys.h) or [ASCII character](https://github.com/Govorunb/doomgeneric/blob/1dbfbcd3eac86984944eef27398c27e0486cff86/doomgeneric/m_controls.c)) values and return `true`.|
|`void GetMouse(out int* deltaX, out int* deltaY, out bool* left, out bool* right, out bool* middle, out int* wheelDelta)`|Optional. Provide mouse input.|`deltaX`/`deltaY` should be the change in pointer location since the last call to `GetMouse`; `left`, `right`, and `middle` are the current state of the mouse buttons (`true` if pressed down); `wheelDelta` is the mouse wheel change since last call to `GetMouse` (negative for scrolling down).
|`void SetWindowTitle(char* title)`|Optional. This is for setting the window title as Doom sets this from WAD file.||
|`void Exit(int exit_code)`|Called when Doom wants to exit.|You're free to close your app when this is called, or ignore it and continue running - though, keep in mind that continuing to run after a non-zero `exit_code` may not end well for your application.|
|`void Log(char* message)`|Optional. Helper for native debugging.|Most likely nothing will call this, so it's safe to omit.|

## How do I run it in my host application?
Create an object that contains the above callbacks. **Make sure they are ordered exactly as the table above shows**. You may need to prevent your compiler/runtime from arbitrarily reordering members ([C# example](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.structlayoutattribute?view=net-8.0)). Replace optional callbacks you don't want to implement with NULL pointers.

First, call [`SetCallbacks`](https://github.com/Govorunb/doomgeneric/blob/1dbfbcd3eac86984944eef27398c27e0486cff86/doomgeneric/doomgeneric_interop.h#L36) with a pointer to your callbacks object. If your host is managed, you **must** pin the object and all callback function pointers to prevent the garbage collector from moving/collecting them.

When you want to start the game, call [`Create`](https://github.com/Govorunb/doomgeneric/blob/1dbfbcd3eac86984944eef27398c27e0486cff86/doomgeneric/doomgeneric_interop.h#L39).

Then, call [`Tick`](https://github.com/Govorunb/doomgeneric/blob/1dbfbcd3eac86984944eef27398c27e0486cff86/doomgeneric/doomgeneric_interop.h#L40) in a loop. You should probably do this on a dedicated thread, as the native side will call your `Sleep(uint32 millis)` callback expecting you to return after that time has passed (i.e. the game expects you to put the thread to sleep for that time).

---

For an example implementation, see the proof-of-concept [C# library](https://github.com/Govorunb/doomgeneric/blob/1dbfbcd3eac86984944eef27398c27e0486cff86/InteropDoom). There's also an [Avalonia-based player](https://github.com/Govorunb/doomgeneric/blob/1dbfbcd3eac86984944eef27398c27e0486cff86/AvaloniaPlayer) available (which uses that library) as an example starting point for a UI application.

## Sound?
Define [`FEATURE_SOUND`](https://github.com/Govorunb/doomgeneric/blob/1dbfbcd3eac86984944eef27398c27e0486cff86/doomgeneric/doomfeatures.h#L36) before you compile the native dll. Then, on the managed host, call [`SetAudioCallbacks`](https://github.com/Govorunb/doomgeneric/blob/1dbfbcd3eac86984944eef27398c27e0486cff86/doomgeneric/doomgeneric_interop_audio.h#L57) with your sound/music callbacks **before `Create`**.

Sound effect data sent to callbacks is in mono 16-bit signed PCM (expanded from [8-bit **unsigned** PCM](https://doomwiki.org/wiki/Sound)). Note that there are 32 bytes (16 samples) of padding before the sound data actually starts. Most sound effects are in 11025hz, but you should follow the [sample rate](https://github.com/Govorunb/doomgeneric/blob/1dbfbcd3eac86984944eef27398c27e0486cff86/doomgeneric/doomgeneric_audio.h#L8) you're given just in case.

Music is sent to callbacks as MIDI (converted internally from [MUS](https://doomwiki.org/wiki/MUS)).

## Other features? (jumping, rebinding controls, modern rendering, etc.)
This is a very limited-scope project for me (I just wanted to get Doom running inside a Unity mod) but if you get it working let me know! I love to see the concept of "Doom on everything" thrive.

# Platforms
This fork has only been tested on Windows.

