# doomgeneric interop fork
This [doomgeneric](https://github.com/ozkl/doomgeneric) fork is focused on embedding Doom into managed hosts through callbacks. Feel free to read [the original repo's README](https://github.com/ozkl/doomgeneric/blob/master/README.md) for more information on doomgeneric.

# How do I start?
Implement these callbacks in your host environment.

|Function|Description|Notes|
|--------|-----------|-----|
|`void Init(int resX, int resY)`|Initialize your platform (create window, framebuffer, etc...).|
|`void DrawFrame(byte* screen_buffer, int buffer_bytes)`|Frame is ready in DG_ScreenBuffer. Copy it to your platform's screen.|You may need to flip your screen depending on your renderer.|
|`void Sleep(uint32 millis)`|Sleep in milliseconds.|I recommend running the Doom loop in a separate thread so you can suspend it.|
|`uint32 GetTicksMillis()`|Milliseconds elapsed since a fixed point (e.g. launch).||
|`bool GetKey(out bool* pressed, out char* key)`|Provide keyboard events.|If there's a new key press/release event to process, assign the `pressed` (`true` for down, `false` for up) and `key` ([DOOMKEY](https://github.com/Govorunb/doomgeneric/blob/b3336ce013865e9f1318ebb454b84a397784bdf4/doomgeneric/doomkeys.h) or [ASCII character](https://github.com/Govorunb/doomgeneric/blob/b3336ce013865e9f1318ebb454b84a397784bdf4/doomgeneric/m_controls.c)) values and return `true`.|
|`void GetMouse(out int* deltaX, out int* deltaY, out bool* left, out bool* right, out bool* middle, out int* wheelDelta)`|Optional. Provide mouse input.|`deltaX`/`deltaY` should be the change in pointer location since the last call to `GetMouse`; `left`, `right`, and `middle` are mouse buttons (`true` if pressed down); `wheelDelta` should be negative if scrolling down, positive if scrolling up, and 0 if no scrolling occurred.
|`void SetWindowTitle(char* title)`|Optional. This is for setting the window title as Doom sets this from WAD file.||
|`void Exit(int exit_code)`|Called when Doom exits.|The game doesn't actually "exit" (call `exit()`) since that would kill the host process.|
|`void Log(char* message)`|Optional.||

Then, put those callbacks (in the same order as listed above) into an object, send it to the dll, and start the game.

For an example implementation, see the [Avalonia-based player](./AvaloniaPlayer).

## How do I run it in my host application?
First, call [`SetCallbacks`](https://github.com/Govorunb/doomgeneric/blob/b3336ce013865e9f1318ebb454b84a397784bdf4/doomgeneric/doomgeneric_interop.h#L34) with your callbacks object. If your host is managed, you will need to marshal that object into a struct pointer and pin all function pointers to prevent them getting moved/cleaned up by the garbage collector.

When you want to start the game, call `Create`.

In a loop, call `Tick`.

## Sound?
Define [`FEATURE_SOUND`](https://github.com/Govorunb/doomgeneric/blob/b3336ce013865e9f1318ebb454b84a397784bdf4/doomgeneric/doomfeatures.h#L36), and call [`SetAudioCallbacks`](https://github.com/Govorunb/doomgeneric/blob/b3336ce013865e9f1318ebb454b84a397784bdf4/doomgeneric/doomgeneric_interop_audio.h#L54) with your sound/music callbacks before `Create`.

Sound effect data sent to callbacks is in mono 16-bit signed PCM (expanded from [8-bit **unsigned** PCM](https://doomwiki.org/wiki/Sound)). Note that padding is preserved. Most sound effects are in 11025hz.

Music is sent to callbacks as MIDI (converted internally from [MUS](https://doomwiki.org/wiki/MUS)).

## Other features? (jumping, rebinding controls, modern rendering, etc.)
This is a very limited-scope project for me (I just wanted to get Doom running inside a Unity mod) but if you get it working let me know! I love to see the concept of "doom on everything" thrive.

# Platforms
This fork has only been tested on Windows.

