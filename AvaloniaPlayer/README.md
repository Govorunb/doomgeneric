## [Avalonia](https://avaloniaui.net) player for doomgeneric
A relatively minimal example implementation of a managed host running doomgeneric through callbacks.

#### Building
Make sure to build the [native project](https://github.com/Govorunb/doomgeneric/blob/1dbfbcd3eac86984944eef27398c27e0486cff86/doomgeneric/doomgeneric.vcxproj) first. Building this project will automatically copy the native assembly to the output folder.

### Graphics
The native draw buffer is wrapped into a [SkiaSharp](https://github.com/mono/SkiaSharp) bitmap and then rendered to the screen by Avalonia through a [custom draw operation](https://github.com/Govorunb/doomgeneric/blob/1dbfbcd3eac86984944eef27398c27e0486cff86/AvaloniaPlayer/Doom/Screen/BitmapHelper.cs#L18).

### Audio
[NAudio](https://github.com/NAudio/NAudio) is used to output SFX and music (along with [DryWetMidi](https://github.com/melanchall/drywetmidi)).
There are some minor issues with adjusting MIDI volume but it works well enough.

### Input
Input events are sent by the [screen control](https://github.com/Govorunb/doomgeneric/blob/1dbfbcd3eac86984944eef27398c27e0486cff86/AvaloniaPlayer/Doom/Screen/DoomScreen.cs#L74).
