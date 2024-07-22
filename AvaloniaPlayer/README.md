## Avalonia player for doomgeneric

A fairly basic example implementation of a managed host running doomgeneric through callbacks.

### Graphics
The native draw buffer is wrapped into a [SkiaSharp](https://github.com/mono/SkiaSharp) bitmap and then rendered to the screen by Avalonia through a [custom draw operation](./Doom/Screen/BitmapHelper.cs#L17).

### Audio
[NAudio](https://github.com/NAudio/NAudio) is used to output SFX and music (along with [DryWetMidi](https://github.com/melanchall/drywetmidi)).
There are some minor issues with adjusting MIDI volume but it works well enough.

### Input
Input events are sent by the [screen control](./Doom/Screen/DoomScreen.cs#L95). The mouse UX is pretty janky.
